using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsScreen : GuiScreen
{
    private GameObject player;
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI armorText;
    private TextMeshProUGUI attackText;
    private TextMeshProUGUI buffsText;
    private TextMeshProUGUI deBuffsText;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("MyPlayer");
        healthText = transform.Find("HealthText").gameObject.GetComponent<TextMeshProUGUI>();
        armorText = transform.Find("ArmorText").gameObject.GetComponent<TextMeshProUGUI>();
        attackText = transform.Find("AttackText").gameObject.GetComponent<TextMeshProUGUI>();
        Close();
    }

    public override void Open()
    {
        UpdateStatsText();
        base.Open();
    }

    public void UpdateStatsText()
    {
        var health = player.GetComponent<PlayerHealth>();
        healthText.text = $"Health: {health.Hp} / {health.MaxHp}";
        armorText.text = "Armor: " + health.Defense;
        attackText.text = "Attack: " + player.GetComponent<Attack>().CurrentWeapon.Damage;
    }
}
