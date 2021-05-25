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
    private ShopScreen shopScreen;
    private GameObject dataToPassGameObject;
    private UIScreenManager uiScreenManager;

    private GameObject invManager;
    void Start()
    {
        invManager = GameObject.FindGameObjectWithTag("InventoryManager");
        merchantInfoBubble = gameObject.transform.Find("MerchantPopup").gameObject;
        merchantInfoBubble.SetActive(false);
        mainCamera = GameObject.FindWithTag("MainCamera");
        dataToPassGameObject = GameObject.FindGameObjectWithTag("PassData");
        uiScreenManager = GameObject.Find("Canvas/Screens").gameObject.GetComponent<UIScreenManager>();
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
            // TRADER ID HAS TO BE THE SAME AS GAMEOBJECTS NAME
            dataToPassGameObject.GetComponent<DataToPassBetweenScenes>().currentActiveTrader = traderID;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MyPlayer")
        {
            merchantInfoBubble.SetActive(false);
            isPlayerInRange = false;

            //shopScreen.GetComponent<ShopScreen>().CloseShopScreen();
            //invManager.GetComponent<PlayerInventory>().ClosingUI();
        }
    }


    public void OpenNpcShop()
    {
        uiScreenManager.Toggle(UIScreenType.Shop);
        if (shopScreen == null)
            shopScreen = GameObject.Find("ShopScreen").GetComponent<ShopScreen>();

        uiScreenManager.Shop.ItemList.Clear();
        foreach (var item in shopItems)
        {
            uiScreenManager.Shop.ItemList.Add(item);
        }
    }



}
