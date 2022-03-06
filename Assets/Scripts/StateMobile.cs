using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMobile : MonoBehaviour
{
    [SerializeField] private float shakeDetectionThreshold = 2f;

    [Space(5)]

    [SerializeField] private float rotationThreshold = 60f;

    private float accelerometerUpdateInterval;
    private float lowPassKernelWidthInSeconds = 1f;
    private float lowPassFilterFactor;

    private Vector3 lowPassValue;

    private bool upsideDown = false;
    private bool shaked = false;

    private void Start()
    {
        accelerometerUpdateInterval = Time.deltaTime;

        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;

        //ClientManager.upsideDownChange(upsideDown);
        //ClientManager.ShakedChange(Shaked);
    }

    private void Update()
    {

        

        bool isShaking;
        bool upSideDownState;

        Vector3 acceleration = Input.acceleration;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        Vector3 deltaAcceleration = acceleration - lowPassValue;

        isShaking = deltaAcceleration.magnitude >= shakeDetectionThreshold;
        upSideDownState = Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown;


        /*if(Input.gyro.attitude.z > rotationThreshold || Input.gyro.attitude.y > rotationThreshold)
            upSideDownState = true;
        else
            upSideDownState = false;*/

        if (shaked != isShaking)
        {
            isShaking = shaked;
            ClientManager.ShakedChange(shaked);
        }

        if(upsideDown != upSideDownState)
        {
            upsideDown = upSideDownState;
            ClientManager.UpsideDownChange(upsideDown);
        }

        Debug.Log(upSideDownState);
        //Debug.Log("z rotation : " + Input.gyro.attitude.z + " y rotation : " + Input.gyro.attitude.y + " x rotation : " + Input.gyro.attitude.x);
        Debug.Log("mobile acceleration : " + deltaAcceleration);
    }
}
