using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string sceneToLoad;
    public Animator transition;
    private bool isEasyMode = false;
    private bool isAutoSaving = false;
    private bool isSomethingEnabled = false;
    public GameObject easyModeRedCross;
    public GameObject autoSaveRedCross;
    public GameObject somethingRedCross; 
    public AudioClip frogCatPigSound;
    public AudioClip hoverOverButtonSound;
    public AudioClip clickButtonSound;
    private GameObject mainCamera;

    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera");
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
    // these functions will enable or disable certain settings
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

    IEnumerator LoadLevel(string sceneToLoad)
    {
        transition.SetTrigger("Start");
        // TO-DO - Disable player movement, Make player invincible

        yield return new WaitForSeconds(0.5f);
        Debug.Log("Scene to load: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);

    }
}
