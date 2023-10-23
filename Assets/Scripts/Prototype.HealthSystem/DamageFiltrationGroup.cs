using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Prototype.HealthSystem
{
    [UpdateInGroup(typeof(HealthSystemGroup))]
    [UpdateBefore(typeof(ApplyDamageSystem))]
    public partial class DamageFiltrationGroup : ComponentSystemGroup { }
  
}
