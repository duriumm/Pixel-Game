using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderNpc : MonoBehaviour
{

    private GameObject merchantInfoBubble;
    private bool isPlayerInRange;
    public AudioClip interractSound;
    private GameObject mainCamera;
    void Start()
    {
        merchantInfoBubble = gameObject.transform.Find("MerchantPopup").gameObject;
        merchantInfoBubble.SetActive(false);
        mainCamera = GameObject.FindWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MyPlayer")
        {
            merchantInfoBubble.SetActive(true);
            isPlayerInRange = true;
            AudioSource.PlayClipAtPoint(interractSound, mainCamera.transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MyPlayer")
        {
            merchantInfoBubble.SetActive(false);
            isPlayerInRange = false;
        }
    }
}
