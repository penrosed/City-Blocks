using System.Collections.Generic;
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

            var buffer = AddBuffer<PrimitivePrefab>(entity);
            foreach (GameObject go in authoring.primitives)
            {
                buffer.Add(GetEntity(go, TransformUsageFlags.Renderable & TransformUsageFlags.NonUniformScale));
            }
        }
    }
}

public struct PrimitivePrefab : IBufferElementData
{
    public Entity primitivePrefab;

    public static implicit operator PrimitivePrefab(Entity primitivePrefab)
    {
        return new PrimitivePrefab { primitivePrefab = primitivePrefab };
    }

    public static implicit operator Entity(PrimitivePrefab element)
    {
        return element.primitivePrefab;
    }
}