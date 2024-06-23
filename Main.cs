using MelonLoader;
using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
using Eflatun.SceneReference;
using GHPC.Mission.Data;
using GHPC.Mission;
using GHPC;
using System.Collections.Generic;
using System.Security.Policy;

[assembly: MelonInfo(typeof(CustomMissionUtility.CustomMissionUtility), "Custom Mission Utility", "1.0.0", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace CustomMissionUtility
{
    public class CustomMissionUtility : MelonMod
    {
        private static AllMissionsMetaDataScriptable all_missions_metadata_so;
        private static List<CustomMission> all_custom_missions = new List<CustomMission>();
        private bool all_missions_loaded = false;


        public void RegisterMission(string id) {
            Type t = Type.GetType(id);
            object mission = System.Activator.CreateInstance(t);
            //t.GetMethod("yes").Invoke(mission, new object[] {});
            all_custom_missions.Add((CustomMission)mission);
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (!Util.menu_screens.Contains(sceneName))
            {
                References.GetVicReferences();
            }

            RegisterMission("WingedReaper");

            if ((sceneName == "MainMenu2_Scene" || sceneName == "MainMenu2-1_Scene") && !all_missions_loaded) {
                if (all_missions_metadata_so == null) all_missions_metadata_so = Resources.FindObjectsOfTypeAll<AllMissionsMetaDataScriptable>().First();

                foreach (CustomMission custom_mission in  all_custom_missions) {
                    CustomMissionData data = custom_mission.MissionData;

                    MissionTheaterScriptable theater = all_missions_metadata_so.Theaters[(int)data.Theater];

                    MissionMetaData mission_metadata = new MissionMetaData();
                    mission_metadata._isCategory = false;
                    mission_metadata.DefaultTime = data.DefaultTime;
                    mission_metadata.MissionName = data.Name;
                    mission_metadata.TimeOptions = new RandomEnvironment.EnvSettingOption[]
                    {
                        new RandomEnvironment.EnvSettingOption() {
                            Time = 125f,
                            RandomWeight = 1
                        }
                    };

                    mission_metadata.FactionInfo = new List<FactionMissionInfo>();
                    if (data.BluFor) mission_metadata.FactionInfo.Add(new FactionMissionInfo(Faction.Blue, data.DescriptionBluFor));
                    if (data.RedFor) mission_metadata.FactionInfo.Add(new FactionMissionInfo(Faction.Red, data.DescriptionRedFor));

                    mission_metadata.MissionSceneReference = new SceneReference();
                    mission_metadata.MissionSceneReference.sceneAssetGuidHex = "[CUSTOM]" + data.Id;

                    theater.Missions = Util.AppendToArray(theater.Missions, mission_metadata);
                }

                all_missions_loaded = true;
            }
        }
    }
}
