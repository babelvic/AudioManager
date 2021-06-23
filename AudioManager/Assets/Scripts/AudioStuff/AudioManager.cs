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
        
        [System.Serializable]
        public class AudioTrack
        {
            public string name;
            public AudioClip clip;
            public AudioMixerGroup mixer;

            public bool loop;

            [Range(0f, 256f)] public float priority;
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
        }

        #endregion

        #region Public Functions

        

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