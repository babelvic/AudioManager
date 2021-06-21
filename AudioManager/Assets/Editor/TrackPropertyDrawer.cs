using System;
using System.Data;
using AudioEngine;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;

namespace Editor
{
    [CustomPropertyDrawer(typeof(AudioManager.AudioTrack))]
    public class TrackPropertyDrawer : PropertyDrawer
    {
        
    }
}