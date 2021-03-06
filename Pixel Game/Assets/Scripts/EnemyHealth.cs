﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int enemyHealth;
    public Slider enemyHealthSlider;
    private GameObject enemyObject;
    private Vector2 enemySpawnPoint;
    private SpriteRenderer spriteRenderer;
    public AudioClip ghastDeathSound;
    public GameObject mainCamera;

    public enum ENEMYTYPE
    {
        GHOST,
        HUMAN,
        BEAST
    }
    public ENEMYTYPE enemyType;

    // Start is called before the first frame update
    void Start()
    {
        enemyHealth = 20; // TEST TO 20
        enemyHealthSlider.value = enemyHealth;
        enemyObject = this.gameObject;
        enemySpawnPoint = enemyObject.transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damageToReceive)
    {
        enemyHealth -= damageToReceive;
        enemyHealthSlider.value = enemyHealth;
        if (enemyHealth <= 0)
        {
            respawnEnemy();
            gameObject.GetComponent<EnemyLootDrops>().DropLoot();
        }
    }

    public void GainHealth(int healthToGain)
    {
        enemyHealth += healthToGain;
        enemyHealthSlider.value = enemyHealth;
    }

    // TO-DO
    // Change respawnEnemy name to something else.. he is dying thats it.
    // TO-DO
    private void respawnEnemy()
    {
        // TO-DO
        // This should be fixed, now its checking for every enemy type
        // TO-DO
        if (enemyType == ENEMYTYPE.GHOST)
        {

            StartCoroutine(FadeOutGhost());
        }
    }

    private IEnumerator FadeOutGhost()
    {
        AudioSource.PlayClipAtPoint(ghastDeathSound, mainCamera.transform.position);

        // Disable ghost is attackable to not get hit by player and also
        // make the whole movementscript disabled so he cant move OR attack
        enemyObject.GetComponent<EnemyAttack>().enabled = false;
        enemyObject.GetComponent<EnemyCollision>().isEnemyAttackable = false;
        enemyObject.GetComponent<EnemyMovement>().enabled = false;

        // Destroy the particle attack obejct so it doesnt get stuck in mid air on ghost death
        enemyObject.GetComponent<EnemyAttack>().DestroyParticleAttackObject();

        // This fade last for 2 sek and turns ghost from 1 in alpha (max) to 
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

        // Set back ghast alpha color to 1 again which is normal max color
        Color clr = spriteRenderer.material.color;
        clr.a = 1f;
        spriteRenderer.material.color = clr;        

        // Enable ghost movement and being attackable again aswell as attack enabling
        enemyObject.GetComponent<EnemyCollision>().isEnemyAttackable = true;
        enemyObject.GetComponent<EnemyMovement>().enabled = true;
        enemyObject.GetComponent<EnemyAttack>().enabled = true;

        // Respawn ghost at startpoint and set his hp and slider to max
        enemyObject.transform.position = enemySpawnPoint;
        enemyHealth = 100;
        enemyHealthSlider.value = enemyHealth;

    }
}
