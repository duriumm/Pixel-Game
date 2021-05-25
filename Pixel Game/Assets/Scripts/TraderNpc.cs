using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderNpc : MonoBehaviour
{
    public List<GameObject> shopItems = new List<GameObject>(); //  We can now add custom items in inspector for every trader
    private GameObject merchantInfoBubble;
    public AudioClip interractSound;
    private GameObject mainCamera;
    private GuiScreenManager guiScreenManager;
    void Start()
    {
        merchantInfoBubble = gameObject.transform.Find("MerchantPopup").gameObject;
        merchantInfoBubble.SetActive(false);
        mainCamera = GameObject.FindWithTag("MainCamera");
        guiScreenManager = GameObject.Find("Canvas/Screens").gameObject.GetComponent<GuiScreenManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MyPlayer")
        {
            merchantInfoBubble.SetActive(true);
            AudioSource.PlayClipAtPoint(interractSound, mainCamera.transform.position);
            guiScreenManager.Shop.ItemList.Clear();
            foreach (var item in shopItems)
                guiScreenManager.Shop.ItemList.Add(item);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MyPlayer")
        {
            merchantInfoBubble.SetActive(false);
        }
    }


    public void OpenNpcShop()
    {
        guiScreenManager.Toggle(GuiScreenType.Shop);
    }



}
