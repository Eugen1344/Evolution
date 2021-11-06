using UnityEngine;

namespace NWH.WheelController3D
{
    public class CameraDefault : MonoBehaviour
    {
        public  Vector3   desiredPosition = Vector3.zero;
        public  float     Distance;
        public  float     DistanceMax           = 10.0f;
        public  float     DistanceMin           = 3.0f;
        public  float     DistanceSmooth        = 0.05f;
        public  float     MouseWheelSensitivity = 5.0f;
        public  Transform TargetLookAt;
        public  float     X_MouseSensitivity = 5.0f;
        public  float     X_Smooth           = 0.05f;
        public  float     Y_MaxLimit         = 80.0f;
        public  float     Y_MinLimit         = -40.0f;
        public  float     Y_MouseSensitivity = 5.0f;
        public  float     Y_Smooth           = 0.1f;
        private float     desiredDistance;
        private float     mouseX;
        private float     mouseY;
        private Vector3   position;
        private float     startingDistance;
        private float     velocityDistance;
        private float     velX;
        private float     velY;
        private float     velZ;


        private void Awake()
        {
            position = transform.position;
        }


        private void Start()
        {
            Distance         = Mathf.Clamp(Distance, DistanceMin, DistanceMax);
            startingDistance = Distance;
            Reset();
        }


        private void Update()
        {
            if (TargetLookAt == null) return;

            HandlePlayerInput();
            CalculateDesiredPosition();
            UpdatePosition();
        }


        public void CalculateDesiredPosition()
        {
            // Evaluate distance
            Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velocityDistance, DistanceSmooth);

            // Calculate desired position -> Note : mouse inputs reversed to align to WorldSpace Axis
            desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
        }


        public Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
        {
            Vector3    direction = new Vector3(0, 0, -distance);
            Quaternion rotation  = Quaternion.Euler(rotationX, rotationY, 0);
            return TargetLookAt.position + rotation * direction;
        }


        public float ClampAngle(float angle, float min, float max)
        {
            while (angle < -360 || angle > 360)
            {
                if (angle < -360) angle += 360;

                if (angle > 360) angle -= 360;
            }

            return Mathf.Clamp(angle, min, max);
        }


        public void HandlePlayerInput()
        {
            float deadZone = 0.01f; // mousewheel deadZone

            if (Input.GetMouseButton(0))
            {
                mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
                mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;
            }

            // this is where the mouseY is limited - Helper script
            mouseY = ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);

            // get Mouse Wheel Input
            if (Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone)
                desiredDistance = Mathf.Clamp(Distance - Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity,
                                              DistanceMin, DistanceMax);
        }


        public void Reset()
        {
            mouseX          = 0;
            mouseY          = 10;
            Distance        = startingDistance;
            desiredDistance = Distance;
        }


        public void UpdatePosition()
        {
            float posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth);
            float posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth);
            float posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, X_Smooth);
            position = new Vector3(posX, posY, posZ);

            transform.position = position;

            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.down, out hit, 0.7f))
                position = new Vector3(posX, hit.point.y + 0.7f, posZ);

            transform.LookAt(TargetLookAt);
        }
    }
}