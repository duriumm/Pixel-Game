using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsScreen : MonoBehaviour
{
    private GameObject player;
    private GameObject dataToPassGameObject;
    private GameObject statsScreen;
    public bool isStatsScreenOpen;
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI armorText;
    private TextMeshProUGUI attackText;
    private TextMeshProUGUI buffsText;
    private TextMeshProUGUI deBuffsText;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("MyPlayer");
        dataToPassGameObject = GameObject.FindGameObjectWithTag("PassData");
        statsScreen = GameObject.Find("StatsScreen");
        isStatsScreenOpen = false;
        healthText = statsScreen.transform.Find("HealthText").gameObject.GetComponent<TextMeshProUGUI>();
        armorText = statsScreen.transform.Find("ArmorText").gameObject.GetComponent<TextMeshProUGUI>();
        attackText = statsScreen.transform.Find("AttackText").gameObject.GetComponent<TextMeshProUGUI>();

        CloseStatsScreen();


}

    public void OpenStatsScreen()
    {
        isStatsScreenOpen = true;
        statsScreen.SetActive(true);
        // Health updates from its own local value which is not regurarly saved in DB since it changes a lot
        dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().UpdatePlayerHealthDB();
        // The rest of the values (armor, attack. buff etc) 
        UpdateStatsText();
        


}

    public void CloseStatsScreen()
    {
        Debug.Log("We closed stats screen");
        isStatsScreenOpen = true;
        statsScreen.SetActive(false);
    }

    public void UpdateStatsText()
    {
        healthText.text = "Health: " + dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().playerHealthDB.ToString();
        armorText.text = "Armor: " + dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().playerArmorDB.ToString();
        attackText.text = "Attack: " + dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().playerAttackDB.ToString();


    }


}
