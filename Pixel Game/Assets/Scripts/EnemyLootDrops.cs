using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLootDrops : MonoBehaviour
{
    // Drag all loot items you want the enemy to have here
    public List<CustomItem> itemList;
    private GameObject instantiatedObject;
    // Start is called before the first frame update
    void Start()
    {
        //DropLoot();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DropLoot()
    {

        // LOOP THRU ALL ITEMS IN LIST
        //for (int i = 0; i < itemList.Count; i++)
        //{
        //    Debug.Log("Loop number"+i);
        //}


        // Set a random number to spawn ONE item from the enemy loot list
        int randomNumber = Random.Range(0, itemList.Count);
        // Spawn the random loot from the list
        Instantiate(itemList[randomNumber], null);
        // Set spawn position to where the enemy is standing currently
        itemList[randomNumber].transform.position = gameObject.transform.position;

        Debug.Log("random loot  is: " + itemList[randomNumber].name);

    }
}
