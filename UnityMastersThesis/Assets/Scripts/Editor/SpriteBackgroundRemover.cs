using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class SpriteBackgroundRemover
    {
        [MenuItem("Tools/Remove Background From Selected Sprite")]
        static void RemoveBackground()
        {
            Texture2D texture = Selection.activeObject as Texture2D;

            if (texture == null)
            {
                Debug.LogError("Select a sprite texture first.");
                return;
            }

            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);

            importer.isReadable = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();

            texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

            Color targetColor = texture.GetPixel(0, texture.height - 1); 
            // Assumes top-left pixel is background

            Color[] pixels = texture.GetPixels();

            for (int i = 0; i < pixels.Length; i++)
            {
                if (Approximately(pixels[i], targetColor))
                {
                    pixels[i] = new Color(0, 0, 0, 0);
                }
            }

            Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            newTexture.filterMode = FilterMode.Point;
            newTexture.SetPixels(pixels);
            newTexture.Apply();

            File.WriteAllBytes(path, newTexture.EncodeToPNG());
            AssetDatabase.Refresh();

            Debug.Log("Background removed successfully.");
        }

        static bool Approximately(Color a, Color b)
        {
            float tolerance = 0.01f;
            return Mathf.Abs(a.r - b.r) < tolerance &&
                   Mathf.Abs(a.g - b.g) < tolerance &&
                   Mathf.Abs(a.b - b.b) < tolerance;
        }
    }
}