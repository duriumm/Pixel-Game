using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconPopupInRange : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject popupIcon;
    public AudioClip popupSound;
    private GameObject mainCamera;
    void Start()
    {
        // Always look on the first index of child objects to get the popup icon. Ensure that popup icon is on first index in npc
        popupIcon = this.gameObject.transform.GetChild(0).gameObject;
        mainCamera = GameObject.FindWithTag("MainCamera");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MyPlayer")
        {
            popupIcon.SetActive(true);
            AudioSource.PlayClipAtPoint(popupSound, mainCamera.transform.position);

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MyPlayer")
        {
            popupIcon.SetActive(false);
        }
    }
}
