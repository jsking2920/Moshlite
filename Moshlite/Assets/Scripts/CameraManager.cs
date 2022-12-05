using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineBrain camBrain;
    [SerializeField] private List<CinemachineVirtualCamera> cams;
    int camIndex = 0;

    private void Start()
    {
        foreach (CinemachineVirtualCamera cam in cams)
        {
            cam.enabled = false;
        }
        cams[0].enabled = true;
    }

    public void SwapToFirst()
    {
        cams[camIndex].enabled = false;
        cams[0].enabled = true;
        camIndex = 0;
    }

    public void SwapToRandom()
    {
        cams[camIndex].enabled = false;
        int r = Random.Range(0, cams.Count);
        cams[r].enabled = true;
        camIndex = r;
    }

    public void SwapToNext()
    {
        cams[camIndex].enabled = false;
        camIndex = camIndex + 1 >= cams.Count ? 0 : camIndex + 1;
        cams[camIndex].enabled = true; 
    }

    // Other Ideas
    // Camera shake
    // fisheye lens animation
    // low res render texture effect
}
