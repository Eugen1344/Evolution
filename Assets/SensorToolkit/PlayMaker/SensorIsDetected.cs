#if PLAYMAKER

using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker 
{
    [ActionCategory("Sensors")]
    [Tooltip("Query a sensor if it currently detects a specific Game Object.")]
    public class SensorIsDetected : SensorToolkitComponentAction<Sensor> 
    {
        [RequiredField]
        [CheckForComponent(typeof(Sensor))]
        [Tooltip("The game object owning the Sensor.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Check if this Game Object is currently detected.")]
        public FsmGameObject checkGameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores the result, is it detected or not.")]
        public FsmBool storeResult;

        [Tooltip("Fires this event if the Game Object is detected.")]
        public FsmEvent detectedEvent;

        [Tooltip("Fires this event if the Game Object isn't detected.")]
        public FsmEvent notDetectedEvent;

        [Tooltip("Check every frame")]
        public bool everyFrame;

        public override void Reset() 
        {
            gameObject = null;
            checkGameObject = null;
            storeResult = null;
            detectedEvent = null;
            notDetectedEvent = null;
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

        void doCheck() {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject))) return;

            var isDetected = sensor.IsDetected(checkGameObject.Value);

            if (!storeResult.IsNone) 
            {
                storeResult.Value = isDetected;
            }

            if (isDetected) 
            {
                Fsm.Event(detectedEvent);
            } 
            else 
            {
                Fsm.Event(notDetectedEvent);
            }
        }
    }
}

#endif
