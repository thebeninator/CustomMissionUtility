using GHPC.Mission;
using UnityEngine;
using GHPC;
using GHPC.Vehicle;
using GHPC.Mission.Data;
using System.Collections.Generic;
using System;

namespace CustomMissionUtility
{
    public class CustomMissionData
    {
        /// <summary>
        /// The name of the mission as it appears in the mission select menu
        /// </summary>
        public string Name;

        /// <summary>
        /// Name of the unity scene WITHOUT the [CUSTOM] tag
        /// </summary>
        public string Id;

        /// <summary>
        /// Is the mission day or night by default?
        /// </summary>
        public float DefaultTime;

        /// <summary>
        /// Can the player play as NATO?
        /// </summary>
        public bool BluFor = false;

        /// <summary>
        /// Can the player play as Pact?
        /// </summary>
        public bool RedFor = false;

        /// <summary>
        /// Mission description when NATO is picked
        /// </summary>
        public string DescriptionBluFor;

        /// <summary>
        /// Mission description when Pact is picked
        /// </summary>
        public string DescriptionRedFor;

        /// <summary>
        /// Determines what map should be loaded
        /// </summary>
        public References.Theater Theater;
    }
}
