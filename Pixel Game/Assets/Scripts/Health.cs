using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private const float KnockbackDuration = 0.5f;
    private const float KnockbackSpeed = 7;
    private Rigidbody2D body;
    protected bool isAttackable = true;
    public bool IsAttackable => isAttackable;
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

    public bool KnockedBack { get; private set; }

    protected virtual void Start()
    {
        body = gameObject.GetComponent<Rigidbody2D>();
        spawnPoint = this.gameObject.transform.position;
        Hp = maxHp;
    }

    protected virtual void Kill()
    {
        if (body != null)
            body.velocity = Vector2.zero;
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
        //Todo: reduce damage depending on armor
        
        if (hurtSound != null)
            AudioSource.PlayClipAtPoint(hurtSound, this.gameObject.transform.position);

        StartCoroutine(HurtEffect());
        Knockback(sourcePoint);
        Hp -= damage;
        if (hp <= 0)
            Kill();
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
        body.velocity = (transform.position - sourcePoint).normalized * KnockbackSpeed;
        StartCoroutine(reduceAcceleration());
    }

    private IEnumerator reduceAcceleration()
    {
        // Setting the KnockedBack property to true lets the Movement script lower acceleration during the knockback
        // to reduce the ability to counteract the knockback
        KnockedBack = true;
        yield return new WaitForSeconds(KnockbackDuration);
        KnockedBack = false;
    }
}
    //}