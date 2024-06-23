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
        public virtual CustomMissionData MissionData { get; }

        public virtual void Setup() {}
    }
}
