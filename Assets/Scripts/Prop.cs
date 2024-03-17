using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

// A simple transform class. Unity's built-in Transforms
// are permanently tied to their objects. In order to
// serialise transform data, I've had to write my own class.
//
[System.Serializable]
[BurstCompile]
public struct PrimitiveTransform
{
    public float3 position;
    public float3 rotation;
    public float3 scale;

    public static implicit operator bool(PrimitiveTransform transform)
    {
        return !(transform.position.Equals(float3.zero) &&
                 transform.rotation.Equals(float3.zero) &&
                 transform.rotation.Equals(float3.zero));
    }
}

// A primitive shape (sphere, cube, etc). The kind of primitive
// is represented by 'type', and its location is found in 'Transform'.
//
[System.Serializable]
public struct Primitive : IBufferElementData
{
    // TODO:
    //   - Change int type to some kind of enum.
    //   - Add some way of changing the texture / base colour of the
    //     primitive.
    //     
    public int type;
    public Color colour;
    public PrimitiveTransform transform;
}

// A prop. A collection of primitive shapes that can look like
// fences, signs, walls, doors, windows, etc.
//
[System.Serializable]
public struct Prop
{
    // TODO:
    //   - Add prop thumbnail field.
    //   - add prop type field. (The type would change the placement
    //     behaviour of the prop. Signs and paintings go on walls, etc.)
    //   - Reintroduce string fields as a FixedString32
    //
    public string name;
    public Primitive[] primitives;
}