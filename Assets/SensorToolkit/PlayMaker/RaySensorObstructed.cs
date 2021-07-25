#if PLAYMAKER

using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;

namespace SensorToolkit.PlayMaker
{
    [ActionCategory("Sensors")]
    [Tooltip("Query a Ray Sensor to determine if it's obstructed.")]
    public class RaySensorObstructed : SensorToolkitComponentAction<RaySensor>
    {
        [RequiredField]
        [CheckForComponent(typeof(RaySensor))]
        [Tooltip("The game object owning the Ray Sensor.")]
        public FsmOwnerDefault gameObject;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores the result, is it obstructed or not.")]
        public FsmBool storeIsObstructed;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores the GameObject obstructing the sensor, if there is one.")]
        public FsmGameObject storeObstruction;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores the distance to the obstruction, if there is one.")]
        public FsmFloat storeDistance;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores the position of the obstruction point, if there is one.")]
        public FsmVector3 storePoint;

        [UIHint(UIHint.Variable)]
        [Tooltip("Stores the normal to the obstruction point, if there is one.")]
        public FsmVector3 storeNormal;

        [Tooltip("Fires this event if obstructed.")]
        public FsmEvent obstructedEvent;

        [Tooltip("Fires this event if not obstructed.")]
        public FsmEvent notObstructedEvent;

        [Tooltip("Check every frame")]
        public bool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            storeIsObstructed = null;
            storeObstruction = null;
            storeDistance = float.PositiveInfinity;
            storePoint = null;
            storeNormal = null;
            obstructedEvent = null;
            notObstructedEvent = null;
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

            storeIsObstructed.Value = raySensor.IsObstructed;
            storeObstruction.Value = storeIsObstructed.Value ? raySensor.ObstructedBy.gameObject : null;
            storeDistance.Value = raySensor.ObstructionRayHit.distance;
			storePoint.Value = raySensor.ObstructionRayHit.point;
			storeNormal.Value = raySensor.ObstructionRayHit.normal;

            if (storeIsObstructed.Value)
            {
                Fsm.Event(obstructedEvent);
            }
            else
            {
                Fsm.Event(notObstructedEvent);
            }
        }
    }
}

#endif