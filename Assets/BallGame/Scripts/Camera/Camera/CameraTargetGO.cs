using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetGO : MonoBehaviour
{
    public static CameraTargetGO Instance;

    public CinemachineBrain brain = null;
    private void Awake()
    {
        Instance = this;
    }
}
