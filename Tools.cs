using System.Linq;
using GHPC.Mission;
using GHPC.Vehicle;
using GHPC;
using UnityEngine;
using GHPC.AI.Platoons;
using GHPC.AI;
using System.Collections.Generic;
using GHPC.AI.Interfaces;

namespace CustomMissionUtility
{
    public class Tools
    {
        public static Vector3 StrToVec3(string vector) {
            string[] locs = vector.Split(' ');
            return new Vector3(float.Parse(locs[0]), float.Parse(locs[1]), float.Parse(locs[2]));
        }

        public static WaypointHolder CreateWaypoints(string name, params Vector3[] waypoints) {
            GameObject waypoint_holder_go = new GameObject(name);

            for (int i = 0; i < waypoints.Length; i++) {
                GameObject waypoint = new GameObject(i.ToString());
                TransformWaypoint t_waypoint = waypoint.AddComponent<TransformWaypoint>();
                waypoint.transform.parent = waypoint_holder_go.transform;
                waypoint.transform.position = waypoints[i];
            }

            WaypointHolder waypoint_holder = waypoint_holder_go.AddComponent<WaypointHolder>();

            return waypoint_holder;
        }

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
        /// Create a new platoon
        /// The first unit will become the platoon leader
        /// </summary>
        public static PlatoonData CreatePlatoon(string name, params Vehicle[] units) {
            GameObject platoon_go = new GameObject(name);

            PlatoonData data = platoon_go.AddComponent<PlatoonData>();

            data.Name = name;
            data.Units = ((Unit[])units).ToList();
            foreach (Vehicle unit in units) {
                unit.transform.parent = platoon_go.transform;
            }

            return data;
        }

        /// <summary>
        /// Create a new empty platoon (a group of vehicles with a leader)
        /// </summary>
        public static PlatoonData CreatePlatoon(string name)
        {
            return new PlatoonData();
        }

        public static Vehicle SpawnVehicle(References.Vehicles id, Vector3 position, Vector3 rotation, bool spawn_active = true, Faction faction = Faction.Neutral, bool override_faction = false) {
            GameObject vic = GameObject.Instantiate(References.GetVehicle(id));
            vic.transform.position = position;
            vic.transform.localEulerAngles = rotation;
            vic.SetActive(spawn_active);

            if (override_faction)
                SetVehicleFaction(vic.GetComponent<Vehicle>(), faction);

            return vic.GetComponent<Vehicle>();
        }

        public static void SetVehicleFaction(Unit vehicle, Faction faction)
        {
            vehicle.Allegiance = faction;
        }
    }
}
