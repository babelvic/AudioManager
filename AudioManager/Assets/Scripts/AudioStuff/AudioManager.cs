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
        private static AudioManager _instance;

        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();
                }

                return _instance;
            }
        }

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

            public bool playOnAwake = false;
            public bool loop = false;
            [InspectorName("3D Settings")]public bool dimensional = false;

            [Range(0, 256)] public int priority = 150;
            [Range(0f, 1f)] public float volume = 1f;
            [Range(-3f, 3f)] public float pitch = 1f;
            [Range(0f, 1f)] public float spatialBlend = 0f;

            [InspectorName("3D Object Container")]public GameObject objectReference;
            public float minDistance = 1f;
            public float maxDistance = 500f;

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
            if (!_instance) Configure();
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
                //Check if the audio is a 3d audio track //If there is an object reference then we can set the audio in the reference for setting it after
                if (t.dimensional && t.objectReference != null)
                {
                    t.h_source = t.objectReference.AddComponent<AudioSource>();
                    t.h_source.clip = t.clip;
                    t.h_source.outputAudioMixerGroup = t.mixer;
                    t.h_source.playOnAwake = t.playOnAwake;
                    t.h_source.loop = t.loop;
                    t.h_source.priority = t.priority;
                    t.h_source.volume = t.volume;
                    t.h_source.pitch = t.pitch;
                    t.spatialBlend = 1f;
                    t.h_source.spatialBlend = t.spatialBlend;
                    t.h_source.minDistance = t.minDistance;
                    t.h_source.maxDistance = t.maxDistance;
                }
                else
                {
                    //Normal Set
                    t.h_source = gameObject.AddComponent<AudioSource>();
                    t.h_source.clip = t.clip;
                    t.h_source.outputAudioMixerGroup = t.mixer;
                    t.h_source.playOnAwake = t.playOnAwake;
                    t.h_source.loop = t.loop;
                    t.h_source.priority = t.priority;
                    t.h_source.volume = t.volume;
                    t.h_source.pitch = t.pitch;
                    t.h_source.spatialBlend = t.spatialBlend;
                }

                //Play Sounds on Awake
                if (t.h_source.playOnAwake) t.h_source.Play();
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
            _instance = this;
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