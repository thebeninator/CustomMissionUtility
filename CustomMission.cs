using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Mission;
using GHPC.Vehicle;
using GHPC;
using UnityEngine;

namespace CustomMissionUtility
{
    public class CustomMission
    {
        public CustomMissionData MissionData;

        public static bool SetupDone = false;

        public void OnLoad() {}

        public void CreateMeta()
        {
            GameObject meta_obj = new GameObject("META");
            MissionData.Meta = meta_obj.AddComponent<MissionSceneMeta>();
            MissionData.Meta._startingUnits = new MissionSceneMeta.StartingUnitData[] { };
        }

        public void SetStartingUnit(Unit unit)
        {
            MissionData.Meta._startingUnits = Util.AppendToArray(MissionData.Meta._startingUnits,
                new MissionSceneMeta.StartingUnitData()
                {
                    Allegiance = ((Vehicle)unit).Allegiance,
                    Unit = unit
                }
            );
        }

        public void SetStartingUnit(GameObject unit)
        {
            SetStartingUnit(unit.GetComponent<Unit>());
        }

        public void Load()
        {
            CreateMeta();
            OnLoad();
        }
    }
}
