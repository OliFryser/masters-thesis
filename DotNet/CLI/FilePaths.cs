using System;

namespace CLI;

public static class FilePaths
{
    private static string BaseDirectory => $"{AppDomain.CurrentDomain.BaseDirectory}/../../..";
    private static string ResourceDirectory => $"{BaseDirectory}/Resources";
    public static string TilemapName => "PalletTown.png";
    public static string TilemapPath => $"{ResourceDirectory}/Tilemaps/{TilemapName}";
    public static string DataPath => $"{OutputPath}/Data";
    
    public static string OutputPath
    {
        get
        {
            field ??= $"{BaseDirectory}/Output/MapElites/{DateTime.Now:yyyyMMdd-HHmmss}";
            return field;
        }
    }

}