using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
partial struct AddBuiltInsToPrimitivesBakingSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var queryPrimitiveTag = SystemAPI.QueryBuilder().WithAll<PrimitiveTag>().WithOptions(EntityQueryOptions.IncludePrefab).Build();
        state.EntityManager.AddComponent<PostTransformMatrix>(queryPrimitiveTag);
        state.EntityManager.AddComponent<Parent>(queryPrimitiveTag);

        var entityList = queryPrimitiveTag.ToEntityArray(Allocator.Temp);
        foreach (var entity in entityList)
        {
            state.EntityManager.SetComponentData(entity, new PostTransformMatrix { Value = float4x4.identity });
        }
        entityList.Dispose();
    }
}
