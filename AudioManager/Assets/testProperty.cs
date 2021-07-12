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

    public event Action<string> LaserShootIAIEvent;
    public event Action<string> LaserShootEvent;

    #endregion

    public List<AudioManager.AudioTrack> test1;

    private string x;

    private void Start()
    {
        LaserShootIAIEvent?.Invoke("LaserShootIAI");
        LaserShootEvent?.Invoke("LaserShoot");
        
    }

    public void testMethod()
    {
    }

}
