using UnityEngine;
using System.Collections;

namespace SensorToolkit.Example
{
    [RequireComponent(typeof(TeamMember))]
    public class SecurityCamera : MonoBehaviour
    {
        public float RotationSpeed;
        public float ScanTime;
        public float TrackTime;
        public float ScanArcAngle;
        public Light SpotLight;
        public Sensor Sensor;
        public Color ScanColour;
        public Color TrackColour;
        public Color AlarmColour;

        Quaternion leftExtreme;
        Quaternion rightExtreme;
        TeamMember team;

        Quaternion targetRotation;

        void Awake()
        {
            leftExtreme = Quaternion.AngleAxis(ScanArcAngle / 2f, Vector3.up) * transform.rotation;
            rightExtreme = Quaternion.AngleAxis(-ScanArcAngle / 2f, Vector3.up) * transform.rotation;
            team = GetComponent<TeamMember>();
        }

        void OnEnable()
        {
            targetRotation = transform.rotation;
            transform.rotation = rightExtreme;
            StartCoroutine(scanState());
        }

        void Update()
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
        }

        IEnumerator scanState()
        {
            StartCoroutine(scanMovement());
            while (true)
            {
                if (AlarmController.Instance.IsAlarmState)
                {
                    StopAllCoroutines();
                    StartCoroutine(alarmState());
                    break;
                }
                else if (getSpottedEnemy() != null)
                {
                    StopAllCoroutines();
                    StartCoroutine(trackState());
                    break;
                }
                yield return null;
            }
        }

        IEnumerator scanMovement()
        {
            SpotLight.color = ScanColour;
            while (true)
            {
                targetRotation = leftExtreme;
                yield return new WaitForSeconds(ScanTime);
                targetRotation = rightExtreme;
                yield return new WaitForSeconds(ScanTime);
            }
        }

        IEnumerator trackState()
        {
            SpotLight.color = TrackColour;
            var enemy = getSpottedEnemy();
            var timer = 0f;
            while (Sensor.IsDetected(enemy))
            {
                targetRotation = Quaternion.LookRotation(enemy.transform.position - transform.position, Vector3.up);
                timer += Time.deltaTime;
                if (timer >= TrackTime)
                {
                    AlarmController.Instance.StartAlarm(enemy);
                    StopAllCoroutines();
                    StartCoroutine(alarmState());
                    break;
                }
                yield return null;
            }
            StopAllCoroutines();
            StartCoroutine(scanState());
        }

        IEnumerator alarmState()
        {
            targetRotation = transform.rotation;
            SpotLight.color = AlarmColour;
            yield return null;
        }

        GameObject getSpottedEnemy()
        {
            var spottedCharacters = Sensor.GetDetectedByComponent<TeamMember>();
            for (int i = 0; i < spottedCharacters.Count; i++)
            {
                if (spottedCharacters[i].Team != team.Team)
                {
                    return spottedCharacters[i].gameObject;
                }
            }
            return null;
        }
    }
}