using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GHPC.Camera;
using GHPC.Vehicle;
using MelonLoader;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomMissionUtility
{
    internal class EditorTools
    {
        public static EditorPlatoon CreatePlatoon()
        {
            EditorPlatoon plt = new EditorPlatoon();
            plt.id = EditorPlatoon.CurrentId;
            plt.tag = "platoon " + plt.id;
            plt.Init();
            Editor.Platoons.Add(plt.id, plt);
            EditorPlatoon.CurrentId += 1;
            return plt;
        }

        public static EditorWaypoint CreateWaypoint(Vector3 pos, Vector3 rot) {
            if (Editor.SELECTED_WAYPOINT_GROUPS.Count == 0) return null;

            GameObject waypoint = GameObject.Instantiate(Editor.waypoint_placeholder);
            waypoint.transform.position = pos;
            waypoint.transform.eulerAngles = rot;

            EditorWaypoint wp = waypoint.AddComponent<EditorWaypoint>();
            wp.group = Editor.SELECTED_WAYPOINT_GROUPS[0];
            wp.go = waypoint;
            wp.go_id = waypoint.transform.Find("Id").GetComponent<TMP_Text>();
            wp.go_group = waypoint.transform.Find("Group").GetComponent<TMP_Text>();
            SwitchMat(waypoint.GetComponent<MeshRenderer>(), Editor.mat_waypoint_hi, 0);

            Editor.SELECTED_WAYPOINT_GROUPS[0].waypoints.Add(wp);
            Editor.WAYPOINT_GROUP_INFO_BOX.UpdateInfo();

            return wp;
        }

        public static void DeleteWaypoint(GameObject waypoint) {
            if (Editor.SELECTED_WAYPOINT_GROUPS.Count == 0) return;

            EditorWaypoint wp = waypoint.GetComponent<EditorWaypoint>();

            if (wp.group != Editor.SELECTED_WAYPOINT_GROUPS[0]) return;

            wp.group.waypoints.Remove(wp);
            Editor.WAYPOINT_GROUP_INFO_BOX.UpdateInfo();
            GameObject.Destroy(waypoint.gameObject);
        }

        public static EditorWaypointGroup CreateWaypointGroup()
        {
            EditorWaypointGroup wpg = new EditorWaypointGroup();
            wpg.id = EditorWaypointGroup.CurrentId;
            wpg.CreateSelectable(Editor.EDITOR_UI.transform.Find("WaypointsRoot/Waypoints/WaypointsList/Viewport/Content"));
            Editor.WaypointGroups.Add(EditorWaypointGroup.CurrentId, wpg);
            EditorWaypointGroup.CurrentId += 1;

            Editor.INFO_BOX.PopulateWaypointOptions();
            Editor.PLATOON_INFO_BOX.PopulateWaypointOptions();

            return wpg;
        }

        public static GameObject CreateUnit(Vector3 pos, Vector3 rot)
        {
            GameObject unit = GameObject.Instantiate(Editor.unit_placeholder, Editor.ALL_UNITS_HOLDER.transform);
            unit.transform.position = pos;
            unit.transform.eulerAngles = rot;

            EditorUnit eu = unit.AddComponent<EditorUnit>();
            eu.vehicle = Editor.spawner_current_vehicle;
            eu.id = EditorUnit.CurrentId;
            eu.platoon = Editor.spawner_current_platoon;
            eu.faction = Editor.unit_spawner_faction;
            eu.go = unit;
            eu.go_text = unit.GetComponentInChildren<TMP_Text>();
            eu.go_text.text = Editor.VicGameIdsEditor[(int)eu.vehicle] + " (" + eu.id + ")";
            eu.Init();

            if (eu.faction == GHPC.Faction.Blue) {
                SwitchMat(unit.GetComponentInChildren<MeshRenderer>(), Editor.mat_blue, 0);
                eu.go_text.color = Color.blue; 
            }

            if (eu.faction != References.GetVehicle(eu.vehicle).GetComponent<Vehicle>().Allegiance) { 
                eu.has_faction_override = true;
                eu.faction_override = eu.faction;
            }

            Editor.Units.Add(eu.id, eu);
            EditorUnit.CurrentId += 1;
            eu.AddToPlatoon();
            return unit;
        }

        public static GameObject CreateUnit(EditorUnit eu, EditorPlatoon plt = null)
        {
            GameObject unit = GameObject.Instantiate(Editor.unit_placeholder, Editor.ALL_UNITS_HOLDER.transform);
            unit.transform.position = (Vector3)eu.pos;
            unit.transform.eulerAngles = (Vector3)eu.rot;

            EditorUnit e = unit.AddComponent<EditorUnit>();
            e.id = eu.id;
            e.starting_unit = eu.starting_unit;
            e.playable_unit = eu.playable_unit;
            e.vehicle = eu.vehicle;
            e.faction = eu.faction;
            e.waypoints = eu.waypoints;
            e.go = unit;
            e.go_text = unit.GetComponentInChildren<TMP_Text>();
            e.go_text.text = Editor.VicGameIdsEditor[(int)e.vehicle] + " (" + e.id + ")";

            if (plt != null)
                e.platoon = plt;
            e.Init();

            if (e.faction == GHPC.Faction.Blue)
            {
                SwitchMat(unit.GetComponentInChildren<MeshRenderer>(), Editor.mat_blue, 0);
                e.go_text.color = Color.blue;
            }

            if (e.faction != References.GetVehicle(e.vehicle).GetComponent<Vehicle>().Allegiance)
            {
                e.has_faction_override = true;
                e.faction_override = e.faction;
            }

            Editor.Units.Add(e.id, e);
            if (plt != null)
                e.AddToPlatoon();

            return unit;
        }

        public static void DeleteUnit(GameObject unit)
        {
            EditorUnit eu = unit.GetComponent<EditorUnit>();
            eu.RemoveFromPlatoon(force_remove: true);
            eu.Remove();
            Editor.Units.Remove(eu.id);
            GameObject.Destroy(unit);
            Editor.ClearUnitSelection();
        }

        public static void Teleport()
        {
            CameraManager.Instance.CameraFollow.transform.position = Editor.SELECTED_UNITS[0].transform.position + new Vector3(0f, 12f, 0f);
            CameraManager._mainCamera.transform.eulerAngles = new Vector3(89f, Editor.SELECTED_UNITS[0].transform.eulerAngles.y, 0f);
        }

        public static void SwitchMat(MeshRenderer mesh_rend, Material mat, int idx) {
            MelonLogger.Msg("called: " + idx + " " + mat.name);

            Material[] new_mats = mesh_rend.materials;
            new_mats[idx] = mat;
            mesh_rend.materials = new_mats;
        }
    }
}
