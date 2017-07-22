using UnityEngine;
using UnityEditor;

internal sealed class SpriteFix : AssetPostprocessor
{
    private const int maxSize = 32;

    private void OnPreprocessTexture()
    {
        // I prefix my texture asset's file names with tex, following 3 lines say "if tex is not in the asset file name, do nothing"
        var fileNameIndex = assetPath.LastIndexOf('/');
        var fileName = assetPath.Substring(fileNameIndex + 1);

        //if (!fileName.Contains("tex")) return;

        // Get the reference to the assetImporter (From the AssetPostProcessor class) and unbox it to a TextureImporter (Which is inherited and extends the AssetImporter with texture specific utilities)
        TextureImporter importer = assetImporter as TextureImporter;

        // The options for the platform string are "Web", "Standalone", "iPhone", "Android"
        // Unity API provides neat single function settings for the most import settings as SetPlatformTextureSettings
        // Parameters are: platform, maxTextureSize, textureFormat, compressionQuality
        // I also change the format based on if the texture has an alpha channel or not because not all formats support an alpha channel
        

        // Set the texture import type drop-down to advanced so our changes reflect in the import settings inspector
        importer.textureType = TextureImporterType.Sprite;
        // Below line may cause problems with systems and plugins that utilize the textures (read/write them) like NGUI so comment it out based on your use-case
        importer.filterMode = FilterMode.Point;
        importer.maxTextureSize = 64;
        importer.compressionQuality = 0;
        //importer.
        // If you are only using the alpha channel for transparency, uncomment the below line. I commented it out because I use the alpha channel for various shaders (e.g. specular map or various other masks)
        //importer.alphaIsTransparency = importer.DoesSourceTextureHaveAlpha();
    }
}