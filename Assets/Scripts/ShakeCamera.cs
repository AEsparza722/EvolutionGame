using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShakeCamera : MonoBehaviour
{
    public static ShakeCamera instance;
    float shakeTimer;
    CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0) 
            { 
            CinemachineBasicMultiChannelPerlin shakeComponent = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            shakeComponent.m_AmplitudeGain = 0;
            }
        }

    }

    public void ShakeCam (float intensity, float seconds)
    {
        CinemachineBasicMultiChannelPerlin shakeComponent = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        shakeComponent.m_AmplitudeGain = intensity;
        shakeTimer = seconds;
    }



}
