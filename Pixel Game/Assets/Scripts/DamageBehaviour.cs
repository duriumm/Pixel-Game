using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBehaviour : MonoBehaviour
{
    public enum Group { Players, Mobs }

    [SerializeField]
    private int attackPower;
    //public int AttackPower => attackPower;
    [SerializeField]
    public bool CanInflictDamage => attackPower > 0;
    [SerializeField]
    private bool canTakeDamage;
    //public bool CanTakeDamage => canTakeDamage;
    [SerializeField]
    private Group group;
    [SerializeField]
    private GameObject damageReceiver;
    public GameObject GameObject => damageReceiver;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var colliderDamageBehaviour = collider.gameObject.GetComponent<DamageBehaviour>();
        
        //Can't take or inflict damage if in same group
        if (colliderDamageBehaviour == null || colliderDamageBehaviour.group == group)
            return;

        if (colliderDamageBehaviour.CanInflictDamage && canTakeDamage)
        {
            var healthBehaviour = damageReceiver.GetComponent<Health>();
            healthBehaviour.TakeDamage(colliderDamageBehaviour.attackPower, collider.transform.position);
        }
    }
}
