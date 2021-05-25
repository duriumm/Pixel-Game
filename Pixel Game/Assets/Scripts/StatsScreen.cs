using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsScreen : UIScreen
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

        Close();


}

    public override void Open()
    {
        isStatsScreenOpen = true;
        statsScreen.SetActive(true);
        UpdateStatsText();
        base.Open();
    }

    public override void Close()
    {
        Debug.Log("We closed stats screen");
        isStatsScreenOpen = true;
        base.Close();
    }

    public void UpdateStatsText()
    {
        var health = player.GetComponent<PlayerHealth>();
        healthText.text = $"Health: {health.Hp} / {health.MaxHp}";
        armorText.text = "Armor: " + health.Defense;
        attackText.text = "Attack: " + player.GetComponent<Attack>().CurrentWeapon.Damage;
    }


}
