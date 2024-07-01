using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomMissionUtility
{
    internal class AllWaypointGroups
    {
        public List<EditorWaypointGroup> WaypointGroups;
    }

    internal class EditorWaypointGroup
    {
        public string GroupName;
        public List<EditorWaypoint> Waypoints;
    }

    internal class EditorWaypoint
    {
        public Vec3Lite position;
    }
}