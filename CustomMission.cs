using System.Collections;
using GHPC;
using System.Collections.Generic;
using GHPC.Mission;
using GHPC.State;
using GHPC.Vehicle;
using System.Linq;
using GHPC.Mission.Data;

namespace CustomMissionUtility
{
    public class CustomMission
    {
        public Dictionary<UnitClass, List<References.Vehicles>> SpawnOrdersBluFor = new Dictionary<UnitClass, List<References.Vehicles>>
        {
            [UnitClass.Tank] = new List<References.Vehicles>() { },
            [UnitClass.IFV] = new List<References.Vehicles>() { },
            [UnitClass.APC] = new List<References.Vehicles>() { },
            [UnitClass.Scout] = new List<References.Vehicles>() { },
            [UnitClass.Transport] = new List<References.Vehicles>() { },
            [UnitClass.FireSupport] = new List<References.Vehicles>() { },
            [UnitClass.Command] = new List<References.Vehicles>() { },
            [UnitClass.Troops] = new List<References.Vehicles>() { },
            [UnitClass.AntiTankTroops] = new List<References.Vehicles>() { },
            [UnitClass.AntiAirTroops] = new List<References.Vehicles>() { },
            [UnitClass.AntiTankEmplacement] = new List<References.Vehicles>() { },
            [UnitClass.MachineGunEmplacement] = new List<References.Vehicles>() { },
            [UnitClass.MobileArtillery] = new List<References.Vehicles>() { },
            [UnitClass.MobileAntiAir] = new List<References.Vehicles>() { },
            [UnitClass.ScoutHelicopter] = new List<References.Vehicles>() { },
            [UnitClass.AttackHelicopter] = new List<References.Vehicles>() { },
            [UnitClass.TransportHelicopter] = new List<References.Vehicles>() { },
        };

        public Dictionary<UnitClass, List<References.Vehicles>> SpawnOrdersRedFor = new Dictionary<UnitClass, List<References.Vehicles>>
        {
            [UnitClass.Tank] = new List<References.Vehicles>() { },
            [UnitClass.IFV] = new List<References.Vehicles>() { },
            [UnitClass.APC] = new List<References.Vehicles>() { },
            [UnitClass.Scout] = new List<References.Vehicles>() { },
            [UnitClass.Transport] = new List<References.Vehicles>() { },
            [UnitClass.FireSupport] = new List<References.Vehicles>() { },
            [UnitClass.Command] = new List<References.Vehicles>() { },
            [UnitClass.Troops] = new List<References.Vehicles>() { },
            [UnitClass.AntiTankTroops] = new List<References.Vehicles>() { },
            [UnitClass.AntiAirTroops] = new List<References.Vehicles>() { },
            [UnitClass.AntiTankEmplacement] = new List<References.Vehicles>() { },
            [UnitClass.MachineGunEmplacement] = new List<References.Vehicles>() { },
            [UnitClass.MobileArtillery] = new List<References.Vehicles>() { },
            [UnitClass.MobileAntiAir] = new List<References.Vehicles>() { },
            [UnitClass.ScoutHelicopter] = new List<References.Vehicles>() { },
            [UnitClass.AttackHelicopter] = new List<References.Vehicles>() { },
            [UnitClass.TransportHelicopter] = new List<References.Vehicles>() { },
        };

        internal Editor Json;

        public MissionMetaData MissionMetaData;

        public virtual CustomMissionData MissionData { get; set; }

        /// <summary>
        /// Called when the mission has completely loaded; create units, platoons, etc. here
        /// </summary>
        public virtual IEnumerator OnMissionFinishedLoading(GameState _) { yield break; }

        /// <summary>
        /// Called when the mission has begun loading; load in custom assets here
        /// </summary>
        public virtual void OnMissionStartedLoading() { }

        public virtual void UnitSpawnOrders() { }

        public virtual IEnumerator MapMarkers(GameState _) { yield break; }

        public void CreateSpawnOrder(References.Vehicles id, Faction faction)
        {
            var spawn_orders = faction == Faction.Blue ? SpawnOrdersBluFor : SpawnOrdersRedFor;
            string name = References.GetVehicle(id).GetComponent<Vehicle>().UniqueName;
            spawn_orders[CMU.unit_prefab_lookup.AllUnits.Where(o => o.Name == name).First().Class].Add(id);
        }

        public int GetSpawnOrder(References.Vehicles id, Faction faction) {
            var spawn_orders = faction == Faction.Blue ? SpawnOrdersBluFor : SpawnOrdersRedFor;
            string name = References.GetVehicle(id).GetComponent<Vehicle>().UniqueName;

            return spawn_orders[CMU.unit_prefab_lookup.AllUnits.Where(o => o.Name == name).First().Class].FindIndex(o => o == id);
        }

        public virtual void MissionData_() { }

    }
}
