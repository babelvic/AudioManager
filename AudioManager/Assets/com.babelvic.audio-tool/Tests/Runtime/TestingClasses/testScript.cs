using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class testScript : MonoBehaviour
{
    #region EventsRegion

    public event Action<string> LaserShootIAIEvent;
    public event Action<string> LaserShootEvent;

    #endregion

    //Start asd asd a sd asd
    void Start ( )
    {
        LaserShootIAIEvent?.Invoke("LaserShootIAI");
        LaserShootEvent?.Invoke("LaserShoot");
      
    }
    
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SceneManager.LoadScene(0);
        }
    }

    void asss(string a, Boolean d = false, int i = 6)
    {
        
    }
}
