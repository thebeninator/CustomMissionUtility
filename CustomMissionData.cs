using System.Collections.Generic;
using GHPC;
using GHPC.Mission;

namespace CustomMissionUtility
{
    public class CustomMissionData
    {
        /// <summary>
        /// The name of the mission as it appears in the mission select menu
        /// </summary>
        public string Name;

        /// <summary>
        /// Name of the unity scene
        /// </summary>
        public string Id;

        /// <summary>
        /// The day/night time options for this mission 
        /// </summary>
        public RandomEnvironment.EnvSettingOption[] TimeOptions;

        /// <summary>
        /// A value from 0.0 to 1.0 that affects weather conditions
        /// 0.0 = always clear skies
        /// 1.0 = always cloudy/rainy
        /// </summary>
        public float CloudBias; 

        /// <summary>
        /// What factions can the player play as? First faction in the array will be the one that is initally selected
        /// "BluFor", "RedFor" 
        /// </summary>
        public Faction[] PlayableFactions;

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
