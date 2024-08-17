using System.Collections;
using GHPC;
using System.Collections.Generic;
using GHPC.Mission;
using GHPC.State;
using GHPC.Vehicle;
using MelonLoader;

namespace CustomMissionUtility
{
    public class CustomMission
    {
        public Dictionary<UnitType, List<References.Vehicles>> SpawnOrdersBluFor = new Dictionary<UnitType, List<References.Vehicles>>
        {
            [UnitType.GroundVehicle] = new List<References.Vehicles>() { },
            [UnitType.AirVehicle] = new List<References.Vehicles>() { },
            [UnitType.Emplacement] = new List<References.Vehicles>() { },
            [UnitType.Infantry] = new List<References.Vehicles>() { }
        };

        public Dictionary<UnitType, List<References.Vehicles>> SpawnOrdersRedFor = new Dictionary<UnitType, List<References.Vehicles>>
        {
            [UnitType.GroundVehicle] = new List<References.Vehicles>() { },
            [UnitType.AirVehicle] = new List<References.Vehicles>() { },
            [UnitType.Emplacement] = new List<References.Vehicles>() { },
            [UnitType.Infantry] = new List<References.Vehicles>() { }
        };

        public virtual CustomMissionData MissionData { get; set; }

        /// <summary>
        /// Called when the mission has completely loaded; create units, platoons, etc. here
        /// </summary>
        public virtual void OnMissionFinishedLoading() {}

        /// <summary>
        /// Called when the mission has begun loading; load in custom assets here
        /// </summary>
        public virtual void OnMissionStartedLoading() {}

        public virtual void UnitSpawnOrders() { }

        public virtual IEnumerator MapMarkers(GameState _) { yield break; }

        public void CreateSpawnOrder(References.Vehicles id, Faction faction)
        {
            var spawn_orders = faction == Faction.Blue ? SpawnOrdersBluFor : SpawnOrdersRedFor;
            Vehicle vic = References.GetVehicle(id).GetComponent<Vehicle>();
            spawn_orders[vic.Type].Add(id);
        }

        public virtual void MissionData_() { }

    }
}
