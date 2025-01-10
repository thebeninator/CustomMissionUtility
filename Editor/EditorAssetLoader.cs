using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader.Utils;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using CustomMissionUtility;

namespace CustomMissionUtility
{
    internal class EditorAssetLoader
    {
        public static void LoadAssets()
        {
            AssetBundle mission_creator_assets = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory, "CMUAssets"));
            Editor.unit_placeholder = mission_creator_assets.LoadAsset<GameObject>("UNIT RED.prefab");
            Editor.unit_placeholder.hideFlags = HideFlags.DontUnloadUnusedAsset;

            Editor.waypoint_placeholder = mission_creator_assets.LoadAsset<GameObject>("Waypoint.prefab");
            Editor.waypoint_placeholder.hideFlags = HideFlags.DontUnloadUnusedAsset;

            Editor.mat_blue = mission_creator_assets.LoadAsset<Material>("unit_blue.mat");
            Editor.mat_blue.hideFlags = HideFlags.DontUnloadUnusedAsset;

            Editor.mat_red = mission_creator_assets.LoadAsset<Material>("unit_red.mat");
            Editor.mat_red.hideFlags = HideFlags.DontUnloadUnusedAsset;

            Editor.mat_selected = mission_creator_assets.LoadAsset<Material>("unit_selected.mat");
            Editor.mat_selected.hideFlags = HideFlags.DontUnloadUnusedAsset;

            Editor.mat_waypoint_hi = mission_creator_assets.LoadAsset<Material>("waypoint_hi.mat");
            Editor.mat_waypoint_hi.hideFlags = HideFlags.DontUnloadUnusedAsset;

            Editor.mat_waypoint_default = mission_creator_assets.LoadAsset<Material>("waypoint_default.mat");
            Editor.mat_waypoint_default.hideFlags = HideFlags.DontUnloadUnusedAsset;

            Editor.mat_default = mission_creator_assets.LoadAsset<Material>("unit_scarecrow_albedo.mat");
            Editor.mat_default.hideFlags = HideFlags.DontUnloadUnusedAsset;

            Editor.editor_ui = mission_creator_assets.LoadAsset<GameObject>("EditorUI.prefab");
            Editor.editor_ui.hideFlags = HideFlags.DontUnloadUnusedAsset;

            Editor.selectable = mission_creator_assets.LoadAsset<GameObject>("Selectable.prefab");
            Editor.selectable.hideFlags = HideFlags.DontUnloadUnusedAsset;

            CollapsibleButton units_collapse = Editor.editor_ui.transform.Find("UnitsRoot/Units/Header/Collapse").gameObject.AddComponent<CollapsibleButton>();
            units_collapse.collapsible = Editor.editor_ui.transform.Find("UnitsRoot/Units/UnitsList").gameObject;
            units_collapse.collapse_icon = Editor.editor_ui.transform.Find("UnitsRoot/Units/Header/Collapse/Text (TMP)").GetComponent<TextMeshProUGUI>();
            Draggable units_drag = Editor.editor_ui.transform.Find("UnitsRoot/Units/Header").gameObject.AddComponent<Draggable>();
            units_drag.parent = Editor.editor_ui.transform.Find("UnitsRoot");

            CollapsibleButton plts_collapse = Editor.editor_ui.transform.Find("PltsRoot/Plts/Header/Collapse").gameObject.AddComponent<CollapsibleButton>();
            plts_collapse.collapsible = Editor.editor_ui.transform.Find("PltsRoot/Plts/PltsList").gameObject;
            plts_collapse.collapse_icon = Editor.editor_ui.transform.Find("PltsRoot/Plts/Header/Collapse/Text (TMP)").GetComponent<TextMeshProUGUI>();
            Draggable plts_drag = Editor.editor_ui.transform.Find("PltsRoot/Plts/Header").gameObject.AddComponent<Draggable>();
            plts_drag.parent = Editor.editor_ui.transform.Find("PltsRoot");

