using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace Prototype
{
    //[UpdateBefore(typeof(LocalToWorldSystem))]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class SpawnerSystemGroup : ComponentSystemGroup
    {

    }
}