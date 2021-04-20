using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class HealthBehaviour : MonoBehaviour
{
    public bool isAttackable = true;
    
    [SerializeField]
    private int health;
    public int Health
    {
        protected set
        {
            health = value;
            slider.value = health;
        }
        get => health;
    }
    [SerializeField]
    protected Slider slider;
    private Vector2 spawnPoint;
    public AudioClip deathSound;
    public AudioClip hurtSound;

    protected virtual void Start()
    {
        spawnPoint = this.gameObject.transform.position;
    }

    protected virtual void Kill()
    {
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, this.gameObject.transform.position);
    }

    protected void Respawn()
    {
        this.gameObject.transform.position = spawnPoint;
        Health = 100;
    }

    public void TakeDamage(int damage, Vector2 sourcePoint)
    {
        if (!isAttackable)
            return;
        
        //Todo: reduce damage depending on armor
        
        if (hurtSound != null)
            AudioSource.PlayClipAtPoint(hurtSound, this.gameObject.transform.position);

        StartCoroutine(HurtEffect());
        Knockback(sourcePoint);
        Health -= damage;
        if (health > 0)
            StartCoroutine(waitForDamageCooldown());
        else
            Kill();
    }

    IEnumerator waitForDamageCooldown()
    {
        isAttackable = false;
        yield return new WaitForSeconds(1);
        isAttackable = true;
    }

    public void GainHealth(int value)
    {
        Health += value;
    }

    private IEnumerator HurtEffect()
    {
        
        yield return new WaitForSeconds(0.1f);
    }

    private void Knockback(Vector3 sourcePoint)
    {
        //Todo: Use coroutine perhaps to push rigid body backwards for a certain amount of time
        Vector2 difference = transform.position - sourcePoint;
        transform.position = new Vector2(transform.position.x + difference.x, transform.position.y + difference.y);
    }
}
