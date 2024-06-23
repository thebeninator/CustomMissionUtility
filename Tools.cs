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
    public class Tools
    {
        public static void SetStartingUnit(Unit unit)
        {
            CustomMissionUtility.MissionMeta._startingUnits = Util.AppendToArray(CustomMissionUtility.MissionMeta._startingUnits,
                new MissionSceneMeta.StartingUnitData()
                {
                    Allegiance = ((Vehicle)unit).Allegiance,
                    Unit = unit
                }
            );
        }

        public static void SetStartingUnit(GameObject unit)
        {
            SetStartingUnit(unit.GetComponent<Unit>());
        }

        public static GameObject SpawnVehicle(References.Vehicles id) {
            return GameObject.Instantiate(References.GetVehicle(id));
        }
    }
}
