using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(ParticleSystem))]
public class OnParticleStoppedEvent : MonoBehaviour
{
    public event Action OnParticleStopped = delegate { };
    private void OnParticleSystemStopped()
    {
        OnParticleStopped.Invoke();
    }


}