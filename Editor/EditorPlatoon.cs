using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomMissionUtility {
    [JsonObject(MemberSerialization.OptIn)]
    internal class AllPlatoons {
        public Dictionary<int, EditorPlatoon> Platoons = new Dictionary<int, EditorPlatoon>();

        [JsonProperty]
        public List<EditorPlatoon> PlatoonsSerialized = new List<EditorPlatoon>();
    }

    [JsonObject(MemberSerialization.OptIn)]
    internal class EditorPlatoon
    {
        public static int CurrentId = 0;

        public GameObject selectable;

        [JsonProperty]
        public int id;

        [JsonProperty]
        public string name;

        [JsonProperty]
        public List<EditorUnit> Units = new List<EditorUnit>();
    }
}
