using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomMissionUtility
{
    public class Tools
    {
        public static GameObject SpawnVehicle(References.Vehicles id) {
            return GameObject.Instantiate(References.GetVehicle(id));
        }
    }
}
