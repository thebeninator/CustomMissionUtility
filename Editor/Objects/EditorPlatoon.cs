using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.AI.Platoons;
using GHPC.Vehicle;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomMissionUtility 
{
    internal class PltSelectable : MonoBehaviour 
    {
        public EditorPlatoon epl; 
    }

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
        public string name = "New Platoon";

        [JsonProperty]
        public List<EditorUnit> Units = new List<EditorUnit>();

        [JsonProperty]
        public bool spawn_active = true;

        [JsonProperty]
        public FormationType formation = FormationType.None;

        [JsonProperty]
        public int waypoints = -1; 

        public void Init()
        {
            selectable = GameObject.Instantiate(Editor.selectable);
            PltSelectable plt = selectable.AddComponent<PltSelectable>();
            plt.epl = this;
            selectable.GetComponentInChildren<TextMeshProUGUI>().text = name + " (" + id + ")";
            selectable.transform.SetParent(Editor.EDITOR_UI.transform.Find("PltsRoot/Plts/PltsList/Viewport/Content"), false);
            selectable.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                if (Editor.SELECTED_PLATOONS.Count != 0 && Editor.SELECTED_PLATOONS[0] == this) {
                    Editor.SELECTED_PLATOONS.Clear();
                    Editor.PLATOON_INFO_BOX.UpdateInfo();
                    return;
                }

                Editor.SELECTED_PLATOONS.Clear();
                Editor.SELECTED_PLATOONS.Add(this);
                Editor.PLATOON_INFO_BOX.DestroySelectables();
                Editor.PLATOON_INFO_BOX.UpdateInfo();
            });
        }

        public void Remove()
        {
            if (id == CurrentId - 1)
            {
                CurrentId--;
            }

            GameObject.Destroy(selectable);
            //Editor.Platoons.Remove(id);

            foreach (EditorUnit eu in Units) {
                eu.RemoveFromPlatoon(add_back_to_units: true);
            }

            Units.Clear();

            Editor.SELECTED_PLATOONS.Clear();
            Editor.INFO_BOX.PopulatePlatoonOptions();
            Editor.INFO_BOX.UpdateInfo();
            Editor.PLATOON_INFO_BOX.UpdateInfo();
        }
    }
}
