using System.Collections;
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
        enemyAttack = gameObject.GetComponent<AiAttack>();
        enemyMovement = gameObject.GetComponent<AiMovement>();
        colliderList = gameObject.GetComponentsInChildren<Collider2D>();
        dataToPassBetweenScenes = GameObject.FindGameObjectWithTag("PassData").GetComponent<DataToPassBetweenScenes>();
    }

    protected override void Kill()
    {
        // Comparing two enums, can be more efficent than this. TODO: Make it more efficent
        if (dataToPassBetweenScenes.currentActivePlayerQuest.questType == Quest.QUESTTYPE.KILL_ENEMIES &&
            enemyType.ToString().Equals(dataToPassBetweenScenes.currentActivePlayerQuest.enemyTypeToKill.ToString()))
        {
            dataToPassBetweenScenes.currentActivePlayerQuest.IncrementKilledEnemies();
        }

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

        StartCoroutine(WaitForRespawn());
    }

    private IEnumerator FadeOutEnemy()
    {
        toggleActive(false);

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
        yield return new WaitForSeconds(respawnTime);
        Respawn();
    }

    protected override void Respawn()
    {
        // Set back enemy alpha color to 1 again which is normal max color
        Color clr = spriteRenderer.material.color;
        clr.a = 1f;
        spriteRenderer.material.color = clr;
        toggleActive(true);
        base.Respawn();
    }

    void toggleActive(bool active)
    {
        if (enemyMovement != null)
            enemyMovement.enabled = active;
        if (enemyAttack != null)
            enemyAttack.enabled = active;
        foreach (var collider in colliderList)
            collider.enabled = active;
        transform.Find("EnemyCanvas").gameObject.SetActive(active);
    }
}
