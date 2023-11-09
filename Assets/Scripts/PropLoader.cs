using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PropLoader : MonoBehaviour
{
    // A simple transform class. Unity's built-in Transforms
    // are permanently tied to their objects. In order to
    // serialise transform data, I've had to write my own class.
    //
    [System.Serializable]
    public class PrimitiveTransform
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }

    // A primitive shape (sphere, cube, etc). The kind of primitive
    // is represented by 'type', and its location is found in 'Transform'.
    //
    // TODO:
    //   - Change int type to some kind of enum.
    //   - Add some way of changing the texture / base colour of the
    //     primitive.
    //
    [System.Serializable]
    public class Primitive
    {
        public int type;
        public PrimitiveTransform transform;
    }

    // A prop. A collection of primitive shapes that can look like
    // fences, signs, walls, doors, windows, etc.
    //
    // TODO:
    //   - Add prop thumbnail field.
    //   - add prop type field. (The type would change the placement
    //     behaviour of the prop. Signs and paintings go on walls, etc.)
    //
    [System.Serializable]
    public class Prop
    {
        public string name;
        public Primitive[] primitives;
    }

    public List<Prop> props;

    // Awake is called before start, as early as possible.
    // Allows prop list to be read on 'Start()'.
    //
    private void Awake()
    {
        // TODO: Maybe asynchronously load props? Depends on
        //       number of props, and how long loading takes.
        try
        {
            IEnumerable<string> files = Directory.EnumerateFiles(
                Application.streamingAssetsPath +
                Path.DirectorySeparatorChar + "Props", "*.json"
            );
            foreach (var file in files)
            {
                string jsonText = new StreamReader(file).ReadToEnd();
                props.Add(JsonUtility.FromJson<Prop>(jsonText));
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Encountered an error while loading props: "+e);
        }
    }
}
