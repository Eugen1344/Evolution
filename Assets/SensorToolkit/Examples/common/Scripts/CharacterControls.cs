using UnityEngine;
using System.Collections;

namespace SensorToolkit.Example
{
    [RequireComponent(typeof(Rigidbody))]
    public class CharacterControls : MonoBehaviour 
    {
        public float MaxMoveForce;
        public float MaxStrafeForce;
        public float MaxTurnForce;
        public Sensor InteractionRange;
        public FixedJoint HoldSlot;

        Rigidbody rb;
        bool isInteracting;
        Holdable held;

        private Vector3 move;
        public Vector3 Move
        {
            get {return move;}
            set {move = value.sqrMagnitude > 1f ? value.normalized : value;}
        }

        private Vector3 face;
        public Vector3 Face
        {
            get {return face;}
            set {face = value.normalized;}
        }

        public Holdable Held { get { return held; } }

        public void PickUp(Holdable holdable)
        {
            if (held != null || isInteracting || !InteractionRange.IsDetected(holdable.gameObject) || holdable.IsHeld) return;
            else StartCoroutine(PickUpRoutine(holdable));
        }

        IEnumerator PickUpRoutine(Holdable holdable)
        {
            float countdown = holdable.PickupTime;
            isInteracting = true;

            while (countdown > 0f)
            {
                countdown -= Time.deltaTime;
                if (holdable.IsHeld || !InteractionRange.IsDetected(holdable.gameObject))
                {
                    // Conditions have changed, holdable can no longer be picked up
                    isInteracting = false;
                    yield break;
                }
                yield return null;
            }
            holdable.PickUp(gameObject);
            if (holdable.Holder == gameObject)
            {
                held = holdable;
                held.transform.position = HoldSlot.transform.position;
                held.transform.rotation = HoldSlot.transform.rotation;
                HoldSlot.connectedBody = held.GetComponent<Rigidbody>();
            }
            isInteracting = false;
        }

        float weightPenalty { get { return held != null ? held.WeightPenalty + 1f : 1f; } }

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            isInteracting = false;
            held = null;
        }

        void FixedUpdate()
        {
            // Turn to face the target direction
            var a = signedAngleXZ(rb.transform.forward, Face);
            var torque = Mathf.Clamp(a / 10f, -1f, 1f) * MaxTurnForce;
            rb.AddTorque(Vector3.up * torque / weightPenalty);

            // If we're interacting then we cannot move
            if (isInteracting) return;

            // Lerp the dot product of the direction I'm facing to the direction I'm moving,
            // this will interpolate between the strafing force and the moving force.
            float forwardDotMove = Vector3.Dot(transform.forward, Move.normalized);
            Vector3 moveForce = Mathf.Lerp(MaxStrafeForce, MaxMoveForce, Mathf.Clamp(forwardDotMove, 0f, 1f)) * Move;
            rb.AddForce(moveForce / weightPenalty);
        }

        float signedAngleXZ(Vector3 a, Vector3 b)
        {
            var aa = Mathf.Atan2(a.x, a.z) * Mathf.Rad2Deg;
            var ba = Mathf.Atan2(b.x, b.z) * Mathf.Rad2Deg;
            return Mathf.DeltaAngle(aa, ba);
        }
    }
}