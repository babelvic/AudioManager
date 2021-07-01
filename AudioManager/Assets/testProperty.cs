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

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Invoke");
        someEvent?.Invoke("BotonPlay");
    }
}
