using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDisabledEvent : MonoBehaviour
{
    public event Action OnDisabled = delegate { };
    private void OnDisable()
    {
        OnDisabled.Invoke();
    }
}