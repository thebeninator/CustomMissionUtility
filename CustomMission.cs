using System.Collections;
using GHPC.State;

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

        public virtual IEnumerator MapMarkers(GameState _) { yield break; }
    }
}
