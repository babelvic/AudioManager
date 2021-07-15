using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioEngine;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class testProperty : MonoBehaviour
{
    #region EventsRegion

    public event Action<string> LaserShootEvent;
    public event Action<string> LaserShootIAIEvent;

    #endregion

    
    private void Start()
    {

    }

    public void TestMethod()
    {
        LaserShootEvent?.Invoke("LaserShoot");
        LaserShootIAIEvent?.Invoke("LaserShootIAI");
        
    }

}
