using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BepInEx;
using HarmonyLib;
using UnityEngine;

using Muse.Goi2.Entity;
using Newtonsoft.Json;

using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace InfoDumpMod
{

    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class InfoDumpMod : BaseUnityPlugin
    {
        public const string pluginGuid = "whereami.infodump.mod";
        public const string pluginName = "Info dump mod";
        public const string pluginVersion = "0.1";

        public void Awake()
        {
            FileLog.Log($"Info mod initialize.");
            var harmony = new Harmony(pluginGuid);
            harmony.PatchAll();
        }
    }


    [HarmonyPatch]
    public static class InfoDumpPatch
    {
        private static bool _LoggedInComplete = false;


     

        // Go to menu when login is finished
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UILauncherMainPanel), "EnablePlayButton")]
        private static void LauncherReady()
        {
            if (_LoggedInComplete) return;
            _LoggedInComplete = true;
            var info = new InfoDump();
            FileLog.Log(JsonConvert.SerializeObject(
                info, 
                new GunItemConverter(), 
                new AmmunitionConverter(), 
                new DamagePacketConverter()));
        }

        public class InfoDump
        {
            //public List<MapInfo> Maps = new List<MapInfo>();
            public List<MapInfo> Maps = new List<MapInfo>();
            public List<SkillInfo> Skills = new List<SkillInfo>();
            public List<ShipInfo> Ships = new List<ShipInfo>();
            //public List<GunInfo> Guns = new List<GunInfo>();
            public List<GunItem> Guns = new List<GunItem>();

            public InfoDump()
            {
                FileLog.Log("__REGIONS__");
                foreach (var map in CachedRepository.Instance.GetAll<Region>())
                {
                    MapInfo i = new MapInfo(map);
                    Maps.Add(i);
                    //Maps = CachedRepository.Instance.GetAll<Region>();
                }
                FileLog.Log("__SHIPS__");
                foreach (var ship in CachedRepository.Instance.GetAll<ShipModel>())
                {
                    ShipInfo i = new ShipInfo(ship);
                    Ships.Add(i);
                }
                FileLog.Log("__GUNS__");
                //foreach (var gun in CachedRepository.Instance.GetAll<GunItem>())
                //{
                //    GunInfo i = new GunInfo(gun);
                //    Guns.Add(i);
                //    FileLog.Log(JsonConvert.SerializeObject(gun, new GunItemConverter(), new AmmunitionConverter(), new DamagePacketConverter()));
                //}
                Guns = CachedRepository.Instance.GetAll<GunItem>().ToList();
                FileLog.Log("__SKILLS__");
                foreach (var skill in CachedRepository.Instance.GetAll<SkillConfig>())
                {
                    SkillInfo i = new SkillInfo(skill);
                    Skills.Add(i);
                }
                FileLog.Log("Finished.");

            }
        }

        public class MapInfo
        {
            public string Name;
            public int Id;
            public string IconPath;
            public string ItemType = "Map";

            public List<int> TeamSizes;
            public int GameMode;
            public string GameModeName;

            public ThreeDTuple BoundryMax;
            public ThreeDTuple BoundryMin;

            public MapInfo(Region region)
            {
                if (region.NameText != null)
                {
                    Name = region.NameText.En;
                }
                else
                {
                    Name = region.Name;
                }
                Id = region.Id;
                IconPath = region.GetIcon();
                TeamSizes = (List<int>)region.TeamSize;
                GameMode = (int)region.GameMode;
                GameModeName = region.GameMode.ToString();
                BoundryMax = region.MapBoundaryMax;
                BoundryMin = region.MapBoundaryMin;
            }
        
        
        }



        public class SkillInfo
        {
            public string Name;
            public int Id;
            public string IconPath;
            public string ItemType = "Skill";

            public int SkillType;
            public bool PvEOnly;

            public float Cooldown;
            public Dictionary<string, SkillEffectInfo> Effects = new Dictionary<string, SkillEffectInfo>();
            //public List<List<int>> Effects;

            public class SkillEffectInfo
            {
                //public string EffectType;
                public float Strength;
                public float Duration;

                public SkillEffectInfo(Muse.Goi2.Entity.SkillEffect effect)
                {
                    //EffectType = effect.Type.ToString();
                    Strength = effect.Strength;
                    Duration = effect.Duration;
                }
            }

            public SkillInfo(SkillConfig skill)
            {
                Name = skill.NameText.En;
                Id = skill.ActivationId;
                SkillType = (int)skill.Type;
                IconPath = skill.GetIcon();
                PvEOnly = skill.CoopOnly;

                Cooldown = skill.CooldownTime;

                //FileLog.Log($"{Name}\n\t{skill.GeneratedDescription}");

                PlayerSkill playerSkill = skill.CreateSkill(0, false);
                foreach (var effect in playerSkill.Effects)
                {
                    Effects.Add(effect.Type.ToString(), new SkillEffectInfo(effect));
                }
            }

        }

        public class ShipInfo
        {
            public string Name;
            public int Id;
            public string IconPath;
            public string ItemType = "Ship";

            public int GunCount;
            public List<int> GunSizes = new List<int>();
            public List<float[]> GunPositions = new List<float[]>();
            public List<float> GunAngles = new List<float>();
            public List<string> GunNames = new List<string>();
            public ShipModelSetting ModelParameters;
            //public List<float> GunAngles;
            //public List<float> GunPositions;

            public ShipInfo(ShipModel ship)
            {
                Name = ship.NameText.En;
                Id = ship.Id;
                IconPath = ship.GetIcon();

                GunCount = ship.GunSlots;
                foreach (var entry in ship.Slots)
                {
                    var slot = entry.Value;
                    var name = entry.Key;
                    if (slot.SlotType != ShipPartSlotType.GUN) continue;
                    GunNames.Add(name);
                    GunSizes.Add((int)slot.SlotSize);
                    GunPositions.Add(new float[2] {slot.Position.X, slot.Position.Z});
                    GunAngles.Add(slot.Orientation.Y);
                }
                ModelParameters = ship.BaseSetting;
            }
        }

        public class GunInfo
        {
            public string Name;
            public int Id;
            public string IconPath;
            public string ItemType = "Gun";

            public string Size;
            public Dictionary<string, string> Params = new Dictionary<string, string>();

            //public string Description;


            //public string WeaponSlot;

            //public float RotationSpeed;
            //public float Jitter;
            //public int ClipSize;
            //public float AoeRadius;
            //public float 

            //public int Size;
            //public int GameMode;

            //public int ClipSize;
            //public float ReloadTime;
            //public float CooldownTime;

            //public float MaxYaw;
            //public float MaxPitch;
            //public float YawSpeed;
            //public float PitchSpeed;

            //public int Buckshot;
            //public float Jitter;

            //public float MuzzleSpeed;
            //public float Range;
            //public float Lifetime;
            //public float ArmingTime;
            //public float Gravity;

            public GunInfo(GunItem item)
            {
                Name = item.NameText.En;
                Id = item.Id;
                IconPath = item.GetIcon();

                //Params = item.Params;
                foreach(KeyValuePair<string, string> entry in item.Params)
                {
                    Params.Add(entry.Key, entry.Value);
                }
                Size = GunItem.SizeName(item.Size);
                //Description 
                //Range = item.Range;
                //MuzzleSpeed = item.GetFloatParam("fMuzzleSpeed", Range);
                //Lifetime = item.GetFloatParam("fShellLife", 0f);
                //Gravity = item.GetFloatParam("fGravity", 0f);
                //ArmingTime = item.GetFloatParam("fArmingDelay", 0f);

                //MaxPitch = item.MaxPitchDegrees;
                //MaxYaw = item.MaxYawDegrees;
                //PitchSpeed = item.PitchSpeedDegrees;
                //YawSpeed = item.YawSpeedDegrees;

                //Jitter = item.GetFloatParam("fJitter", 0);
                //Buckshot = item.GetIntParam("iRaysPerShot", 1);

                //FileLog.Log($"Lifetimes: {Lifetime}, {Range / MuzzleSpeed}");

            }
        }
    }



}
