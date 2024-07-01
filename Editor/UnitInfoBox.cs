using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CustomMissionUtility
{
    internal class UnitInfoBox : MonoBehaviour
    {
        Vec3FieldHandler position; 
        Vec3FieldHandler rotation;
        TextMeshProUGUI unit_name_text;
        TMP_Dropdown dropdown;

        void Awake() { 
            position = transform.Find("Position").GetComponent<Vec3FieldHandler>();
            rotation = transform.Find("Rotation").GetComponent<Vec3FieldHandler>();
            unit_name_text = transform.Find("UnitName").GetComponent<TextMeshProUGUI>();
            dropdown = transform.Find("Dropdown").GetComponent<TMP_Dropdown>();
        }

        void SetUnitName(string s) {
            unit_name_text.text = s; 
        } 

        public void UpdateInfo() {
            if (Editor.SELECTED_OBJECTS.Count == 0) {
                SetUnitName("No unit selected");
                position.Clear();
                rotation.Clear();
                return;
            }

            if (Editor.SELECTED_OBJECTS.Count > 1) {
                SetUnitName("Multiple units selected");
                position.Clear();
                rotation.Clear();
                return;
            }

            EditorUnit unit = Editor.SELECTED_OBJECTS[0].GetComponent<EditorUnit>();
            SetUnitName(References.GetVehicle(unit.vehicle).name + " (" + unit.id + ")");
            position.UpdateVec(Editor.SELECTED_OBJECTS[0].transform.position);
            rotation.UpdateVec(Editor.SELECTED_OBJECTS[0].transform.eulerAngles);
        }
    }
}