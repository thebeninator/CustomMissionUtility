using GHPC.Camera;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using MelonLoader.Utils;
using System.IO;
using TMPro;
using MelonLoader.TinyJSON;
using System.Security.Principal;
using MelonLoader;
using System.Linq;

namespace CustomMissionUtility
{
    internal class Editor : MonoBehaviour
    {
        public static GameObject editor_ui;
        public static GameObject unit_placeholder;
        public static GameObject unit_selectable;

        public static GameObject ALL_UNITS_HOLDER;
        public static GameObject EDITOR_UI;
        public static List<GameObject> SELECTED_OBJECTS;
        public static UnitInfoBox INFO_BOX;
        public static TMP_Dropdown DROPDOWN;

        public static string selected_type = "UNIT"; 

        [JsonProperty]
        public static AllWaypointGroups ALL_WAYPOINT_GROUPS;

        [JsonProperty]
        public static AllVehicles ALL_VEHICLES;

        void Update() {
            if (!CameraManager.Instance.CameraFollow.IsInFreeFlyMode) return;

            if (Input.GetMouseButtonDown(1)) {
                CameraManager.Instance.CameraFollow.enabled = !CameraManager.Instance.CameraFollow.enabled;
                if (!Cursor.visible)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                var cam_follow = CameraManager.Instance.CameraFollow;

                Ray ray = new Ray(cam_follow.BufferedCamera.transform.position, cam_follow.CurrentAimVector);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, 4000f))
                {
                    if (raycastHit.collider.gameObject.name.Contains("UNIT PLACEHOLDER"))
                    {
                        SingleUnitSelected(raycastHit.collider.gameObject);
                        return;
                    }
                }

                ClearUnitSelection();
            }

