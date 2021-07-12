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

    public event Action<string> BotonPlayOFGEvent;
    public event Action<string> BotonPlayEvent;

    #endregion

    public List<AudioManager.AudioTrack> test1;

    public event Action<string> someEvent;
    public event Action<string> otherEvent;
    public event Action<string> noseEvent;
    public event Action noEvent;

    private string x;

    private void Start()
    {
        BotonPlayOFGEvent?.Invoke("BotonPlayOFG");
        BotonPlayEvent?.Invoke("BotonPlay");
        someEvent?.Invoke("BotonPlay");
    }

    public void testMethod()
    {
        
    }

}
