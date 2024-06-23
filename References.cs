using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GHPC.Vehicle;

namespace CustomMissionUtility
{
    public class References
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
            Ural_NVA, 
            Ural_Soviet,
            T62,
            T64A,
            T64B,
            T80B,
            BTR70,
            GopnikCar,
            T55A,
            T72M,
            T72M_Gills,
            T72M1,
            PT76B,
            M1,
            M1IP,
            M60A1,
            M60A3,
            M2Bradley,
            M113,
            M923,
            Static_TOW,
            Count,
        }

        private static bool vics_done = false;
        private static Vehicle[] vehicles;
        private static Dictionary<int, GameObject> VicsLookup = new Dictionary<int, GameObject>() { };
        private static string[] VicGameIds = new string[] {
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
            "Ural",
            "Ural Soviet",
            "T62",
            "T64A",
            "T64B",
            "T80B",
            "BTR70",
            "UAZ469",
            "T55A",
            "T72M",
            "T72M Gill (Variant)",
            "T72M1",
            "PT76B",
            "M1",
            "M1IP",
            "M60A1",
            "M60A3",
            "M2 Bradley",
            "M113",
            "M923",
            "TOW"
        };   

        private static GameObject FindVehicle(string id) {
            return vehicles.Where(v => v.name == id).FirstOrDefault().gameObject;
        }

        private static void AddVehicleRef(int ref_id, string game_id) {
            VicsLookup.Add(ref_id, FindVehicle(game_id));
        }

        public static GameObject GetVehicle(Vehicles id) {
            return VicsLookup[(int)id];
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
