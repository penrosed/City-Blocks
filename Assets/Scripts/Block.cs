using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public struct PropInstance
{
    // The palette index of the prop we want to instanciate.
    public int index;
    public PrimitiveTransform transform; // The prop's transform
}

// The Block class.
//   - Attempts to deserialise itself from the text passed
//     into the TextAsset 'JSON'.
//   - Constructed from a palette of props (see 'Prop.cs').
//   - These props are placed within the block by referencing
//     their index & specifiying a transform (x,y,z, etc.)
//
[System.Serializable]
public class Block : MonoBehaviour
{
    // Cram JSON data into the block manually in the Unity Inspector.
    // (Will not be in final builds, due to #if UNITY_EDITOR)
#if UNITY_EDITOR
    [SerializeField] private Object _JSONFile;
    private Object _JSONFile_OLD;
#endif

    // Member variables!
    [HideInInspector] public TextAsset JSON;
    [HideInInspector] public string title;
    public string creator;
    public Prop[] palette;
    public PropInstance[] layout;

    private void Awake()
    {
        // TODO:
        //   - Add schema verification for all JSON read.
        //     Will ensure no bad data is loaded.
        //   - Maybe asynchronously load props? Depends on
        //     number of props, and how long loading takes.
        try
        {
            // Populate this class with our JSON text!
            JsonUtility.FromJsonOverwrite(JSON.text, this);
        }
        catch (IOException e)
        {
            Debug.LogError("Encountered an error while loading block:\n"+e);
        }

        // Set the Block Object's name to the title specified in JSON.
        this.name = title;

        // TODO:
        //   - Outsource primitive instantiation to a separate
        //     Factory object. Would allow the queueing of
        //     Instantiation, possibly saving performance.
        //
        // Go through each instantiated prop in our layout...
        foreach (PropInstance i in layout)
        {
            // Create the parent object with the relevant position, etc.
            GameObject prop = new GameObject(palette[i.index].name);
            prop.transform.parent = this.transform;
            prop.transform.localPosition = i.transform.position;
            prop.transform.localRotation = Quaternion.Euler(i.transform.rotation);
            prop.transform.localScale = i.transform.scale;

            // For each primitive within the prop...
            foreach (Primitive p in palette[i.index].primitives)
            {
                // Instantiate that primitive & parent it to the prop.
                GameObject primitiveReference = (GameObject)Instantiate(
                    PrimitiveLookup.primitives[p.type], prop.transform, false
                );
                primitiveReference.transform.localPosition = p.transform.position;
                primitiveReference.transform.localRotation = Quaternion.Euler(p.transform.rotation);
                primitiveReference.transform.localScale = p.transform.scale;

                // Apply the colour property to the material of the prop.
                // Each prop has it's own LODGroup, for performance.
                foreach (LOD lod in primitiveReference.GetComponent<LODGroup>().GetLODs())
                {
                    // Assumes one renderer per LOD.
                    lod.renderers[0].material.color = p.colour;
                }
            }
        }
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        // If '_JSONFile' is updated in the inspector, attempt
        // to read its data and set the 'JSON' TextAsset.
        if (_JSONFile != _JSONFile_OLD)
        {
            _JSONFile_OLD = _JSONFile;
            try
            {
                string _path = AssetDatabase.GetAssetPath(_JSONFile);
                JSON = new TextAsset(new StreamReader(_path).ReadToEnd());
                /*Debug.Log("\""+_JSONFile+"\" attached to Block Object in Inspector");*/
            }
            catch
            {
                Debug.LogWarning("Could not read JSON data from \""+_JSONFile+"\"");
            }
        }
    }
#endif
}