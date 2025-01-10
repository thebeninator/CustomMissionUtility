using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.AI.Interfaces;
using GHPC.AI.Platoons;
using GHPC.Effects;
using MelonLoader;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomMissionUtility
{
    internal class PlatoonInfoBox : MonoBehaviour
    {
        public static List<string> formations = new List<string>() {
            "None",
            "Column",
            "Line",
            "Diamond",
            "Wedge",
            "SweptRight",
            "SweptLeft",
            "Vee"
        };

        TextMeshProUGUI platoon_name_text;
        TMP_InputField name_field;
        TMP_Dropdown formation_dropdown;
        TMP_Dropdown waypoints_dropdown;
        Transform units_list; 
        Button delete;
        Toggle spawn_active;

        public List<string> waypoints_options = new List<string>() { "" };

        void Start() {
            platoon_name_text = transform.Find("Contents/PltName").GetComponent<TextMeshProUGUI>();
            name_field = transform.Find("Contents/InputField (TMP)").GetComponent<TMP_InputField>();
            formation_dropdown = transform.Find("Contents/DropdownFormation").GetComponent<TMP_Dropdown>();
            waypoints_dropdown = transform.Find("Contents/DropdownWaypoints").GetComponent<TMP_Dropdown>();
            delete = transform.Find("Contents/Delete").GetComponent<Button>();
            spawn_active = transform.Find("Contents/SpawnActive").GetComponent<Toggle>();
            units_list = transform.Find("Contents/UnitsRoot/Units/UnitsList/Viewport/Content").GetComponent<RectTransform>();

            name_field.onSubmit.AddListener(delegate (string s)
            {
                Editor.SELECTED_PLATOONS[0].name = s;
                Editor.INFO_BOX.PopulatePlatoonOptions();
                Editor.INFO_BOX.UpdateInfo();
                UpdateInfo();
            });

            delete.onClick.AddListener(delegate ()
            {
                Editor.Platoons.Remove(Editor.SELECTED_PLATOONS[0].id);
                Editor.SELECTED_PLATOONS[0].Remove();
            });

            formation_dropdown.onValueChanged.AddListener(delegate (int i)
            {
                Editor.SELECTED_PLATOONS[0].formation = (FormationType)i;
                UpdateInfo();
            });

            waypoints_dropdown.onValueChanged.AddListener(delegate (int i)
            {
                EditorPlatoon plt = Editor.SELECTED_PLATOONS[0];

                if (i == 0)
                {
                    plt.waypoints = -1;
                    return;
                }

                plt.waypoints = Editor.WAYPOINT_GROUPS_SELECTABLE_LIST.GetChild(i - 1).GetComponent<WPGSelectable>().group.id;
            });

            name_field.interactable = false;
            delete.interactable = false;
            spawn_active.interactable = false;
            formation_dropdown.interactable = false;
            waypoints_dropdown.interactable = false;
            formation_dropdown.AddOptions(formations);
        }

        void SetPlatoonName(string s)
        {
            platoon_name_text.text = s;
        }

        public void DestroySelectables() {
            foreach (Transform t in units_list) {
                GameObject.Destroy(t.gameObject);
            }
        }

        public void PopulateWaypointOptions()
        {
            waypoints_options = new List<string>() { "" };

            foreach (EditorWaypointGroup group in Editor.WaypointGroups.Values)
            {
                waypoints_options.Add(group.name + " (" + group.id + ")");
            }

            waypoints_dropdown.ClearOptions();
            waypoints_dropdown.AddOptions(waypoints_options);
        }

        public void UpdateInfo()
        {
            if (Editor.SELECTED_PLATOONS.Count == 0)
            {
                DestroySelectables();
                SetPlatoonName("No platoon selected");
                name_field.text = "";
                name_field.interactable = false;
                delete.interactable = false;
                spawn_active.interactable = false;
                formation_dropdown.interactable = false;
                waypoints_dropdown.interactable = false;

                return;
            }

            if (Editor.SELECTED_PLATOONS.Count > 1)
            {
                SetPlatoonName("Multiple platoons selected");
                name_field.text = "";
                return;
            }

            name_field.interactable = true;
            delete.interactable = true;
            spawn_active.interactable = true;
            formation_dropdown.interactable = true;
            waypoints_dropdown.interactable = true;

            EditorPlatoon platoon = Editor.SELECTED_PLATOONS[0];
            spawn_active.isOn = platoon.spawn_active;
            SetPlatoonName(platoon.name + " (" + platoon.id + ")");
            name_field.text = platoon.name;
            formation_dropdown.value = (int)platoon.formation;
            platoon.selectable.GetComponentInChildren<TextMeshProUGUI>().text = platoon.name + " (" + platoon.id + ")";

            DestroySelectables();

            foreach (EditorUnit unit in platoon.Units)
            {
                unit.CreateSelectable(units_list, true);
            }

            if (platoon.waypoints != -1)
            {
                waypoints_dropdown.value = waypoints_options.FindIndex(o => o == Editor.WaypointGroups[platoon.waypoints].name + " (" + platoon.waypoints + ")");
            }
            else
            {
                waypoints_dropdown.value = 0;
            };
        }
    }
}
