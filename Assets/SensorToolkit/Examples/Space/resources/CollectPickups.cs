using UnityEngine;
using System.Collections;

namespace SensorToolkit.Example
{
    public class CollectPickups : MonoBehaviour
    {
        public Sensor PickupSensor;
        public Sensor InteractionRange;
        public SteeringRig Steering;

        Holdable target;

        void Update()
        {
            // If we don't currently have a target pickup then target the nearest one. If we do
            // have a target and we are within detection range then pick it up.
            if (target == null)
            {
                target = PickupSensor.GetNearestByComponent<Holdable>();
                if (target != null)
                {
                    Steering.IgnoreList.Clear();
                    Steering.IgnoreList.Add(target.gameObject);
                    Steering.DestinationTransform = target.transform;
                }
            }
            else if (InteractionRange.IsDetected(target.gameObject))
            {
                // Pickup the target.. (Destroy it to show it has been picked up)
                Destroy(target.gameObject);
                target = null;
            }
        }
    }
}