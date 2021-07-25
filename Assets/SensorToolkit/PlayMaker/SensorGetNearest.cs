#if PLAYMAKER

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker
{
    [ActionCategory("Sensors")]
    [Tooltip("Query a sensor for it's nearest detected gameobject.")]
    public class SensorGetNearest : SensorToolkitComponentAction<Sensor>
    {
        [RequiredField]
        [CheckForComponent(typeof(Sensor))]
        [Tooltip("The game object owning the Sensor.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Find the nearest detected GameObject with the specified tag.")]
        public FsmString tag;

        [Tooltip("Find the nearest detected GameObject with the specified name.")]
        public FsmString name;

        [Tooltip("Find the nearest detected GameObject with the specified component type and store it here.")]
        [UIHint(UIHint.Variable)]
        public FsmObject storeComponent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores the nearest GameObject detected by the sensor, if there are any.")]
        public FsmGameObject storeNearest;

        [Tooltip("Fires this event if there is a detected GameObject that matches the search filter.")]
        public FsmEvent detectedEvent;

        [Tooltip("Fires this event if no GameObject is detected that matches the search filter.")]
        public FsmEvent noneDetectedEvent;

        [Tooltip("Check every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            tag = null;
            name = null;
            storeComponent = null;
            storeNearest = null;
            detectedEvent = null;
            noneDetectedEvent = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            doCheck();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            doCheck();
        }

        public override string ErrorCheck()
        {
            if (!storeComponent.IsNone
                && !storeComponent.ObjectType.IsSubclassOf(typeof(UnityEngine.Component)))
            {
                return "'Store Component type must be a subclass of UnityEngine.Component";
            }

            return base.ErrorCheck();
        }

        void doCheck()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject))) return;

            if (!storeComponent.IsNone)
            {
                Type t = storeComponent.ObjectType;
                if (!string.IsNullOrEmpty(tag.Value))
                {
                    if (!string.IsNullOrEmpty(name.Value))
                    {
                        storeComponent.Value = sensor.GetNearestByNameAndTagAndComponent(name.Value, tag.Value, t);
                    }
                    else
                    {
                        storeComponent.Value = sensor.GetNearestByTagAndComponent(tag.Value, t);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(name.Value))
                    {
                        storeComponent.Value = sensor.GetNearestByNameAndComponent(name.Value, t);
                    }
                    else
                    {
                        storeComponent.Value = sensor.GetNearestByComponent(t);
                    }
                }
                if (storeComponent.Value != null)
                {
                    storeNearest.Value = (storeComponent.Value as UnityEngine.Component).gameObject;
                }
                else
                {
                    storeNearest.Value = null;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(tag.Value))
                {
                    if (!string.IsNullOrEmpty(name.Value))
                    {
                        storeNearest.Value = sensor.GetNearestByNameAndTag(name.Value, tag.Value);
                    }
                    else
                    {
                        storeNearest.Value = sensor.GetNearestByTag(tag.Value);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(name.Value))
                    {
                        storeNearest.Value = sensor.GetNearestByName(name.Value);
                    }
                    else
                    {
                        storeNearest.Value = sensor.GetNearest();
                    }
                }
            }

            if (storeNearest.Value != null)
            {
                Fsm.Event(detectedEvent);
            }
            else
            {
                Fsm.Event(noneDetectedEvent);
            }
        }
    }
}

#endif