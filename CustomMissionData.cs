namespace CustomMissionUtility
{
    public class CustomMissionData
    {
        /// <summary>
        /// The name of the mission as it appears in the mission select menu
        /// </summary>
        public string Name;

        /// <summary>
        /// Name of the unity scene AND asset bundle WITHOUT the [CUSTOM] tag
        /// </summary>
        public string Id;

        /// <summary>
        /// Name of the folder that this mission is in 
        /// </summary>
        public string FolderName;

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
