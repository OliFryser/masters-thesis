using System;

namespace CLI;

public static class FilePaths
{
    public static string BaseDirectory => $"{AppDomain.CurrentDomain.BaseDirectory}/../../..";
    private static string ResourceDirectory => $"{BaseDirectory}/Resources";
    public static string TilemapName => "PalletTown.png";
    public static string TilemapPath => $"{ResourceDirectory}/Tilemaps/{TilemapName}";
    public static string OutputPath => $"{FilePaths.BaseDirectory}/Output/MapElites/{DateTime.Now:yyyyMMdd-HHmmss}";

}