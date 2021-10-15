using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private AudioClip grass_footstep_1;
    private AudioClip dirt_footstep;
    static AudioSource audioSource;
    private AudioClip currentActiveFootstepSound;
    private GameObject footStepColliderObject;


    void Start()
    {

        grass_footstep_1 = Resources.Load<AudioClip>("grass_footstep_1");
        dirt_footstep = Resources.Load<AudioClip>("dirt_footstep");
        audioSource = GetComponent<AudioSource>();
        currentActiveFootstepSound = grass_footstep_1;
        footStepColliderObject = GameObject.FindGameObjectWithTag("MyPlayer").transform.Find("FootStepAudioCollider").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound()
    {
        audioSource.pitch = Random.Range(0.90f, 1.10f);
        audioSource.PlayOneShot(currentActiveFootstepSound);
    }

    public void TurnOnThenOffAudioFootstepCollider()
    {
        footStepColliderObject.SetActive(false);
        footStepColliderObject.SetActive(true);
    }
    // Not great using magic strings here so preferably an enum for future use in audioManager
    public void SwitchSoundType(string typeOfGround)
    {
        switch (typeOfGround)
        {
            case "Dirt":
                Debug.Log("it was dirt");
                currentActiveFootstepSound = dirt_footstep;
                break;
            case "Grass":
                Debug.Log("it was grass in my ass");
                currentActiveFootstepSound = grass_footstep_1;
                break;
            default:
                break;
        }

    }
}
