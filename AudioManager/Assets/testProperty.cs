using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioEngine;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class testProperty : MonoBehaviour
{
    public List<AudioManager.AudioTrack> test1;

    public event Action<string> someEvent;
    public event Action<string> otherEvent;
    public event Action<string> noseEvent;
    public event Action noEvent;

    private string x;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        someEvent?.Invoke("BotonPlay");
    }
}
