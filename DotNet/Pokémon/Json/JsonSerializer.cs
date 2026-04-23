using System.Collections.Generic;
using System.IO;
using MapElites.Json.Converters;
using MapElites.Models;
using Newtonsoft.Json;

namespace Pokémon.Json
{
    public static class JsonSerializer
    {
        private static JsonSerializerSettings Settings => new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new ConstrainedArchiveConverter<Key, ConstrainedEntry<Individual,Behavior>, Individual, Behavior>(),
                new ArchiveConverter<Key, Entry, Individual, Behavior>(),
            },
        };
        
        public static void SaveToFile(
            string filePath,
            IArchive<Key, Entry, Individual, Behavior> archive, 
            int mapDimension)
        {
            string json = ConvertToJson(mapDimension, archive);
            WriteToFile(filePath, json);
        }
        
        public static void SaveToFile(
            string filePath,
            IArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive, 
            int mapDimension)
        {
            string json = ConvertToJson(mapDimension, archive);
            WriteToFile(filePath, json);
        }
        
        public static SaveData ReadFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return ReadFromJson(json);
        }
        
        public static ConstrainedSaveData ReadConstrainedSaveDataFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return ReadConstrainedSaveDataFromJson(json);
        }
        
        private static string ConvertToJson(
            int mapDimension,
            IArchive<Key, Entry, Individual, Behavior> archive)
        {
            SaveData saveData = new SaveData()
            {
                MapDimension = mapDimension,
                Archive = (Archive<Key, Entry, Individual, Behavior>)archive,
            };
            return JsonConvert.SerializeObject(saveData, Settings);
        }
        
        private static string ConvertToJson(
            int mapDimension,
            IArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive)
        {
            ConstrainedSaveData saveData = new ConstrainedSaveData
            {
                MapDimensions = mapDimension,
                Archive = (ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior>)archive,
            };
            return JsonConvert.SerializeObject(saveData, Settings);
        }
        
        private static void WriteToFile(string filePath, string json)
        {
            using StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(json);
            streamWriter.Close();
        }

        private static SaveData ReadFromJson(string json)
            => JsonConvert.DeserializeObject<SaveData>(json, Settings);
        
        private static ConstrainedSaveData ReadConstrainedSaveDataFromJson(string json)
            => JsonConvert.DeserializeObject<ConstrainedSaveData>(json, Settings);
    }
}