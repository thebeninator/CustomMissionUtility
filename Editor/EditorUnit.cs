using System;
using System.Collections.Generic;
using System.Linq;
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

        [JsonProperty]
        public References.Vehicles vehicle = 0;

        [JsonProperty]
        public int id; 

        [JsonProperty]
        public Vec3Lite pos;

        [JsonProperty]
        public Vec3Lite rot;

        public void Init() {
            selectable = GameObject.Instantiate(Editor.selectable);
            selectable.GetComponentInChildren<TextMeshProUGUI>().text = Editor.VicGameIdsEditor[(int)vehicle] + " (" + id + ")";
            selectable.transform.SetParent(Editor.EDITOR_UI.transform.Find("UnitsRoot/Units/UnitsList/Viewport/Content"), false);
            selectable.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                Editor.SingleUnitSelected(gameObject);
            });
        }

        public void UpdateName() {
            selectable.GetComponentInChildren<TextMeshProUGUI>().text = Editor.VicGameIdsEditor[(int)vehicle] + " (" + id + ")";
        }

        public void Remove() {
            if (id == CurrentId - 1) {
                CurrentId--; 
            }

            GameObject.Destroy(selectable);
            GameObject.Destroy(transform.gameObject);
        }

        public void Serialize() {
            pos = (Vec3Lite)transform.position;
            rot = (Vec3Lite)transform.eulerAngles;
        }
    }
}