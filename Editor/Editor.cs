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
        public int UnitCurrentId;

        [JsonProperty]
        public int PlatoonCurrentId;

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

            if (current_mode == SpawnerMode.Unit)
            {
                UnitControlHandler();
            }
            else if (current_mode == SpawnerMode.Waypoint) {
                WaypointControlHandler();
            }
        }

        void WaypointControlHandler() {
            if (Cursor.lockState == CursorLockMode.None) return;

            if (Input.GetKeyDown(KeyCode.B))
            {
                EditorWaypointGroup group = EditorTools.CreateWaypointGroup();
            }

            if (Input.GetMouseButtonDown(0))
            {
                var cam_follow = CameraManager.Instance.CameraFollow;

                Ray ray = new Ray(cam_follow.BufferedCamera.transform.position, cam_follow.CurrentAimVector);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, 4000f))
                {
                    if (raycastHit.collider.gameObject.name.Contains("Waypoint"))
                    {
                        if (SELECTED_WAYPOINT_GROUPS.Count == 0 || SELECTED_WAYPOINT_GROUPS[0] != raycastHit.collider.gameObject.GetComponent<EditorWaypoint>().group)
                            WaypointGroupSelected(raycastHit.collider.gameObject.GetComponent<EditorWaypoint>().group);
                        return;
                    }
                }

                ClearWaypointGroupSelection();
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                var cam_follow = CameraManager.Instance.CameraFollow;

                Ray ray = new Ray(cam_follow.BufferedCamera.transform.position, cam_follow.CurrentAimVector);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, 4000f))
                {
                    if (raycastHit.collider.gameObject.name.Contains("Waypoint"))
                    {
                        EditorTools.DeleteWaypoint(raycastHit.collider.gameObject);
                        return;
                    }

                    EditorTools.CreateWaypoint(
                        raycastHit.point + new Vector3(0f, 1f, 0f),
                        new Vector3(0f, CameraManager._mainCamera.transform.eulerAngles.y, 0f));
                }
            }
        }

        void UnitControlHandler() {
            if (Cursor.lockState == CursorLockMode.None) return;

            if (Input.GetKeyDown(KeyCode.Z)) {
                UpdateSpawnerFaction(Faction.Blue);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                UpdateSpawnerFaction(Faction.Red);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                UpdateSpawnerFaction(Faction.Neutral);
            }

            if (Input.GetMouseButtonDown(0))
            {
                var cam_follow = CameraManager.Instance.CameraFollow;

                Ray ray = new Ray(cam_follow.BufferedCamera.transform.position, cam_follow.CurrentAimVector);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, 4000f))
                {
                    if (raycastHit.collider.gameObject.name.Contains("UNIT RED"))
                    {
                        SingleUnitSelected(raycastHit.collider.gameObject);
                        return;
                    }
                }

                ClearUnitSelection();
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                var cam_follow = CameraManager.Instance.CameraFollow;

                Ray ray = new Ray(cam_follow.BufferedCamera.transform.position, cam_follow.CurrentAimVector);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, 4000f))
                {
                    if (raycastHit.collider.gameObject.name.Contains("UNIT RED"))
                    {
                        EditorTools.DeleteUnit(raycastHit.collider.gameObject);
                        return;
                    }

                    GameObject unit = EditorTools.CreateUnit(
                        raycastHit.point + new Vector3(0f, 1f, 0f),
                        new Vector3(0f, CameraManager._mainCamera.transform.eulerAngles.y, 0f));

                    SingleUnitSelected(unit);
                }
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

        public static void UpdateSpawnerFaction(Faction faction) {
            unit_spawner_faction = faction;
            SPAWNER_FACTION.text = fac_to_string[unit_spawner_faction];
            SPAWNER_FACTION.color = fac_to_colour[unit_spawner_faction];
        }

        public static void WaypointGroupSelected(EditorWaypointGroup group) {
            SELECTED_WAYPOINT_GROUPS.Clear();
            SELECTED_WAYPOINT_GROUPS.Add(group);

            foreach (EditorWaypoint wp in SELECTED_WAYPOINT_GROUPS[0].waypoints) {
                EditorTools.SwitchMat(wp.go.GetComponent<MeshRenderer>(), mat_waypoint_hi, 0);             
            }

            WAYPOINT_GROUP_INFO_BOX.UpdateInfo();
        }

        public static void ClearWaypointGroupSelection() {
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
            if (SELECTED_UNITS.Count != 0) {
                EditorTools.SwitchMat(SELECTED_UNITS[0].GetComponent<MeshRenderer>(), mat_default, 1);
            }

            SELECTED_UNITS.Clear();
            INFO_BOX.UpdateInfo();
        }

        void Save() {
            UnitCurrentId = EditorUnit.CurrentId;
            PlatoonCurrentId = EditorPlatoon.CurrentId;

            UnitsSerialized = Units.Select(u => u.Value).ToList();
            PlatoonsSerialized = Platoons.Select(u => u.Value).ToList();
            WaypointGroupsSerialized = WaypointGroups.Select(u => u.Value).ToList();

            foreach (EditorUnit u in UnitsSerialized)
            {
                u.Serialize();
            }

            foreach (EditorPlatoon ep in PlatoonsSerialized) {
                foreach (EditorUnit u in ep.Units) {
                    u.Serialize();
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
                Formatting = Formatting.Indented, // replace later
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            File.WriteAllText(MelonEnvironment.ModsDirectory + "/mission.json", json);
        }

        void Load() {
            ClearUnitSelection();
            Clear();

            string json = File.ReadAllText(MelonEnvironment.ModsDirectory + "/mission.json");
            Editor c = JsonConvert.DeserializeObject<Editor>(json);
            UnitsSerialized = c.UnitsSerialized;
            foreach (EditorUnit eu in UnitsSerialized)
            {
                EditorTools.CreateUnit((Vector3)eu.pos, (Vector3)eu.rot, eu.id);
            }

            EditorUnit.CurrentId = UnitsSerialized.Last().id + 1; 
        }

        public void Clear()
        {
            foreach (EditorUnit eu in Units.Values)
            {
                eu.Remove();
            }

            Units.Clear();
            UnitsSerialized.Clear();
        }

        public static void LoadAssets() {
            AssetBundle mission_creator_assets = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory, "CMUAssets"));
            unit_placeholder = mission_creator_assets.LoadAsset<GameObject>("UNIT RED.prefab");
            unit_placeholder.hideFlags = HideFlags.DontUnloadUnusedAsset;

            waypoint_placeholder = mission_creator_assets.LoadAsset<GameObject>("Waypoint.prefab");
            waypoint_placeholder.hideFlags = HideFlags.DontUnloadUnusedAsset;

            mat_blue = mission_creator_assets.LoadAsset<Material>("unit_blue.mat");
            mat_blue.hideFlags = HideFlags.DontUnloadUnusedAsset;

            mat_red = mission_creator_assets.LoadAsset<Material>("unit_red.mat");
            mat_red.hideFlags = HideFlags.DontUnloadUnusedAsset;

            mat_selected = mission_creator_assets.LoadAsset<Material>("unit_selected.mat");
            mat_selected.hideFlags = HideFlags.DontUnloadUnusedAsset;

            mat_waypoint_hi = mission_creator_assets.LoadAsset<Material>("waypoint_hi.mat");
            mat_waypoint_hi.hideFlags = HideFlags.DontUnloadUnusedAsset;

            mat_waypoint_default = mission_creator_assets.LoadAsset<Material>("waypoint_default.mat");
            mat_waypoint_default.hideFlags = HideFlags.DontUnloadUnusedAsset;

            mat_default = mission_creator_assets.LoadAsset<Material>("unit_scarecrow_albedo.mat");
            mat_default.hideFlags = HideFlags.DontUnloadUnusedAsset;

            editor_ui = mission_creator_assets.LoadAsset<GameObject>("EditorUI.prefab");
            editor_ui.hideFlags = HideFlags.DontUnloadUnusedAsset;

            selectable = mission_creator_assets.LoadAsset<GameObject>("Selectable.prefab");
            selectable.hideFlags = HideFlags.DontUnloadUnusedAsset;

            CollapsibleButton units_collapse = editor_ui.transform.Find("UnitsRoot/Units/Header/Collapse").gameObject.AddComponent<CollapsibleButton>();
            units_collapse.collapsible = editor_ui.transform.Find("UnitsRoot/Units/UnitsList").gameObject;
            units_collapse.collapse_icon = editor_ui.transform.Find("UnitsRoot/Units/Header/Collapse/Text (TMP)").GetComponent<TextMeshProUGUI>();
            Draggable units_drag = editor_ui.transform.Find("UnitsRoot/Units/Header").gameObject.AddComponent<Draggable>();
            units_drag.parent = editor_ui.transform.Find("UnitsRoot");

            CollapsibleButton plts_collapse = editor_ui.transform.Find("PltsRoot/Plts/Header/Collapse").gameObject.AddComponent<CollapsibleButton>();
            plts_collapse.collapsible = editor_ui.transform.Find("PltsRoot/Plts/PltsList").gameObject;
            plts_collapse.collapse_icon = editor_ui.transform.Find("PltsRoot/Plts/Header/Collapse/Text (TMP)").GetComponent<TextMeshProUGUI>();
            Draggable plts_drag = editor_ui.transform.Find("PltsRoot/Plts/Header").gameObject.AddComponent<Draggable>();
            plts_drag.parent = editor_ui.transform.Find("PltsRoot");

            Draggable info_drag = editor_ui.transform.Find("Info/Header").gameObject.AddComponent<Draggable>();
            info_drag.parent = editor_ui.transform.Find("Info");
            CollapsibleButton info_collapse = editor_ui.transform.Find("Info/Header/Collapse").gameObject.AddComponent<CollapsibleButton>();
            info_collapse.collapsible = editor_ui.transform.Find("Info/Contents").gameObject;
            info_collapse.collapse_icon = editor_ui.transform.Find("Info/Header/Collapse/Text (TMP)").GetComponent<TextMeshProUGUI>();

            editor_ui.transform.Find("Info/Contents/Position").gameObject.AddComponent<Vec3FieldHandler>();
            editor_ui.transform.Find("Info/Contents/Rotation").gameObject.AddComponent<Vec3FieldHandler>();
            editor_ui.transform.Find("Info").gameObject.AddComponent<UnitInfoBox>();

            Draggable plt_drag = editor_ui.transform.Find("PltInfo/Header").gameObject.AddComponent<Draggable>();
            plt_drag.parent = editor_ui.transform.Find("PltInfo");
            CollapsibleButton plt_collapse = editor_ui.transform.Find("PltInfo/Header/Collapse").gameObject.AddComponent<CollapsibleButton>();
            plt_collapse.collapsible = editor_ui.transform.Find("PltInfo/Contents").gameObject;
            plt_collapse.collapse_icon = editor_ui.transform.Find("PltInfo/Header/Collapse/Text (TMP)").GetComponent<TextMeshProUGUI>();

            editor_ui.transform.Find("PltInfo").gameObject.AddComponent<PlatoonInfoBox>();

            TMP_Dropdown dropdown = editor_ui.transform.Find("Info/Contents/Dropdown").GetComponent<TMP_Dropdown>();
            var options = VicGameIdsEditor.ToList();
            dropdown.AddOptions(options);

            TMP_Dropdown dropdown_spawner = editor_ui.transform.Find("UnitSpawner/Panel/Dropdown").GetComponent<TMP_Dropdown>();
            dropdown_spawner.AddOptions(options);

            CollapsibleButton waypoints_collapse = editor_ui.transform.Find("WaypointsRoot/Waypoints/Header/Collapse").gameObject.AddComponent<CollapsibleButton>();
            waypoints_collapse.collapsible = editor_ui.transform.Find("WaypointsRoot/Waypoints/WaypointsList").gameObject;
            waypoints_collapse.collapse_icon = editor_ui.transform.Find("WaypointsRoot/Waypoints/Header/Collapse/Text (TMP)").GetComponent<TextMeshProUGUI>();
            Draggable waypoints_drag = editor_ui.transform.Find("WaypointsRoot/Waypoints/Header").gameObject.AddComponent<Draggable>();
            waypoints_drag.parent = editor_ui.transform.Find("WaypointsRoot");

            CollapsibleButton waypoints_info_collapse = editor_ui.transform.Find("WaypointsInfo/Header/Collapse").gameObject.AddComponent<CollapsibleButton>();
            waypoints_info_collapse.collapsible = editor_ui.transform.Find("WaypointsInfo/Contents").gameObject;
            waypoints_info_collapse.collapse_icon = editor_ui.transform.Find("WaypointsInfo/Header/Collapse/Text (TMP)").GetComponent<TextMeshProUGUI>();
            Draggable waypoints_info_drag = editor_ui.transform.Find("WaypointsInfo/Header").gameObject.AddComponent<Draggable>();
            waypoints_info_drag.parent = editor_ui.transform.Find("WaypointsInfo");

            editor_ui.transform.Find("WaypointsInfo").gameObject.AddComponent<WaypointGroupInfoBox>(); 
        }
    }
}