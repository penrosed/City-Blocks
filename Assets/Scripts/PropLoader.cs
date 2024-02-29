using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PropLoader : MonoBehaviour
{
    public List<Prop> propsPalette; // The user's palette of available props.

    // Awake is called before start, as early as possible.
    // Allows prop palette to be read on 'Start()'.
    //
    private void Awake()
    {
        // TODO:
        //   - Add schema verification for all JSON read.
        //     Will ensure no bad data is loaded.
        //   - Maybe asynchronously load props? Depends on
        //     number of props, and how long loading takes.
        try
        {
            IEnumerable<string> files = Directory.EnumerateFiles(
                Application.streamingAssetsPath +
                Path.DirectorySeparatorChar + "Props", "*.json"
            );
            foreach (var file in files)
            {
                string jsonText = new StreamReader(file).ReadToEnd();
                propsPalette.Add(JsonUtility.FromJson<Prop>(jsonText));
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Encountered an error while loading props: "+e);
        }
    }
}
