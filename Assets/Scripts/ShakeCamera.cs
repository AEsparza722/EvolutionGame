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

    public void ShakeCam (float intensity, float seconds, Vector3 attackPosition)
    {
        CinemachineBasicMultiChannelPerlin shakeComponent = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        Vector2 direction = Camera.main.transform.position - attackPosition;
        //20 = 0.6
        float distance = direction.magnitude;
        float normalizedDistance = 0f - (distance / 50f) * 1f; //Algoritmo cambiar tama;o de indicador
        float normalizedReal = (1 + normalizedDistance);
        intensity = intensity * normalizedReal;
        Debug.Log(intensity);

        shakeComponent.m_AmplitudeGain = intensity;
        shakeTimer = seconds;
    }



}
