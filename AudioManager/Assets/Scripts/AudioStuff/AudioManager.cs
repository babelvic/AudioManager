using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.Audio;

namespace AudioEngine
{
    [System.Serializable]
    public class AudioManager : MonoBehaviour
    {
        //Static reference for singleton pattern
        public static AudioManager instance;

        //A member for debuging or not messages from this class
        public bool debug;
        
        //The track array member for setting audio in the inspector
        public List<AudioTrack> tracks;
        
        //The mixers of the game project
        public List<AudioTrackMixer> mixers;
        
        //MixerGroupFindStuff
        public List<string> mixerGroupPopup;
        public List<int> mixerIndex;
        
        [System.Serializable]
        public class AudioTrack
        {
            public string name;
            public AudioClip clip;
            public AudioMixerGroup mixer;

            public bool loop;

            [Range(0, 256)] public int priority;
            [Range(0f, 1f)] public float volume;
            [Range(-3f, 3f)] public float pitch;
            [Range(0f, 1f)] public float spatialBlend;

            [HideInInspector] public AudioSource h_source;
            
        }

        [System.Serializable]
        public class AudioTrackMixer
        {
            public string name;
            public AudioMixerGroup mixerGroup;
            public bool dropdownMixer;
        }

        #region UnityFunctions

        private void Awake()
        {
            if (!instance) Configure();
            else Destroy(this.gameObject);
            
            //Set the audio
            SetAudioInScene();
        }

        #endregion

        #region Public Functions

        public void SetAudioInScene()
        {
            foreach (AudioTrack t in tracks)
            {
                t.h_source = gameObject.AddComponent<AudioSource>();
                t.h_source.clip = t.clip;
                t.h_source.outputAudioMixerGroup = t.mixer;
                t.h_source.loop = t.loop;
                t.h_source.priority = t.priority;
                t.h_source.volume = t.volume;
                t.h_source.pitch = t.pitch;
                t.h_source.spatialBlend = t.spatialBlend;
            }
        }

        public void PlayTrack(string name)
        {
            foreach (var t in tracks)
            {
                if (t.name == name)
                {
                    t.h_source.Play();
                    break;
                }
            }
        }

        #endregion
        
        #region Private Functions

        #region Setting Functions

        private void Configure()
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        #endregion

        #region Logs Functions

        private void Log(string _msg)
        {
            if (!debug) return;
            Debug.Log($"[{this.name}]: {_msg}");
        }
        
        private void LogWarning(string _msg)
        {
            if (!debug) return;
            Debug.LogWarning($"[{this.name}]: {_msg}");
        }

        #endregion
        
        #endregion
    }
}