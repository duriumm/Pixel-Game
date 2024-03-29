﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : Health
{
    [SerializeField]
    private float respawnTime = 100;
    private SpriteRenderer spriteRenderer;
    private AiAttack enemyAttack;
    private AiMovement enemyMovement;
    private Collider2D[] colliderList;
    private DataToPassBetweenScenes dataToPassBetweenScenes;
    private GameObject player;
    public enum ENEMYTYPE
    {
        GHOST,
        HUMAN,
        BEAST
    }
    public ENEMYTYPE enemyType;

    public override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyAttack = gameObject.GetComponent<AiAttack>();
        enemyMovement = gameObject.GetComponent<AiMovement>();
        colliderList = gameObject.GetComponentsInChildren<Collider2D>();
        dataToPassBetweenScenes = GameObject.FindGameObjectWithTag("PassData").GetComponent<DataToPassBetweenScenes>();
        player = GameObject.Find("MyCharacter");
    }

    protected override void Kill()
    {
        dataToPassBetweenScenes.ActiveQuests.TryIncrementKilledEnemies(gameObject.name);
        base.Kill();
        gameObject.GetComponent<EnemyLootDrops>().DropLoot();
        StartCoroutine(PlayDeathAnimation());
    }

    private IEnumerator PlayDeathAnimation()
    {
        disableEnemy(true);
        switch (enemyType)
        {
            case ENEMYTYPE.GHOST:
                yield return FadeOutEnemy();
                break;
            case ENEMYTYPE.BEAST:
                //Todo: implement beast death animation
                yield return FadeOutEnemy();
                break;
            case ENEMYTYPE.HUMAN:
                //Todo: implement human death animation
                yield return FadeOutEnemy();
                break;
        }
        yield return WaitForRespawn();
    }

    private IEnumerator FadeOutEnemy()
    {
        // This fade last for 2 sek and turns enemy from 1 in alpha (max) to 
        // 0 in alpha (lowest)
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            Color c = spriteRenderer.material.color;
            c.a = f;
            spriteRenderer.material.color = c;
            yield return new WaitForSeconds(0.10f);
        }
    }

    IEnumerator WaitForRespawn()
    {
        float minRespawnSqrDistance = 200;
        float sqrDistanceToPlayer;
        do
        {
            yield return new WaitForSeconds(respawnTime);
            sqrDistanceToPlayer = Vector2.SqrMagnitude(player.transform.position - transform.position);
            if (sqrDistanceToPlayer < minRespawnSqrDistance)
                Debug.Log("Distance to player too short for respawn: " + sqrDistanceToPlayer);
        }
        while (sqrDistanceToPlayer < minRespawnSqrDistance);
        Respawn();
    }

    protected override void Respawn()
    {
        // Set back enemy alpha color to 1 again which is normal max color
        Color clr = spriteRenderer.material.color;
        clr.a = 1f;
        spriteRenderer.material.color = clr;
        disableEnemy(false);
        base.Respawn();
    }

    void disableEnemy(bool disable)
    {
        if (enemyMovement != null)
            enemyMovement.enabled = !disable;
        if (enemyAttack != null)
            enemyAttack.enabled = !disable;
        foreach (var collider in colliderList)
            collider.enabled = !disable;
        transform.Find("EnemyCanvas").gameObject.SetActive(!disable);
    }
}
