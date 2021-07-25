#if PLAYMAKER

using System.Collections;
using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker
{
    [ActionCategory("Sensors")]
    [Tooltip("Compute the steered direction from a target direction which avoids nearby obstacles.")]
    public class GetSteeredDirection : SensorToolkitComponentAction<SteeringRig>
    {
        [RequiredField]
        [CheckForComponent(typeof(SteeringRig))]
        [Tooltip("The game object owning the steering rig.")]
        public FsmOwnerDefault gameObject;

        [Tooltip("The target direction to be steered.")]
        public FsmVector3 targetDirection;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Stores the steered direction.")]
        public FsmVector3 storeSteeredDirection;

        [Tooltip("Compute steered vector every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            targetDirection = null;
            storeSteeredDirection = null;
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

            if (targetDirection == null) return;

            storeSteeredDirection.Value = steeringRig.GetSteeredDirection(targetDirection.Value);
        }
    }
}

#endif