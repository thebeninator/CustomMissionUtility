using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomMissionUtility
{
    internal class WaypointGroupInfoBox : MonoBehaviour
    {
        TMP_InputField name_field;
        TextMeshProUGUI waypoint_group_name;
        Transform waypoints_list;
        Button delete;
       
        void Awake() {
            name_field = transform.Find("Contents/InputField (TMP)").GetComponent<TMP_InputField>();
            waypoint_group_name = transform.Find("Contents/WaypointName").GetComponent<TextMeshProUGUI>();
            waypoints_list = transform.Find("Contents/WaypointsRoot/Waypoints/WaypointsList/Viewport/Content").GetComponent<Transform>();
            delete = transform.Find("Contents/Delete").GetComponent<Button>();

            name_field.onSubmit.AddListener(delegate (string s)
            {
                Editor.SELECTED_WAYPOINT_GROUPS[0].name = s;
                Editor.SELECTED_WAYPOINT_GROUPS[0].selectable.GetComponentInChildren<TextMeshProUGUI>().text = s;
                UpdateInfo();

                Editor.INFO_BOX.PopulateWaypointOptions();
                Editor.INFO_BOX.UpdateInfo();

                Editor.PLATOON_INFO_BOX.PopulateWaypointOptions();
                Editor.PLATOON_INFO_BOX.UpdateInfo();
            });

            delete.onClick.AddListener(delegate () 
            { 
                  
            });

            name_field.interactable = false;
            delete.interactable = false;
        }

        public void DestroySelectables()
        {
            waypoints_list.DetachChildren();
            foreach (Transform t in waypoints_list)
            {
                GameObject.Destroy(t.gameObject);
            }
        }

        public void UpdateInfo() {
            if (Editor.SELECTED_WAYPOINT_GROUPS.Count == 0) {
                DestroySelectables();
                waypoint_group_name.text = "No waypoint group selected";
                name_field.interactable = false;
                delete.interactable = false;
                return;
            }

            name_field.interactable = true;
            delete.interactable = true;

            DestroySelectables();
            EditorWaypointGroup waypoint_group = Editor.SELECTED_WAYPOINT_GROUPS[0];
            waypoint_group_name.text = waypoint_group.name + " (" + waypoint_group.id + ")";
            name_field.text = waypoint_group.name;

            foreach (EditorWaypoint waypoint in waypoint_group.waypoints) {
                waypoint.CreateSelectable(waypoints_list);
                waypoint.go_id.text = waypoint.selectable.transform.GetSiblingIndex().ToString();
                waypoint.go_group.text = waypoint_group.name;
            }
        }
    }
}
