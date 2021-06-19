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
            _height = dropdown ? 100 : 50;
            
            return _height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            position.width -= 34;
            position.height = 18;

            Rect dropdownRect = new Rect(position);
            dropdownRect.width = 5;
            dropdownRect.height = 30;

            dropdown = EditorGUI.Foldout(dropdownRect, dropdown, String.Empty);

            position.x += 50;
            position.width -= 15;
            

            Rect fieldRect = new Rect(position);

            SerializedProperty clipField = property.FindPropertyRelative("clip");

            SerializedProperty nameField = property.FindPropertyRelative(nameof(AudioManager.AudioTrack.name));

            if (clipField.objectReferenceValue != null)
            {
                nameField.stringValue = ((AudioClip) clipField.objectReferenceValue).name;
            }

            if (dropdown)
            {
                Space(ref fieldRect);
                //Draw Clip
                EditorGUI.ObjectField(fieldRect, clipField);
            
                Space(ref fieldRect);
                //Draw Name
                EditorGUI.TextField(fieldRect, nameField.stringValue);

                var mixerField = property.FindPropertyRelative("mixer");

                var loopField = new PropertyField(property.FindPropertyRelative("loop"), "Loop Track");
                
                SerializedProperty priorityField = property.FindPropertyRelative("priority");
                SerializedProperty volumeField = property.FindPropertyRelative("volume");
                SerializedProperty pitchField = property.FindPropertyRelative("pitch");
                SerializedProperty SpatialBlendField = property.FindPropertyRelative("spatialBlend");

                Space(ref fieldRect);
                //Draw Values
                EditorGUI.Slider(fieldRect, priorityField, 0f, 256f);
                Space(ref fieldRect);
                EditorGUI.Slider(fieldRect, volumeField, 0f, 1);
                Space(ref fieldRect);
                EditorGUI.Slider(fieldRect, pitchField, -3f, 3);
                Space(ref fieldRect);
                EditorGUI.Slider(fieldRect, SpatialBlendField, 0f, 1f);
                Space(ref fieldRect);
                
                DrawUILine(Color.cyan);
            }
        }

        public void Space(ref Rect pos)
        {
            pos.y += 30f;
        }
        
        public static void DrawUILine(Color color, int thickness = 2, int padding = 30)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            r.height = thickness;
            r.y+=padding/2;
            r.x-=2;
            r.width +=6;
            EditorGUI.DrawRect(r, color);
        }
    }
}