using UnityEngine;
using System.Collections;

namespace SensorToolkit.Example
{
    [RequireComponent(typeof(CharacterControls))]
    public class PlayerInput : MonoBehaviour
    {
        public Sensor InteractionRange;

        CharacterControls cc;
        
        void Start()
        {
            cc = GetComponent<CharacterControls>();
        }

        void Update()
        {
            var horiz = Input.GetAxis("Horizontal");
            var vert = Input.GetAxis("Vertical");
            cc.Move = new Vector3(horiz, 0f, vert);

            // Project mouse position onto worlds plane at y=0
            var mousePosScreen = Input.mousePosition;
            var camPosition = Camera.main.transform.position;
            var camForward = Camera.main.transform.forward;
            var camDepthToGroundPlane = -camPosition.y / camForward.y;
            mousePosScreen.z = camDepthToGroundPlane;
            var mousePosGroundPlane = Camera.main.ScreenToWorldPoint(mousePosScreen);
            cc.Face = (mousePosGroundPlane - transform.position).normalized;

            // Pickup the pickup if its in range
            var pickup = InteractionRange.GetNearestByComponent<Holdable>();
            if (pickup != null && !pickup.IsHeld)
            {
                cc.PickUp(pickup);
            }
        }
    }
}