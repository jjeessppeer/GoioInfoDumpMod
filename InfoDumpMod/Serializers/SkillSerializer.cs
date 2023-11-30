using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using Muse.Goi2.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace InfoDumpMod.Serializers
{
    public class SkillConverter : JsonConverter<SkillConfig>
    {
        public override void WriteJson(JsonWriter writer, SkillConfig value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            jo["Name"] = value.NameText.En;
            jo["Id"] = value.ActivationId;
            jo["IconPath"] = value.GetIcon();
            jo["ItemType"] = "Skill";
            jo["SkillType"] = (int)value.Type;
            jo["SkillTypeName"] = value.Type.ToString();
            jo["PvEOnly"] = value.CoopOnly;
            jo["Cooldown"] = value.CooldownTime;

            Dictionary<string, Muse.Goi2.Entity.SkillEffect> effects = new Dictionary<string, Muse.Goi2.Entity.SkillEffect>();
            PlayerSkill playerSkill = value.CreateSkill(0, false);
            foreach (var effect in playerSkill.Effects)
            {
                effects.Add(effect.Type.ToString(), effect);
            }
            jo["Effects"] = JToken.FromObject(effects, serializer);
            jo.WriteTo(writer);
        }

        public override SkillConfig ReadJson(JsonReader reader, Type objectType, SkillConfig existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class SkillEffectConverter : JsonConverter<Muse.Goi2.Entity.SkillEffect>
    {
        public override void WriteJson(JsonWriter writer, Muse.Goi2.Entity.SkillEffect value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            jo["Strength"] = value.Strength;
            jo["Duration"] = value.Duration;
            jo.WriteTo(writer);
        }

        public override Muse.Goi2.Entity.SkillEffect ReadJson(JsonReader reader, Type objectType, Muse.Goi2.Entity.SkillEffect existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
