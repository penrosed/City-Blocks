using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

public class PrimitiveEntityLookupAuthoring : MonoBehaviour
{
    public List<GameObject> primitives;

    public class Baker : Baker<PrimitiveEntityLookupAuthoring>
    {
        public override void Bake(PrimitiveEntityLookupAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            var buffer = AddBuffer<PrimitiveBufferElement>(entity);
            foreach (GameObject go in authoring.primitives)
            {
                buffer.Add(GetEntity(go, TransformUsageFlags.None));
            }
        }
    }
}

[InternalBufferCapacity(8)]
public struct PrimitiveBufferElement : IBufferElementData
{
    public Entity primitive;

    public static implicit operator PrimitiveBufferElement(Entity primitive)
    {
        return new PrimitiveBufferElement { primitive = primitive };
    }

    public static implicit operator Entity(PrimitiveBufferElement element)
    {
        return element.primitive;
    }
}