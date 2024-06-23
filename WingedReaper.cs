using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomMissionUtility;
using UnityEngine;
using MelonLoader;

public class WingedReaper : CustomMission
{
    public new CustomMissionData MissionData = new CustomMissionData()
    {
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
        "Gunship Loadout: 105mm howitzer M102, 40mm cannon L/60, 20mm rotary cannon M61"),
    };

    public new void OnLoad() {
        GameObject m1ip = Tools.SpawnVehicle(References.Vehicles.M1IP);
        SetStartingUnit(m1ip);
    }

    public void yes() {
        MelonLogger.Msg("yes");
    }
}