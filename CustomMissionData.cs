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
        /// A value from 0.0 to 1.0 that affects the weather conditions
        /// 0.0 = always clear skies
        /// 1.0 = always cloudy/rainy
        /// </summary>
        public float CloudBias; 

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
