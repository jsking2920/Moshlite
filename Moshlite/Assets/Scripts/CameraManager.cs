using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineBrain camBrain;
    [SerializeField] private List<MoshCam> cams;
    private int camIndex = 0;

    // Serialized noise profiles isnce getting them is a pain otherwise
    [SerializeField] private NoiseSettings handheldShake; // base
    [SerializeField] private NoiseSettings sixDWobble; // for fisheye effect
    [SerializeField] private NoiseSettings sixDShake; // for shake effect

    // Cinemachine blends
    private CinemachineBlendDefinition easeInAndOutBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 1.0f);
    private CinemachineBlendDefinition jumpCutBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0.0f);

    private void Start()
    {
        foreach (MoshCam cam in cams)
        {
            cam.vcam.enabled = false;
        }
        cams[0].vcam.enabled = true;
    }

    public void SwapToFirst(bool jumpCut = false)
    {
        if (jumpCut)
        {
            camBrain.m_DefaultBlend = jumpCutBlend;
        }
        else
        {
            camBrain.m_DefaultBlend = easeInAndOutBlend;
        }
        cams[camIndex].ResetCam(); // clean up animations that may be running still
        cams[camIndex].vcam.enabled = false;

        cams[0].vcam.enabled = true;
        camIndex = 0;
    }

    public void SwapToRandom(bool jumpCut = false)
    {
        if (cams.Count < 2) return; // prevents infinite loop

        if (jumpCut)
        {
            camBrain.m_DefaultBlend = jumpCutBlend;
        }
        else
        {
            camBrain.m_DefaultBlend = easeInAndOutBlend;
        }
        cams[camIndex].ResetCam();
        cams[camIndex].vcam.enabled = false;

        int r = UnityEngine.Random.Range(0, cams.Count);
        while (r == camIndex)
        {
            r = UnityEngine.Random.Range(0, cams.Count);
        }

        cams[r].vcam.enabled = true;
        camIndex = r;
    }

    public void SwapToNext(bool jumpCut = false)
    {
        if (jumpCut)
        {
            camBrain.m_DefaultBlend = jumpCutBlend;
        }
        else
        {
            camBrain.m_DefaultBlend = easeInAndOutBlend;
        }
        cams[camIndex].ResetCam();
        cams[camIndex].vcam.enabled = false;

        camIndex = camIndex + 1 >= cams.Count ? 0 : camIndex + 1;
        cams[camIndex].vcam.enabled = true; 
    }

    public void ToggleFisheye()
    {
        cams[camIndex].ToggleFisheye(sixDWobble);
    }
}
