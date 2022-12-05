using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoshCam : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    private CinemachineBasicMultiChannelPerlin noiseModule;
    private float defaultFOV;

    private NoiseSettings defaultNoiseSetting; // should be handheld mild
    private float defaultNoiseAmplitude = 1.0f; // set to 0 if this cam shouldn't shake by default
    private float defaultNoiseFrequency = 0.5f;

    private bool isAnimating = false;
    private Coroutine curFisheyeAnimation = null;

    private bool isFisheyeGoingUp = false;

    private void Start()
    {
        noiseModule = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        defaultFOV = vcam.m_Lens.FieldOfView;
        defaultNoiseSetting = noiseModule.m_NoiseProfile;
        defaultNoiseAmplitude = noiseModule.m_AmplitudeGain;
        defaultNoiseFrequency = noiseModule.m_FrequencyGain;
        ResetCam();
    }

    public void ResetCam()
    {
        if (isAnimating)
        {
            StopAllCoroutines();
            curFisheyeAnimation = null;
            isAnimating = false;
        }

        vcam.m_Lens.FieldOfView = defaultFOV;
        noiseModule.m_NoiseProfile = defaultNoiseSetting;
        noiseModule.m_AmplitudeGain = Random.Range(defaultNoiseAmplitude - 0.1f, defaultNoiseAmplitude + 0.1f);
        noiseModule.m_FrequencyGain = Random.Range(defaultNoiseFrequency - 0.1f, defaultNoiseFrequency + 0.1f);
    }

    public void ToggleFisheye(NoiseSettings shakeSetting)
    {
        if (isAnimating)
        {
            StopAllCoroutines();
            curFisheyeAnimation = null;
            isAnimating = false;
        }

        if (isFisheyeGoingUp)
        {
            isAnimating = true;
            curFisheyeAnimation = StartCoroutine(FisheyeOffAnimation(shakeSetting));
            isFisheyeGoingUp = false;
        }
        else
        {
            isAnimating = true;
            curFisheyeAnimation = StartCoroutine(FisheyeOnAnimation(shakeSetting));
            isFisheyeGoingUp = true;
        }

    }

    public IEnumerator FisheyeOnAnimation(NoiseSettings shakeSetting)
    {
        float maxFOV = defaultFOV + 60.0f;

        // use 6D wobble for this shake effect
        noiseModule.m_NoiseProfile = shakeSetting;
        float peakShakeAmplitude = 10.0f;
        float peakShakeFrequency = 2.0f;

        float i = Mathf.InverseLerp(defaultFOV, maxFOV, vcam.m_Lens.FieldOfView);
        while (i < 1.0f)
        {
            i += 4.0f * Time.deltaTime;
            vcam.m_Lens.FieldOfView = Mathf.Lerp(defaultFOV, maxFOV, i);
            noiseModule.m_AmplitudeGain = Mathf.Lerp(defaultNoiseAmplitude, peakShakeAmplitude, i);
            noiseModule.m_FrequencyGain = Mathf.Lerp(defaultNoiseFrequency, peakShakeFrequency, i);
            yield return null;
        }
        vcam.m_Lens.FieldOfView = maxFOV;
        noiseModule.m_AmplitudeGain = peakShakeAmplitude;
        noiseModule.m_FrequencyGain = peakShakeFrequency;

        curFisheyeAnimation = null;
        isAnimating = false;
    }

    public IEnumerator FisheyeOffAnimation(NoiseSettings shakeSetting)
    {
        float maxFOV = defaultFOV + 60.0f;

        // use 6D wobble for this shake effect
        noiseModule.m_NoiseProfile = shakeSetting;
        float peakShakeAmplitude = 10.0f;
        float peakShakeFrequency = 2.0f;

        float i = Mathf.InverseLerp(defaultFOV, maxFOV, vcam.m_Lens.FieldOfView);

        while (i > 0.0f)
        {
            i -= 4.0f * Time.deltaTime;
            vcam.m_Lens.FieldOfView = Mathf.Lerp(defaultFOV, maxFOV, i);
            noiseModule.m_AmplitudeGain = Mathf.Lerp(defaultNoiseAmplitude, peakShakeAmplitude, i);
            noiseModule.m_FrequencyGain = Mathf.Lerp(defaultNoiseFrequency, peakShakeFrequency, i);
            yield return null;
        }
        isAnimating = false;
        curFisheyeAnimation = null;
        ResetCam();
    }

    public void AddShake()
    {
        if (curFisheyeAnimation != null) return;

        isAnimating = true;
        noiseModule.m_AmplitudeGain = Mathf.Clamp(noiseModule.m_AmplitudeGain + 1.0f, 0.0f, defaultNoiseAmplitude + 10.0f);
        noiseModule.m_FrequencyGain += 0.5f;
        StartCoroutine(DecreaseShake()); // reduces shake back to normal after a delay
    }

    private IEnumerator DecreaseShake()
    {
        yield return new WaitForSeconds(0.5f);
        noiseModule.m_AmplitudeGain -= 1.0f;
        noiseModule.m_FrequencyGain -= 0.5f;
    }
}
