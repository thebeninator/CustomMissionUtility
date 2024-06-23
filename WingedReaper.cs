using System;
using CustomMissionUtility;
using UnityEngine;
using MelonLoader;

public class WingedReaper : CustomMission
{
    public override CustomMissionData MissionData
    {
        get {
            return new CustomMissionData()
            {
                Name = "Winged Reaper",
                Id = "ac130_mission_winged_reaper",
                DefaultTime = 125f,

                Theater = References.Theater.EasternHills,

                RedFor = false,
                BluFor = true,

                DescriptionBluFor = String.Join("\n\n",
                    "Situation: Reconnaissance aircraft have reported that a large enemy force is assembling in this region. You have been sent in to engage and destroy all enemy assets.",
                    "Mission: Destroy all enemy vehicles",
                    "Enemy: Unknown quantities of tanks, IFVs, APCs, and trucks",
                    "Friendly: 1x AC-130E",
                    "Gunship Loadout: 105mm howitzer M102, 40mm cannon L/60, 20mm rotary cannon M61"
                ),
            };
        }
    }

    public override void Setup() {
        MelonLogger.Msg("Hello!");
        GameObject m1ip = Tools.SpawnVehicle(References.Vehicles.M1IP);
        Tools.SetStartingUnit(m1ip);
    }
}