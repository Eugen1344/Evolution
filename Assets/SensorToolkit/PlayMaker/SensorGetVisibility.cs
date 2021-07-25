#if PLAYMAKER

using System.Collections;
using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker
{
    [ActionCategory("Sensors")]
    [Tooltip("Get the computed visibility of a GameObject from a sensor. Visibility is calculated when line of sight tests are enabled, it is the ratio of rays from the sensor to the GameObject that are unobstructed.")]
    public class SensorGetVisibility : SensorToolkitComponentAction<Sensor>
    {
        [RequiredField]
        [CheckForComponent(typeof(Sensor))]
        [Tooltip("The game object owning the Sensor.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The game object to check the visibility of.")]
        public FsmGameObject checkObject;

        [Tooltip("Stores the visibility ratio of the game object.")]
        public FsmFloat storeVisibility;

        [Tooltip("Check every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            storeVisibility = 0f;
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

            storeVisibility.Value = sensor.GetVisibility(checkObject.Value);
        }
    }
}

#endif