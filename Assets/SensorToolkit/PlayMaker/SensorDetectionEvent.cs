#if PLAYMAKER

using System.Collections;
using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker
{
    [ActionCategory("Sensors")]
    [Tooltip("React to a sensors detection events. Can listen for new GameObjects being detected, or losing sight of GameObjects that are already detected.")]
    public class SensorDetectionEvent : SensorToolkitComponentAction<Sensor>
    {
        public enum EventType { NewDetection, LostDetection }

        [RequiredField]
        [CheckForComponent(typeof(Sensor))]
        [Tooltip("The game object owning the Sensor.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The sensor event that triggers this action.")]
        public EventType trigger;

        [Tooltip("The FSM event to send.")]
        public FsmEvent sendEvent;

        [Tooltip("Stores the GameObject that was Detected/Lost.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject storeGameObject;

        public override void Reset()
        {
            gameObject = null;
            trigger = EventType.NewDetection;
            sendEvent = null;
            storeGameObject = null;
        }

        public override void OnEnter()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject))) return;

            if (trigger == EventType.NewDetection)
            {
                sensor.OnDetected.AddListener(doEvent);
            }
            else
            {
                sensor.OnLostDetection.AddListener(doEvent);
            }
        }

        public override void OnExit()
        {
            if (sensor == null) return;

            if (trigger == EventType.NewDetection)
            {
                sensor.OnDetected.RemoveListener(doEvent);
            }
            else
            {
                sensor.OnLostDetection.RemoveListener(doEvent);
            }
        }

        void doEvent(UnityEngine.GameObject go, Sensor sensor)
        {
            storeGameObject.Value = go;
            Fsm.Event(sendEvent);
        }
    }
}

#endif