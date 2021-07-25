#if PLAYMAKER

using System.Collections;
using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker
{
    [ActionCategory("Sensors")]
    [Tooltip("Manually perform a test on a sensor to refresh it's list of detected GameObjects.")]
    public class PulseSensor : SensorToolkitComponentAction<Sensor>
    {
        [RequiredField]
        [CheckForComponent(typeof(Sensor))]
        [Tooltip("The game object owning the Sensor.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("Perform test every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            gameObject = null;
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

        void doCheck()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject))) return;

            sensor.Pulse();
        }
    }
}

#endif
