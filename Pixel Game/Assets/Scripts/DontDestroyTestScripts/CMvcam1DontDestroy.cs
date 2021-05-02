using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMvcam1DontDestroy : MonoBehaviour
{
    private static bool cMvcam1Exists;
    void Start()
    {
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
