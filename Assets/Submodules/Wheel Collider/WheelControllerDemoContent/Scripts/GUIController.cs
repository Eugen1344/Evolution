using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NWH.WheelController3D
{
    public class GUIController : MonoBehaviour
    {
        public GameObject     canvas;
        public Vehicle        currentVehicle;
        public FrictionPreset genericFrictionPreset;
        public FrictionPreset gravelFrictionPreset;
        public FrictionPreset iceFrictionPreset;
        public FrictionPreset snowFrictionPreset;
        public Text           speedText;
        public FrictionPreset tarmacDryFrictionPreset;
        public FrictionPreset tarmacWetFrictionPreset;

        [SerializeField]
        public List<Vehicle> vehicles;

        private int               speed;
        private int               vehicleSelector;
        private WheelController[] wcs;


        private void Start()
        {
            currentVehicle = vehicles[vehicleSelector];
            currentVehicle.GetComponent<CarController>().Active(true);
            wcs = currentVehicle.GetComponentsInChildren<WheelController>();
        }


        private void Update()
        {
            currentVehicle = ChangeVehicle(ref vehicleSelector);
            if (currentVehicle == null) return;
            wcs = currentVehicle.GetComponentsInChildren<WheelController>();
            Camera.main.GetComponent<CameraDefault>().TargetLookAt = currentVehicle.transform;

            SetMeter();
            SetSpeed(currentVehicle.velocity * 3.6f);
        }


        public void AdjustFriction(FrictionPreset p)
        {
            foreach (WheelController w in wcs)
            {
                w.activeFrictionPreset = p;
            }
        }


        public void NextVehicle()
        {
            vehicleSelector++;
            ChangeVehicle(ref vehicleSelector);
        }


        public Vehicle ChangeVehicle(ref int vehicleIndex)
        {
            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.Active(false);
            }

            if (vehicleIndex >= vehicles.Count || vehicleIndex < 0) vehicleIndex = 0;

            vehicles[vehicleIndex].Active(true);
            return vehicles[vehicleIndex];
        }


        public void DecreaseBump()
        {
            foreach (WheelController w in wcs)
            {
                w.damper.bumpForce -= w.damper.bumpForce * 0.1f;
                w.damper.bumpForce =  Mathf.Clamp(w.damper.bumpForce, 1000f, 5000f);
            }
        }


        public void DecreaseCamber()
        {
            foreach (WheelController w in wcs)
            {
                float camber = w.camber - 2f;
                camber = Mathf.Clamp(camber, -15f, 15f);
                w.SetCamber(camber);
            }
        }


        public void DecreaseRebound()
        {
            foreach (WheelController w in wcs)
            {
                w.damper.reboundForce -= w.damper.reboundForce * 0.1f;
                w.damper.reboundForce =  Mathf.Clamp(w.damper.reboundForce, 1000f, 5000f);
            }
        }


        public void DecreaseRimOffset()
        {
            foreach (WheelController w in wcs)
            {
                w.rimOffset -= 0.05f;
                w.rimOffset =  Mathf.Clamp(w.rimOffset, -0.2f, 0.2f);
            }
        }


        public void DecreaseSpringLength()
        {
            foreach (WheelController w in wcs)
            {
                w.springLength -= w.springLength * 0.1f;
                w.springLength =  Mathf.Clamp(w.springLength, 0.15f, 0.6f);
            }
        }


        public void DecreaseSpringStrength()
        {
            foreach (WheelController w in wcs)
            {
                w.springMaximumForce -= w.springMaximumForce * 0.1f;
                w.springMaximumForce =  Mathf.Clamp(w.springMaximumForce, 14000f, 45000f);
            }
        }


        public void IncreaseBump()
        {
            foreach (WheelController w in wcs)
            {
                w.damper.bumpForce += w.damper.bumpForce * 0.1f;
                w.damper.bumpForce =  Mathf.Clamp(w.damper.bumpForce, 1000f, 5000f);
            }
        }


        public void IncreaseCamber()
        {
            foreach (WheelController w in wcs)
            {
                float camber = w.camber + 2f;
                camber = Mathf.Clamp(camber, -15f, 15f);
                w.SetCamber(camber);
            }
        }


        public void IncreaseRebound()
        {
            foreach (WheelController w in wcs)
            {
                w.damper.reboundForce += w.damper.reboundForce * 0.1f;
                w.damper.reboundForce =  Mathf.Clamp(w.damper.reboundForce, 1000f, 5000f);
            }
        }


        public void IncreaseRimOffset()
        {
            foreach (WheelController w in wcs)
            {
                w.rimOffset += 0.05f;
                w.rimOffset =  Mathf.Clamp(w.rimOffset, -0.2f, 0.2f);
            }
        }


        public void IncreaseSpringLength()
        {
            foreach (WheelController w in wcs)
            {
                w.springLength += w.springLength * 0.1f;
                w.springLength =  Mathf.Clamp(w.springLength, 0.15f, 0.6f);
            }
        }


        public void IncreaseSpringStrength()
        {
            foreach (WheelController w in wcs)
            {
                w.springMaximumForce += w.springMaximumForce * 0.1f;
                w.springMaximumForce =  Mathf.Clamp(w.springMaximumForce, 14000f, 45000f);
            }
        }


        public void LevelReset()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }


        public void SetSpeed(float currentSpeed)
        {
            speed = Mathf.RoundToInt(currentSpeed);
        }


        public void SurfaceGeneric()
        {
            AdjustFriction(genericFrictionPreset);
        }


        public void SurfaceGravel()
        {
            AdjustFriction(gravelFrictionPreset);
        }


        public void SurfaceIce()
        {
            AdjustFriction(iceFrictionPreset);
        }


        public void SurfaceSnow()
        {
            AdjustFriction(snowFrictionPreset);
        }


        public void SurfaceTarmacDry()
        {
            AdjustFriction(tarmacDryFrictionPreset);
        }


        public void SurfaceTarmacWet()
        {
            AdjustFriction(tarmacWetFrictionPreset);
        }


        private void SetMeter()
        {
            speedText.text = Mathf.Abs(speed).ToString();
        }
    }
}