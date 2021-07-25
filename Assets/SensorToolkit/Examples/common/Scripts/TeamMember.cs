using UnityEngine;
using System.Collections;

namespace SensorToolkit.Example
{
    public enum Teams { Yellow, Magenta, None };

    public class TeamMember : MonoBehaviour
    {
        public Teams StartTeam;
        public Material YellowMaterial;
        public Material MagentaMaterial;
        public MeshRenderer[] TeamRenderers;

        private Teams team;
        public Teams Team
        {
            get { return initialised ? team : StartTeam; }
            set
            {
                team = value;
                var mat = team == Teams.Yellow ? YellowMaterial : MagentaMaterial;
                for (int i = 0; i < TeamRenderers.Length; i++)
                {
                    var renderer = TeamRenderers[i];
                    renderer.sharedMaterial = mat;
                }
            }
        }

        bool initialised = false;

        void Start()
        {
            Team = StartTeam;
            initialised = true;
        }
    }
}