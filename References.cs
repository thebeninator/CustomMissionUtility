using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GHPC.Vehicle;

namespace CustomMissionUtility
{
    public static class References
    {
        public enum Theater {
            EasternHills, 
            Outskirts,
            NorthFields,
            PointAlpha,
            Grafenwoehr
        }

        public enum Vehicles { 
            BMP1_NVA,
            BMP1_Soviet,
            BMP1P_NVA,
            BMP1P_Soviet,
            BMP2_NVA,
            BMP2_Soviet,
            BRDM2_NVA,
            BRDM2_Soviet,
            BTR60_NVA,
            BTR60_Soviet,
            Static_9K111_NVA,
            Static_9K111_Soviet,
            Static_SPG9_NVA,
            Static_SPG9_Soviet,
            Ural_NVA, 
            Ural_Soviet,
            T62,
            T64R,
            T64A_1974,
            T64A_1979,
            T64A_1981,
            T64A_1983,
            T64A_1984,
            T64B,
            T80B,
            BTR70,
            GopnikCar,
            T55A,
            T72_Ural,
            T72_LEM,
            T72_LEM_Mod,
            T72_UV1,
            T72_UV2,
            T72M,
            T72M1,
            PT76B,
            T34,
            T54A,
            M1,
            M1IP,
            M60A1,
            M60A1_AOS,
            M60A1_RISE_Early,
            M60A1_RISE_Late,
            M60A3,
            M60A3_TTS,
            M2Bradley,
            M113,
            M923,
            Static_TOW,
            Mi24_Falanga,
            Mi24V_NVA,
            Mi24V_Soviet,
            Mi_8,
            Mi_2,
            OH_58A,
            AH_1,
            Count,
        }

        internal static string[] VicGameIds = new string[] {
            "BMP1",
            "BMP1 Soviet",
            "BMP1P (Variant)",
            "BMP1P (Variant) Soviet",
            "BMP2",
            "BMP2 Soviet",
            "BRDM2",
            "BRDM2 Soviet",
            "BTR60PB",
            "BTR60PB Soviet",
            "9K111",
            "9K111 Soviet",
            "SPG9",
            "SPG9 Soviet",
            "Ural",
            "Ural Soviet",
            "T62",
            "T64R",
            "T64A 1974",
            "T64A 1979",
            "T64A 1981",
            "T64A 1983",
            "T64A 1984",            
            "T64B",
            "T80B",
            "BTR70",
            "UAZ469",
            "T55A",
            "T72 Ural",
            "T72 Gill (Variant)",
            "T72 Ural LEM",
            "T72 UV1 (Variant)",
            "T72 UV2 Variant",
            "T72M",
            "T72M1",
            "PT76B",
            "T-34-85",
            "T54A",
            "M1",
            "M1IP",
            "M60A1",
            "M60A1 AOS",
            "M60A1 RISE Passive Early",
            "M60A1 RISE Passive Late",
            "M60A3",
            "M60A3 TTS",
            "M2 Bradley",
            "M113",
            "M923",
            "TOW",
            "Mi24",
            "Mi24V NVA",
            "Mi24V soviet",
            "Mi-8",
            "Mi-2",
            "OH-58A",
            "AH-1"
        };

        private static bool vics_done = false;
        private static Vehicle[] vehicles;
        private static Dictionary<int, GameObject> VicsLookup = new Dictionary<int, GameObject>() { };

        public static GameObject GetVehicle(Vehicles id)
        {
            return VicsLookup[(int)id];
        }

        private static GameObject FindVehicle(string id)
        {
            return vehicles.Where(v => v.name == id).FirstOrDefault().gameObject;
        }

        private static void AddVehicleRef(int ref_id, string game_id)
        {
            VicsLookup.Add(ref_id, FindVehicle(game_id));
        }

        internal static void GetVicReferences() {
            if (vics_done) return;

            vehicles = Resources.FindObjectsOfTypeAll<Vehicle>();

            for (int i = 0; i < (int)Vehicles.Count; i++) {
                AddVehicleRef(i, VicGameIds[i]);
            }

            vics_done = true;
        }
    }
}
