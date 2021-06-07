using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : AiMovement
{
    protected override void Start()
    {
        if (Target == null)
            SetTarget(GameObject.FindGameObjectWithTag("MyPlayer").transform);
        var aiAttack = gameObject.GetComponent<AiAttack>();
        float attackRange = aiAttack == null ? 0 : aiAttack.AttackRange;
        followLowerDistance = attackRange * 0.7f;
        //Stand still within 3% of retreatUpperDistance to prevent rapid switching between follow and retreat
        retreatUpperDistance = followLowerDistance * 0.97f;
        base.Start();
    }
}
