﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : Health
{
    private SpriteRenderer spriteRenderer;

    public enum ENEMYTYPE
    {
        GHOST,
        HUMAN,
        BEAST
    }
    public ENEMYTYPE enemyType;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Kill()
    {
        base.Kill();
        gameObject.GetComponent<EnemyLootDrops>().DropLoot();
        switch (enemyType)
        {
            case ENEMYTYPE.GHOST:
                StartCoroutine(FadeOutEnemy());
                break;
            case ENEMYTYPE.BEAST:
                //Todo: implement beast death animation
                StartCoroutine(FadeOutEnemy());
                break;
            case ENEMYTYPE.HUMAN:
                //Todo: implement human death animation
                StartCoroutine(FadeOutEnemy());
                break;
        }
    }

    private IEnumerator FadeOutEnemy()
    {
        // Disable enemy is attackable to not get hit by player and also
        // make the whole movementscript disabled so he cant move OR attack
        gameObject.GetComponent<EnemyAttack>().enabled = false;
        isAttackable = false;
        gameObject.GetComponent<EnemyMovement>().enabled = false;

        // If enemy has shot attack, destroy the shot object so it doesnt get stuck in mid air on enemy death
        gameObject.GetComponent<EnemyShotAttack>()?.DestroyShots();

        // This fade last for 2 sek and turns enemy from 1 in alpha (max) to 
        // 0 in alpha (lowest)
        //Debug.Log("start fade");
        for (float f = 1f; f >= -0.05f; f-= 0.05f)
        {
            Color c = spriteRenderer.material.color;
            c.a = f;
            spriteRenderer.material.color = c;
            yield return new WaitForSeconds(0.10f);
        }
        //Debug.Log("END fade");

        // Set back enemy alpha color to 1 again which is normal max color
        Color clr = spriteRenderer.material.color;
        clr.a = 1f;
        spriteRenderer.material.color = clr;        

        // Enable enemy movement and being attackable again aswell as attack enabling
        isAttackable = true;
        gameObject.GetComponent<EnemyMovement>().enabled = true;
        gameObject.GetComponent<EnemyAttack>().enabled = true;

        Respawn();
    }


}