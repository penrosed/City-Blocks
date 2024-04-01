using System.IO;
using UnityEngine;
using UnityEditor;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Burst;
using System;

[System.Serializable]
public struct PropInstance : IBufferElementData, IEnableableComponent
{
    // The palette index of the prop we want to instanciate.
    public int index;
    public PrimitiveTransform transform; // The prop's transform
}

// The Block struct.
//   - Constructed from a palette of props (see 'Prop.cs').
//   - These props are placed within the block by referencing
//     their index & specifiying a transform (x,y,z, etc.)
// TODO:
//   - Reintroduce string fields as a FixedString32
//
[System.Serializable]
public struct Block
{
    public string name;
    // public string creator;
    public Prop[] palette;
    public PropInstance[] layout;
}


// The BlockLoader class.
//   - Attempts to deserialise itself from the text passed
//     into the TextAsset 'JSON'.
//
public class BlockLoader : MonoBehaviour
{
    // Cram JSON data into the block manually in the Unity Inspector.
    // (Will not be in final builds, due to #if UNITY_EDITOR)
#if UNITY_EDITOR
    [SerializeField] private UnityEngine.Object _JSONFile;
    private UnityEngine.Object _JSONFile_OLD;
    private TextAsset JSON;
#endif

    private World _world;
    public Block block;
    private EntityArchetype _blockArchetype;
    private EntityArchetype _propDataArchetype;

    private void Awake()
    {
        _world = World.DefaultGameObjectInjectionWorld;
        _blockArchetype = _world.EntityManager.CreateArchetype(typeof(LocalTransform), typeof(LocalToWorld), typeof(PropPalette));
        _propDataArchetype = _world.EntityManager.CreateArchetype(typeof(LocalToWorld), typeof(Parent));

#if UNITY_EDITOR
        try
        {
            string _path = AssetDatabase.GetAssetPath(_JSONFile);
            JSON = new TextAsset(new StreamReader(_path).ReadToEnd());
            /*Debug.Log("\""+_JSONFile+"\" attached to Block Object in Inspector");*/
        }
        catch
        {
            Debug.LogWarning("Could not read JSON data from \"" + _JSONFile + "\"");
        }
#endif
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            CreateBlock(JSON, new Vector3(UnityEngine.Random.Range(-5.0f, 5.0f), 0, UnityEngine.Random.Range(-5.0f, 5.0f)));
        }
    }
#endif

    public void CreateBlock(TextAsset blockJSON, Vector3 position)
    {
        // TODO:
        //   - Add schema verification for all JSON read.
        //     Will ensure no bad data is loaded.
        try
        {
            // Populate a Block struct from our JSON!
            block = JsonUtility.FromJson<Block>(blockJSON.text);

            if (_world.IsCreated)
            {
                // Create our EntityCommandBuffer to queue up our EntityManager commands.
                EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

                // Set up our block Entity.
                Entity blockEnt = ecb.CreateEntity(_blockArchetype);
                ecb.SetComponent(blockEnt, new LocalTransform
                {
                    Position = position,
                    Scale = 1f,
                    Rotation = Unity.Mathematics.quaternion.identity,
                });
                ecb.SetComponent(blockEnt, new LocalToWorld { Value = Unity.Mathematics.float4x4.identity });
                ecb.SetName(blockEnt, block.name);

#if UNITY_EDITOR
                // Create a blank entity to parent our palette entities to, for editor organisation.
                Entity paletteFolderEnt = ecb.CreateEntity();
                ecb.SetName(paletteFolderEnt, "palette");
                ecb.AddComponent<LocalToWorld>(paletteFolderEnt);
                ecb.AddComponent(paletteFolderEnt, new Parent { Value = blockEnt });
#endif

                // Set up our palette data. One entity per prop.
                foreach (Prop p in block.palette)
                {
                    Entity propDataEnt = ecb.CreateEntity(_propDataArchetype);
                    ecb.SetName(propDataEnt, p.name);
#if UNITY_EDITOR
                    ecb.SetComponent(propDataEnt, new Parent { Value = paletteFolderEnt });
#endif
                    // Add our prop data to the prop data entity
                    var nativePropData = new NativeArray<Primitive>(p.primitives, Allocator.Temp);
                    ecb.AddBuffer<Primitive>(propDataEnt).AddRange(nativePropData);
                    nativePropData.Dispose();

                    // Give our block entity a reference to our prop data entity.
                    ecb.AppendToBuffer<PropPalette>(blockEnt, propDataEnt);
                }

                // Set up our layout buffer.
                var nativeLayoutData = new NativeArray<PropInstance>(block.layout, Allocator.Temp);
                ecb.AddBuffer<PropInstance>(blockEnt).AddRange(nativeLayoutData);
                nativeLayoutData.Dispose();

                // Play back our EM commands.
                ecb.Playback(_world.EntityManager);
                ecb.Dispose();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Encountered an error while loading block:\n" + e);
        }
    }
}

public struct PropPalette : IBufferElementData
{
    public Entity propData;

    [BurstCompile]
    public static implicit operator PropPalette(Entity propData)
    {
        return new PropPalette { propData = propData };
    }

    [BurstCompile]
    public static implicit operator Entity(PropPalette element)
    {
        return element.propData;
    }
}