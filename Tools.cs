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
using GHPC.Weapons;

namespace CustomMissionUtility
{
    public class Tools
    {
        /// <summary>
        /// Tells the game what vehicle the player should start in for a given faction
        /// </summary>
        public static void SetStartingUnit(Unit unit, Faction faction)
        {
            CustomMissionUtility.MissionMeta._startingUnits = Util.AppendToArray(CustomMissionUtility.MissionMeta._startingUnits,
                new MissionSceneMeta.StartingUnitData()
                {
                    Allegiance = faction,
                    Unit = unit
                }
            );
        }

        /// <summary>
        /// Create a new platoon (a group of vehicles with a leader)
        /// </summary>
        public static PlatoonData CreatePlatoon(string name, params Vehicle[] units) {
            GameObject platoon_go = new GameObject(name);

            PlatoonData data = platoon_go.AddComponent<PlatoonData>();

            data.Name = name;
            data.Units = ((Unit[])units).ToList();

            return data;
        }

        /// <summary>
        /// Create a new empty platoon (a group of vehicles with a leader)
        /// </summary>
        public static PlatoonData CreatePlatoon(string name)
        {
            return new PlatoonData();
        }

        public static Vehicle SpawnVehicle(References.Vehicles id, Vector3 position, Vector3 rotation, bool spawn_active = true) {
            GameObject vic = GameObject.Instantiate(References.GetVehicle(id));
            vic.transform.position = position;
            vic.transform.localEulerAngles = rotation;
            vic.SetActive(spawn_active);
            return vic.GetComponent<Vehicle>();
        }
    }
}
