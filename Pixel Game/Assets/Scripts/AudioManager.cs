using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private AudioClip grass_footstep_1;
    private AudioClip dirt_footstep;
    static AudioSource audioSource;
    private AudioClip currentActiveFootstepSound;


    void Start()
    {

        grass_footstep_1 = Resources.Load<AudioClip>("grass_footstep_1");
        dirt_footstep = Resources.Load<AudioClip>("dirt_footstep");
        audioSource = GetComponent<AudioSource>();
        currentActiveFootstepSound = grass_footstep_1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound()
    {
        //Debug.Log("PLAYED");
        audioSource.pitch = Random.Range(0.90f, 1.10f);
        audioSource.PlayOneShot(currentActiveFootstepSound);
    }
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
