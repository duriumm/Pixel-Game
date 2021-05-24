using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    private const float KnockbackDuration = 0.5f;
    private const float KnockbackSpeed = 7;
    public int Defense { get; set; } 
    private Rigidbody2D rigidbody;
    [SerializeField]
    private int maxHp = 100;
    public int MaxHp
    {
        get => maxHp;
        set => maxHp = value;
    }
    private int hp;
    public int Hp
    {
        set
        {
            hp = Math.Min(maxHp, value);
            slider.value = hp;
        }
        get => hp;
    }
    public bool HasFullHp => hp == maxHp; // If hp == maxHp, bool is true :)

    [SerializeField]
    protected Slider slider;
    private Vector2 spawnPoint;
    [SerializeField]
    private AudioClip deathSound;
    [SerializeField]
    private AudioClip hurtSound;

    public bool KnockedBack { get; private set; }

    public virtual void Start()
    {
        Hp = maxHp;
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
    }

    public virtual void OnSceneChange()
    {
        spawnPoint = gameObject.transform.position;
    }

    protected virtual void Kill()
    {
        if (rigidbody != null)
            rigidbody.velocity = Vector2.zero;
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, this.gameObject.transform.position);
    }

    protected virtual void Respawn()
    {
        this.gameObject.transform.position = spawnPoint;
        Hp = maxHp;
    }

    public void TakeDamage(int damage, Vector2 sourcePoint)
    {
        damage = Math.Max(1, damage - Defense);

        if (hurtSound != null)
            AudioSource.PlayClipAtPoint(hurtSound, this.gameObject.transform.position);

        StartCoroutine(HurtEffect());
        Knockback(sourcePoint);
        Hp -= damage;
        if (hp <= 0)
            Kill();
    }

    private IEnumerator HurtEffect()
    {
        yield return new WaitForSeconds(0.1f);
    }

    private void Knockback(Vector3 sourcePoint)
    {
        rigidbody.velocity = (transform.position - sourcePoint).normalized * KnockbackSpeed;
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
