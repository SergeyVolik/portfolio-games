using Unity.Entities;
using UnityEngine;

namespace Prototype.Audio
{
    public class SFXDatabaseComponent : IComponentData
    {
        public AudioSFXDatabase value;
    }

    public class SFXDatabaseAuthoring : MonoBehaviour
    {
        public AudioSFXDatabase database;

        class Baker : Baker<SFXDatabaseAuthoring>
        {
            public override void Bake(SFXDatabaseAuthoring authoring)
            {

                var e = GetEntity(TransformUsageFlags.None);


                AddComponentObject(e, new SFXDatabaseComponent
                {
                    value = authoring.database
                });
            }
        }
    }
}