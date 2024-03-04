using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A simple transform class. Unity's built-in Transforms
// are permanently tied to their objects. In order to
// serialise transform data, I've had to write my own class.
//
[System.Serializable]
public struct PrimitiveTransform
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
}

// A primitive shape (sphere, cube, etc). The kind of primitive
// is represented by 'type', and its location is found in 'Transform'.
//
[System.Serializable]
public struct Primitive
{
    // TODO:
    //   - Change int type to some kind of enum.
    //   - Add some way of changing the texture / base colour of the
    //     primitive.
    //     
    public string type;
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
    //
    public string name;
    public Primitive[] primitives;
}