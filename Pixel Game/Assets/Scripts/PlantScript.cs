using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantScript : MonoBehaviour
{
    private GameObject seed;
    private GameObject halfway;
    private GameObject ready;
    public GameObject myPlayer;
    public bool isPlantReadyForPickup = false;

    public enum GROWTHSTAGES
    {
        SEED,
        HALFWAY,
        READY,
    }

    GROWTHSTAGES currentGrowthStage;
    

    void Start()
    {
        // In start we set the seed GFX to the only visible one
        // and also set the ENUM to Seed stage to easier control it
        seed = this.gameObject.transform.GetChild(0).gameObject;
        seed.SetActive(true);

        halfway = this.gameObject.transform.GetChild(1).gameObject;
        halfway.SetActive(false);

        ready = this.gameObject.transform.GetChild(2).gameObject;
        ready.SetActive(false);

        currentGrowthStage = GROWTHSTAGES.SEED;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickUpPlant()
    {
        Debug.Log("Picked up plant!");
        // TO-DO
        // Make sparkling green glow when player gets healed
        // TO-DO
        myPlayer.GetComponent<PlayerHealth>().Hp += 20;
        Destroy(gameObject);

    }
    // BUG
    // Plant sometimes does NOT evolve in GFX even though
    // the currentGrowthStage is being changed.
    // BUG
    public void IncrementPlantStage()
    {
        if(currentGrowthStage != GROWTHSTAGES.READY)
        {
            currentGrowthStage++;
            if(currentGrowthStage == GROWTHSTAGES.SEED)
            {
                seed.SetActive(true);
                halfway.SetActive(false);
                ready.SetActive(false);
            }
            else if(currentGrowthStage == GROWTHSTAGES.HALFWAY)
            {
                seed.SetActive(false);
                halfway.SetActive(true);
                ready.SetActive(false);
            }
        }
        else
        {
            seed.SetActive(false);
            halfway.SetActive(true);
            ready.SetActive(true);
            // not using this at the moment. Checking colliders within plant script below instead
            //gameObject.tag = "LootableItem";
        }
        Debug.Log("Current plantstage:" + currentGrowthStage);   
    }

    private void OnTriggerEnter2D(Collider2D collision)

    {
        if(collision.gameObject.tag == "InterractCollider")
        {
            // TO-DO
            // Fix that the plants edges highlight here to see easier pickup
            // TO-DO
            isPlantReadyForPickup = true;
            Debug.Log("Plant script interracted with players interraction collider!!!");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "InterractCollider")
        {
            // TO-DO
            // Fix that the plants edges STOP THE highlight 
            // here to see easier when pickup range is left
            // TO-DO
            isPlantReadyForPickup = false;
            Debug.Log("Player left plant script pickup range!");
        }
    }
}
