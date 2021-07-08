using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] protected Slider slider;
    private Vector2 spawnPoint;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    private ScreenShake screenShake;

    Movement movement;

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
    public int Defense { get; set; }

    public virtual void Start()
    {
        Hp = maxHp;
        OnSceneChange();
        movement = GetComponent<Movement>();
        screenShake = GameObject.FindGameObjectWithTag("VCam").GetComponent<ScreenShake>();
    }

    public virtual void OnSceneChange()
    {
        spawnPoint = gameObject.transform.position;
    }

    protected virtual void Kill()
    {
        if (movement != null)
            movement.Velocity = Vector2.zero;
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

        screenShake.StartScreenShake();

        StartCoroutine(HurtEffect());
        StartCoroutine(movement.KnockBack(sourcePoint));
        Hp -= damage;
        if (hp <= 0)
            Kill();
    }

    private IEnumerator HurtEffect()
    {
        // Maybe do something interesting here
        yield return new WaitForSeconds(0.1f);
    }
}
