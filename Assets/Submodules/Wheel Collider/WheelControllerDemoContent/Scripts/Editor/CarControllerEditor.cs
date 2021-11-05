using NWH.NUI;
using UnityEditor;

namespace NWH.WheelController3D
{
    /// <summary>
    ///     Editor for WheelController.
    /// </summary>
    [CustomEditor(typeof(CarController))]
    [CanEditMultipleObjects]
    public class CarControllerEditor : NUIEditor
    {
        public override bool OnInspectorNUI()
        {
            if (!base.OnInspectorNUI()) return false;

            drawer.Info(
                "This is a minimalistic car controller intended for demo purposes. The vehicle behavior will not be representative " +
                "of a real vehicle since powertrain is not simulated, there are no differentials, etc. For a complete vehicle solution using WheelController3D " +
                "check out NWH Vehicle Physics 2.");

            drawer.Field("physicsSubsteps");

            drawer.BeginSubsection("Torque");
            drawer.Field("maxMotorTorque");
            drawer.Field("maxBrakeTorque");
            drawer.Field("torqueSpeedCurve");
            drawer.EndSubsection();

            drawer.BeginSubsection("Steering");
            drawer.Field("maxSteeringAngle");
            drawer.Field("minSteeringAngle");
            drawer.EndSubsection();

            drawer.BeginSubsection("Wheels");
            drawer.ReorderableList("wheels");
            drawer.EndSubsection();

            drawer.Field("antiRollBarForce");

            drawer.EndEditor(this);
            return true;
        }
    }
}