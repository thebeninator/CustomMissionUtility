﻿using GHPC.Mission;
using UnityEngine;
using GHPC;
using GHPC.Vehicle;
using GHPC.Mission.Data;
using System.Collections.Generic;

namespace CustomMissionUtility
{
    public class CustomMission
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

        public List<FactionMissionInfo> FactionInfo;


        public MissionSceneMeta Meta;

        public void CreateMeta()
        {
            GameObject meta_obj = new GameObject("META");
            Meta = meta_obj.AddComponent<MissionSceneMeta>();
            Meta._startingUnits = new MissionSceneMeta.StartingUnitData[] { };
        }

        public void SetStartingUnit(Unit unit)
        {
            Meta._startingUnits = Util.AppendToArray(Meta._startingUnits,
                new MissionSceneMeta.StartingUnitData()
                {
                    Allegiance = ((Vehicle)unit).Allegiance,
                    Unit = unit
                }
            );
        }

        public void Initialize()
        {
            CreateMeta();
        }
    }
}
