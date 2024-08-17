using System;
using System.Collections.Generic;
using GHPC.AI.Interfaces;
using GHPC.Vehicle;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomMissionUtility
{
    internal class WPGSelectable : MonoBehaviour
    {
        public EditorWaypointGroup group;
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class EditorWaypointGroup
    {
        public static int CurrentId = 0;

        public GameObject selectable; 

        [JsonProperty]
        public int id; 

        [JsonProperty]
        public string name = "New Waypoint Group";

        [JsonProperty]
        public List<EditorWaypoint> waypoints = new List<EditorWaypoint>();

        public void CreateSelectable(Transform list)
        {
            selectable = GameObject.Instantiate(Editor.selectable);
            selectable.GetComponentInChildren<TextMeshProUGUI>().text = name + " (" + id + ")";
            selectable.transform.SetParent(list, false);
            WPGSelectable s = selectable.AddComponent<WPGSelectable>();
            s.group = this;
            selectable.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                if (Editor.SELECTED_WAYPOINT_GROUPS.Count != 0 && Editor.SELECTED_WAYPOINT_GROUPS[0] == this)
                {
                    Editor.ClearWaypointGroupSelection();
                    return;
                }

                Editor.WaypointGroupSelected(this);
            });
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class EditorWaypoint : MonoBehaviour
    {
        public GameObject selectable;
        public TMP_Text go_id;
        public TMP_Text go_group;
        public GameObject go;
        public EditorWaypointGroup group;

        [JsonProperty]
        public Vec3Lite pos;

        public void Serialize()
        {
            pos = (Vec3Lite)transform.position;
        }

        public void CreateSelectable(Transform list) {
            selectable = GameObject.Instantiate(Editor.selectable);
            selectable.transform.SetParent(list, false);
            selectable.GetComponentInChildren<TextMeshProUGUI>().text = selectable.transform.GetSiblingIndex().ToString();
        }
    }
}