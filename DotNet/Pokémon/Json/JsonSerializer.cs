using System.Collections.Generic;
using System.IO;
using MapElites.Json;
using MapElites.Models;
using Newtonsoft.Json;

namespace Pokémon.Json
{
    public static class JsonSerializer
    {
        private static JsonSerializerSettings Settings => new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new KeyConverter() },
        };
        
        public static void SaveToFile(
            string filePath,
            IArchive<Key, Entry, Individual, Behavior> archive, 
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
        
        private static string ConvertToJson(
            int mapDimension, 
            IArchive<Key, Entry, Individual, Behavior> archive)
        {
            ArchiveSaveData<Key, Entry> archiveSaveData = ArchiveConverter.GetSaveDataFromArchive(archive);
            SaveData saveData = new SaveData
            {
                MapDimension = mapDimension,
                Archive = archiveSaveData,
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
    }
}