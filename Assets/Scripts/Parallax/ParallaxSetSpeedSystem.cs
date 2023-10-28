using Unity.Entities;

namespace Prototype.Parallax
{
    [UpdateInGroup(typeof(ParallaxSystemGroup))]
    public partial struct ParallaxSetSpeedSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (buff, setSpeedCommand, rootE) in SystemAPI.Query<DynamicBuffer<ParallaxObjects>, SetParallaxSpeedCommand>().WithEntityAccess())
            {
                foreach (var item in buff)
                {
                    SystemAPI.SetComponent<ParallaxSpeed>(item.entity, new ParallaxSpeed
                    {
                        Value = setSpeedCommand.Value
                    });
                }

                SystemAPI.SetComponentEnabled<SetParallaxSpeedCommand>(rootE, false);
            }
        }
    }

    [UpdateInGroup(typeof(ParallaxSystemGroup))]
    public partial struct ParallaxSetMoveVectorSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (buff, setSpeedCommand, rootE) in SystemAPI.Query<DynamicBuffer<ParallaxObjects>, SetMoveVectorCommand>().WithEntityAccess())
            {
                foreach (var item in buff)
                {
                    SystemAPI.SetComponent<ParallaxMoveVector>(item.entity, new ParallaxMoveVector
                    {
                        Value = new Unity.Mathematics.float3(setSpeedCommand.Value.x, setSpeedCommand.Value.y, 0) 
                    });
                }

                SystemAPI.SetComponentEnabled<SetMoveVectorCommand>(rootE, false);
            }
        }
    }
}
