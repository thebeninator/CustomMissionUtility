using UnityEngine;
using UnityEngine.UI;
using MelonLoader;
using TMPro;

namespace CustomMissionUtility
{
    internal class CollapsibleButton : MonoBehaviour
    {
        public GameObject collapsible;
        public TextMeshProUGUI collapse_icon;
        public Button button;

        public void Collapse() {
            collapsible.SetActive(!collapsible.activeSelf);
            collapse_icon.text = collapsible.activeSelf ? "v" : ">";
        }

        void Awake() { 
            button = GetComponent<Button>();

            button.onClick.AddListener(Collapse); 
        }
    }
}
