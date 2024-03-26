using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Burst;

[BurstCompile]
public partial struct PrimitiveSpawnSystem : ISystem
{
    private EntityCommandBuffer _ecb;
    private EntityArchetype _propRootArchetype;

    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PrimitivePrefab>();
        _ecb = new EntityCommandBuffer(Allocator.Persistent);
        _propRootArchetype = state.EntityManager.CreateArchetype(typeof(LocalTransform), typeof(Parent), typeof(LocalToWorld));
    }

    [BurstCompile]
    void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var primitiveBuffer = SystemAPI.GetSingletonBuffer<PrimitivePrefab>(true);

        foreach (var (transform, palette, layout, root)
            in SystemAPI.Query<RefRW<LocalTransform>, DynamicBuffer<PropPalette>, DynamicBuffer<PropInstance>>().WithEntityAccess())
        {
            foreach (var datum in layout)
            {
                var propRoot = state.EntityManager.CreateEntity(_propRootArchetype);

                quaternion propRotation = Rotate(datum.transform.rotation);
                SystemAPI.SetComponent(propRoot, LocalTransform.FromPositionRotation(datum.transform.position, propRotation));
                SystemAPI.SetComponent(propRoot, new Parent { Value = root });
                SystemAPI.SetComponent(propRoot, new LocalToWorld { Value = float4x4.identity });
                /*ecb.SetName(propRoot, _entityManager.GetName(palette[datum.index]));*/

                var propBuffer = SystemAPI.GetBuffer<Primitive>(palette[datum.index]);
                foreach (var primitive in propBuffer)
                {
                    var newPrim = state.EntityManager.Instantiate(primitiveBuffer[primitive.type]);
                    SystemAPI.SetComponent(newPrim, new Parent { Value = propRoot });

                    quaternion primRotation = Rotate(primitive.transform.rotation);
                    SystemAPI.SetComponent(newPrim, LocalTransform.FromPositionRotation(primitive.transform.position, primRotation));

                    SystemAPI.SetComponent(newPrim, new PostTransformMatrix
                    {
                        Value = float4x4.Scale(primitive.transform.scale.x, primitive.transform.scale.y, primitive.transform.scale.z)
                    });

                    var colourQuery = SystemAPI.QueryBuilder().WithAll<URPMaterialPropertyBaseColor>().Build();
                    var colourQueryMask = colourQuery.GetEntityQueryMask();
                    _ecb.SetComponentForLinkedEntityGroup(newPrim, colourQueryMask, new URPMaterialPropertyBaseColor
                    {
                        Value = new float4
                        {
                            x = primitive.colour.r,
                            y = primitive.colour.g,
                            z = primitive.colour.b,
                            w = primitive.colour.a
                        }
                    });
                }
            }
        }

        _ecb.Playback(state.EntityManager);
    }

    [BurstCompile]
    readonly quaternion Rotate(float3 eulerAngles)
    {
        quaternion rotation = quaternion.identity;
        if (!eulerAngles.Equals(float3.zero))
        {
            rotation = math.mul(rotation, quaternion.RotateX(math.radians(eulerAngles.x)));
            rotation = math.mul(rotation, quaternion.RotateY(math.radians(eulerAngles.y)));
            rotation = math.mul(rotation, quaternion.RotateZ(math.radians(eulerAngles.z)));
        }
        return rotation;
    }
}
