using System;
using AudioEngine;
using UnityEditor;
using UnityEngine;
using AudioEngine;
using UnityEditor.UIElements;

namespace Editor
{
    [CustomPropertyDrawer(typeof(AudioManager.AudioTrack))]
    public class TrackPropertyDrawer : PropertyDrawer
    {
        private float _height;
        private bool dropdown;
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _height = dropdown ? 50 : 100;
            
            return _height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            position.width -= 34;
            position.height = 18;

            Rect dropdownRect = new Rect(position);
            dropdownRect.width = 5;
            dropdownRect.height = 20;

            dropdown = EditorGUI.Foldout(dropdownRect, dropdown, String.Empty);

            position.x += 15;
            position.width -= 15;

            /*
             *
             * public string name;
            public AudioClip clip;
            public AudioMixer mixer;

            public bool loop;

            [Range(0f, 256f)] public float priority;
            [Range(0f, 1f)] public float volume;
            [Range(-3f, 3f)] public float pitch;
            [Range(0f, 1f)] public float spatialBlend;
             * 
             */

            Rect fieldRect = new Rect(position);

            SerializedProperty clipField = property.FindPropertyRelative("clip");

            SerializedProperty nameField = property.FindPropertyRelative(nameof(AudioManager.AudioTrack.name));

            AudioClip oldClip = ((AudioClip) clipField.objectReferenceValue);

            if (clipField.objectReferenceValue.GetType() == typeof(AudioClip) && clipField != null && ((AudioClip) clipField.objectReferenceValue) != oldClip)
            {
                nameField.stringValue = ((AudioClip) clipField.objectReferenceValue).name;
            }

            //Draw Clip
            EditorGUI.ObjectField(position, clipField);
            
            //Draw Name
            EditorGUI.TextField(position, nameField.stringValue);

            var mixerField = property.FindPropertyRelative("mixer");

            var loopField = new PropertyField(property.FindPropertyRelative("loop"), "Loop Track");

            SerializedProperty priorityField = property.FindPropertyRelative("priority");
            SerializedProperty volumeField = property.FindPropertyRelative("volume");
            SerializedProperty pitchField = property.FindPropertyRelative("pitch");
            SerializedProperty SpatialBlendField = property.FindPropertyRelative("spatialBlend");

            //Draw Values
            EditorGUI.Slider(position, priorityField, 0f, 256f);
            EditorGUI.Slider(position, volumeField, 0f, 1);
            EditorGUI.Slider(position, pitchField, -3f, 3);
            EditorGUI.Slider(position, SpatialBlendField, 0f, 1f);

        }
    }
}