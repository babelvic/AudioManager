using System;
using System.Collections;
using System.Collections.Generic;
using AudioEngine;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class testProperty : MonoBehaviour
{
    public List<AudioManager.AudioTrack> test1;

    public event Action<string> someEvent;

    private void Update()
    {
        someEvent?.Invoke("BotonPlay");
    }
}
