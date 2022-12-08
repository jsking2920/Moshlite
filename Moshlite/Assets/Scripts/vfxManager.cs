using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class vfxManager : MonoBehaviour
{
    /* Other ideas for effects 
        - Invert colors
        - lower res of render texture 
        - Dithering / switch to really low res color space to create a lot of banding
        - Weird projection
        - bring blur and bloom way up for a bit
        - Moth particle effects
        - Bokeh depth of field
        - Rewind effect
        - Animate blend between pp volumes
    */

    // TODO: figure out how to edit profile settings in script instead of toggling nearl identical volumes
    [SerializeField] private Volume baseVolume;
    [SerializeField] private Volume redVolume;
    [SerializeField] private Volume wackyVolume;

    private VolumeState state = VolumeState.normal;
    private enum VolumeState { normal, red, wacky };

    private void Start()
    {
        baseVolume.enabled = true;
        redVolume.enabled = false;
        wackyVolume.enabled = false;
    }

    public void ToggleRed()
    {
        switch (state)
        {
            case VolumeState.normal:
                baseVolume.enabled = false;
                redVolume.enabled = true;
                state = VolumeState.red;
                break;
            case VolumeState.red:
                baseVolume.enabled = true;
                redVolume.enabled = false;
                state = VolumeState.normal;
                break;
            case VolumeState.wacky:
                wackyVolume.enabled = false;
                redVolume.enabled = true;
                state = VolumeState.red;
                break;
        }
    }

    public void ToggleWacky()
    {
        switch (state)
        {
            case VolumeState.normal:
                baseVolume.enabled = false;
                wackyVolume.enabled = true;
                state = VolumeState.wacky;
                break;
            case VolumeState.red:
                wackyVolume.enabled = true;
                redVolume.enabled = false;
                state = VolumeState.wacky;
                break;
            case VolumeState.wacky:
                wackyVolume.enabled = false;
                baseVolume.enabled = true;
                state = VolumeState.normal;
                break;
        }
    }
}
