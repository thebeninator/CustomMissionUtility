using MelonLoader;
using System;
using System.Linq;

namespace CustomMissionUtility
{
    public class Main : MelonMod
    {
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (!Util.menu_screens.Contains(sceneName))
            {
                References.GetVicReferences();
            }

            CustomMission t = new CustomMission() {
                Name = "Winged Reaper",
                Id = "ac130_mission_winged_reaper",
                DefaultTime = 125f,

                Theater = References.Theater.EasternHills,

                RedFor = false, 
                BluFor = true,

                DescriptionBluFor = String.Join("\n\n",
                    "Situation: Reconnaissance aircraft have reported that a large enemy force is assembling in this region. You have been diverted to engage and destroy all enemy assets.",
                    "Mission: Destroy all enemy vehicles",
                    "Enemy: Unknown quantities of tanks, IFVs, APCs, and trucks",
                    "Friendly: 1x AC-130E",
                    "Gunship Loadout: 105mm howitzer M102, 40mm cannon L/60, 20mm rotary cannon M61")

            };
        }
    }
}
