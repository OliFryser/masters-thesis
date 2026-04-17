using System.IO;
using MapElites.Json;
using MapElites.Models;
using Newtonsoft.Json;

namespace Pokémon.Json
{
    public static class JsonSerializer
    {
        public static string ConvertToJson(int mapDimension, Archive<Key, Entry, Individual, Behavior> archive)
        {
            ArchiveSaveData<Key, Entry> archiveSaveData = ArchiveConverter.GetSaveDataFromArchive(archive);
            SaveData saveData = new SaveData
            {
                MapDimension = mapDimension,
                Archive = archiveSaveData,
            };
            
            return JsonConvert.SerializeObject(saveData);
        }

        public static SaveData ReadFromJson(string json)
            => JsonConvert.DeserializeObject<SaveData>(json);

        public static void WriteToFile(string filePath, string json)
        {
            using StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(json);
            streamWriter.Close();
        }
    }
}