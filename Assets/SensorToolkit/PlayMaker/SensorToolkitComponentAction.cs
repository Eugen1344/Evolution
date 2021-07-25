#if PLAYMAKER

using HutongGames.PlayMaker;
using UnityEngine;

namespace SensorToolkit.PlayMaker
{
    public abstract class SensorToolkitComponentAction<T> : FsmStateAction where T : Component
    {
        private GameObject cachedGameObject;
        protected T component;

        protected RaySensor raySensor { get { return component as RaySensor; } }
        protected RaySensor2D raySensor2D { get { return component as RaySensor2D; } }
        protected RangeSensor rangeSensor { get { return component as RangeSensor; } }
        protected TriggerSensor triggerSensor { get { return component as TriggerSensor; } }
        protected Sensor sensor { get { return component as Sensor; } }
        protected BaseVolumeSensor volumeSensor { get { return component as BaseVolumeSensor; } }
        protected BaseAreaSensor areaSensor { get { return component as BaseAreaSensor; } }
        protected FOVCollider fovCollider { get { return component as FOVCollider; } }
        protected FOVCollider2D fovCollider2D { get { return component as FOVCollider2D; } }
        protected SteeringRig steeringRig { get { return component as SteeringRig; } }
        protected SteeringRig2D steeringRig2D { get { return component as SteeringRig2D; } }

        protected bool UpdateCache(GameObject go)
        {
            if (go == null)
            {
                return false;
            }

            if (component == null || cachedGameObject != go)
            {
                component = go.GetComponent<T>();
                cachedGameObject = go;

                if (component == null)
                {
                    LogWarning("Missing component: " + typeof(T).FullName + " on: " + go.name);
                }
            }

            return component != null;
        }
    }
}

#endif