using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Muse.Goi2.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InfoDumpMod.Serializers
{
    public class RegionConverter : JsonConverter<Region>
    {
        public override void WriteJson(JsonWriter writer, Region value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            if (value.NameText != null)
            {
                jo["Name"] = value.NameText.En;
            }
            else
            {
                jo["Name"] = value.Name;
            }
            jo[nameof(value.Id)] = value.Id;
            jo["IconPath"] = value.GetIcon();
            jo["ItemType"] = "Map";
            jo[nameof(value.TeamSize)] = JToken.FromObject((List<int>)value.TeamSize, serializer);
            jo[nameof(value.GameMode)] = (int)value.GameMode;
            jo["GameModeName"] = value.GameMode.ToString();
            jo["BoundryMax"] = JToken.FromObject(value.MapBoundaryMax, serializer);
            jo["BoundryMin"] = JToken.FromObject(value.MapBoundaryMin, serializer);

            jo.WriteTo(writer);
        }

        public override Region ReadJson(JsonReader reader, Type objectType, Region existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
