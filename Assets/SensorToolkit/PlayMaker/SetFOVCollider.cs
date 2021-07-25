#if PLAYMAKER

using System.Collections;
using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker
{
    [ActionCategory("Sensors")]
    [Tooltip ("Sets the properties of a FOVCollider object. Note that rebuilding the collider can incur a large performance cost, so it is not recommended on a per-frame basis.")]
    public class SetFOVCollider : SensorToolkitComponentAction<FOVCollider>
    {
        [RequiredField]
        [CheckForComponent(typeof(FOVCollider))]
        public FsmOwnerDefault gameObject;

        public FsmFloat length;
        public FsmFloat baseSize;
        [HasFloatSlider(0,180f)]
        public FsmFloat FOVAngle;
        [HasFloatSlider(0, 180f)]
        public FsmFloat elevationAngle;
        public FsmInt resolution;

        public override void Reset()
	    {
            length = 5f;
            baseSize = 0.5f;
            FOVAngle = 90f;
            resolution = 1;
            elevationAngle = 90f;
	    }

	    public override void OnEnter()
	    {
            setCollider();
            Finish();
	    }
	   
	    void setCollider()
	    {
            if (!UpdateCache(Fsm.GetOwnerDefaultTarget(gameObject))) return;

            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go == null)
            {
                return;
            }

            fovCollider.Length = length.Value;
            fovCollider.BaseSize = baseSize.Value;
            fovCollider.FOVAngle = FOVAngle.Value;
            fovCollider.ElevationAngle = elevationAngle.Value;
            fovCollider.Resolution = resolution.Value;
            fovCollider.CreateCollider();
        }
    }
}

#endif