using System;
using System.Linq;
using DOTS.Battle.Cameras;
using DOTS.Battle.Curves;
using DOTS.Grid;
using DOTS.Rounds;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Battle
{
    public partial struct MoveUnitSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MousePosition>();
            state.RequireForUpdate<SpatialHashProperties>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var roundState in SystemAPI.Query<RoundState>())
            {
                if (roundState.RoundStateType != RoundStateType.Playing)
                {
                    return;
                }
            }

            var deltaTime = SystemAPI.Time.DeltaTime;
            
            var hashProperties = SystemAPI.GetSingleton<SpatialHashProperties>();
            var mousePosition = SystemAPI.GetSingleton<MousePosition>(); 
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            
            var movingEntities = SystemAPI.QueryBuilder().WithAll<MoveSpeed>().Build().CalculateEntityCount();
            var quadrantHashMap = new NativeParallelMultiHashMap<int, Entity>(movingEntities, Allocator.Temp);
            
            foreach (var (transform, moveSpeed, entity) in SystemAPI.Query<LocalTransform, MoveSpeed>().WithEntityAccess())
            {
                var hashKey = GetHashKey(transform.Position, hashProperties);
                quadrantHashMap.Add(hashKey, entity);
            }
            
            quadrantHashMap.Dispose();
                
            DebugDrawQuadrant(mousePosition.Value, hashProperties);
            
            state.Dependency = new MoveToTargetJob
            {
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            }.ScheduleParallel(state.Dependency);

        }

        private int GetHashKey(float3 position, SpatialHashProperties properties)
        {
            return (int)(math.floor(position.x / properties.CellSize) + properties.Multiplier * math.floor(position.z / properties.CellSize));
        }

        private void DebugDrawQuadrant(float3 position, SpatialHashProperties properties)
        {
            var cellSize = properties.CellSize;
            var lowerLeft = new float3(math.floor(position.x / cellSize) * cellSize, 0.1f, math.floor(position.z / cellSize) * cellSize);
            Debug.DrawLine(lowerLeft, lowerLeft + new float3(1, 0, 0) * cellSize, Color.magenta);
            Debug.DrawLine(lowerLeft, lowerLeft + new float3(0, 0, 1) * cellSize, Color.magenta);
            Debug.DrawLine(lowerLeft + new float3(1, 0, 0) * cellSize, lowerLeft + new float3(1, 0, 1) * cellSize, Color.magenta);
            Debug.DrawLine(lowerLeft + new float3(0, 0, 1) * cellSize, lowerLeft + new float3(1, 0, 1) * cellSize, Color.magenta);
        }

        // [BurstCompile]
        // public struct HashParticlesJob : IJobParallelFor {
        //     [ReadOnly] public NativeArray<TempUnitHashAndIndex> Units;
        //     public NativeArray<HashAndIndex> HashAndIndices;
        //     public float CellSize;
        //     
        //     public void Execute(int index) {
        //         var unit = Units[index];
        //         var hash = Hash(Helpers.GetPosition(unit.Position, CellSize));
        //     
        //         HashAndIndices[index] = new HashAndIndex { Hash = hash, Index = index };
        //     }
        // }
    }

    [BurstCompile]
    public partial struct MoveToTargetJob : IJobEntity
    {
        [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;
        [ReadOnly] public float DeltaTime;

        public EntityCommandBuffer.ParallelWriter ECB;
        
        [BurstCompile]
        private void Execute(in TargetEntity target, ref MoveSpeed moveSpeed, ref CurveTimer curveTimer,
            in Entity targeter, [ChunkIndexInQuery] int sortKey)
        {
            if (!TransformLookup.HasComponent(target.Value))
            {
                moveSpeed.CurrentSpeedModifier = 0f;
                curveTimer.Reset();
                return;
            }

            var transform = TransformLookup[targeter];
            var currentTargetPosition = TransformLookup[target.Value].Position;
            
            if (math.distance(currentTargetPosition, transform.Position) <= 3f)
            {
                moveSpeed.CurrentSpeedModifier = 0f;
                curveTimer.Reset();
                return;
            }
            
            currentTargetPosition.y = transform.Position.y;
            var currentDir = math.normalizesafe(currentTargetPosition - transform.Position);
            transform.Position += currentDir * moveSpeed.CurrentSpeedModifier * moveSpeed.MaxSpeed * DeltaTime;
            transform.Rotation = quaternion.LookRotationSafe(currentDir, math.up());
            
            ECB.SetComponent(sortKey, targeter, transform);
        }
    }
}