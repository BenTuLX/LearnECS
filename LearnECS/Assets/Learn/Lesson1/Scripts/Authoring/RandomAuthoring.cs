using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Learn.Lesson1.Scripts
{
    public struct RandomCom : IComponentData
    {
        public Random random;
    }

    public class RandomAuthoring : MonoBehaviour
    {
        public uint seed = 1;
    }

    class RandomBaker : Baker<RandomAuthoring>
    {
        public override void Bake(RandomAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new RandomCom { random = new Random(authoring.seed) });
        }
    }
}