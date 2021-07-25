#if PLAYMAKER

using System.Collections;
using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker
{
    [ActionCategory("Sensors")]
    [Tooltip("Sets a steering rigs target direction which it should face. Assign it a GameObject or a Vector2 position to face towards. If you want thw Steering rig to stop strafing then assign both the direction and gameobject parameters to None. For use with the 2D Steering rig.")]
    public class SetSteeringTargetToFace2D : SensorToolkitComponentAction<SteeringRig2D>
    {
        [RequiredField]
        [CheckForComponent(typeof(SteeringRig2D))]
        [Tooltip("The game object owning the steering rig.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Optional. The direction that should be faced towards.")]
        public FsmVector2 directionToFace;

        [UIHint(UIHint.Variable)]
        [Tooltip("Optional. The gameobject that should be faced towards.")]
        public FsmGameObject gameObjectToFace;

        [Tooltip("Set steering targets each frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            directionToFace = null;
            gameObjectToFace = null;
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
            if (!gameObjectToFace.IsNone && !directionToFace.IsNone)
            {
                return "You cannot set both a GameObject and a Vector2 direction to face. Set one parameter to None.";
            }

            return base.ErrorCheck();
        }

        void doCheck()
        {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject))) return;

            if (!gameObjectToFace.IsNone && gameObjectToFace.Value != null)
            {
                steeringRig.ClearDirectionToFace();
                steeringRig.FaceTowardsTransform = gameObjectToFace.Value.transform;
            }
            else
            {
                steeringRig.FaceTowardsTransform = null;
                steeringRig.DirectionToFace = directionToFace.Value;
            }
        }
    }
}

#endif