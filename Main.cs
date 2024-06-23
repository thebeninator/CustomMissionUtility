using MelonLoader;
using System;
using System.Linq;
using UnityEngine;

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
        }
    }
}
