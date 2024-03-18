using Unity.Entities;
using UnityEngine;

public struct PrimitiveTag : IComponentData { }

public class PrimitiveTagAuthoring : MonoBehaviour
{
    public class baker : Baker<PrimitiveTagAuthoring>
    {
        public override void Bake(PrimitiveTagAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Renderable & TransformUsageFlags.NonUniformScale);
            AddComponent(entity, new PrimitiveTag { });
        }
    }
}
