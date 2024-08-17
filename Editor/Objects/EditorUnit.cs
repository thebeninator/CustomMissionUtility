﻿using System;
using System.Collections.Generic;
using System.Linq;
using GHPC;
using GHPC.Vehicle;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomMissionUtility
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class EditorUnit : MonoBehaviour
    {
        public static int CurrentId = 0;

        public GameObject selectable;

        public GameObject platoon_selectable; 

        public EditorPlatoon platoon;

        public Faction faction;

        public GameObject go;

        public TMP_Text go_text;

        [JsonProperty]
        public EditorWaypointGroup waypoints;

        [JsonProperty]
        public References.Vehicles vehicle = 0;

        [JsonProperty]
        public int id; 

        [JsonProperty]
        public Vec3Lite pos;

        [JsonProperty]
        public Vec3Lite rot;

        [JsonProperty]
        public bool spawn_active = true;

        public void CreateSelectable(Transform list, bool platoon = false) {
            GameObject s = GameObject.Instantiate(Editor.selectable);
            s.GetComponentInChildren<TextMeshProUGUI>().text = Editor.VicGameIdsEditor[(int)vehicle] + " (" + id + ")";
            s.transform.SetParent(list, false);
            s.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                if (Editor.SELECTED_UNITS.Count != 0 && Editor.SELECTED_UNITS[0] == gameObject)
                {
                    Editor.ClearUnitSelection();
                    return;
                }

                Editor.SingleUnitSelected(gameObject);
            });

            if (platoon)
            {
                platoon_selectable = s;
            }
            else {
                selectable = s;
            }
        }

        public void Init() {
            CreateSelectable(Editor.EDITOR_UI.transform.Find("UnitsRoot/Units/UnitsList/Viewport/Content"));
        }

        public void UpdateName() {
            string name = Editor.VicGameIdsEditor[(int)vehicle] + " (" + id + ")";
       
            selectable.GetComponentInChildren<TextMeshProUGUI>().text = name;
            if (platoon_selectable)
                platoon_selectable.GetComponentInChildren<TextMeshProUGUI>().text = name;

            go_text.text = name;
        }

        public void AddToPlatoon()
        {
            if (platoon == null) return;
            if (platoon.Units.Contains(this)) return;
            Editor.Units.Remove(id);
            platoon.Units.Add(this);
            Editor.PLATOON_INFO_BOX.UpdateInfo();
        }
        public void RemoveFromPlatoon(bool add_back_to_units = false)
        {
            if (platoon == null) return;
            if (add_back_to_units)
                Editor.Units.Add(id, this);
            platoon.Units.Remove(this);
            Editor.PLATOON_INFO_BOX.UpdateInfo();
        }

        public void Remove() {
            if (id == CurrentId - 1) {
                CurrentId--; 
            }

            GameObject.Destroy(selectable);
            if (platoon_selectable)
                GameObject.Destroy(platoon_selectable);

            GameObject.Destroy(transform.gameObject);

            RemoveFromPlatoon();
        }

        public void Serialize() {
            pos = (Vec3Lite)transform.position;
            rot = (Vec3Lite)transform.eulerAngles;
        }
    }
}