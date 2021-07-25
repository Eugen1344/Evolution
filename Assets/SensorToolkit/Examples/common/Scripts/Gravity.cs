using UnityEngine;
using System.Collections;

namespace SensorToolkit.Example
{
    public class Gravity : MonoBehaviour
    {
        public Vector3 GravityForce;

        void Awake()
        {
            Physics.gravity = GravityForce;
        }
    }
}