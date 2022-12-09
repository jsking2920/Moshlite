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
    private Coroutine curAnimation = null;

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
        StopAllCoroutines();
        curAnimation = null;
        isAnimating = false;
        isFisheyeGoingUp = false;

        // vcam.LookAt = null;
        vcam.m_Lens.FieldOfView = defaultFOV;
        noiseModule.m_NoiseProfile = defaultNoiseSetting;
        noiseModule.m_AmplitudeGain = Random.Range(defaultNoiseAmplitude - 0.1f, defaultNoiseAmplitude + 0.1f);
        noiseModule.m_FrequencyGain = Random.Range(defaultNoiseFrequency - 0.1f, defaultNoiseFrequency + 0.1f);
    }

    public void SetLookAtTarget(Transform target)
    {
        vcam.LookAt = target;
    }

    public void ToggleFisheye(NoiseSettings shakeSetting)
    {
        if (isAnimating)
        {
            StopCoroutine(curAnimation);
            curAnimation = null;
            isAnimating = false;
        }

        if (isFisheyeGoingUp)
        {
            isAnimating = true;
            curAnimation = StartCoroutine(FisheyeOffAnimation(shakeSetting));
            isFisheyeGoingUp = false;
        }
        else
        {
            isAnimating = true;
            curAnimation = StartCoroutine(FisheyeOnAnimation(shakeSetting));
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

        curAnimation = null;
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
        curAnimation = null;
        ResetCam();
    }


    // Replaced all this garbage with an Impulse source + listener
    /*
    public void AddShake()
    {
        // Ignore command to add shake if fisheye is happening
        if (curFisheyeAnimation != null || isFisheyeGoingUp == true) return;

        isAnimating = true;
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        float amplitudeToAdd = 0.75f;
        float frequencyToAdd = 7.5f;

        for(int i = 0; i < 8; i++)
        {
            noiseModule.m_AmplitudeGain = Mathf.Clamp(noiseModule.m_AmplitudeGain + (amplitudeToAdd / 10.0f), 0.0f, defaultNoiseAmplitude + 8.0f);
            noiseModule.m_FrequencyGain = Mathf.Clamp(noiseModule.m_FrequencyGain + (frequencyToAdd / 10.0f), 0.0f, defaultNoiseFrequency + 35.0f);
            yield return new WaitForSeconds(0.02f);
        }

        StartCoroutine(DecreaseShake());
    }

    private IEnumerator DecreaseShake()
    {
        float amplitudeToDecrease = 0.75f;
        float frequencyToDecrease = 7.5f;

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < 10; i++)
        {
            noiseModule.m_AmplitudeGain = Mathf.Clamp(noiseModule.m_AmplitudeGain - (amplitudeToDecrease / 10.0f), 0.0f, defaultNoiseAmplitude + 8.0f);
            noiseModule.m_FrequencyGain = Mathf.Clamp(noiseModule.m_FrequencyGain - (frequencyToDecrease / 10.0f), 0.0f, defaultNoiseFrequency + 35.0f);
            yield return new WaitForSeconds(0.04f);
        }

        if (noiseModule.m_AmplitudeGain <= defaultNoiseAmplitude + 0.15f || noiseModule.m_FrequencyGain <= defaultNoiseFrequency + 1.0f)
        {
            isAnimating = false;
            noiseModule.m_AmplitudeGain = defaultNoiseAmplitude;
            noiseModule.m_FrequencyGain = defaultNoiseFrequency;
        }
    }
    */
}