            if (Input.GetKeyDown(KeyCode.V)) {
                var cam_follow = CameraManager.Instance.CameraFollow;

                Ray ray = new Ray(cam_follow.BufferedCamera.transform.position, cam_follow.CurrentAimVector);
                RaycastHit raycastHit;

                if (Physics.Raycast(ray, out raycastHit, 4000f))
                {
                    if (raycastHit.collider.gameObject.name.Contains("UNIT PLACEHOLDER")) {
                        DeleteUnit(raycastHit.collider.gameObject);
                        return;
                    }

                    GameObject unit = CreateUnit(
                        raycastHit.point + new Vector3(0f, 1f, 0f), 
                        new Vector3(0f, cam_follow.transform.eulerAngles.y, 0f));

                    SingleUnitSelected(unit);                    
                }
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.H)) {
                Load();      
            }
        }
        void Awake()
        {
            EDITOR_UI = GameObject.Instantiate(editor_ui);
            ALL_UNITS_HOLDER = new GameObject("EDITOR UNITS");
            SELECTED_OBJECTS = new List<GameObject>() { };
            INFO_BOX = EDITOR_UI.GetComponentInChildren<UnitInfoBox>();
            ALL_WAYPOINT_GROUPS = new AllWaypointGroups();
            ALL_VEHICLES = new AllVehicles();
            DROPDOWN = EDITOR_UI.transform.Find("Info/Dropdown").GetComponent<TMP_Dropdown>();
        }

        GameObject CreateUnit(Vector3 pos, Vector3 rot)
        {
            GameObject unit = GameObject.Instantiate(unit_placeholder, ALL_UNITS_HOLDER.transform);
            unit.transform.position = pos;
            unit.transform.eulerAngles = rot;

            EditorUnit e = unit.AddComponent<EditorUnit>();
            e.id = EditorUnit.CurrentId;
            e.Init();
            ALL_VEHICLES.Units.Add(e.id, e);
            EditorUnit.CurrentId += 1; 
            return unit;
        }

        GameObject CreateUnit(Vector3 pos, Vector3 rot, int id)
        {
            GameObject unit = GameObject.Instantiate(unit_placeholder, ALL_UNITS_HOLDER.transform);
            unit.transform.position = pos;
            unit.transform.eulerAngles = rot;

            EditorUnit e = unit.AddComponent<EditorUnit>();
            e.id = id;
            e.Init();
            ALL_VEHICLES.Units.Add(e.id, e);

            return unit;
        }

        void DeleteUnit(GameObject unit) {
            EditorUnit eu = unit.GetComponent<EditorUnit>();
            ALL_VEHICLES.Units.Remove(eu.id);
            eu.Remove();
            GameObject.Destroy(unit);
            ClearUnitSelection();
        }

        void SingleUnitSelected(GameObject unit)
        {
            SELECTED_OBJECTS.Clear();
            SELECTED_OBJECTS.Add(unit);
            DROPDOWN.value = unit.GetComponent<EditorUnit>().id + 1;
            INFO_BOX.UpdateInfo();
        }

        void ClearUnitSelection()
        {
            SELECTED_OBJECTS.Clear();
            INFO_BOX.UpdateInfo();
            DROPDOWN.value = 0;
        }

        void Save() {
            ALL_VEHICLES.Serialize();

            string json = JsonConvert.SerializeObject(ALL_VEHICLES, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            File.WriteAllText(MelonEnvironment.ModsDirectory + "/WeaponDataFile.json", json);
        }

        void Load() {
            ClearUnitSelection();
            ALL_VEHICLES.Clear();

            string json = File.ReadAllText(MelonEnvironment.ModsDirectory + "/WeaponDataFile.json");
            AllVehicles c = JsonConvert.DeserializeObject<AllVehicles>(json);
            ALL_VEHICLES = c;
            foreach (EditorUnit eu in ALL_VEHICLES.UnitsSerialized)
            {
                CreateUnit((Vector3)eu.pos, (Vector3)eu.rot, eu.id);
            }

            EditorUnit.CurrentId = ALL_VEHICLES.UnitsSerialized.Last().id + 1; 
        }

        void Teleport(Vector3 pos) { 
            
        }

        public static void LoadAssets() {
            AssetBundle mission_creator_assets = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory, "CMUAssets"));
            unit_placeholder = mission_creator_assets.LoadAsset<GameObject>("UNIT PLACEHOLDER.prefab");
            unit_placeholder.hideFlags = HideFlags.DontUnloadUnusedAsset;

            editor_ui = mission_creator_assets.LoadAsset<GameObject>("EditorUI.prefab");
            editor_ui.hideFlags = HideFlags.DontUnloadUnusedAsset;

            unit_selectable = mission_creator_assets.LoadAsset<GameObject>("UnitSelectable.prefab");
            unit_selectable.hideFlags = HideFlags.DontUnloadUnusedAsset;

            CollapsibleButton waypoints_collapse = editor_ui.transform.Find("WaypointsRoot/Waypoints/Header/Collapse").gameObject.AddComponent<CollapsibleButton>();
            waypoints_collapse.collapsible = editor_ui.transform.Find("WaypointsRoot/Waypoints/WaypointsList").gameObject;
            waypoints_collapse.collapse_icon = editor_ui.transform.Find("WaypointsRoot/Waypoints/Header/Collapse/Text (TMP)").GetComponent<TextMeshProUGUI>();
            Draggable waypoints_drag = editor_ui.transform.Find("WaypointsRoot/Waypoints/Header").gameObject.AddComponent<Draggable>();
            waypoints_drag.parent = editor_ui.transform.Find("WaypointsRoot");

            CollapsibleButton units_collapse = editor_ui.transform.Find("UnitsRoot/Units/Header/Collapse").gameObject.AddComponent<CollapsibleButton>();
            units_collapse.collapsible = editor_ui.transform.Find("UnitsRoot/Units/UnitsList").gameObject;
            units_collapse.collapse_icon = editor_ui.transform.Find("UnitsRoot/Units/Header/Collapse/Text (TMP)").GetComponent<TextMeshProUGUI>();
            Draggable units_drag = editor_ui.transform.Find("UnitsRoot/Units/Header").gameObject.AddComponent<Draggable>();
            units_drag.parent = editor_ui.transform.Find("UnitsRoot");

            Draggable info_drag = editor_ui.transform.Find("Info/Panel").gameObject.AddComponent<Draggable>();
            info_drag.parent = editor_ui.transform.Find("Info");

            editor_ui.transform.Find("Info/Position").gameObject.AddComponent<Vec3FieldHandler>();
            editor_ui.transform.Find("Info/Rotation").gameObject.AddComponent<Vec3FieldHandler>();
            editor_ui.transform.Find("Info").gameObject.AddComponent<UnitInfoBox>();

            TMP_Dropdown dropdown = editor_ui.transform.Find("Info/Dropdown").GetComponent<TMP_Dropdown>();
            var options = References.VicGameIds.ToList();
            options.Insert(0, "");
            dropdown.AddOptions(options);
        }
    }
}
