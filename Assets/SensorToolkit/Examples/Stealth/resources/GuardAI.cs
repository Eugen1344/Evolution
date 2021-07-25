using UnityEngine;
using System.Collections;

namespace SensorToolkit.Example
{
    [RequireComponent(typeof(GunWithClip), typeof(TeamMember))]
    public class GuardAI : MonoBehaviour
    {
        public GameObject GunPivot;
        public SteeringRig Steering;
        public Sensor Sight;
        public Transform[] PatrolPath;
        public float WaypointArriveDistance;
        public float PauseTime;
        public float WanderDistance;
        public float SoundAlarmTime;

        GunWithClip gun;
        TeamMember team;
        bool ascending = true;

        void Start()
        {
            gun = GetComponent<GunWithClip>();
            team = GetComponent<TeamMember>();
            StartCoroutine(PatrolState());
        }

        IEnumerator PatrolState()
        {
            var nextWaypoint = getNearestWaypointIndex();

            Start:

            if (attackEnemyIfSpotted()) yield break;
            if (chaseIfAlarmSounded()) yield break;

            Steering.DestinationTransform = PatrolPath[nextWaypoint];
            if ((transform.position - PatrolPath[nextWaypoint].position).magnitude < WaypointArriveDistance)
            {
                // We've arrived at our target waypoint. Select the next waypoint.
                nextWaypoint = ascending ? nextWaypoint + 1 : nextWaypoint - 1;
                // If this was the last waypoint in the sequence then pause for a moment before following
                // the waypoints in reverse.
                if (nextWaypoint >= PatrolPath.Length || nextWaypoint < 0)
                {
                    ascending = !ascending;
                    StartCoroutine(PauseState()); yield break;
                }
            }

            yield return null;
            goto Start;
        }

        IEnumerator PauseState()
        {
            Steering.DestinationTransform = null;
            Steering.Destination = transform.position + wanderVector();
            float timer = PauseTime;
            while (timer > 0f)
            {
                if (attackEnemyIfSpotted()) yield break;
                if (chaseIfAlarmSounded()) yield break;

                timer -= Time.deltaTime;
                yield return null;
            }
            StartCoroutine(PatrolState()); yield break;
        }

        IEnumerator AttackState(GameObject ToAttack)
        {
            Steering.DestinationTransform = null;
            Steering.FaceTowardsTransform = ToAttack.transform;
            var alarmTimer = SoundAlarmTime;

            Start:

            if (ToAttack == null)
            {
                StartCoroutine(PauseState());
                yield break;
            }

            alarmTimer -= Time.deltaTime;
            if (alarmTimer <= 0f)
            {
                AlarmController.Instance.StartAlarm(ToAttack);
            }

            if (!Sight.IsDetected(ToAttack))
            {
                Steering.FaceTowardsTransform = null;
                GunPivot.transform.localRotation = Quaternion.identity; // Return gun rotation back to resting position
                StartCoroutine(Investigate(ToAttack.transform.position));
                yield break;
            }

            // Roate the gun in hand to face the enemy, reload if empty, otherwise fire the gun.
            GunPivot.transform.LookAt(new Vector3(ToAttack.transform.position.x, GunPivot.transform.position.y, ToAttack.transform.position.z));
            if (gun.IsEmptyClip) gun.Reload();
            else gun.Fire();

            yield return null;
            goto Start;
        }

        IEnumerator Investigate(Vector3 position)
        {
            Steering.DestinationTransform = null;
            Steering.Destination = position;
            float timer = 0f;

            Start:

            if (attackEnemyIfSpotted()) yield break;

            timer += Time.deltaTime;
            if (timer > 5f || !Steering.IsSeeking)
            {
                StartCoroutine(PauseState()); yield break;
            }

            yield return null;
            goto Start;
        }

        IEnumerator Chase()
        {
            Start:

            // Find a path from myself to the object who tripped the alarm, if it's a short path then we must be in the same
            // room, so just attack them. If it's a long path then follow it until we see them, and then attack them. If we
            // reach the end of the chase path and still don't see an enemy then start again, create a new path.
            var chasePath = AlarmController.Instance.PathToWhoTrippedAlarm(gameObject);
            if (chasePath.Length > 1)
            {
                var nextWaypoint = 0;

                StartFollowPath:

                if (attackEnemyIfSpotted()) yield break;

                Steering.DestinationTransform = chasePath[nextWaypoint];
                if ((transform.position - chasePath[nextWaypoint].position).magnitude < WaypointArriveDistance)
                {
                    nextWaypoint++;
                    if (nextWaypoint >= chasePath.Length)
                    {
                        goto Start;
                    }
                }

                yield return null;
                goto StartFollowPath;
            }
            else
            {
                StartCoroutine(AttackState(AlarmController.Instance.WhoTrippedAlarm));
                yield break;
            }
        }

        bool attackEnemyIfSpotted()
        {
            var spottedCharacters = Sight.GetDetectedByComponent<TeamMember>();
            for (int i = 0; i < spottedCharacters.Count; i++)
            {
                if (spottedCharacters[i].Team != team.Team)
                {
                    StartCoroutine(AttackState(spottedCharacters[i].gameObject));
                    return true;
                }
            }
            return false;
        }

        bool chaseIfAlarmSounded()
        {
            if (AlarmController.Instance.IsAlarmState)
            {
                StartCoroutine(Chase());
                return true;
            }
            return false;
        }

        int getNearestWaypointIndex()
        {
            float nearestDist = 0f;
            int nearest = -1;
            for (int i = 0; i < PatrolPath.Length; i++)
            {
                var dist = (transform.position - PatrolPath[i].position).sqrMagnitude;
                if (dist < nearestDist || nearest == -1)
                {
                    nearest = i;
                    nearestDist = dist;
                }
            }
            return nearest;
        }

        Vector3 wanderVector()
        {
            var rv = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
            return rv * WanderDistance;
        }
    }
}