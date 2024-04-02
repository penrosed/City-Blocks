using UnityEditor;

// https://forum.unity.com/threads/why-is-there-meta-files-in-the-streamingassets-directory.656065/#post-9102703
//
// An asset post-processing script to remove .META files from
// the StreamingAssets folder.
//
public class ExcludeStreamingAssetsMeta : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (var importedAsset in importedAssets)
        {
            if (importedAsset.Contains("Assets/StreamingAssets"))
            {
                // Remove generated meta file
                string metaFilePath = $"{importedAsset}.meta";
                AssetDatabase.MoveAssetToTrash(metaFilePath);
            }
        }
    }
}