using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Effects;
using GHPC.Weaponry.Artillery;
using GHPC.Weapons.Artillery;

namespace CustomMissionUtility
{
    public class FireSupport
    {
        public static AmmoCodexScriptable HE_152mm;
        public static AmmoCodexScriptable HE_155mm;

        public static BatteryMunitionsChoice HE_152mm_battery_munition;
        public static BatteryMunitionsChoice HE_155mm_battery_munition;

        public static ArtilleryBattery HE_152mm_battery;
        public static ArtilleryBattery HE_155mm_battery;

        internal static void Init() {
            if (HE_152mm != null) return;

            AmmoType _152mm_he = new AmmoType();
            _152mm_he.RhaPenetration = 40f;
            _152mm_he.Caliber = 152f;
            _152mm_he.Category = AmmoType.AmmoCategory.Explosive;
            _152mm_he.CertainRicochetAngle = 15f;
            _152mm_he._radius = 0.076f;
            _152mm_he.Coeff = 0.152f;
            _152mm_he.DetonateSpallCount = 200;
            _152mm_he.ImpactAudio = GHPC.Audio.ImpactAudioType.ArtilleryGun;
            _152mm_he.ImpactEffectDescriptor = new ParticleEffectsManager.ImpactEffectDescriptor() { 
                HasImpactEffect = true,
                ImpactCategory = ParticleEffectsManager.Category.HighExplosive,
                EffectSize = ParticleEffectsManager.EffectSize.Artillery,
                RicochetType = ParticleEffectsManager.RicochetType.LargeTracer,
                Flags = ParticleEffectsManager.ImpactModifierFlags.Large,
                MinFilterStrictness = ParticleEffectsManager.FilterStrictness.Low,
            };
            _152mm_he.Mass = 20f;
            _152mm_he.MaximumRange = 6000f;
            _152mm_he.MaxSpallRha = 35f;
            _152mm_he.MinSpallRha = 15f;
            _152mm_he.MuzzleVelocity = 1000f;
            _152mm_he.Name = "152mm HE shell";
            _152mm_he.SectionalArea = 0.0014f;
            _152mm_he.TntEquivalentKg = 20f;
            _152mm_he.UseTracer = true;
            _152mm_he.VisualType = GHPC.Weapons.LiveRoundMarshaller.LiveRoundVisualType.Shell;

            AmmoType _155mm_he = new AmmoType();
            Util.ShallowCopy(_155mm_he, _152mm_he);
            _155mm_he.Name = "155mm HE shell";

            HE_152mm = new AmmoCodexScriptable();
            HE_152mm.AmmoType = _152mm_he;
            HE_152mm.name = "CMU 152mm he";

            HE_155mm = new AmmoCodexScriptable();
            HE_155mm.AmmoType = _155mm_he;
            HE_155mm.name = "CMU 155mm he";

            HE_152mm_battery_munition = new BatteryMunitionsChoice() { 
                Ammo = HE_152mm,
                Type = IndirectFireMunitionType.AntiPersonnel,
            };

            HE_155mm_battery_munition = new BatteryMunitionsChoice()
            {
                Ammo = HE_155mm,
                Type = IndirectFireMunitionType.AntiPersonnel,
            };
        }
    }
}
