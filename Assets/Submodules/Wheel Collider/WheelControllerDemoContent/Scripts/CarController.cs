using System;
using System.Collections.Generic;
using NWH.NPhysics;
using UnityEngine;

/// <summary>
/// Simple vehicle controller intended as a demo script to help showcase WC3D.
/// If you need a complete vehicle physics package that uses WC3D check out NWH Vehicle Physics.
/// Owners of WC3D get 30% off: https://assetstore.unity.com/packages/tools/physics/nwh-vehicle-physics-107332
/// </summary>
namespace NWH.WheelController3D
{
    [Serializable]
    public class _Wheel
    {
        public bool            power;
        public bool            steer;
        public WheelController wheelController;
    }

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NRigidbody))]
    public class CarController : Vehicle
    {
        [Range(3, 40)]
        public int physicsSubsteps = 15;

        [SerializeField]
        public List<_Wheel> wheels;

        private NRigidbody _nRigidbody;


        private void Awake()
        {
            _nRigidbody = GetComponent<NRigidbody>();
            if (_nRigidbody == null) gameObject.AddComponent<NRigidbody>();

            _nRigidbody.Substeps = physicsSubsteps;
        }


        public void FixedUpdate()
        {
            if (vehicleIsActive)
            {
                _nRigidbody.Substeps = physicsSubsteps;

                xAxis = Input.GetAxis("Horizontal");
                yAxis = Input.GetAxis("Vertical");

                float absYAxis = yAxis < 0 ? -yAxis : yAxis;

                velocity    = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).z;
                smoothXAxis = Mathf.SmoothDamp(smoothXAxis, xAxis, ref xAxisVelocity, 0.12f);

                float motorTorque = torqueSpeedCurve.Evaluate(Mathf.Abs(velocity * 0.02f)) * maxMotorTorque;

                foreach (_Wheel w in wheels)
                {
                    w.wheelController.brakeTorque = 0f;
                    w.wheelController.motorTorque = 0f;

                    if (Input.GetKey(KeyCode.Space)) w.wheelController.brakeTorque = maxBrakeTorque;

                    if (velocity < -0.4f && yAxis > 0.1f || velocity > 0.4f && yAxis < -0.1f)
                        w.wheelController.brakeTorque = maxBrakeTorque * Mathf.Abs(yAxis);

                    if (w.wheelController.brakeTorque < 0.01f)
                        if (velocity >= -0.5f && yAxis > 0.1f || velocity <= 0.5f && yAxis < -0.1f)
                            w.wheelController.motorTorque = motorTorque * yAxis;


                    if (w.steer)
                        w.wheelController.steerAngle =
                            Mathf.Lerp(maxSteeringAngle, minSteeringAngle, Mathf.Abs(velocity) * 0.05f) * xAxis;
                }
            }

            ApplyAntirollBar();
        }


        public void ApplyAntirollBar()
        {
            for (int i = 0; i < wheels.Count; i += 2)
            {
                WheelController leftWheel  = wheels[i].wheelController;
                WheelController rightWheel = wheels[i + 1].wheelController;

                if (!leftWheel.springOverExtended && !leftWheel.springBottomedOut && !rightWheel.springOverExtended &&
                    !rightWheel.springBottomedOut)
                {
                    float leftTravel  = leftWheel.springTravel;
                    float rightTravel = rightWheel.springTravel;

                    float arf = (leftTravel - rightTravel) * antiRollBarForce;

                    if (leftWheel.isGrounded)
                        leftWheel.parent.GetComponent<Rigidbody>()
                                 .AddForceAtPosition(leftWheel.wheel.up * -arf, leftWheel.wheel.worldPosition);

                    if (rightWheel.isGrounded)
                        rightWheel.parent.GetComponent<Rigidbody>()
                                  .AddForceAtPosition(rightWheel.wheel.up * arf, rightWheel.wheel.worldPosition);
                }
            }
        }


        public void OnBrakeValueChanged(float a)
        {
            maxBrakeTorque = a;
        }


        public void OnMotorValueChanged(float v)
        {
            maxMotorTorque = v;
        }
    }
}