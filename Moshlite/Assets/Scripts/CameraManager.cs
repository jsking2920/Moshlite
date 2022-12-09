using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineBrain camBrain;
    private CinemachineImpulseSource impulseSource;
    [SerializeField] private List<MoshCam> cams;
    private int camIndex = 0;

    // Serialized noise profiles isnce getting them is a pain otherwise
    [SerializeField] private NoiseSettings handheldShake; // base
    [SerializeField] private NoiseSettings sixDWobble; // for fisheye effect

    // Cinemachine blends
    private CinemachineBlendDefinition easeInAndOutBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 1.0f);
    private CinemachineBlendDefinition jumpCutBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0.0f);

    /* Other ideas for effects 
        - Jump cut back and forth between two cameras quickly
        - Use velocity to determine things like go to next or prev cam, jump cut or pan, intensity of shake
        - Rapidly transition through all cams for a bit
        - ***Set depth of field post processing parameters for each individual cam***
    */

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();

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

    public void AddShake()
    {
        impulseSource.GenerateImpulseWithForce(5.0f); // TODO: figure out how to scale this by velocity of midi note
    }

    public void OnReset()
    {
        cams[camIndex].ResetCam();
    }
}
