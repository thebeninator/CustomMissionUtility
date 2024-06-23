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

        private static bool vics_done = false;
        private static Vehicle[] vehicles;

        public static Dictionary<string, GameObject> Vehicles = new Dictionary<string, GameObject>() { };

        private static GameObject FindVehicle(string id) {
            return vehicles.Where(v => v.name == id).FirstOrDefault().gameObject;
        }

        private static void AddVehicleRef(string ref_id, string game_id) { 
            Vehicles.Add(ref_id, FindVehicle(game_id));
        }

        internal static void GetVicReferences() {
            if (vics_done) return;

            vehicles = Resources.FindObjectsOfTypeAll<Vehicle>();

            AddVehicleRef("BMP1 NVA", "BMP1");
            AddVehicleRef("BMP1 SOV", "BMP1 Soviet");

            AddVehicleRef("BMP1P NVA", "BMP1P (Variant)");
            AddVehicleRef("BMP1P SOV", "BMP1P (Variant) Soviet");

            AddVehicleRef("BMP2 NVA", "BMP2");
            AddVehicleRef("BMP2 SOV", "BMP2 Soviet");

            AddVehicleRef("BRDM2 NVA", "BRDM2");
            AddVehicleRef("BRDM2 SOV", "BRDM2 Soviet");

            AddVehicleRef("BTR60 NVA", "BTR60PB");
            AddVehicleRef("BTR60 SOV", "BTR60PB Soviet");

            AddVehicleRef("9K111 NVA", "9K111");
            AddVehicleRef("9K111 SOV", "9K111 Soviet");

            AddVehicleRef("URAL NVA", "Ural");
            AddVehicleRef("URAL SOV", "Ural Soviet");

            AddVehicleRef("T62", "T62");
            AddVehicleRef("T64A", "T64A");
            AddVehicleRef("T64B", "T64B");
            AddVehicleRef("T80B", "T80B");
            AddVehicleRef("BTR70", "BTR70");
            AddVehicleRef("GOPNIK CAR", "UAZ469");

            AddVehicleRef("T55A", "T55A");
            AddVehicleRef("T72M", "T72M");
            AddVehicleRef("T72M GILLS", "T72M Gill (Variant)");
            AddVehicleRef("T72M1", "T72M1");
            AddVehicleRef("PT76B", "PT76B");

            AddVehicleRef("M1", "M1");
            AddVehicleRef("M1IP", "M1IP");

            AddVehicleRef("M60A1", "M60A1");
            AddVehicleRef("M60A3", "M60A3");

            AddVehicleRef("M2 BRADLEY", "M2 Bradley");

            AddVehicleRef("M113", "M113");
            AddVehicleRef("M923", "M923");

            AddVehicleRef("TOW", "TOW");

            vics_done = true;
        }
    }
}
