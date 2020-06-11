using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
using UnityEngine.SceneManagement;

public class ShowAd : MonoBehaviour
{

    public void Start()
    {
#if UNITY_ADS
        Advertisement.Initialize("2698706", false);
#endif
    }
#if UNITY_ADS


    public void DisplayAD()
    {
        
        
         if (Advertisement.IsReady() && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            var options = new ShowOptions { resultCallback = AdFinishedCallback,  };
            Advertisement.Show("video", options);
        }
        else
        {
            SceneManager.LoadScene(0);
        }


    }
    void AdFinishedCallback(ShowResult result)
    {

        SceneManager.LoadScene(0);

    }
#endif
}
