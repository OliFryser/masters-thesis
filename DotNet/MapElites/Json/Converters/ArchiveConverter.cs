using System;
using System.Collections.Generic;
using MapElites.Models;
using Newtonsoft.Json;

namespace MapElites.Json.Converters
{
    public class ArchiveConverter<TKey, TEntry, TIndividual, TBehavior> :
        JsonConverter<Archive<TKey, TEntry, TIndividual, TBehavior>>
        where TKey : BaseKey<TKey>
        where TEntry : Entry<TIndividual, TBehavior>
    {
        public override void WriteJson(JsonWriter writer, Archive<TKey, TEntry, TIndividual, TBehavior>? value,
            JsonSerializer serializer)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            writer.WriteStartObject();
            writer.WritePropertyName("Archive");
            serializer.Serialize(writer, value.GetArchiveAsDictionary());
            writer.WritePropertyName("BucketCapacity");
            serializer.Serialize(writer, value.BucketCapacity);

            writer.WriteEndObject();
        }

        public override Archive<TKey, TEntry, TIndividual, TBehavior>? ReadJson(
            JsonReader reader,
            Type objectType,
            Archive<TKey, TEntry, TIndividual, TBehavior>? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            int bucketCapacity = 0;
            Dictionary<TKey, TEntry>? dictionary = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject) break;

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    string? propertyName = reader.Value?.ToString()!;
                    reader.Read();

                    if (propertyName == "Archive")
                    {
                        dictionary = serializer.Deserialize<Dictionary<TKey, TEntry>>(reader);
                    }
                    else if (propertyName == "BucketCapacity")
                    {
                        bucketCapacity = serializer.Deserialize<int>(reader);
                    }
                }
            }

            return dictionary != null
                ? new Archive<TKey, TEntry, TIndividual, TBehavior>(bucketCapacity, dictionary)
                : null;
        }
    }
}