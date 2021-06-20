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
        private float _height;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            _height = property.isExpanded ? 200 : 50;
            
            return _height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            position.width -= 34;
            position.height = 18;

            Rect dropdownRect = new Rect(position);
            dropdownRect.width = 10;
            dropdownRect.height = 30;

            property.isExpanded = EditorGUI.Foldout(dropdownRect, property.isExpanded, string.Empty);

            position.x += 50;
            position.width -= 15;
            

            Rect fieldRect = new Rect(position);

            SerializedProperty clipField = property.FindPropertyRelative("clip");

            SerializedProperty nameField = property.FindPropertyRelative(nameof(AudioManager.AudioTrack.name));

            if (clipField.objectReferenceValue != null)
            {
                nameField.stringValue = ((AudioClip) clipField.objectReferenceValue).name;
            }


            if (property.isExpanded)
            {
                Space(ref fieldRect, 5f);
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

                //Draw Values
                Space(ref fieldRect);
                EditorGUI.Slider(fieldRect, priorityField, 0f, 256f);
                Space(ref fieldRect);
                EditorGUI.Slider(fieldRect, volumeField, 0f, 1);
                Space(ref fieldRect);
                EditorGUI.Slider(fieldRect, pitchField, -3f, 3);
                Space(ref fieldRect);
                EditorGUI.Slider(fieldRect, SpatialBlendField, 0f, 1f);
                Space(ref fieldRect);
                
                DrawUILine(fieldRect.x, fieldRect.y);
                Space(ref fieldRect);
            }

            EditorGUI.EndProperty();
        }

        public void Space(ref Rect pos, float space = 30f)
        {
            pos.y += space;
        }
        
        public static void DrawUILine(float posX, float posY, float thickness = 47, float padding = 30)
        {
            Rect r = new Rect(posX, posY, thickness, padding);
            r.width = EditorGUIUtility.currentViewWidth;
            r.height = 2;
            r.y+=padding * 0.3f;
            r.x-=78;
            r.width -= thickness;
            EditorGUI.DrawRect(r, Color.cyan);
        }
    }
}