            Draggable info_drag = Editor.editor_ui.transform.Find("Info/Header").gameObject.AddComponent<Draggable>();
            info_drag.parent = Editor.editor_ui.transform.Find("Info");
            CollapsibleButton info_collapse = Editor.editor_ui.transform.Find("Info/Header/Collapse").gameObject.AddComponent<CollapsibleButton>();
            info_collapse.collapsible = Editor.editor_ui.transform.Find("Info/Contents").gameObject;
            info_collapse.collapse_icon = Editor.editor_ui.transform.Find("Info/Header/Collapse/Text (TMP)").GetComponent<TextMeshProUGUI>();

            Editor.editor_ui.transform.Find("Info/Contents/Position").gameObject.AddComponent<Vec3FieldHandler>();
            Editor.editor_ui.transform.Find("Info/Contents/Rotation").gameObject.AddComponent<Vec3FieldHandler>();
            Editor.editor_ui.transform.Find("Info").gameObject.AddComponent<UnitInfoBox>();

            Draggable plt_drag = Editor.editor_ui.transform.Find("PltInfo/Header").gameObject.AddComponent<Draggable>();
            plt_drag.parent = Editor.editor_ui.transform.Find("PltInfo");
            CollapsibleButton plt_collapse = Editor.editor_ui.transform.Find("PltInfo/Header/Collapse").gameObject.AddComponent<CollapsibleButton>();
            plt_collapse.collapsible = Editor.editor_ui.transform.Find("PltInfo/Contents").gameObject;
            plt_collapse.collapse_icon = Editor.editor_ui.transform.Find("PltInfo/Header/Collapse/Text (TMP)").GetComponent<TextMeshProUGUI>();

            Editor.editor_ui.transform.Find("PltInfo").gameObject.AddComponent<PlatoonInfoBox>();

            TMP_Dropdown dropdown = Editor.editor_ui.transform.Find("Info/Contents/Dropdown").GetComponent<TMP_Dropdown>();
            var options = Editor.VicGameIdsEditor.ToList();
            dropdown.AddOptions(options);

            TMP_Dropdown dropdown_spawner = Editor.editor_ui.transform.Find("UnitSpawner/Panel/Dropdown").GetComponent<TMP_Dropdown>();
            dropdown_spawner.AddOptions(options);

            CollapsibleButton waypoints_collapse = Editor.editor_ui.transform.Find("WaypointsRoot/Waypoints/Header/Collapse").gameObject.AddComponent<CollapsibleButton>();
            waypoints_collapse.collapsible = Editor.editor_ui.transform.Find("WaypointsRoot/Waypoints/WaypointsList").gameObject;
            waypoints_collapse.collapse_icon = Editor.editor_ui.transform.Find("WaypointsRoot/Waypoints/Header/Collapse/Text (TMP)").GetComponent<TextMeshProUGUI>();
            Draggable waypoints_drag = Editor.editor_ui.transform.Find("WaypointsRoot/Waypoints/Header").gameObject.AddComponent<Draggable>();
            waypoints_drag.parent = Editor.editor_ui.transform.Find("WaypointsRoot");

            CollapsibleButton waypoints_info_collapse = Editor.editor_ui.transform.Find("WaypointsInfo/Header/Collapse").gameObject.AddComponent<CollapsibleButton>();
            waypoints_info_collapse.collapsible = Editor.editor_ui.transform.Find("WaypointsInfo/Contents").gameObject;
            waypoints_info_collapse.collapse_icon = Editor.editor_ui.transform.Find("WaypointsInfo/Header/Collapse/Text (TMP)").GetComponent<TextMeshProUGUI>();
            Draggable waypoints_info_drag = Editor.editor_ui.transform.Find("WaypointsInfo/Header").gameObject.AddComponent<Draggable>();
            waypoints_info_drag.parent = Editor.editor_ui.transform.Find("WaypointsInfo");

            Editor.editor_ui.transform.Find("WaypointsInfo").gameObject.AddComponent<WaypointGroupInfoBox>();
        }
    }
}
