using GHPC.Camera;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using MelonLoader.Utils;
using System.IO;
using TMPro;
using System.Linq;
using MelonLoader;
using UnityEngine.UI;
using GHPC.Player;
using GHPC;
using GHPC.Vehicle;

namespace CustomMissionUtility
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Editor : MonoBehaviour
    {
        public static string[] VicGameIdsEditor = new string[] {
            "BMP-1 NVA",
            "BMP-1 Soviet",
            "BMP-1P NVA",
            "BMP-1P Soviet",
            "BMP-2 NVA",
            "BMP-2 Soviet",
            "BRDM-2 NVA",
            "BRDM-2 Soviet",
            "BTR-60PB NVA",
            "BTR-60PB Soviet",
            "9K111 NVA",
            "9K111 Soviet",
            "SPG-9 NVA",
            "SPG-9 Soviet",
            "Ural NVA",
            "Ural Soviet",
            "T-62",
            "T-64R",
            "T-64A (1974)",
            "T-64A (1979)",
            "T-64A (1981)",
            "T-64A (1983)",
            "T-64A (1984)",
            "T-64B",
            "T-80B",
            "BTR-70",
            "UAZ-469",
            "T-55A",
            "T-72 Ural",
            "T-72 LEM (Gills)",
            "T-72 LEM",
            "T-72 UV1",
            "T-72 UV2",
            "T-72M",
            "T-72M1",
            "PT-76B",
            "T-34-85",
            "T-54A",
            "M1",
            "M1IP",
            "M60A1",
            "M60A1 AOS",
            "M60A1 RISE Passive (Early)",
            "M60A1 RISE Passive (Late)",
            "M60A3",
            "M60A3 TTS",
            "M2 Bradley",
            "M113",
            "M923",
            "TOW",
            "Mi-24 NVA",
            "Mi-24V NVA",
            "Mi-24V Soviet",
            "Mi-8",
            "Mi-2",
            "OH-58A",
            "AH-1"
        };
        public enum SpawnerMode {
            Unit,
            Waypoint
        }
        public static Dictionary<Faction, string> fac_to_string = new Dictionary<Faction, string>()
        {
            [Faction.Blue] = "NATO",
            [Faction.Red] = "Pact",
            [Faction.Neutral] = "Neutral"
        };
        public static Dictionary<Faction, Color> fac_to_colour = new Dictionary<Faction, Color>()
        {
            [Faction.Blue] = Color.blue,
            [Faction.Red] = Color.red,
            [Faction.Neutral] = Color.gray
        };

        public static GameObject editor_ui;
        public static GameObject unit_placeholder;
        public static GameObject waypoint_placeholder;
        public static GameObject selectable;
        public static Material mat_blue;
        public static Material mat_red;
        public static Material mat_selected;
        public static Material mat_default;
        public static Material mat_waypoint_hi;
        public static Material mat_waypoint_default;

        public static GameObject ALL_UNITS_HOLDER;
        public static GameObject EDITOR_UI;
        public static UnitInfoBox INFO_BOX;
        public static PlatoonInfoBox PLATOON_INFO_BOX;
        public static WaypointGroupInfoBox WAYPOINT_GROUP_INFO_BOX;
        public static Transform PLATOONS_SELECTABLE_LIST;
        public static TMP_Dropdown SPAWNER_PLATOON_DROPDOWN;
        public static TextMeshProUGUI SPAWNER_FACTION;

        public static Transform WAYPOINT_GROUPS_SELECTABLE_LIST;

        public static List<GameObject> SELECTED_UNITS;
        public static List<EditorPlatoon> SELECTED_PLATOONS;
        public static List<EditorWaypointGroup> SELECTED_WAYPOINT_GROUPS;

        public static Dictionary<int, EditorUnit> Units;
        public static Dictionary<int, EditorPlatoon> Platoons;
        public static Dictionary<int, EditorWaypointGroup> WaypointGroups;

        public static References.Vehicles spawner_current_vehicle;
        public static EditorPlatoon spawner_current_platoon;

        public static SpawnerMode current_mode = SpawnerMode.Unit;
        public static Faction unit_spawner_faction = Faction.Red;

        [JsonProperty]
        public List<EditorUnit> UnitsSerialized;

        [JsonProperty]
        public List<EditorPlatoon> PlatoonsSerialized;

        [JsonProperty]
        public List<EditorWaypointGroup> WaypointGroupsSerialized;

        [JsonProperty]
        public List<References.Vehicles> SpawnOrdersBlue = new List<References.Vehicles>();

        [JsonProperty]
        public List<References.Vehicles> SpawnOrdersRed = new List<References.Vehicles>();

        [JsonProperty]
        public int UnitCurrentId;

        [JsonProperty]
        public int PlatoonCurrentId;

        [JsonProperty]
        public int WaypointCurrentId;

        List<References.Vehicles> GetSpawnOrderList(Faction faction)
        {
            return faction == Faction.Red ? SpawnOrdersRed : SpawnOrdersBlue;
        }

        void AddSpawnOrder(EditorUnit u) {
            List<References.Vehicles> order_list = GetSpawnOrderList(u.faction);
            if (!order_list.Contains(u.vehicle))
            {
                order_list.Add(u.vehicle);
            }
        }

        public static void UpdateSpawnerFaction(Faction faction)
        {
            unit_spawner_faction = faction;
            SPAWNER_FACTION.text = fac_to_string[unit_spawner_faction];
            SPAWNER_FACTION.color = fac_to_colour[unit_spawner_faction];
        }

        public static void WaypointGroupSelected(EditorWaypointGroup group)
        {
            SELECTED_WAYPOINT_GROUPS.Clear();
            SELECTED_WAYPOINT_GROUPS.Add(group);

            foreach (EditorWaypoint wp in SELECTED_WAYPOINT_GROUPS[0].waypoints)
            {
                EditorTools.SwitchMat(wp.go.GetComponent<MeshRenderer>(), mat_waypoint_hi, 0);
            }

            WAYPOINT_GROUP_INFO_BOX.UpdateInfo();
        }

        public static void ClearWaypointGroupSelection()
        {
            foreach (EditorWaypoint wp in Editor.SELECTED_WAYPOINT_GROUPS[0].waypoints)
            {
                EditorTools.SwitchMat(wp.go.GetComponent<MeshRenderer>(), Editor.mat_waypoint_default, 0);
            }

            SELECTED_WAYPOINT_GROUPS.Clear();
            WAYPOINT_GROUP_INFO_BOX.UpdateInfo();
        }

        public static void SingleUnitSelected(GameObject unit)
        {
            EditorUnit eu = unit.GetComponent<EditorUnit>();

            if (SELECTED_UNITS.Count != 0)
            {
                EditorTools.SwitchMat(SELECTED_UNITS[0].GetComponent<MeshRenderer>(), mat_default, 1);
            }

            SELECTED_UNITS.Clear();
            SELECTED_UNITS.Add(unit);
            EditorTools.SwitchMat(SELECTED_UNITS[0].GetComponent<MeshRenderer>(), mat_selected, 1);
            INFO_BOX.dropdown.value = (int)(eu.vehicle);
            INFO_BOX.UpdateInfo();
        }

        public static void ClearUnitSelection()
        {
            if (SELECTED_UNITS.Count != 0)
            {
                EditorTools.SwitchMat(SELECTED_UNITS[0].GetComponent<MeshRenderer>(), mat_default, 1);
            }

            SELECTED_UNITS.Clear();
            INFO_BOX.UpdateInfo();
        }

        void Update() {
            if (!CameraManager.Instance.CameraFollow.IsInFreeFlyMode) return;

            if (Input.GetKeyDown(KeyCode.U)) Save();
            if (Input.GetKeyDown(KeyCode.H)) Load();

            if (Input.GetMouseButtonDown(1))
            {
                CameraManager.Instance.CameraFollow.enabled = !CameraManager.Instance.CameraFollow.enabled;
                Cursor.lockState = !Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
                PlayerInput.Instance.enabled = !PlayerInput.Instance.enabled;
                Cursor.visible = !Cursor.visible;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) && Cursor.lockState != CursorLockMode.None) {
                INFO_BOX.gameObject.SetActive(true);
                PLATOON_INFO_BOX.gameObject.SetActive(true);
                EDITOR_UI.transform.Find("PltsRoot").gameObject.SetActive(true);
                EDITOR_UI.transform.Find("UnitsRoot").gameObject.SetActive(true);
                EDITOR_UI.transform.Find("UnitSpawner").gameObject.SetActive(true);

                EDITOR_UI.transform.Find("WaypointsRoot").gameObject.SetActive(false);
                EDITOR_UI.transform.Find("WaypointsInfo").gameObject.SetActive(false);

                current_mode = SpawnerMode.Unit;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2) && Cursor.lockState != CursorLockMode.None)
            {
                INFO_BOX.gameObject.SetActive(false);
                PLATOON_INFO_BOX.gameObject.SetActive(false);
                EDITOR_UI.transform.Find("PltsRoot").gameObject.SetActive(false);
                EDITOR_UI.transform.Find("UnitsRoot").gameObject.SetActive(false);
                EDITOR_UI.transform.Find("UnitSpawner").gameObject.SetActive(false);

                EDITOR_UI.transform.Find("WaypointsRoot").gameObject.SetActive(true);
                EDITOR_UI.transform.Find("WaypointsInfo").gameObject.SetActive(true);

                current_mode = SpawnerMode.Waypoint;
            }

            switch (current_mode) 
            {
                case SpawnerMode.Unit: 
                    EditorController.UnitControlHandler();
                    break;
                case SpawnerMode.Waypoint:
                    EditorController.WaypointControlHandler();
                    break;
            }
        }

        void Awake()
        {
            EDITOR_UI = GameObject.Instantiate(editor_ui);
            ALL_UNITS_HOLDER = new GameObject("EDITOR UNITS");
            SELECTED_UNITS = new List<GameObject>() {};
            SELECTED_PLATOONS = new List<EditorPlatoon>() {};
            SELECTED_WAYPOINT_GROUPS = new List<EditorWaypointGroup>() {};
            PLATOON_INFO_BOX = EDITOR_UI.GetComponentInChildren<PlatoonInfoBox>();
            INFO_BOX = EDITOR_UI.GetComponentInChildren<UnitInfoBox>();
            WAYPOINT_GROUP_INFO_BOX = EDITOR_UI.GetComponentInChildren<WaypointGroupInfoBox>(true);
            PLATOONS_SELECTABLE_LIST = EDITOR_UI.transform.Find("PltsRoot/Plts/PltsList/Viewport/Content");
            WAYPOINT_GROUPS_SELECTABLE_LIST = EDITOR_UI.transform.Find("WaypointsRoot/Waypoints/WaypointsList/Viewport/Content");
            SPAWNER_PLATOON_DROPDOWN = EDITOR_UI.transform.Find("UnitSpawner/Panel/DropdownPlt").GetComponent<TMP_Dropdown>();
            SPAWNER_FACTION = EDITOR_UI.transform.Find("UnitSpawner/Panel/Faction").GetComponent<TextMeshProUGUI>();

            Units = new Dictionary<int, EditorUnit> {};
            Platoons = new Dictionary<int, EditorPlatoon> {};
            WaypointGroups = new Dictionary<int, EditorWaypointGroup> {};
            spawner_current_vehicle = 0;

            INFO_BOX.SetActive(false);

            EDITOR_UI.transform.Find("UnitSpawner/Panel/Dropdown").GetComponent<TMP_Dropdown>().onValueChanged.AddListener(delegate (int i) {
                spawner_current_vehicle = (References.Vehicles)i;
                UpdateSpawnerFaction(References.GetVehicle(spawner_current_vehicle).GetComponent<Vehicle>().Allegiance);
            });

            EDITOR_UI.transform.Find("PltsRoot/Plts/Header/Add").GetComponent<Button>().onClick.AddListener(delegate() {
                EditorTools.CreatePlatoon();
                INFO_BOX.PopulatePlatoonOptions();
                INFO_BOX.UpdateInfo();
            });

            EDITOR_UI.transform.Find("WaypointsRoot/Waypoints/Header/Add").GetComponent<Button>().onClick.AddListener(delegate () {
                EditorTools.CreateWaypointGroup();
            });

            // future me will deal with this 
            INFO_BOX.position.x_field.onSubmit.AddListener(delegate (string s)
            {
                EditorUnit selected = SELECTED_UNITS[0].GetComponent<EditorUnit>();
                selected.go.transform.position = new Vector3(float.Parse(s), float.Parse(INFO_BOX.position.y_field.text), float.Parse(INFO_BOX.position.z_field.text));
            });
            INFO_BOX.position.y_field.onSubmit.AddListener(delegate (string s)
            {
                EditorUnit selected = SELECTED_UNITS[0].GetComponent<EditorUnit>();
                selected.go.transform.position = new Vector3(float.Parse(INFO_BOX.position.x_field.text), float.Parse(s), float.Parse(INFO_BOX.position.z_field.text));
            });
            INFO_BOX.position.z_field.onSubmit.AddListener(delegate (string s)
            {
                EditorUnit selected = SELECTED_UNITS[0].GetComponent<EditorUnit>();
                selected.go.transform.position = new Vector3(float.Parse(INFO_BOX.position.x_field.text), float.Parse(INFO_BOX.position.y_field.text), float.Parse(s));
            });
            INFO_BOX.rotation.x_field.onSubmit.AddListener(delegate (string s)
            {
                EditorUnit selected = SELECTED_UNITS[0].GetComponent<EditorUnit>();
                selected.go.transform.eulerAngles = new Vector3(float.Parse(s), float.Parse(INFO_BOX.rotation.y_field.text), float.Parse(INFO_BOX.rotation.z_field.text));
            });
            INFO_BOX.rotation.y_field.onSubmit.AddListener(delegate (string s)
            {
                EditorUnit selected = SELECTED_UNITS[0].GetComponent<EditorUnit>();
                selected.go.transform.eulerAngles = new Vector3(float.Parse(INFO_BOX.rotation.x_field.text), float.Parse(s), float.Parse(INFO_BOX.rotation.z_field.text));
            });
            INFO_BOX.rotation.z_field.onSubmit.AddListener(delegate (string s)
            {
                EditorUnit selected = SELECTED_UNITS[0].GetComponent<EditorUnit>();
                selected.go.transform.eulerAngles = new Vector3(float.Parse(INFO_BOX.rotation.x_field.text), float.Parse(INFO_BOX.rotation.y_field.text), float.Parse(s));
            });

            SPAWNER_PLATOON_DROPDOWN.onValueChanged.AddListener(delegate (int i)
            {
                if (i == 0)
                {
                    spawner_current_platoon = null;
                    return;
                }
                spawner_current_platoon = PLATOONS_SELECTABLE_LIST.GetChild(i - 1).GetComponent<PltSelectable>().epl;
            });

            CameraManager.Instance.CameraFollow.MinimumElevation = -89f;
            CameraManager.Instance.CameraFollow.fovRange = new Vector2(0.1f, 135f);
        }

        void Save() {
            MelonLogger.Msg("saved");

            UnitCurrentId = EditorUnit.CurrentId;
            PlatoonCurrentId = EditorPlatoon.CurrentId;

            UnitsSerialized = Units.Select(u => u.Value).ToList();
            PlatoonsSerialized = Platoons.Select(u => u.Value).ToList();
            WaypointGroupsSerialized = WaypointGroups.Select(u => u.Value).ToList();

            foreach (EditorUnit u in UnitsSerialized)
            {
                u.Serialize();
                AddSpawnOrder(u);
            }

            foreach (EditorPlatoon ep in PlatoonsSerialized) {
                foreach (EditorUnit u in ep.Units) {
                    u.Serialize();
                    AddSpawnOrder(u);
                }
            }

            foreach (EditorWaypointGroup wpg in WaypointGroupsSerialized)
            {
                foreach (EditorWaypoint w in wpg.waypoints)
                {
                    w.Serialize();                 
                }
            }

            string json = JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            File.WriteAllText(MelonEnvironment.ModsDirectory + "/mission.json", json);
        }

        void Load()
        {
            MelonLogger.Msg("loaded");

            ClearUnitSelection();
            Clear();
          
            string json = File.ReadAllText(MelonEnvironment.ModsDirectory + "/mission.json");
            Editor c = JsonConvert.DeserializeObject<Editor>(json);

            foreach (EditorWaypointGroup _wpg in c.WaypointGroupsSerialized) {
                EditorWaypointGroup wpg = new EditorWaypointGroup();
                wpg.id = _wpg.id;
                wpg.name = _wpg.name;
                wpg.CreateSelectable(Editor.EDITOR_UI.transform.Find("WaypointsRoot/Waypoints/WaypointsList/Viewport/Content"));
                WaypointGroups.Add(wpg.id, wpg);

                foreach (EditorWaypoint _wp in _wpg.waypoints) {
                    GameObject waypoint = GameObject.Instantiate(waypoint_placeholder);
                    waypoint.transform.position = (Vector3)_wp.pos;

                    EditorWaypoint wp = waypoint.AddComponent<EditorWaypoint>();
                    wp.group = wpg;
                    wp.go = waypoint;
                    wp.go_id = waypoint.transform.Find("Id").GetComponent<TMP_Text>();
                    wp.go_group = waypoint.transform.Find("Group").GetComponent<TMP_Text>();

                    wpg.waypoints.Add(wp);
                }
            }

            foreach (EditorUnit eu in c.UnitsSerialized)
            {
                EditorTools.CreateUnit(eu);
            }

            foreach (EditorPlatoon ep in c.PlatoonsSerialized) {
                EditorPlatoon plt = new EditorPlatoon();
                plt.name = ep.name;
                plt.id = ep.id;
                plt.tag = ep.tag;
                plt.waypoints = ep.waypoints;
                plt.formation = ep.formation;
                plt.Init();
                Platoons.Add(plt.id, plt);
                
                foreach (EditorUnit eu in ep.Units) {
                    EditorTools.CreateUnit(eu, plt);
                }            
            }

            PLATOON_INFO_BOX.PopulateWaypointOptions();
            INFO_BOX.PopulatePlatoonOptions();
            INFO_BOX.UpdateInfo();

            EditorUnit.CurrentId = c.UnitCurrentId;
            EditorPlatoon.CurrentId = c.PlatoonCurrentId;
            EditorWaypointGroup.CurrentId = c.WaypointCurrentId;
        }

        public void Clear()
        {
            foreach (EditorUnit eu in Units.Values)
            {
                eu.Remove();
            }

            foreach (EditorPlatoon ep in Platoons.Values)
            {
                foreach (EditorUnit eu in ep.Units)
                {
                    eu.Remove();
                }
                ep.Remove();
            }

            Units.Clear();
            Platoons.Clear();
        }
    }
}