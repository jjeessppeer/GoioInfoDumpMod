using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Muse.Goi2.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfoDumpMod
{
    public class GunItemConverter : JsonConverter<GunItem>
    {
        public override void WriteJson(JsonWriter writer, GunItem value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            jo["Name"] = value.NameText.En;
            jo[nameof(value.Id)] = value.Id;
            jo["IconPath"] = value.GetIcon();
            jo["ItemType"] = "Gun";

            jo["Params"] = JToken.FromObject(value.Params, serializer);
            jo["Size"] = GunItem.SizeName(value.Size);

            jo["Damage"] = JToken.FromObject(value.DefaultAmmunition, serializer);

            jo.WriteTo(writer);
        }

        public override GunItem ReadJson(JsonReader reader, Type objectType, GunItem existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class AmmunitionConverter : JsonConverter<Ammunition>
    {
        public override void WriteJson(JsonWriter writer, Ammunition value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            jo[nameof(value.DirectHit)] = JToken.FromObject(value.DirectHit, serializer);
            jo[nameof(value.BurstHit)] = JToken.FromObject(value.BurstHit, serializer);
            jo.WriteTo(writer);
        }

        public override Ammunition ReadJson(JsonReader reader, Type objectType, Ammunition existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class DamagePacketConverter : JsonConverter<DamagePacket>
    {
        public override void WriteJson(JsonWriter writer, DamagePacket value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            jo[nameof(value.Type)] = value.Type.ToString();
            jo[nameof(value.Amount)] = value.Amount;
            jo[nameof(value.Effect)] = value.Effect.ToString();
            jo[nameof(value.EffectDuration)] = value.EffectDuration;
            jo[nameof(value.EffectPower)] = value.EffectPower;
            jo.WriteTo(writer);
        }

        public override DamagePacket ReadJson(JsonReader reader, Type objectType, DamagePacket existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
