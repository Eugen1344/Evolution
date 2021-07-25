using UnityEngine;
using System.Collections;

namespace SensorToolkit.Example
{
    [RequireComponent(typeof(RaySensor))]
    public class Bullet : MonoBehaviour
    {
        public float Speed;
        public float Damage;
        public float MaxAge;
        public float ImpactForce;
        public GameObject HitEffect;

        float age;
        RaySensor raySensor;

        void Start()
        {
            raySensor = GetComponent<RaySensor>();
            raySensor.SensorUpdateMode = RaySensor.UpdateMode.Manual;
            age = 0;
        }

        void Update()
        {
            age += Time.deltaTime;
            if (age > MaxAge)
            {
                explode(Vector3.up);
                return;
            }

            var deltaPos = transform.forward * Speed * Time.deltaTime;
            raySensor.Length = deltaPos.magnitude;
            raySensor.Pulse();

            transform.position += deltaPos;
        }

        public void HitObject(GameObject g, Sensor s)
        {
            var health = g.GetComponent<Health>();
            if (health != null)
            {
                health.Impact(Damage, transform.forward * ImpactForce, raySensor.GetRayHit(g).point);
            }
            explode(raySensor.GetRayHit(g).normal);
        }

        public void HitWall()
        {
            explode(raySensor.ObstructionRayHit.normal);
        }

        void explode(Vector3 direction)
        {
            if (HitEffect != null)
            {
                Instantiate(HitEffect, transform.position, Quaternion.LookRotation(direction));
            }
            Destroy(gameObject);
        }
    }
}