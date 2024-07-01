﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

namespace CustomMissionUtility
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AllVehicles
    {
        public Dictionary<int, EditorUnit> Units = new Dictionary<int, EditorUnit>();

        [JsonProperty]
        public List<EditorUnit> UnitsSerialized = new List<EditorUnit>();

        public void Serialize() {
            UnitsSerialized = Units.Select(u => u.Value).ToList();
            foreach(EditorUnit u in UnitsSerialized)
            {
                u.Serialize();
            }
        }

        public void Clear() {
            foreach (EditorUnit eu in Units.Values) {
                eu.Remove();
            }

            Units.Clear();
            UnitsSerialized.Clear();
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class EditorUnit : MonoBehaviour
    {
        public static int CurrentId = 0;

        public GameObject selectable;

        [JsonProperty]
        public References.Vehicles vehicle = 0;

        [JsonProperty]
        public int id; 

        [JsonProperty]
        public Vec3Lite pos;

        [JsonProperty]
        public Vec3Lite rot;

        public void Init() {
            selectable = GameObject.Instantiate(Editor.unit_selectable);
            selectable.GetComponentInChildren<TextMeshProUGUI>().text = References.GetVehicle(vehicle).name + " (" + id + ")";
            selectable.transform.SetParent(Editor.EDITOR_UI.transform.Find("UnitsRoot/Units/UnitsList/Viewport/Content"), false);
        }

        public void Remove() {
            if (id == CurrentId - 1) {
                CurrentId--; 
            }

            GameObject.Destroy(selectable);
            GameObject.Destroy(transform.gameObject);
        }

        public void Serialize() { 
            pos = new Vec3Lite(transform.position.x, transform.position.y, transform.position.z);
            rot = new Vec3Lite(transform.eulerAngles.x,  transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }
}