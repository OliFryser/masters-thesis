using System;
using System.Collections.Generic;
using MapElites.Models;
using Newtonsoft.Json;

namespace MapElites.Json.Converters
{
    public class ConstrainedArchiveConverter<TKey, TEntry, TIndividual, TBehavior> : JsonConverter<
        ConstrainedArchive<TKey, TEntry, TIndividual, TBehavior>>
        where TKey : BaseKey<TKey>
        where TEntry : ConstrainedEntry<TIndividual, TBehavior>
    {
        public override void WriteJson(JsonWriter writer,
            ConstrainedArchive<TKey, TEntry, TIndividual, TBehavior>? value, JsonSerializer serializer)
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

        public override ConstrainedArchive<TKey, TEntry, TIndividual, TBehavior>?
            ReadJson(JsonReader reader, Type objectType,
                ConstrainedArchive<TKey, TEntry, TIndividual, TBehavior>? existingValue,
                bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            int bucketCapacity = 0;
            Dictionary<TKey, ConstrainedArchive<TKey, TEntry, TIndividual, TBehavior>.Entries>? dictionary = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject) break;

                if (reader.TokenType == JsonToken.PropertyName)
                {
                    string? propertyName = reader.Value?.ToString();
                    reader.Read();

                    if (propertyName == "Archive")
                    {
                        dictionary = serializer.Deserialize<
                            Dictionary<TKey, ConstrainedArchive<TKey, TEntry, TIndividual, TBehavior>.Entries>>(reader);
                    }
                    else if (propertyName == "BucketCapacity")
                    {
                        bucketCapacity = serializer.Deserialize<int>(reader);
                    }
                }
            }

            return dictionary != null
                ? new ConstrainedArchive<TKey, TEntry, TIndividual, TBehavior>(bucketCapacity, dictionary)
                : null;
        }
    }
}