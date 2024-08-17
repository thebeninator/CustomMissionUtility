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

[assembly: MelonInfo(typeof(CustomMissionUtility.CustomMissionUtility), "Custom Mission Utility", "1.0.0", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]
[assembly: MelonPriority(-9999)]

namespace CustomMissionUtility
{
    public class CustomMissionUtility : MelonMod
    {
        private static AllMissionsMetaDataScriptable all_missions_metadata_so;
        private static List<CustomMission> all_custom_missions = new List<CustomMission>();
        private static Dictionary<string, int> custom_mission_lookup = new Dictionary<string, int>();
        private static Dictionary<string, DynamicMissionMetadataScriptable> custom_mission_flex_metadata = new Dictionary<string, DynamicMissionMetadataScriptable>();
        private bool all_missions_loaded = false;
        private static UnitPrefabLookupScriptable unit_prefab_lookup;
        internal static MissionSceneMeta MissionMeta;

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
            Editor.LoadAssets();

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
            if (custom_mission_lookup.ContainsKey(sceneName)) {
                GameObject editor = new GameObject("EDITOR");
                editor.AddComponent<Editor>();

                GameObject meta_obj = new GameObject("META");
                MissionMeta = meta_obj.AddComponent<MissionSceneMeta>();
                MissionMeta._startingUnits = new MissionSceneMeta.StartingUnitData[] { };
                MissionMeta.DynamicMetadata = custom_mission_flex_metadata[sceneName];

                GameObject random_env_obj = new GameObject("RAND ENV");
                random_env_obj.transform.parent = meta_obj.transform;
                RandomEnvironment rand_env = random_env_obj.AddComponent<RandomEnvironment>();

                rand_env.Settings = new RandomEnvironment.EnvSettings(); 
                all_custom_missions[custom_mission_lookup[sceneName]].OnMissionStartedLoading();

                /*
                GameObject test_spawnpoint = new GameObject("f");
                VehicleSpawnPoint sp = test_spawnpoint.AddComponent<VehicleSpawnPoint>();
                sp.PlatoonName = "BLYAT";
                sp.PlatoonMode = true;
                sp.PlatoonMemberCount = 4;
                sp.VariantIndex = 0;
                sp.Pattern = new PlatoonPatternScriptable();
                sp.Pattern.AllUnits = new UnitClass[] { UnitClass.IFV, UnitClass.IFV, UnitClass.IFV, UnitClass.IFV };
                sp.Pattern.VariantIndices = new int[] { 0, 0, 0, 0 };
                */
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (custom_mission_lookup.ContainsKey(sceneName))
            {         
                all_custom_missions[custom_mission_lookup[sceneName]].OnMissionFinishedLoading();
                StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(all_custom_missions[custom_mission_lookup[sceneName]].MapMarkers), GameStatePriority.Medium);
            }

            if ((sceneName == "MainMenu2_Scene" || sceneName == "MainMenu2-1_Scene") && !all_missions_loaded) {
                unit_prefab_lookup = Resources.FindObjectsOfTypeAll<UnitPrefabLookupScriptable>().First();
                References.GetVicReferences();

                if (all_missions_metadata_so == null) all_missions_metadata_so = Resources.FindObjectsOfTypeAll<AllMissionsMetaDataScriptable>().First();

                foreach (CustomMission custom_mission in all_custom_missions)
                {
                    custom_mission.UnitSpawnOrders();

                    CustomMissionData data = custom_mission.MissionData;
                    MissionTheaterScriptable theater = all_missions_metadata_so.Theaters[(int)data.Theater];

                    MissionMetaData mission_metadata = new MissionMetaData();
                    mission_metadata._isCategory = false;
                    mission_metadata.MissionName = data.Name;
                    mission_metadata.TimeOptions = data.TimeOptions;
                    mission_metadata.CloudBias = data.CloudBias;

                    mission_metadata.FactionInfo = new List<FactionMissionInfo>();

                    if (data.PlayableFactions.Contains(Faction.Blue)) mission_metadata.FactionInfo.Add(new FactionMissionInfo(Faction.Blue, data.DescriptionBluFor));
                    if (data.PlayableFactions.Contains(Faction.Red)) mission_metadata.FactionInfo.Add(new FactionMissionInfo(Faction.Red, data.DescriptionRedFor));

                    mission_metadata.MissionSceneReference = new SceneReference();
                    mission_metadata.MissionSceneReference.sceneAssetGuidHex = data.Id + "[CUSTOM]";

                    mission_metadata.FlexMissionData = new DynamicMissionMetadataScriptable();
                    mission_metadata.FlexMissionData.name = data.Id + "_META";

                    DynamicMissionMetadata flex_mission_data = new DynamicMissionMetadata();
                    flex_mission_data.PlayerFaction = data.PlayableFactions[0];
                    flex_mission_data.FriendlySpawnInfo = new FactionSpawnInfo();
                    flex_mission_data.EnemySpawnInfo = new FactionSpawnInfo();

                    foreach (KeyValuePair<UnitType, List<References.Vehicles>> order in custom_mission.SpawnOrdersBluFor) {
                        for (int i = 0; i < order.Value.Count; i++)
                        {
                            FactionSpawnInfo.SpawnOrder spawn = new FactionSpawnInfo.SpawnOrder();
                            string name = References.GetVehicle(order.Value[i]).GetComponent<Vehicle>().UniqueName;
                            spawn.Key = name;
                            spawn.VariantIndex = i;
                            spawn.Type = order.Key;
                            spawn.Class = unit_prefab_lookup.AllUnits.Where(o => o.Name == name).First().Class;

                            if (data.PlayableFactions[0] == Faction.Blue)
                                flex_mission_data.FriendlySpawnInfo.SpawnOrders.Add(spawn);
                            else {
                                flex_mission_data.EnemySpawnInfo.SpawnOrders.Add(spawn);
                            }
                        }
                    }

                    foreach (KeyValuePair<UnitType, List<References.Vehicles>> order in custom_mission.SpawnOrdersRedFor)
                    {
                        for (int i = 0; i < order.Value.Count; i++)
                        {
                            FactionSpawnInfo.SpawnOrder spawn = new FactionSpawnInfo.SpawnOrder();
                            string name = References.GetVehicle(order.Value[i]).GetComponent<Vehicle>().UniqueName;
                            spawn.Key = name;
                            spawn.VariantIndex = i;
                            spawn.Type = order.Key;
                            spawn.Class = unit_prefab_lookup.AllUnits.Where(o => o.Name == name).First().Class;

                            if (data.PlayableFactions[0] == Faction.Red)
                                flex_mission_data.FriendlySpawnInfo.SpawnOrders.Add(spawn);
                            else
                            {
                                flex_mission_data.EnemySpawnInfo.SpawnOrders.Add(spawn);
                            }
                        }
                    }

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
                }

                all_missions_loaded = true;
            }
        }
    }
}
