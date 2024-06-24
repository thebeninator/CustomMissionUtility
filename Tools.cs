using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Mission;
using GHPC.Vehicle;
using GHPC;
using UnityEngine;
using GHPC.AI.Platoons;
using MelonLoader;

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

        public static PlatoonData CreatePlatoon(string name, params Vehicle[] units) {
            GameObject platoon_go = new GameObject(name);

            PlatoonData data = platoon_go.AddComponent<PlatoonData>();

            data.Name = name;
            data.Units = ((Unit[])units).ToList();

            return data;
        }

        public static PlatoonData CreatePlatoon(string name)
        {
            return new PlatoonData();
        }

        public static Vehicle SpawnVehicle(References.Vehicles id, bool spawn_active = true) {
            GameObject vic = GameObject.Instantiate(References.GetVehicle(id));
            vic.SetActive(spawn_active);
            return vic.GetComponent<Vehicle>();
        }
    }
}
