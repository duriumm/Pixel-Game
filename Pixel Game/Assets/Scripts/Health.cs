using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    protected bool isAttackable = true;
    [SerializeField]
    private int maxHp = 100;
    private int hp;
    public int Hp
    {
        protected set
        {
            hp = Math.Min(maxHp, value);
            slider.value = hp;
        }
        get => hp;
    }
    public bool HasFullHp => hp == maxHp;
    [SerializeField]
    protected Slider slider;
    private Vector2 spawnPoint;
    [SerializeField]
    private AudioClip deathSound;
    [SerializeField]
    private AudioClip hurtSound;

    protected virtual void Start()
    {
        spawnPoint = this.gameObject.transform.position;
        Hp = maxHp;
    }

    protected virtual void Kill()
    {
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, this.gameObject.transform.position);
    }

    protected void Respawn()
    {
        this.gameObject.transform.position = spawnPoint;
        Hp = maxHp;
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
        Hp -= damage;
        if (hp > 0)
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
        Hp += value;
    }

    private IEnumerator HurtEffect()
    {
        
        yield return new WaitForSeconds(0.1f);
    }

    private void Knockback(Vector3 sourcePoint)
    {
        //Todo: use rigid body
        Vector2 difference = transform.position - sourcePoint;
        transform.position = new Vector2(transform.position.x + difference.x, transform.position.y + difference.y);
    }
}
