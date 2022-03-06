using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMobile : MonoBehaviour
{
    [SerializeField] private float shakeDetectionThreshold = 0.65f;

    [Space(5)]

    [SerializeField] private float rotationThreshold = 60f;

    private float accelerometerUpdateInterval;
    private float lowPassKernelWidthInSeconds = 1f;
    private float lowPassFilterFactor;

    private Vector3 lowPassValue;

    private bool upsideDown = false;
    private bool shaked = false;

    private bool changeThisFrame = false;

    private float changeTimeShaked;

    private void Start()
    {
        accelerometerUpdateInterval = Time.deltaTime;

        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
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

        isShaking = deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold;
        upSideDownState = Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown;

        /*if(Input.gyro.attitude.z > rotationThreshold || Input.gyro.attitude.y > rotationThreshold)
            upSideDownState = true;
        else
            upSideDownState = false;*/

        changeTimeShaked += Time.deltaTime;

        if (shaked != isShaking && !changeThisFrame)
        {
            changeThisFrame = true;
            changeTimeShaked = 0f;
        }

        if (changeTimeShaked > 0.5f || isShaking)
        {
            changeThisFrame = false;
            shaked = isShaking;
        }

        if (upsideDown != upSideDownState)
        {
            upsideDown = upSideDownState;
        }

        //ClientManager.ShakedChange(shaked);

        //ClientManager.UpsideDownChange(upsideDown);

        Debug.Log("Shaked : " + shaked + ", UpsideDown " + upsideDown + ", Acceleration : " + acceleration.sqrMagnitude);
    }
}
