using System.IO;
using MapElites.Models;
using Newtonsoft.Json;

namespace Pokémon.Json
{
    public static class JsonSerializer
    {
        public static void SaveToFile(
            string filePath,
            Archive<Key, Entry, Individual, Behavior> archive, 
            int mapDimension)
        {
            string json = ConvertToJson(mapDimension, archive);
            WriteToFile(filePath, json);
        }
        
        public static void SaveToFile(
            string filePath,
            ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive, 
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
            Archive<Key, Entry, Individual, Behavior> archive)
        {
            SaveData saveData = new SaveData
            {
                MapDimension = mapDimension,
                Archive = archive,
            };
            return JsonConvert.SerializeObject(saveData);
        }
        
        private static string ConvertToJson(
            int mapDimension,
            ConstrainedArchive<Key, ConstrainedEntry<Individual, Behavior>, Individual, Behavior> archive)
        {
            ConstrainedSaveData saveData = new ConstrainedSaveData
            {
                MapDimensions = mapDimension,
                Archive = archive,
            };
            return JsonConvert.SerializeObject(saveData);
        }
        
        private static void WriteToFile(string filePath, string json)
        {
            using StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(json);
            streamWriter.Close();
        }

        private static SaveData ReadFromJson(string json)
            => JsonConvert.DeserializeObject<SaveData>(json);
        
        private static ConstrainedSaveData ReadConstrainedSaveDataFromJson(string json)
            => JsonConvert.DeserializeObject<ConstrainedSaveData>(json);
    }
}