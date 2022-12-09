using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Handles events triggered by inputs from drumset (or whatever midi instrument)
// See player input asset and/or this for mappings: https://rolandus.zendesk.com/hc/en-us/articles/115000201706-TD-25-Default-MIDI-Note-Number-Map
[RequireComponent(typeof(PlayerInput))]
public class DrumInputManager : MonoBehaviour
{
    public bool debugMode = false;

    [SerializeField] private CharacterManager _characterManager;
    [SerializeField] private vfxManager _vfxManager;
    [SerializeField] private CameraManager _cameraManager;
    [SerializeField] private LightingManager _lightingManager;

    enum Preset { main, breakdown };
    private Preset _curPreset = Preset.main;

    // All functions triggered by unity event from Player Input component
    #region Player Input Callbacks
    public void OnDeviceLost()
    {
        Debug.Log("Device lost!");
    }
    
    public void OnDeviceRegained()
    {
        Debug.Log("Device regained!");
    }

    public void OnControlsChanged()
    {
        Debug.Log("Controls changed!");
    }
    #endregion

    #region Action callbacks
    public void OnKick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: <space>
        if (debugMode) Debug.Log("Kick!");

        switch (_curPreset)
        {
            case Preset.main:
                _characterManager.RandomSmallDance();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnSnare(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: b
        if (debugMode) Debug.Log("Snare!");

        switch (_curPreset)
        {
            case Preset.main:
                _characterManager.RandomMosh();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnSnareRim(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: 1
        if (debugMode) Debug.Log("Snare rim");

        switch (_curPreset)
        {
            case Preset.main:
                _vfxManager.ToggleWatercolor();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnHiHatOpenBow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: v
        if (debugMode) Debug.Log("hi hat open bow");

        switch (_curPreset)
        {
            case Preset.main:
                _characterManager.MoshTowardsCenter();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnHiHatOpenEdge(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: c
        if (debugMode) Debug.Log("hi hat open edge");

        switch (_curPreset)
        {
            case Preset.main:
                _characterManager.CrowdKill();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnHiHatClosedBow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: x
        if (debugMode) Debug.Log("hi hat closed bow");

        switch (_curPreset)
        {
            case Preset.main:
                _characterManager.BangHeads();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnHiHatPedal(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: z
        if (debugMode) Debug.Log("hi hat pedal");

        switch (_curPreset)
        {
            case Preset.main:
                _characterManager.PulseScale();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnLeftCrashBow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: d
        if (debugMode) Debug.Log("left crash bow");

        switch (_curPreset)
        {
            case Preset.main:
                _cameraManager.ToggleFisheye(); // Makes lens toggle between high FOV + shake and normal
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnLeftCrashEdge(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: f
        if (debugMode) Debug.Log("left crash edge");

        switch (_curPreset)
        {
            case Preset.main:
                _cameraManager.AddShake(); // Shake camera additively
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnRightCrashBow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: y
        if (debugMode) Debug.Log("right crash bow");

        switch (_curPreset)
        {
            case Preset.main:
                _characterManager.Jump();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnRightCrashEdge(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: t
        if (debugMode) Debug.Log("right crash edge");

        switch (_curPreset)
        {
            case Preset.main:
                _characterManager.RandomMosh();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnRideBow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: j
        if (debugMode) Debug.Log("ride bow");

        switch (_curPreset)
        {
            case Preset.main:
                _vfxManager.OnReset();
                _characterManager.OnReset();
                _cameraManager.OnReset();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnRideEdge(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: u
        if (debugMode) Debug.Log("ride edge");

        switch (_curPreset)
        {
            case Preset.main:
                _cameraManager.SwapToRandom(true); // false for ease in/out; true for jump cut
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnRideBell(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: i
        if (debugMode) Debug.Log("ride bell");

        switch (_curPreset)
        {
            case Preset.main:
                _cameraManager.SwapToFirst(false); // false for ease in/out; true for jump cut
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnTom1(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: g
        if (debugMode) Debug.Log("tom 1");

        switch (_curPreset)
        {
            case Preset.main:
                _vfxManager.ToggleRed();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnTom1Rim(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: 2
        if (debugMode) Debug.Log("tom 1 rim");

        switch (_curPreset)
        {
            case Preset.main:
                _vfxManager.ToggleWacky();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnTom2(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: h
        if (debugMode) Debug.Log("tom 2");

        switch (_curPreset)
        {
            case Preset.main:
                _characterManager.ReverseGravityZones();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnTom2Rim(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: 3
        if (debugMode) Debug.Log("tom 2 rim");

        switch (_curPreset)
        {
            case Preset.main:
                _characterManager.ToggleGravity();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnTom3(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: n
        if (debugMode) Debug.Log("tom 3");

        switch (_curPreset)
        {
            case Preset.main:
                _lightingManager.ToggleLights();
                break;
            case Preset.breakdown:
                break;
        }
    }

    public void OnTom3Rim(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        // Debug keyboard key: 4
        if (debugMode) Debug.Log("tom 3 rim");

        switch (_curPreset)
        {
            case Preset.main:
                _lightingManager.FlickerEffect();
                break;
            case Preset.breakdown:
                break;
        }
    }
    #endregion
}
