using MelonLoader;
using System;
using System.Linq;
using UnityEngine;
using Eflatun.SceneReference;
using GHPC.Mission.Data;
using GHPC.Mission;
using GHPC;
using System.Collections.Generic;
using MelonLoader.Utils;
using System.IO;
using System.Reflection;
using HarmonyLib;
using GHPC.State;
using GHPC.Weaponry;
using UnityEngine.SceneManagement;
using GHPC.Vehicle;
using Newtonsoft.Json;
using System.Xml.Linq;
using static UnityEngine.UI.GridLayoutGroup;
using GHPC.AI;
using System.Collections;
using GHPC.Event;
using GHPC.Weapons.Artillery;

[assembly: MelonInfo(typeof(CustomMissionUtility.CMU), "Custom Mission Utility", "1.0.0", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]
[assembly: MelonPriority(-9999)]

namespace CustomMissionUtility
{
    public class CMU : MelonMod
    {
        private static AllMissionsMetaDataScriptable all_missions_metadata_so;
        private static List<CustomMission> all_custom_missions = new List<CustomMission>();
        private static Dictionary<string, int> custom_mission_lookup = new Dictionary<string, int>();
        private static Dictionary<string, DynamicMissionMetadataScriptable> custom_mission_flex_metadata = new Dictionary<string, DynamicMissionMetadataScriptable>();
        private bool all_missions_loaded = false;
        internal static UnitPrefabLookupScriptable unit_prefab_lookup;
        internal static MissionSceneMeta MissionMeta;
        public static FireMissionManager FireMissionManager;

        private static Dictionary<int, WaypointHolder> waypoint_holders = new Dictionary<int, WaypointHolder>();
        private static Queue<FreeformPlatoonTicket> freeform_platoon_queue = new Queue<FreeformPlatoonTicket>();

        public static Dictionary<int, DynamicSpawnPoint> DynamicSpawnpointsSolo = new Dictionary<int, DynamicSpawnPoint>();
        public static Dictionary<int, DynamicSpawnPoint> DynamicSpawnpointsPlatoon = new Dictionary<int, DynamicSpawnPoint>();

        private class FreeformPlatoonTicket
        {
            public VehicleSpawnPoint spawner;
            public List<EditorUnit> editor_units;
        }

        private static void GenerateSpawnOrders(CustomMissionData data, DynamicMissionMetadata flex_mission_data, Dictionary<UnitClass, List<References.Vehicles>> orders, Faction faction) {
            foreach (KeyValuePair<UnitClass, List<References.Vehicles>> order in orders)
            {
                for (int i = 0; i < order.Value.Count; i++)
                {
                    FactionSpawnInfo.SpawnOrder spawn = new FactionSpawnInfo.SpawnOrder();
                    Vehicle vic = References.GetVehicle(order.Value[i]).GetComponent<Vehicle>();
                    string name = vic.UniqueName;
                    spawn.Key = name;
                    spawn.VariantIndex = i;
                    spawn.Type = vic.Type;
                    spawn.Class = order.Key;

                    if (data.PlayableFactions[0] == faction)
                        flex_mission_data.FriendlySpawnInfo.SpawnOrders.Add(spawn);
                    else
                    {
                        flex_mission_data.EnemySpawnInfo.SpawnOrders.Add(spawn);
                    }
                }
            }
        }

        public class HotReloadTest : MonoBehaviour {
            void Update() {
                if (Input.GetKeyDown(KeyCode.Slash))
                {
                    LoadCustomMissionMetaData(all_custom_missions[0], reload: true);
                    MelonLogger.Msg("hot reload");
                }
            }
        }

        private IEnumerator FreeformPlatoonHandler(GameState _) {
            foreach (FreeformPlatoonTicket ticket in freeform_platoon_queue) {
                for (int i = 0; i < ticket.spawner.SpawnedUnits.Count; i++) {
                    Unit unit = ticket.spawner.SpawnedUnits[i];
                    unit.transform.position = (Vector3)ticket.editor_units[i].pos;
                    unit.transform.eulerAngles = (Vector3)ticket.editor_units[i].rot;
                }
            }

            freeform_platoon_queue.Clear();

            yield break;
        }

        [HarmonyPatch(typeof(SceneReference))]
        [HarmonyPatch("Name", MethodType.Getter)]
        public static class CustomSceneReference
        {
            public static bool Prefix(ref string __result, SceneReference __instance) {
                string name = __instance.sceneAssetGuidHex;

                if (name.Contains("[CUSTOM]")) {
                    __result = name.Remove(name.IndexOf("["));
                    return false;
                }

                return true;
            }
        }

        public override void OnEarlyInitializeMelon()
        {
            EditorAssetLoader.LoadAssets();
            FireSupport.Init();

            string custom_missions_path = MelonEnvironment.ModsDirectory + "\\CustomMissions";
            string[] mission_folders_paths = Directory.GetDirectories(custom_missions_path);

            foreach (string mission_folder_path in mission_folders_paths)
            {
                string dll_path = Directory.GetFiles(mission_folder_path, "*.dll").First();

                foreach (string mission_bundle_path in Directory.GetFiles(mission_folder_path, "*.mission")) {
                    AssetBundle.LoadFromFile(mission_bundle_path);
                }

                Assembly dll = MelonAssembly.LoadMelonAssembly(dll_path).Assembly;
                Type[] types = dll.GetTypes();

                HarmonyInstance.PatchAll(dll);
               
                foreach (Type type in types)
                {
                    if (type.IsSubclassOf(typeof(CustomMission))) {
                        CustomMission mission = Activator.CreateInstance(type) as CustomMission;
                        custom_mission_lookup.Add(mission.MissionData.Id, all_custom_missions.Count());
                        all_custom_missions.Add(mission);
                    };
                }
            }
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (!custom_mission_lookup.ContainsKey(sceneName)) return;

            GameObject editor = new GameObject("EDITOR");
            editor.AddComponent<Editor>();

            GameObject meta_obj = new GameObject("META");
            MissionMeta = meta_obj.AddComponent<MissionSceneMeta>();
            MissionMeta._startingUnits = new MissionSceneMeta.StartingUnitData[] { };    
            MissionMeta.DynamicMetadata = custom_mission_flex_metadata[sceneName];
            meta_obj.transform.SetAsFirstSibling();

            GameObject random_env_obj = new GameObject("RAND ENV");
            random_env_obj.transform.parent = meta_obj.transform;
            RandomEnvironment rand_env = random_env_obj.AddComponent<RandomEnvironment>();
            rand_env.Settings = new RandomEnvironment.EnvSettings();

            GameObject event_manager_obj = new GameObject("EVENT MANAGER");
            event_manager_obj.transform.parent = meta_obj.transform;
            event_manager_obj.AddComponent<EventManager>();

            GameObject fire_mission_manager_obj = new GameObject("FIRE MISSION MANAGER");
            fire_mission_manager_obj.transform.parent = meta_obj.transform;
            FireMissionManager = fire_mission_manager_obj.AddComponent<FireMissionManager>();

            waypoint_holders.Clear();
            DynamicSpawnpointsSolo.Clear();
            DynamicSpawnpointsPlatoon.Clear();

            CustomMission mission = all_custom_missions[custom_mission_lookup[sceneName]];
            string json = File.ReadAllText(MelonEnvironment.ModsDirectory + "/mission.json");
            if (json.Length > 0)
            {
                Editor c = JsonConvert.DeserializeObject<Editor>(json);

                foreach (EditorWaypointGroup wpg in c.WaypointGroupsSerialized)
                {
                    List<Vector3> points = new List<Vector3>();

                    foreach (EditorWaypoint wp in wpg.waypoints)
                    {
                        points.Add((Vector3)wp.pos);
                    }

                    waypoint_holders.Add(wpg.id, Tools.CreateWaypoints(wpg.name, points.ToArray()));
                }

                foreach (EditorUnit u in c.UnitsSerialized)
                {
                    string name = References.GetVehicle(u.vehicle).GetComponent<Vehicle>().UniqueName;
                    UnitPrefabLookupScriptable.UnitPrefabMetadata prefab = unit_prefab_lookup.AllUnits.Where(o => o.Name == name).First();

                    GameObject spawn_point = new GameObject("solo spawnpoint " + u.id);
                    spawn_point.transform.position = (Vector3)u.pos;
                    spawn_point.transform.eulerAngles = (Vector3)(u.rot);
                    VehicleSpawnPoint vsp = spawn_point.AddComponent<VehicleSpawnPoint>();

                    vsp.PlatoonMode = false;
                    vsp.VariantIndex = mission.GetSpawnOrder(u.vehicle, u.faction);
                    vsp.ValidClasses = new UnitClass[] { prefab.Class };
                    vsp.Team = SceneController.TargetSpawningFaction == u.faction ? MissionTeam.Friendly : MissionTeam.Enemy;
                    vsp.Waypoints = u.waypoints != -1 ? waypoint_holders[u.waypoints] : null;
                    vsp.NoPlayerControl = !u.playable_unit;

                    if (u.starting_unit)
                    {
                        MissionMeta.FlexSpawnPoint = vsp;
                    }

                    DynamicSpawnpointsSolo.Add(u.id, vsp);
                }

                foreach (EditorPlatoon p in c.PlatoonsSerialized)
                {
                    GameObject spawn_point = new GameObject("platoon spawnpoint " + p.name + " " + p.id);
                    spawn_point.transform.position = (Vector3)p.Units[0].pos;
                    spawn_point.transform.eulerAngles = (Vector3)(p.Units[0].rot);
                    VehicleSpawnPoint vsp = spawn_point.AddComponent<VehicleSpawnPoint>();

                    vsp.PlatoonMode = true;

                    List<UnitClass> valid_classes = new List<UnitClass>();
                    List<int> var_indices = new List<int>();

                    foreach (EditorUnit u in p.Units)
                    {
                        string name = References.GetVehicle(u.vehicle).GetComponent<Vehicle>().UniqueName;
                        UnitPrefabLookupScriptable.UnitPrefabMetadata prefab = unit_prefab_lookup.AllUnits.Where(o => o.Name == name).First();
                        valid_classes.Add(prefab.Class);
                        var_indices.Add(mission.GetSpawnOrder(u.vehicle, u.faction));

                        if (u.starting_unit)
                        {
                            MissionMeta.FlexSpawnPoint = vsp;
                        }
                    }

                    PlatoonPatternScriptable pattern = new PlatoonPatternScriptable();
                    pattern.AllUnits = valid_classes.ToArray();
                    pattern.VariantIndices = var_indices.ToArray();
                    vsp.Pattern = pattern;

                    vsp.TerrainAware = true;
                    vsp.ValidClasses = valid_classes.ToArray();
                    vsp.Team = SceneController.TargetSpawningFaction == p.Units[0].faction ? MissionTeam.Friendly : MissionTeam.Enemy;
                    vsp.PlatoonMemberCount = p.Units.Count;
                    vsp.PlatoonName = p.name;
                    vsp.StartFormation = p.formation;
                    vsp.Waypoints = p.waypoints != -1 ? waypoint_holders[p.waypoints] : null;

                    if (p.formation == GHPC.AI.Platoons.FormationType.None)
                    {
                        List<Transform> stationary_spawns = new List<Transform>();
                        foreach (EditorUnit u in p.Units)
                        {
                            GameObject s = new GameObject();
                            s.transform.position = (Vector3)u.pos;
                            s.transform.eulerAngles = (Vector3)u.rot;
                            stationary_spawns.Add(s.transform);
                        }
                        vsp.StationarySpawns = stationary_spawns.ToArray();
                    }
                    else
                    {
                        FreeformPlatoonTicket ticket = new FreeformPlatoonTicket();
                        ticket.spawner = vsp;
                        ticket.editor_units = p.Units;
                        // freeform_platoon_queue.Enqueue(ticket);
                    }

                    DynamicSpawnpointsPlatoon.Add(p.id, vsp);
                }
            }

            mission.OnMissionStartedLoading();
            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(FreeformPlatoonHandler), GameStatePriority.Lowest);           
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (custom_mission_lookup.ContainsKey(sceneName))
            {
                CustomMission mission = all_custom_missions[custom_mission_lookup[sceneName]];
                StateController.RunOrDefer(GameState.MissionUnitsLoaded, new GameStateEventHandler(mission.OnMissionFinishedLoading), GameStatePriority.Lowest);
                StateController.RunOrDefer(GameState.GameInitialization, new GameStateEventHandler(mission.MapMarkers), GameStatePriority.Highest);
            }

            if (sceneName == "MainMenu2_Scene" || sceneName == "MainMenu2-1_Scene") {
                GameObject c = new GameObject();
                c.AddComponent<HotReloadTest>();
            }

            if ((sceneName == "MainMenu2_Scene" || sceneName == "MainMenu2-1_Scene") && !all_missions_loaded) {
                unit_prefab_lookup = Resources.FindObjectsOfTypeAll<UnitPrefabLookupScriptable>().First();
                References.GetVicReferences();

                if (all_missions_metadata_so == null) all_missions_metadata_so = Resources.FindObjectsOfTypeAll<AllMissionsMetaDataScriptable>().First();

                foreach (CustomMission custom_mission in all_custom_missions)
                {
                    LoadCustomMissionMetaData(custom_mission);
                }

                all_missions_loaded = true;
            }
        }

        internal static void LoadCustomMissionMetaData(CustomMission custom_mission, bool reload = false) {
            custom_mission.UnitSpawnOrders();

            CustomMissionData data = custom_mission.MissionData;
            MissionTheaterScriptable theater = all_missions_metadata_so.Theaters[(int)data.Theater];

            string json = File.ReadAllText(MelonEnvironment.ModsDirectory + "/mission.json");

            if (json.Length > 0)
            {
                Editor c = JsonConvert.DeserializeObject<Editor>(json);
                List<References.Vehicles> spawn_orders_blue = c.SpawnOrdersBlue;
                List<References.Vehicles> spawn_orders_red = c.SpawnOrdersRed;

                foreach (References.Vehicles vic in spawn_orders_blue) custom_mission.CreateSpawnOrder(vic, Faction.Blue);           
                foreach (References.Vehicles vic in spawn_orders_red)  custom_mission.CreateSpawnOrder(vic, Faction.Red);              
            }

            MissionMetaData mission_metadata = !reload ? new MissionMetaData() : custom_mission.MissionMetaData;
            mission_metadata._isCategory = false;
            mission_metadata.MissionName = data.Name;
            mission_metadata.TimeOptions = data.TimeOptions;
            mission_metadata.CloudBias = data.CloudBias;

            mission_metadata.FactionInfo = new List<FactionMissionInfo>();

            if (data.PlayableFactions.Contains(Faction.Blue)) mission_metadata.FactionInfo.Add(new FactionMissionInfo(Faction.Blue, data.DescriptionBluFor));
            if (data.PlayableFactions.Contains(Faction.Red)) mission_metadata.FactionInfo.Add(new FactionMissionInfo(Faction.Red, data.DescriptionRedFor));

            mission_metadata.MissionSceneReference = !reload ? new SceneReference() : custom_mission.MissionMetaData.MissionSceneReference;
            mission_metadata.MissionSceneReference.sceneAssetGuidHex = data.Id + "[CUSTOM]";

            mission_metadata.FlexMissionData = !reload ? new DynamicMissionMetadataScriptable() : custom_mission.MissionMetaData.FlexMissionData;
            mission_metadata.FlexMissionData.name = data.Id + "_META";

            DynamicMissionMetadata flex_mission_data = !reload ? new DynamicMissionMetadata() : custom_mission.MissionMetaData.FlexMissionData.MissionData;
            flex_mission_data.PlayerFaction = data.PlayableFactions[0];
            flex_mission_data.FriendlySpawnInfo = new FactionSpawnInfo();
            flex_mission_data.EnemySpawnInfo = new FactionSpawnInfo();

            GenerateSpawnOrders(data, flex_mission_data, custom_mission.SpawnOrdersBluFor, Faction.Blue);
            GenerateSpawnOrders(data, flex_mission_data, custom_mission.SpawnOrdersRedFor, Faction.Red);

            /*
            FactionSpawnInfo.SpawnOrder spawn = new FactionSpawnInfo.SpawnOrder();
            spawn.Key = "M1IP";
            spawn.Type = UnitType.GroundVehicle;
            spawn.VariantIndex = 0;
            flex_mission_data.FriendlySpawnInfo.SpawnOrders.Add(spawn);
            DynamicMissionAmmoAdjustment ammo = new DynamicMissionAmmoAdjustment();
            ammo.AmmoSet = Resources.FindObjectsOfTypeAll<AmmoLogisticsScriptable>().Where(x => x.name == "Fulda85 US 105mm AP").First();
            ammo.SelectedIndex = 2;
            flex_mission_data.FriendlyAmmoData.Add(ammo);
            */

            mission_metadata.FlexMissionData.MissionData = flex_mission_data;

            if (!reload)
            {
                custom_mission_flex_metadata.Add(data.Id, mission_metadata.FlexMissionData);

                List<MissionMetaData> new_theater_missions = theater.Missions.ToList();

                if (theater.Missions[0].MissionName != "--Modded Missions--")
                {
                    new_theater_missions.Insert(0, new MissionMetaData()
                    {
                        MissionName = "--Modded Missions--",
                        MissionSceneReference = new SceneReference() { sceneAssetGuidHex = "6549ea28410661146aa7184b50807d52" }
                    });
                }

                new_theater_missions.Insert(1, mission_metadata);
                theater.Missions = new_theater_missions.ToArray();
                custom_mission.MissionMetaData = mission_metadata;
            }
        }
    }
}
