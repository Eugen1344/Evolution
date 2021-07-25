using UnityEngine;
using System.Collections;

namespace SensorToolkit.Example
{
    public class Holdable : MonoBehaviour
    {
        public float PickupTime;
        public float WeightPenalty;

        Collider[] colliders;
        bool collidersDisabled;

        public GameObject Holder { get; private set; }
        public bool IsHeld { get { return Holder != null; } }

        public void PickUp(GameObject holder)
        {
            if (!IsHeld) Holder = holder;
        }

        public void Drop()
        {
            Holder = null;
        }

        void Start()
        {
            colliders = GetComponentsInChildren<Collider>();
            collidersDisabled = false;
        }

        void Update()
        {
            if (IsHeld && !collidersDisabled)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = false;
                    collidersDisabled = true;
                }
            }
            else if (!IsHeld && collidersDisabled)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = true;
                    collidersDisabled = false;
                }
            }
        }
    }
}