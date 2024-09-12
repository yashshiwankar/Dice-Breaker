using Cinemachine;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    private CinemachineVirtualCamera CMVirtualCam;
    private float shakeTimer, startingIntensity, shakeTimerTotal;
    CinemachineBasicMultiChannelPerlin CMPerlin;
    public static CamShake instance { get; private set; }
    void Start()
    {
        if (instance  == null)
            instance = this;

        CMVirtualCam = GetComponent<CinemachineVirtualCamera>();
        CMPerlin = CMVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        CMPerlin.m_AmplitudeGain = 0;
    }

    public void ShakeCamera(float intensity, float time)
    {
        CMPerlin = CMVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        CMPerlin.m_AmplitudeGain = intensity;
        startingIntensity = intensity;
        shakeTimer = time;
        shakeTimerTotal = time;
    }
    
    void Update()
    {
        
        if(shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            CMPerlin = CMVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            CMPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, (1 - (shakeTimer / shakeTimerTotal)));
        }  
        
    }
}
