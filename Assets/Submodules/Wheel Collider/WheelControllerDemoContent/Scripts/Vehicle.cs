using UnityEngine;

namespace NWH.WheelController3D
{
    public class Vehicle : MonoBehaviour
    {
        public float antiRollBarForce;
        public float maxBrakeTorque;

        public float maxMotorTorque;

        public float maxSteeringAngle = 35;
        public float minSteeringAngle = 20;
        public bool  vehicleIsActive = true;

        public AnimationCurve torqueSpeedCurve;

        [HideInInspector]
        public float velocity;

        protected float smoothXAxis;
        protected float xAxis;
        protected float xAxisVelocity;
        protected float yAxis;


        public void Active(bool state)
        {
            vehicleIsActive = state;
        }
    }
}