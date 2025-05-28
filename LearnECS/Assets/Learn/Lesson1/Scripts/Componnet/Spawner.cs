using Unity.Entities;
using Unity.Mathematics;

namespace Learn.Lesson1.Scripts
{
    /// 继承IComponentData
    /// 类型为struct，表示为非托管类型的Component
    public struct Spawner : IComponentData
    {
        public Entity Prefab; // 要实例化的prefab， 通过backing将子场景中的prefab转换得到
        public float3 SpawnPosition; // 生成的位置
        public float NextSpawnTime; // 下次生成的时间
        public float SpawnRate; // 生成的频率
    }
}