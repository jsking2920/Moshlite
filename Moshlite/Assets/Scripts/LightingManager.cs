using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    /* Other ideas for effects 
        - All lights off / on
        - Flicker lights
        - Make everyting red
        - Flare all the lights up for a big swell
    */

    public void FlickerEffect()
    {
        CharacterManager.S.Flicker();
    }

    public void ToggleLights()
    {
        CharacterManager.S.ToggleLights();
    }
}
