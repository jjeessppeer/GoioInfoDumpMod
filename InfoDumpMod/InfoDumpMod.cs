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
using InfoDumpMod.Serializers;
using LitJson;
using System.Threading;

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
        public static HashSet<int> AllGuns;

        // Go to menu when login is finished
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UILauncherMainPanel), "EnablePlayButton")]
        private static void LauncherReady()
        {
            if (_LoggedInComplete) return;
            _LoggedInComplete = true;

            FileLog.Log($"Getting gun data.");
            SubDataActions.GetShipAndGuns(delegate (JsonData json)
            {
                JsonData jsonData = json["allguns"];
                var allGuns = new HashSet<int>();
                for (int i = 0; i < jsonData.Count; i++)
                {
                    allGuns.Add((int)jsonData[i]);
                }
                AllGuns = allGuns;
                //allGuns.Contains(g.Id)
                //var legalGuns = CachedRepository.Instance.GetBy<GunItem>((GunItem g) => allGuns.Contains(g.Id) && g.Size == ShipPartSlotSize.SMALL).ToList();
                FileLog.Log($"Serializing...");
                var info = new InfoDump();
                FileLog.Log(JsonConvert.SerializeObject(
                    info,
                    new GunItemConverter(),
                    new AmmunitionConverter(),
                    new DamagePacketConverter(),
                    new RegionConverter(),
                    new SkillConverter(),
                    new SkillEffectConverter(),
                    new ShipConverter(),
                    new ShipPartConverter()
                ));
            });

            
        }

        public class InfoDump
        {
            public List<ShipModel> Ships = new List<ShipModel>();
            public List<Region> Maps = new List<Region>();
            public List<GunItem> Guns = new List<GunItem>();
            public List<SkillConfig> Skills = new List<SkillConfig>();

            public InfoDump()
            {
                Maps = CachedRepository.Instance.GetAll<Region>().ToList();
                Ships = CachedRepository.Instance.GetAll<ShipModel>().ToList();
                Guns = CachedRepository.Instance.GetAll<GunItem>().ToList();
                Skills = CachedRepository.Instance.GetAll<SkillConfig>().ToList();

                
            }
        }

    }



}
