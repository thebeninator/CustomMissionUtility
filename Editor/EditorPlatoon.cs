using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Vehicle;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomMissionUtility {
    [JsonObject(MemberSerialization.OptIn)]
    internal class EditorPlatoon
    {
        public static int CurrentId = 0;

        public GameObject selectable;

        [JsonProperty]
        public int id;

        [JsonProperty]
        public string tag; 

        [JsonProperty]
        public string name;

        [JsonProperty]
        public List<EditorUnit> Units = new List<EditorUnit>();

        public void Init()
        {
            selectable = GameObject.Instantiate(Editor.selectable);
            selectable.GetComponentInChildren<TextMeshProUGUI>().text = name + " (" + id + ")";
            selectable.transform.SetParent(Editor.EDITOR_UI.transform.Find("PltsRoot/Plts/PltsList/Viewport/Content"), false);
            selectable.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
            });
        }
    }
}
