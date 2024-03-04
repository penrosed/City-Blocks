using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;

public partial struct PrimitiveSpawnSystem : ISystem
{
    void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PrimitiveBufferElement>();
    }

    void OnUpdate(ref SystemState state) 
    {
        state.Enabled = false;
        var primitiveBuffer = SystemAPI.GetSingletonBuffer<PrimitiveBufferElement>(true);

        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var newEntity = entityManager.Instantiate(primitiveBuffer[0]);
        Debug.Log("H");
    }
}
