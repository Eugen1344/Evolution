#if PLAYMAKER

using System.Collections;
using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker
{
    [ActionCategory("Sensors")]
    [Tooltip ("Sets the properties of a FOVCollider2D object. Note that rebuilding the collider can incur a large performance cost, so it is not recommended on a per-frame basis.")]
    public class SetFOVCollider2D : SensorToolkitComponentAction<FOVCollider2D>
    {
        [RequiredField]
        [CheckForComponent(typeof(FOVCollider2D))]
        public FsmOwnerDefault gameObject;

        public FsmFloat length;
        public FsmFloat baseSize;
        [HasFloatSlider(0,180f)]
        public FsmFloat FOVAngle;
        public FsmInt resolution;

        public override void Reset()
	    {
            length = 5f;
            baseSize = 0.5f;
            FOVAngle = 90f;
            resolution = 1;
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

            fovCollider2D.Length = length.Value;
            fovCollider2D.BaseSize = baseSize.Value;
            fovCollider2D.FOVAngle = FOVAngle.Value;
            fovCollider2D.Resolution = resolution.Value;
            fovCollider2D.CreateCollider();
        }
    }
}

#endif