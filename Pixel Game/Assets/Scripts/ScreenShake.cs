using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{
    
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin virtualCameraNoise;
    [SerializeField]
    private float secondsToShake = 0.2f;

    void Start()
    {
        virtualCamera = gameObject.GetComponent<CinemachineVirtualCamera>();
        virtualCameraNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {

    }

    public void StartScreenShake()
    {
        StartCoroutine("ShakeScreen");
    }
    private IEnumerator ShakeScreen()
    {
        virtualCameraNoise.m_AmplitudeGain = 3f;
        yield return new WaitForSeconds(secondsToShake);
        virtualCameraNoise.m_AmplitudeGain = 0f;

    }
}
