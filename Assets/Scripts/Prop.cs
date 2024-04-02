using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

// A simple transform class. Unity's built-in Transforms
// are permanently tied to their objects. In order to
// serialise transform data, I've had to write my own class.
//
[System.Serializable]
public struct PrimitiveTransform : IEquatable<PrimitiveTransform>
{
    public float3 position;
    public float3 rotation;
    public float3 scale;

    public static bool operator ==(PrimitiveTransform lhs, PrimitiveTransform rhs) => lhs.Equals(rhs);
    public static bool operator !=(PrimitiveTransform lhs, PrimitiveTransform rhs) => !lhs.Equals(rhs);

    public bool Equals(PrimitiveTransform other)
    {
        if (position.Equals(other.position) &&
            rotation.Equals(other.rotation) &&
            scale.Equals(other.scale))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    [BurstCompile]
    public static implicit operator PrimitiveTransform(Transform transform)
    {
        return new PrimitiveTransform
        {
            position = transform.localPosition,
            rotation = transform.localRotation.eulerAngles,
            scale = transform.localScale
        };
    }

}

// A primitive shape (sphere, cube, etc). The kind of primitive
// is represented by 'type', and its location is found in 'Transform'.
//
[System.Serializable]
public struct Primitive : IBufferElementData, IEquatable<Primitive>
{
    // TODO:
    //   - Change int type to some kind of enum.
    //   - Add some way of changing the texture / base colour of the
    //     primitive.
    //     
    public int type;
    public Color colour;
    public PrimitiveTransform transform;

    public static bool operator ==(Primitive lhs, Primitive rhs) => lhs.Equals(rhs);
    public static bool operator !=(Primitive lhs, Primitive rhs) => !lhs.Equals(rhs);

    public bool Equals(Primitive other)
    {
        if (type == other.type && colour == other.colour && transform == other.transform)
        {
            return true;
        }
        return false;
    }
}

// A prop. A collection of primitive shapes that can look like
// fences, signs, walls, doors, windows, etc.
//
[System.Serializable]
public struct Prop : IEquatable<Prop>
{
    // TODO:
    //   - Add prop thumbnail field.
    //   - add prop type field. (The type would change the placement
    //     behaviour of the prop. Signs and paintings go on walls, etc.)
    //
    public string name;
    public Primitive[] primitives;

    public static bool operator ==(Prop lhs, Prop rhs) => lhs.Equals(rhs);
    public static bool operator !=(Prop lhs, Prop rhs) => !lhs.Equals(rhs);

    public bool Equals(Prop other)
    {
        if (primitives.Length == other.primitives.Length)
        {
            for (int i = 0; i < primitives.Length; i++)
            {
                if (primitives[i] != other.primitives[i])
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}