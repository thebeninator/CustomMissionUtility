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

        /// <summary>
        /// Called when the mission has completely loaded; create units, platoons, etc. here
        /// </summary>
        public virtual void OnMissionFinishedLoading() {}

        /// <summary>
        /// Called when the mission has begun loading; load in custom assets here
        /// </summary>
        public virtual void OnMissionStartedLoading() {}
    }
}
