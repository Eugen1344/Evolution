#if PLAYMAKER

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker
{
    [ActionCategory("Sensors")]
    [Tooltip("Query a sensor for all GameObjects it has detected.")]
    public class SensorGetDetected : SensorToolkitComponentAction<Sensor>
    {
        [RequiredField]
        [CheckForComponent(typeof(Sensor))]
        [Tooltip("The game object owning the Sensor.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Filter detected GameObjects with the specified tag.")]
        public FsmString tag;

        [Tooltip("Filter detected GameObjects with the specified name.")]
        public FsmString name;

        [Tooltip("Filter detected GameObjects with the specified component type and store them here.")]
        [UIHint(UIHint.Variable)]
        [ArrayEditor(VariableType.Unknown)]
        public FsmArray storeComponents;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores GameObjects detected by the sensor, if there is one.")]
        [ArrayEditor(VariableType.GameObject)]
        public FsmArray storeDetected;

        [Tooltip("Fires this event if there is at least one detected GameObject that matches the search filters.")]
        public FsmEvent detectedEvent;

        [Tooltip("Fires this event if no GameObject is detected that matches the search filters.")]
        public FsmEvent noneDetectedEvent;

        [Tooltip("Check every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            tag = null;
            name = null;
            storeComponents = null;
            storeDetected = null;
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
            if (!storeComponents.IsNone
                && !storeComponents.ObjectType.IsSubclassOf(typeof(UnityEngine.Component)))
            {
                return "'Store Components' array type must be a subclass of UnityEngine.Component";
            }

            return base.ErrorCheck();
        }

        void doCheck()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject))) return;

            if (!storeComponents.IsNone)
            {
                Type t = storeComponents.ObjectType;
                if (!t.IsSubclassOf(typeof(UnityEngine.Component)))
                {
                    LogError("'Store Components' array type must be a subclass of UnityEngine.Component");
                    return;
                }
                if (!string.IsNullOrEmpty(tag.Value))
                {
                    if (!string.IsNullOrEmpty(name.Value))
                    {
                        storeComponents.Values = sensor.GetDetectedByNameAndTagAndComponent(name.Value, tag.Value, t).ToArray();
                    }
                    else
                    {
                        storeComponents.Values = sensor.GetDetectedByTagAndComponent(tag.Value, t).ToArray();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(name.Value))
                    {
                        storeComponents.Values = sensor.GetDetectedByNameAndComponent(name.Value, t).ToArray();
                    }
                    else
                    {
                        storeComponents.Values = sensor.GetDetectedByComponent(t).ToArray();
                    }
                }
                if (storeComponents.Values != null)
                {
                    storeDetected.Values = storeComponents.Values.Select(c => (c as UnityEngine.Component).gameObject).ToArray();
                }
                else
                {
                    storeDetected.Values = null;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(tag.Value))
                {
                    if (!string.IsNullOrEmpty(name.Value))
                    {
                        storeDetected.Values = sensor.GetDetectedByNameAndTag(name.Value, tag.Value).ToArray();
                    }
                    else
                    {
                        storeDetected.Values = sensor.GetDetectedByTag(tag.Value).ToArray();
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(name.Value))
                    {
                        storeDetected.Values = sensor.GetDetectedByName(name.Value).ToArray();
                    }
                    else
                    {
                        storeDetected.Values = sensor.GetDetected().ToArray();
                    }
                }
            }

            if (storeDetected.Values.Length > 0)
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
