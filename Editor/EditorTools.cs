using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomMissionUtility
{
    internal class EditorTools
    {
        public static EditorPlatoon CreatePlatoon()
        {
            EditorPlatoon plt = new EditorPlatoon();
            plt.id = EditorPlatoon.CurrentId;
            plt.tag = "platoon " + plt.id;
            plt.Init();
            Editor.Platoons.Add(plt.id, plt);
            EditorPlatoon.CurrentId += 1;
            return plt;
        }

        public static GameObject CreateUnit(Vector3 pos, Vector3 rot)
        {
            GameObject unit = GameObject.Instantiate(Editor.unit_placeholder, Editor.ALL_UNITS_HOLDER.transform);
            unit.transform.position = pos;
            unit.transform.eulerAngles = rot;

            EditorUnit eu = unit.AddComponent<EditorUnit>();
            eu.vehicle = Editor.spawner_current_vehicle;
            eu.id = EditorUnit.CurrentId;
            eu.Init();
            Editor.Units.Add(eu.id, eu);
            EditorUnit.CurrentId += 1;
            return unit;
        }

        public static GameObject CreateUnit(Vector3 pos, Vector3 rot, int id)
        {
            GameObject unit = GameObject.Instantiate(Editor.unit_placeholder, Editor.ALL_UNITS_HOLDER.transform);
            unit.transform.position = pos;
            unit.transform.eulerAngles = rot;

            EditorUnit e = unit.AddComponent<EditorUnit>();
            e.id = id;
            e.Init();
            Editor.Units.Add(e.id, e);

            return unit;
        }

        public static void DeleteUnit(GameObject unit)
        {
            EditorUnit eu = unit.GetComponent<EditorUnit>();
            Editor.Units.Remove(eu.id);
            eu.Remove();
            GameObject.Destroy(unit);
            Editor.ClearUnitSelection();
        }

        void Teleport(Vector3 pos)
        {

        }
    }
}
