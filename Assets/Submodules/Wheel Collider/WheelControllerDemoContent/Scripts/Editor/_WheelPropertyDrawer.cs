using NWH.NUI;
using UnityEditor;
using UnityEngine;

namespace NWH.WheelController3D
{
    /// <summary>
    ///     Editor for WheelController.
    /// </summary>
    [CustomPropertyDrawer(typeof(_Wheel))]
    [CanEditMultipleObjects]
    public class _WheelPropertyDrawer : NUIPropertyDrawer
    {
        public override bool OnNUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!base.OnNUI(position, property, label)) return false;

            drawer.Field("power");
            drawer.Field("steer");
            drawer.Field("wheelController");

            drawer.EndProperty();
            return true;
        }
    }
}