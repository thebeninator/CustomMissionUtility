using MelonLoader;
using System;
using System.Linq;
using UnityEngine;
using Eflatun.SceneReference;
using GHPC.Mission.Data;
using GHPC.Mission;
using GHPC;
using System.Collections.Generic;
using MelonLoader.Utils;
using System.IO;
using System.Reflection;
using MelonLoader.Modules;

[assembly: MelonInfo(typeof(CustomMissionUtility.CustomMissionUtility), "Custom Mission Utility", "1.0.0", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]
[assembly: MelonPriority(-9999)]

namespace CustomMissionUtility
{
    public class CustomMissionUtility : MelonMod
    {
        private static AllMissionsMetaDataScriptable all_missions_metadata_so;
        private static List<CustomMission> all_custom_missions = new List<CustomMission>();
        private static Dictionary<string, int> custom_mission_lookup = new Dictionary<string, int>();
        private bool all_missions_loaded = false;
        internal static MissionSceneMeta MissionMeta;

        private void CreateMeta()
        {
            GameObject meta_obj = new GameObject("META");
            MissionMeta = meta_obj.AddComponent<MissionSceneMeta>();
            MissionMeta._startingUnits = new MissionSceneMeta.StartingUnitData[] { };
        }

        private void RegisterMission(Type type)
        {
            object mission = Activator.CreateInstance(type);
            custom_mission_lookup.Add("[CUSTOM]" + (mission as CustomMission).MissionData.Id, all_custom_missions.Count());
            all_custom_missions.Add(mission as CustomMission);
        }

        public override void OnEarlyInitializeMelon()
        {
            string custom_missions_path = Path.Combine(MelonEnvironment.ModsDirectory + "\\CustomMissions");
            string[] mission_folders_paths = Directory.GetDirectories(custom_missions_path);

            foreach (string mission_folder_path in mission_folders_paths)
            {
                string dll_path = Directory.GetFiles(mission_folder_path, "*.dll").First();

                foreach (string mission_bundle_path in Directory.GetFiles(mission_folder_path, "*.mission")) {
                    AssetBundle.LoadFromFile(mission_bundle_path);
                }

                Assembly dll = MelonAssembly.LoadMelonAssembly(dll_path).Assembly;
                Type[] types = dll.GetTypes();

                HarmonyInstance.PatchAll(dll);

                foreach (Type type in types)
                {
                    if (type.IsSubclassOf(typeof(CustomMission))) RegisterMission(type);                
                }
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (custom_mission_lookup.ContainsKey(sceneName)) { 
                CreateMeta();
                all_custom_missions[custom_mission_lookup[sceneName]].OnMissionStartedLoading();
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (!Util.menu_screens.Contains(sceneName))
            {
                References.GetVicReferences();
            }

            if (custom_mission_lookup.ContainsKey(sceneName))
            {
                all_custom_missions[custom_mission_lookup[sceneName]].OnMissionFinishedLoading();
            }

            if ((sceneName == "MainMenu2_Scene" || sceneName == "MainMenu2-1_Scene") && !all_missions_loaded) {                   
                if (all_missions_metadata_so == null) all_missions_metadata_so = Resources.FindObjectsOfTypeAll<AllMissionsMetaDataScriptable>().First();

                foreach (CustomMission custom_mission in all_custom_missions) {
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
