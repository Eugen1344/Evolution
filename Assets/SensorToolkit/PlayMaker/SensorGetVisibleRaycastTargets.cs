#if PLAYMAKER

using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker
{
    [ActionCategory("Sensors")]
    [Tooltip("For a given detected object get an array of all its ray cast targets that passed line of sight tests.")]
    public class SensorGetVisibleRaycastTargets : SensorToolkitComponentAction<BaseVolumeSensor>
    {
        [RequiredField]
        [CheckForComponent(typeof(BaseVolumeSensor))]
        [Tooltip("The game object owning the sensor.")]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [Tooltip("The target object whose raycast targets should be queried.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject targetObject;

        [Tooltip("Store the array of visible LOSTarget Transforms here.")]
        [UIHint(UIHint.Variable)]
        [ArrayEditor(VariableType.GameObject)]
        public FsmArray storeTargetTransforms;

        [Tooltip("Store the array of visible target positions here.")]
        [UIHint(UIHint.Variable)]
        [ArrayEditor(VariableType.Vector3)]
        public FsmArray storeTargetPositions;

        [Tooltip("Check every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            targetObject = null;
            storeTargetTransforms = null;
            storeTargetPositions = null;
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

            if (!storeTargetTransforms.IsNone)
            {
                var transforms = volumeSensor.GetVisibleTransforms(targetObject.Value);
                var gameObjects = new UnityEngine.GameObject[transforms.Count];
                for (int i = 0; i < gameObjects.Length; i++)
                {
                    gameObjects[i] = transforms[i].gameObject;
                }
                storeTargetTransforms.Values = gameObjects;
            }
            if (!storeTargetPositions.IsNone)
            {
                var visiblePositions = volumeSensor.GetVisiblePositions(targetObject.Value);
                var boxedPositions = new object[visiblePositions.Count];
                for (int i = 0; i < visiblePositions.Count; i++)
                {
                    boxedPositions[i] = visiblePositions[i];
                }
                storeTargetPositions.Values = boxedPositions;
            }
        }
    }
}

#endif