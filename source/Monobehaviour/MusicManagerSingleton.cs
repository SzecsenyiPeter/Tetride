using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManagerSingleton : MonoBehaviour {

    static MusicManagerSingleton instance = null;
    public static MusicManagerSingleton Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(instance);
    }

}
