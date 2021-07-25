#if PLAYMAKER

using System.Collections;
using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker
{
    [ActionCategory("Sensors")]
    [Tooltip("Sets a steering rigs destination. Set either a GameObject or a Vector2 position to track to. For use with the 2D Steering rig.")]
    public class SetSteeringDestination2D : SensorToolkitComponentAction<SteeringRig2D>
    {
        [RequiredField]
        [CheckForComponent(typeof(SteeringRig2D))]
        [Tooltip("The game object owning the steering rig.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Optional. The gameobject that should be moved to.")]
        public FsmGameObject destinationGameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Optional. The position that should be moved to.")]
        public FsmVector2 destinationPosition;

        [Tooltip("Set steering targets each frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            destinationPosition = null;
            destinationGameObject = null;
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
            if (!destinationGameObject.IsNone && !destinationPosition.IsNone)
            {
                return "You cannot set both a destination GameObject and a destination position. One of these must be None.";
            }

            return base.ErrorCheck();
        }

        void doCheck()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject))) return;

            if (!destinationGameObject.IsNone && destinationGameObject.Value != null)
            {
                steeringRig.DestinationTransform = destinationGameObject.Value.transform;
            }
            else
            {
                steeringRig.DestinationTransform = null;
            }
            if (!destinationPosition.IsNone)
            {
                steeringRig.Destination = destinationPosition.Value;
            }
        }
    }
}

#endif