using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Muse.Goi2.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfoDumpMod.Serializers
{
    public class ShipConverter : JsonConverter<ShipModel>
    {
        public override void WriteJson(JsonWriter writer, ShipModel value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            jo["Name"] = value.NameText.En;
            jo["Id"] = value.Id;
            jo["IconPath"] = value.GetIcon();
            jo["ItemType"] = "Ship";

            jo["GunCount"] = value.GunSlots;
            jo["Slots"]= JToken.FromObject(value.Slots, serializer);

            jo.WriteTo(writer);
        }

        public override ShipModel ReadJson(JsonReader reader, Type objectType, ShipModel existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class ShipPartConverter : JsonConverter<ShipPartSlotConfig>
    {
        public override void WriteJson(JsonWriter writer, ShipPartSlotConfig value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            jo["Size"] = (int)value.SlotSize;
            jo["Position"] = JToken.FromObject(value.Position, serializer);
            jo["Angle"] = value.Orientation.Y;
            jo.WriteTo(writer);
        }

        public override ShipPartSlotConfig ReadJson(JsonReader reader, Type objectType, ShipPartSlotConfig existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }


}
