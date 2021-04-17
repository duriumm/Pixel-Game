using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeAttack : EnemyAttack
{
    protected override void Attack()
    {
        var playerHealth = playerGameObject.GetComponent<PlayerHealth>();
        playerHealth.TakeDamage(20);
        playerGameObject.GetComponent<Rigidbody2D>().MovePosition(playerGameObject.transform.position * 2 - transform.position);
    }
}
