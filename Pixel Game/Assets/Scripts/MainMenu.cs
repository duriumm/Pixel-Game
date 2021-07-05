using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string sceneToLoad;
    public Animator transition;
    private bool isEasyMode = false;
    private bool isAutoSaving = false;
    private bool isSomethingEnabled = false;
    public GameObject easyModeRedCross, autoSaveRedCross, somethingRedCross;
    //public GameObject autoSaveRedCross;
    //public GameObject somethingRedCross; 
    public AudioClip frogCatPigSound, hoverOverButtonSound, clickButtonSound;
    //public AudioClip hoverOverButtonSound;
    //public AudioClip clickButtonSound;
    private GameObject mainCamera;
    public GameObject spriteGameObject;
    private Image movingSpriteImage;
    public Sprite ghastSprite;
    public Sprite frogSprite;
    private bool isSpriteFrog = true;
    public GameObject fatNpc;

    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");

        movingSpriteImage = spriteGameObject.GetComponent<Image>();
        movingSpriteImage.sprite = frogSprite;
        
    }
    public void ToggleFatNpc()
    {
        fatNpc.SetActive(!fatNpc.activeSelf);
    }

    public void ToggleImageSprite()
    {
        isSpriteFrog = !isSpriteFrog;
        Sprite spriteToUse = isSpriteFrog ? frogSprite : ghastSprite;
        movingSpriteImage.sprite = spriteToUse;
        Debug.Log("we toggled, sprite to use is: " + spriteToUse.name);
    }
    public void PlayClickButtonSound()
    {
        AudioSource.PlayClipAtPoint(clickButtonSound, mainCamera.transform.localPosition);
    }
    public void PlayHoverButtonSound()
    {
        AudioSource.PlayClipAtPoint(hoverOverButtonSound, mainCamera.transform.localPosition);
    }
    public void PlayFrogCatPigSound()
    {
        AudioSource.PlayClipAtPoint(frogCatPigSound, mainCamera.transform.localPosition);
    }
    // These 3 functions below can be made into one function. But for now it will be like this.
    // For now the functions only enable a red cross showing up but for future implementation
    // these functions will enable or disable certain settings in the settings menu
    public void ToggleEasyMode()
    {
        isEasyMode = !isEasyMode;
        easyModeRedCross.SetActive(isEasyMode);
        string enabledOrDisabled = isEasyMode ? "Enabled" : "Disabled";
        Debug.Log("Easy mode is: " + enabledOrDisabled);
    }
    public void ToggleAutoSave()
    {
        isAutoSaving = !isAutoSaving;
        autoSaveRedCross.SetActive(isAutoSaving);
        string enabledOrDisabled = isAutoSaving ? "Enabled" : "Disabled";
        Debug.Log("AutoSaving is: " + enabledOrDisabled);
    }
    public void ToggleSomething()
    {
        isSomethingEnabled = !isSomethingEnabled;
        somethingRedCross.SetActive(isSomethingEnabled);
        string enabledOrDisabled = isSomethingEnabled ? "Enabled" : "Disabled";
        Debug.Log("Something button is: " + enabledOrDisabled);
    }

    public void PlayGame()
    {
        StartCoroutine(LoadLevel(sceneToLoad));
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game quit :), just not in the editor");
    }

    // Starting the fading when loading new scene
    IEnumerator LoadLevel(string sceneToLoad)
    {
        transition.SetTrigger("Start");


        yield return new WaitForSeconds(0.5f);
        Debug.Log("Scene to load: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);

    }
}
