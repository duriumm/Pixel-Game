using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CMvcam1DontDestroy : MonoBehaviour
{
    private static bool cMvcam1Exists;
    private GameObject player;
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private string nameOfMainMenu = "Main_Menu_Lasse";
    void Start()
    {
        if(SceneManager.GetActiveScene().name != nameOfMainMenu)
        {
            player = GameObject.FindGameObjectWithTag("MyPlayer");
            cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("VCam").GetComponent<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = player.transform;
        }

        if (!cMvcam1Exists)
        {
            cMvcam1Exists = true;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


}
