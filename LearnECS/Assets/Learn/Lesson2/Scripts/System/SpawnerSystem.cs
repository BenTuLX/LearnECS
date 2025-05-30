using Unity.Entities;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Transforms;

namespace Learn.Lesson2.Scripts
{
    [BurstCompile]
    [RequireMatchingQueriesForUpdate]
    public partial struct SpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RandomCom>();
            state.RequireForUpdate<Spawner>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            // 获取RandomCom组件
            var random = SystemAPI.GetSingletonRW<RandomCom>();
            
            EntityCommandBuffer.ParallelWriter ecb = GetEntiyCommandBuffer(ref state);

            new ProcessSpawnerJob
            {
                Random = random,
                Ecb = ecb,
                ElapsedTime = SystemAPI.Time.ElapsedTime
            }.ScheduleParallel();
        }

        private EntityCommandBuffer.ParallelWriter GetEntiyCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb.AsParallelWriter();
        }
    }

    [BurstCompile]
    public partial struct ProcessSpawnerJob : IJobEntity
    {
        [NativeDisableUnsafePtrRestriction]public RefRW<RandomCom> Random;
        public EntityCommandBuffer.ParallelWriter Ecb;
        public double ElapsedTime;
        
        private void Execute([ChunkIndexInQuery] int chunkIndex, ref Spawner spawner)
        {
            if (spawner.NextSpawnTime < ElapsedTime)
            {
                Entity newEntity = Ecb.Instantiate(chunkIndex, spawner.Prefab);
                float3 pos = new float3(Random.ValueRW.random.NextFloat(-5f,5f), Random.ValueRW.random.NextFloat(-3f,3f),0);
                Ecb.SetComponent(chunkIndex, newEntity, LocalTransform.FromPosition(pos));
                spawner.NextSpawnTime = (float)ElapsedTime + spawner.SpawnRate;
            }
        }
    }
}

