using System;
using ImageAnalysis;

var resourceDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}/../../../Resources/";
var tilemap = resourceDirectory + "Tilemaps/PalletTown.png";
    
ImageAnalyzer analyzer = new ImageAnalyzer(tilemap, resourceDirectory + "PalletTown");
analyzer.Analyze();