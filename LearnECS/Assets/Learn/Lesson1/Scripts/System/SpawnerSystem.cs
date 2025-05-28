using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

namespace Learn.Lesson1.Scripts
{
    [BurstCompile]
    public partial struct SpawnerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }

        public void OnDestroy(ref SystemState state)
        {
        }

        public void OnUpdate(ref SystemState state)
        {
            // 获取RandomCom组件
            var random = SystemAPI.GetSingletonRW<RandomCom>();
            // 通过SystemAPI.Query获取所有Spawner组件
            // 因为需要对spawner进行写入，用RefRW进行封装。 如果只读，则用RefRO，节省性能。 在获取组件字段时，也尽量遵循这个原则
            foreach (var spawner in SystemAPI.Query<RefRW<Spawner>>())
            {
                if (spawner.ValueRO.NextSpawnTime < SystemAPI.Time.ElapsedTime)
                {
                    // 实例化spawner中的Prefab
                    var newEntity = state.EntityManager.Instantiate(spawner.ValueRO.Prefab);
                    // 设置初始位置，更新LocalTransform组件的值
                    // 这里通过LocalTransform.FromPosition api得到一个新的LocalTransform组件
                    //state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPosition(spawner.ValueRO.SpawnPosition));
                    
                    // 计算随机位置
                    float3 pos = new float3(random.ValueRW.random.NextFloat(-5f,5f), random.ValueRW.random.NextFloat(-3f,3f),0);
                    state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPosition(pos));
                    spawner.ValueRW.NextSpawnTime = spawner.ValueRO.SpawnRate + (float)SystemAPI.Time.ElapsedTime;
                }
            }
        }
    }
}

