using UnityEngine;
using System.Collections;

namespace SensorToolkit.Example
{
    public class AlarmController : MonoBehaviour
    {
        public Light PointLight;
        public Color AlarmColour;
        public float FlashFrequency;
        public Transform[] LinearLevelWaypoints;

        bool alarmStarted = false;
        GameObject whoTrippedAlarm;

        public static AlarmController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<AlarmController>();
                }
                return instance;
            }
        }
        private static AlarmController instance;

        public bool IsAlarmState
        {
            get { return alarmStarted && WhoTrippedAlarm != null; }
        }

        public GameObject WhoTrippedAlarm
        {
            get { return whoTrippedAlarm; }
        }

        public void StartAlarm(GameObject whoTrippedAlarm)
        {
            if (!alarmStarted)
            {
                this.whoTrippedAlarm = whoTrippedAlarm;
                StartCoroutine(alarmRoutine());
            }
        }

        /*
         * Extremely basic pathfinding which assumes a linear level layout. Finds the closest waypoint index to
         * 'from' and to the 'whoTrippedAlarm' gameobjects. The path is then the sequence of waypoint indices between
         * these two in LinearLevelWaypoints. 
         */
        public Transform[] PathToWhoTrippedAlarm(GameObject from)
        {
            var nearestToFrom = getNearestWaypointIndex(from.transform.position);
            var nearestToTripper = getNearestWaypointIndex(whoTrippedAlarm.transform.position);
            var nWaypoints = Mathf.Abs(nearestToFrom - nearestToTripper);
            bool ascending = nearestToTripper > nearestToFrom;
            var path = new Transform[nWaypoints];
            for (int i = 0; i < nWaypoints; i++)
            {
                var offset = ascending ? i : -i;
                path[i] = LinearLevelWaypoints[nearestToFrom + offset];
            }

            return path;
        }

        IEnumerator alarmRoutine()
        {
            alarmStarted = true;
            PointLight.color = AlarmColour;
            var startIntensity = PointLight.intensity;

            while (true)
            {
                var intensity = (Mathf.Sin(FlashFrequency * Time.time * Mathf.PI * 2f) + 1f) / 2f * startIntensity;
                PointLight.intensity = intensity;
                yield return null;
            }
        }

        int getNearestWaypointIndex(Vector3 toPos)
        {
            float nearestDist = 0f;
            int nearest = -1;
            for (int i = 0; i < LinearLevelWaypoints.Length; i++)
            {
                var dist = (toPos - LinearLevelWaypoints[i].position).sqrMagnitude;
                if (dist < nearestDist || nearest == -1)
                {
                    nearest = i;
                    nearestDist = dist;
                }
            }
            return nearest;
        }
    }
}
