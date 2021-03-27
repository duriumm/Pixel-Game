using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderNpc : MonoBehaviour
{
    public List<GameObject> shopItems = new List<GameObject>(); //  We can now add custom items in inspector for every trader
    private GameObject merchantInfoBubble;
    public bool isPlayerInRange;
    public AudioClip interractSound;
    public string traderID;
    
    private GameObject mainCamera;
    private GameObject shopScreen;
    private GameObject dataToPassGameObject;

    private GameObject invManager;
    void Start()
    {
        invManager = GameObject.FindGameObjectWithTag("InventoryManager");
        merchantInfoBubble = gameObject.transform.Find("MerchantPopup").gameObject;
        merchantInfoBubble.SetActive(false);
        mainCamera = GameObject.FindWithTag("MainCamera");
        shopScreen = GameObject.Find("ShopScreen");
        dataToPassGameObject = GameObject.FindGameObjectWithTag("PassData");
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

            // Assign current active trader to be this trader.
            dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().currentActiveTrader = traderID;
            Debug.Log(this.gameObject.name + " is ready fpr shopping");
            // Clear the shopScreen list so we only put in the NPC list of items in a empty list
            shopScreen.GetComponent<ShopScreen>().shopScreenItemList.Clear();
            foreach (var item in shopItems)
            {
                shopScreen.GetComponent<ShopScreen>().shopScreenItemList.Add(item);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MyPlayer")
        {
            merchantInfoBubble.SetActive(false);
            isPlayerInRange = false;

            shopScreen.GetComponent<ShopScreen>().CloseShopScreen();
            invManager.GetComponent<PlayerInventory>().ClosingUI();
        }
    }


}
