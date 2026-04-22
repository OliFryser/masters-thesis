using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pokémon.Json
{
    public class KeyConverter : JsonConverter<Dictionary<Key, Entry>>
    {
        public override void WriteJson(JsonWriter writer, Dictionary<Key, Entry>? value, Newtonsoft.Json.JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (value == null)
            {
                writer.WriteEndObject();
                return;
            }
            
            foreach ((Key key, Entry entry) in value)
            {
                writer.WritePropertyName(key.ToString());
                serializer.Serialize(writer, entry);
            }

            writer.WriteEndObject();
        }

        public override Dictionary<Key, Entry> ReadJson(
            JsonReader reader, 
            Type objectType, 
            Dictionary<Key, Entry>? existingValue, 
            bool hasExistingValue,
            Newtonsoft.Json.JsonSerializer serializer)
        {
            Dictionary<Key, Entry> dictionary = existingValue ?? new Dictionary<Key, Entry>();
            if (reader.TokenType != JsonToken.StartObject)
            {
                return dictionary;
            }
            
            while (reader.Read() && reader.TokenType != JsonToken.EndObject)
            {
                string keyName = (string)reader.Value!;

                if (!Key.TryParse(keyName, out Key? key) || key == null)
                {
                    continue;
                }
                reader.Read();
                Entry val = serializer.Deserialize<Entry>(reader) 
                            ?? throw new InvalidOperationException("Wrongly formatted JSON");
                
                dictionary.Add(key, val);
            }
            
            return dictionary;
        }
    }
}