#if PLAYMAKER

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker
{
    [ActionCategory("Sensors")]
    [Tooltip("Query a sensor for the nearest detected GameObject to a specified world position.")]
    public class SensorGetNearestToPoint : SensorToolkitComponentAction<Sensor>
    {
        [RequiredField]
        [CheckForComponent(typeof(Sensor))]
        [Tooltip("The game object owning the Sensor.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The world position to measure distance from.")]
        [UIHint(UIHint.Variable)]
        public FsmVector3 P;

        [Tooltip("Filter detected GameObjects with the specified tag.")]
        public FsmString tag;

        [Tooltip("Filter detected GameObjects with the specified name.")]
        public FsmString name;

        [Tooltip("Filter detected GameObjects with the specified component and store the one belonging to the nearest GameObject here.")]
        [UIHint(UIHint.Variable)]
        public FsmObject storeComponent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores the GameObject nearest to the specified world position, if there are any.")]
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
            P = null;
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
                        storeComponent.Value = sensor.GetNearestToPointByNameAndTagAndComponent(P.Value, name.Value, tag.Value, t);
                    }
                    else
                    {
                        storeComponent.Value = sensor.GetNearestToPointByTagAndComponent(P.Value, tag.Value, t);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(name.Value))
                    {
                        storeComponent.Value = sensor.GetNearestToPointByNameAndComponent(P.Value, name.Value, t);
                    }
                    else
                    {
                        storeComponent.Value = sensor.GetNearestToPointByComponent(P.Value, t);
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
                        storeNearest.Value = sensor.GetNearestToPointByNameAndTag(P.Value, name.Value, tag.Value);
                    }
                    else
                    {
                        storeNearest.Value = sensor.GetNearestToPointByTag(P.Value, tag.Value);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(name.Value))
                    {
                        storeNearest.Value = sensor.GetNearestToPointByName(P.Value, name.Value);
                    }
                    else
                    {
                        storeNearest.Value = sensor.GetNearestToPoint(P.Value);
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
