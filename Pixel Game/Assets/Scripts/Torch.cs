using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : MonoBehaviour
{
    UnityEngine.Experimental.Rendering.Universal.Light2D torch2dLightComponent;


    private float maxTorchLightIntensity = 2.0f;
    private bool isTorchBelow_1_Intensity = false;
    private ParticleSystem fireParticleEffect;
    public bool isTorchActive = false;

    private float minOuterRadius = 3.45f;
    private float maxOuterRadius = 3.55f;
    private float regularTorchLightOuterRadius = 3.5f;

    private bool hasTorchBurntOut = false;


    void Start()
    {
        fireParticleEffect = this.gameObject.GetComponent<ParticleSystem>();
        torch2dLightComponent = this.gameObject.GetComponent<UnityEngine.Experimental.Rendering.Universal.Light2D>();
        torch2dLightComponent.intensity = maxTorchLightIntensity;

        

        isTorchActive = false;
        fireParticleEffect.Stop();
        torch2dLightComponent.enabled = false;



    }

    void Update()
    {

    }




    public void toggleTorch()
    {
        if(isTorchActive == true && hasTorchBurntOut == false) { 
            torch2dLightComponent.enabled = true;
            fireParticleEffect.Play();
            InvokeRepeating("decreaseTorchFuelAndFlicker", 0.0f, 0.2f); //  Gör en funktion varje 0.2f sekund

        }
        else if(isTorchActive == false && hasTorchBurntOut == false)
        {
            Debug.Log("Torch active status: " + isTorchActive);
            stopDrainingTorchFuel();
        }
        else
        {
            Debug.Log("TORCH HAS OUT");
        }
    }

    private void decreaseTorchFuelAndFlicker()
    {
        torch2dLightComponent.intensity -= 0.02f;

        if(torch2dLightComponent.intensity < 1.0f && isTorchBelow_1_Intensity == false)
        {
            isTorchBelow_1_Intensity = true;
            torch2dLightComponent.pointLightOuterRadius = (regularTorchLightOuterRadius / 2.0f); // Halves the size of light radius when low on fuel
            minOuterRadius = 1.70f;
            maxOuterRadius = 1.80f;
            fireParticleEffect.startSize = (fireParticleEffect.startSize / 2); // Halves the size of particles when low on fuel
        }
        // At low intensity value we turn the torch off 
        else if(torch2dLightComponent.intensity < 0.2f)
        {
            stopDrainingTorchFuel();
            hasTorchBurntOut = true;
        }

        // This random number generator is to make the light radius flicker
        float rndNumber = Random.Range(minOuterRadius, maxOuterRadius);
        torch2dLightComponent.pointLightOuterRadius = rndNumber;
    }

    private void stopDrainingTorchFuel()
    {
        CancelInvoke();
        fireParticleEffect.Stop();
        torch2dLightComponent.enabled = false;
        isTorchActive = false;
    }




}
