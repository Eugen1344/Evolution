using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SensorToolkit.Example
{
    [RequireComponent(typeof(CharacterControls), typeof(GunWithClip), typeof(TeamMember))]
    public class SoldierAI : MonoBehaviour
    {
        public Sensor Sight;
        public Sensor InteractionRange;
        public SteeringRig SteerSensor;

        CharacterControls movement;
        GunWithClip gun;
        Teams team;

        void Start()
        {
            movement = GetComponent<CharacterControls>();
            gun = GetComponent<GunWithClip>();
            team = GetComponent<TeamMember>().Team;
            StartCoroutine(DefaultState());
        }

        List<GameObject> enemiesSpotted
        {
            get
            {
                var detected = Sight.GetDetected();
                for (int i = detected.Count-1; i >= 0 ; i--)
                {
                    var detectedTeam = detected[i].GetComponent<TeamMember>();
                    if (detectedTeam == null || detectedTeam.Team == team || detectedTeam.Team == Teams.None)
                    {
                        detected.RemoveAt(i);
                    }
                }
                return detected;
            }
        }

        List<GameObject> friendsSpotted
        {
            get
            {
                var detected = Sight.GetDetected();
                for (int i = detected.Count - 1; i >= 0; i--)
                {
                    var detectedTeam = detected[i].GetComponent<TeamMember>();
                    if (detectedTeam == null || detectedTeam.Team != team || detectedTeam.Team == Teams.None)
                    {
                        detected.RemoveAt(i);
                    }
                }
                return detected;
            }
        }

        List<Holdable> pickupsSpotted
        {
            get
            {
                return Sight.GetDetectedByComponent<Holdable>();
            }
        }

        GameObject myBaseCache;
        GameObject MyBase
        {
            get
            {
                if (myBaseCache == null)
                {
                    myBaseCache = GameObject.Find(team == Teams.Yellow ? "YellowBase" : "MagentaBase");
                }
                return myBaseCache;
            }
        }

        IEnumerator DefaultState()
        {
            Start:

            if (movement.Held != null)
            {
                StartCoroutine(CarryToBaseState()); yield break;
            }

            var pickups = pickupsSpotted;
            if (pickups.Count > 0)
            {
                if (pickups[0].IsHeld && pickups[0].Holder.GetComponent<TeamMember>().Team != team)
                {
                    StartCoroutine(AttackState(pickups[0].Holder)); yield break;
                }
                else if (Random.value > 0.9f)
                {
                    StartCoroutine(PickUpState()); yield break;
                }
            }
            if (enemiesSpotted.Count > 0) { StartCoroutine(AttackState()); yield break; }

            float countdown = 1f;
            while (countdown > 0f)
            {
                // Make way to the center of the map
                var targetDirection = SteerSensor.GetSteeredDirection(-transform.position);
                movement.Move = targetDirection;
                movement.Face = targetDirection;

                countdown -= Time.deltaTime;
                yield return null;
            }

            goto Start;
        }

        IEnumerator AttackState(GameObject target = null)
        {
            if (target == null)
            {
                var enemies = enemiesSpotted;
                if (enemies.Count == 0) { StartCoroutine(DefaultState()); yield break; }
                target = enemies[0];
            }

            var cooldown = Random.Range(0.5f, 2f);
            if ((target.transform.position - transform.position).magnitude > 10f)
            {
                // Charge
                while (cooldown > 0f)
                {
                    if (target == null) break;

                    var targetDirection = (target.transform.position - transform.position).normalized;
                    movement.Move = SteerSensor.GetSteeredDirection(targetDirection);
                    movement.Face = targetDirection;

                    gun.Fire();
                    if (gun.IsEmptyClip)
                    {
                        gun.Reload();
                    }
                    cooldown -= Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                // Strafe
                var strafeDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
                while (cooldown > 0f)
                {
                    if (target == null) break;

                    var targetDirection = (target.transform.position - transform.position).normalized;
                    movement.Move = SteerSensor.GetSteeredDirection(strafeDirection);
                    movement.Face = targetDirection;

                    gun.Fire();
                    if (gun.IsEmptyClip && !gun.IsReloading)
                    {
                        gun.Reload();
                        if (Random.value > 0.5f) { StartCoroutine(FleeState()); yield break; }
                    }
                    cooldown -= Time.deltaTime;
                    yield return null;
                }
            }

            StartCoroutine(DefaultState());
        }

        IEnumerator FleeState()
        {
            var cooldown = Random.Range(2f, 4f);
            var enemies = enemiesSpotted;
            if (enemies.Count == 0) { StartCoroutine(DefaultState()); yield break; }
            Vector3 enemyRepulse = Vector3.zero;
            for (int i = 0; i < enemies.Count; i++)
            {
                enemyRepulse -= (enemies[i].transform.position - transform.position).normalized;
            }
            enemyRepulse = enemyRepulse.normalized;

            while (cooldown > 0f)
            {
                movement.Move = SteerSensor.GetSteeredDirection(enemyRepulse);
                movement.Face = movement.Move;

                cooldown -= Time.deltaTime;
                yield return null;
            }

            StartCoroutine(DefaultState()); yield break;
        }

        IEnumerator PickUpState()
        {
            var pickups = pickupsSpotted;
            if (pickups.Count == 0) { StartCoroutine(DefaultState()); yield break; }
            var pickup = pickups[0];

            float countdown = 2f;
            while (countdown > 0f)
            {
                countdown -= Time.deltaTime;
                if (pickup.IsHeld) { StartCoroutine(DefaultState()); yield break; }

                movement.Move = SteerSensor.GetSteeredDirection(pickup.transform.position - transform.position);
                movement.Face = movement.Move;
                if (InteractionRange.IsDetected(pickup.gameObject))
                {
                    movement.PickUp(pickup);
                    break;
                }

                yield return null;
            }

            StartCoroutine(DefaultState());
        }

        IEnumerator CarryToBaseState()
        {
            Start:

            if (movement.Held == null)
            {
                StartCoroutine(DefaultState()); yield break;
            }

            var baseDirection = MyBase.transform.position - transform.position;
            movement.Move = SteerSensor.GetSteeredDirection(baseDirection);
            movement.Face = movement.Move;

            yield return null;

            goto Start;
        }
    }
}