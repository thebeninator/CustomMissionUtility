using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.AI.Interfaces;
using GHPC.Camera;
using GHPC.Vehicle;
using MelonLoader;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CustomMissionUtility.References;

namespace CustomMissionUtility
{
    internal class UnitInfoBox : MonoBehaviour
    {
        public Vec3FieldHandler position;
        public Vec3FieldHandler rotation;
        TextMeshProUGUI unit_name_text;
        Toggle spawn_active;
        Button teleport;
        Button delete;
        public TMP_Dropdown dropdown;
        public TMP_Dropdown platoon;
        public TMP_Dropdown waypoints;

        public List<string> platoons_options = new List<string>() {""};
        public List<string> waypoints_options = new List<string>() {""};

        void Awake() { 
            position = transform.Find("Contents/Position").GetComponent<Vec3FieldHandler>();
            rotation = transform.Find("Contents/Rotation").GetComponent<Vec3FieldHandler>();
            unit_name_text = transform.Find("Contents/UnitName").GetComponent<TextMeshProUGUI>();
            dropdown = transform.Find("Contents/Dropdown").GetComponent<TMP_Dropdown>();
            teleport = transform.Find("Contents/Teleport").GetComponent<Button>();
            delete = transform.Find("Contents/Delete").GetComponent<Button>();
            platoon = transform.Find("Contents/DropdownPlt").GetComponent<TMP_Dropdown>();
            waypoints = transform.Find("Contents/DropdownWaypoints").GetComponent<TMP_Dropdown>();
            spawn_active = transform.Find("Contents/SpawnActive").GetComponent<Toggle>();

            dropdown.onValueChanged.AddListener(delegate(int i) {
                EditorUnit selected = Editor.SELECTED_UNITS[0].GetComponent<EditorUnit>();
                selected.vehicle = (References.Vehicles)i;
                selected.faction = References.GetVehicle(selected.vehicle).GetComponent<Vehicle>().Allegiance;
                selected.go_text.color = selected.faction == GHPC.Faction.Red ? Color.red : Color.blue;
                EditorTools.SwitchMat(selected.go.GetComponentInChildren<MeshRenderer>(), selected.faction == GHPC.Faction.Red ? Editor.mat_red : Editor.mat_blue, 0);
                selected.UpdateName();
                UpdateInfo();
                Editor.PLATOON_INFO_BOX.UpdateInfo();
            });

           delete.onClick.AddListener(delegate() {
                EditorTools.DeleteUnit(Editor.SELECTED_UNITS[0]);
            });

            teleport.onClick.AddListener(delegate () {
                EditorTools.Teleport();
            });

            spawn_active.onValueChanged.AddListener(delegate (bool b)
            {
                EditorUnit selected = Editor.SELECTED_UNITS[0].GetComponent<EditorUnit>();
                selected.spawn_active = b;
            });

            platoon.onValueChanged.AddListener(delegate (int i)
            {
                EditorUnit eu = Editor.SELECTED_UNITS[0].GetComponent<EditorUnit>();
                if (i == 0)
                {
                    eu.RemoveFromPlatoon(true);
                    eu.platoon = null;
                    Editor.PLATOON_INFO_BOX.UpdateInfo();
                    return;
                }
                eu.RemoveFromPlatoon();
                eu.platoon = Editor.PLATOONS_SELECTABLE_LIST.GetChild(i - 1).GetComponent<PltSelectable>().epl;
                eu.AddToPlatoon();
                Editor.PLATOON_INFO_BOX.UpdateInfo();
            });

            waypoints.onValueChanged.AddListener(delegate (int i)
            {
                EditorUnit eu = Editor.SELECTED_UNITS[0].GetComponent<EditorUnit>();

                if (i == 0) {
                    eu.waypoints = null;
                    return;
                }

                eu.waypoints = Editor.WAYPOINT_GROUPS_SELECTABLE_LIST.GetChild(i - 1).GetComponent<WPGSelectable>().group;
            });
        }

        void SetUnitName(string s) {
            unit_name_text.text = s; 
        }

        public void PopulatePlatoonOptions() {
            platoons_options = new List<string>() {""};

            foreach (EditorPlatoon plt in Editor.Platoons.Values)
            {
                platoons_options.Add(plt.name + " (" + plt.id + ")");
            }

            platoon.ClearOptions();
            platoon.AddOptions(platoons_options);

            Editor.SPAWNER_PLATOON_DROPDOWN.ClearOptions();
            Editor.SPAWNER_PLATOON_DROPDOWN.AddOptions(platoons_options);
        }

        public void PopulateWaypointOptions() { 
            waypoints_options = new List<string>() {""};

            foreach (EditorWaypointGroup group in Editor.WaypointGroups.Values) { 
                waypoints_options.Add(group.name + " (" + group.id + ")");
            }

            waypoints.ClearOptions();
            waypoints.AddOptions(waypoints_options);
        }

        public void SetActive(bool active) {
            dropdown.interactable = active;
            delete.interactable = active;
            teleport.interactable = active;
            platoon.interactable = active;
            spawn_active.interactable = active;
            waypoints.interactable = active;
            position.SetActive(active);
            rotation.SetActive(active);
        }

        public void UpdateInfo() {
            if (Editor.SELECTED_UNITS.Count == 0) {
                SetUnitName("No unit selected");
                SetActive(false);
                return;
            }

            if (Editor.SELECTED_UNITS.Count > 1) {
                SetUnitName("Multiple units selected");
                return;
            }

            SetActive(true);
            EditorUnit unit = Editor.SELECTED_UNITS[0].GetComponent<EditorUnit>();
            SetUnitName(Editor.VicGameIdsEditor[(int)unit.vehicle] + " (" + unit.id + ")");
            position.UpdateVec(Editor.SELECTED_UNITS[0].transform.position);
            rotation.UpdateVec(Editor.SELECTED_UNITS[0].transform.eulerAngles);
            spawn_active.isOn = unit.spawn_active;

            if (Editor.spawner_current_platoon != null)
            {
                Editor.SPAWNER_PLATOON_DROPDOWN.value = platoons_options.FindIndex(o => o == Editor.spawner_current_platoon.name + " (" + Editor.spawner_current_platoon.id + ")");
            }
            else {
                Editor.SPAWNER_PLATOON_DROPDOWN.value = 0;
            }

            if (unit.platoon != null)
            {
                platoon.value = platoons_options.FindIndex(o => o == unit.platoon.name + " (" + unit.platoon.id + ")");
            }
            else
            {
                platoon.value = 0;
            };

            if (unit.waypoints != null)
            {
                waypoints.value = waypoints_options.FindIndex(o => o == unit.waypoints.name + " (" + unit.waypoints.id + ")");
            }
            else
            {
                waypoints.value = 0;
            };
        }
    }
}