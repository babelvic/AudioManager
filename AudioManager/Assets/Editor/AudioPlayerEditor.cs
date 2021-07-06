using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AudioEngine;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(AudioPlayer))]
public class AudioPlayerEditor : Editor
{
     AudioPlayer manager;

     private SerializedProperty s_audioEvents;

     private ReorderableList _reorderableAudioEvents;

     

     private void OnEnable()
     {
         manager = target as AudioPlayer;

         manager.selectAllEvents = true;

         s_audioEvents = serializedObject.FindProperty(nameof(manager.audioEvent));
         
         #region ReorderableListTracks

         _reorderableAudioEvents = new ReorderableList(serializedObject, s_audioEvents, true, true, false, false);
         _reorderableAudioEvents.drawHeaderCallback = DrawHeaderTracks;
         _reorderableAudioEvents.drawElementCallback = DrawAudioEvents;
         _reorderableAudioEvents.drawFooterCallback = DrawFooterTracks;
         _reorderableAudioEvents.drawNoneElementCallback = DrawBackgroundNoTracks;
         

         _reorderableAudioEvents.elementHeightCallback = delegate(int index) {
             var element = _reorderableAudioEvents.serializedProperty.GetArrayElementAtIndex(index);
             var margin = EditorGUIUtility.standardVerticalSpacing;
             if (element.isExpanded) return 98 + margin;
             else return 20 + margin;
         };
         
         #endregion
     }

     public override void OnInspectorGUI()
     {
         serializedObject.Update();

         using (new EditorGUILayout.VerticalScope("Box"))
         {

             using (new EditorGUILayout.HorizontalScope("Box"))
             {
                 EditorGUILayout.LabelField("Subscribe All Events");
                 
                 EditorGUILayout.Space();
                 
                 manager.selectAllEvents = EditorGUILayout.Toggle(manager.selectAllEvents);
             
                 EditorGUILayout.Space();
             }

             if (manager.selectAllEvents)
             {
                 List<MonoBehaviour> scripts = manager.GetComponents<MonoBehaviour>().Where(s => s.GetType().Name != manager.GetType().Name).ToList();

                 List<EventInfo> matchEvents = GetMatchEvents(scripts);

                 foreach (var mEvent in matchEvents)
                 {
                     manager.allEventsNames.Add(mEvent.Name);
                     manager.allEventsTypes.Add(mEvent.DeclaringType.AssemblyQualifiedName);
                 }

                 using (new EditorGUILayout.VerticalScope("Box"))
                 {
                     foreach (var mEvent in matchEvents)
                     {
                         EditorGUILayout.LabelField(mEvent.Name);
                     }
                 }
             }
             else
             {
                 //Horizontal Space for add and remove tracks
                 using (new EditorGUILayout.HorizontalScope())
                 {
                     if (GUILayout.Button("Add Track"))
                     {
                         manager.audioEvent.Add(new AudioPlayer.AudioEvent());
                     }

                     GUILayout.Space(10f);

                     if (GUILayout.Button("Remove Track"))
                     {
                         if (manager.audioEvent.Count - 1 >= 0) manager.audioEvent.RemoveAt(manager.audioEvent.Count - 1);
                     }
                 }

                 _reorderableAudioEvents.DoLayoutList();
             }

         }

         serializedObject.ApplyModifiedProperties();
     }

     public void DrawAudioEvents(Rect position, int index, bool isActive, bool isFocused)
     {
         SerializedProperty property = _reorderableAudioEvents.serializedProperty.GetArrayElementAtIndex(index);
         
         position.width -= 34;
         position.height = 18;
        
         Rect dropdownRect = new Rect(position);
         dropdownRect.width = 10;
         dropdownRect.height = 10;
         dropdownRect.x += 10;
         dropdownRect.y += 5;
         
         property.isExpanded = EditorGUI.Foldout(dropdownRect, property.isExpanded, "Audio Event");
         
         position.x += 50;
         position.width -= 15;
        
         Rect fieldRect = new Rect(position);

         if (property.isExpanded)
         {
             if (manager.audioEvent.Count - 1 >= index)
             {
                 Space(ref fieldRect);
                 List<MonoBehaviour> scripts = manager.GetComponents<MonoBehaviour>().Where(s => s.GetType().Name != manager.GetType().Name).ToList();
             
                 if (scripts.Count > 0)
                 {
                     List<EventInfo> matchEvents = GetMatchEvents(scripts);

                     if (matchEvents.Count > 0)
                     {
                         string[] methodNames = matchEvents.Select(e => $"{e.DeclaringType} / {e.Name}()").ToArray();
                     
                         int methodIndex = EditorGUI.Popup(fieldRect, "Event Player", index, methodNames);
                         if (methodIndex >= 0)
                         {
                             manager.audioEvent[index].TypeName = matchEvents[methodIndex].DeclaringType.AssemblyQualifiedName;
                             manager.audioEvent[index].SelectedEventName = matchEvents[methodIndex].Name;
                         }
                         else manager.audioEvent[index].SelectedEventName = matchEvents[0].Name;
                     }
                     
                     Space(ref fieldRect, 40);
                     
                     Rect buttonRect = new Rect(fieldRect.position, new Vector2(50, fieldRect.height));
                     buttonRect.x += (EditorGUIUtility.currentViewWidth * 0.5f)-buttonRect.x;
                     if (GUI.Button(buttonRect, "X"))
                     {
                         manager.audioEvent.Remove(manager.audioEvent.ElementAt(index));
                     }
                     
                     Space(ref fieldRect, 15);
                     
                     DrawUILine(fieldRect.x, fieldRect.y);
                     
                     Space(ref fieldRect);
                 }
             }
         }
         else
         {
             Rect buttonRect = new Rect(dropdownRect.position, new Vector2(50, 20));
             buttonRect.y -= 5;
             buttonRect.x += (EditorGUIUtility.currentViewWidth - (buttonRect.x * 2.5f));
             if (GUI.Button(buttonRect, "X"))
             {
                 manager.audioEvent.Remove(manager.audioEvent.ElementAt(index));
             }
         }
     }

     List<EventInfo> GetMatchEvents(List<MonoBehaviour> scripts)
     {
         List<EventInfo> allEvents = scripts.SelectMany(s => s.GetType().GetEvents()).ToList();
         List<EventInfo> matchEvents = new List<EventInfo>();
                     
         foreach (var eventInfo in allEvents)
         {
             if (eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters().Select(p => p.GetType()).SequenceEqual(typeof(AudioManager).GetMethod("PlayTrack")?.GetParameters().Select(p => p.GetType())))
             {
                 matchEvents.Add(eventInfo);
             }
         }

         return matchEvents;
     }
     
     void DrawHeaderTracks(Rect rect)
     {
         var  blueStylePreset = new GUIStyle(GUI.skin.label);
         blueStylePreset.normal.textColor = new Color(.1f, .6f, .8f);
         string name = "Audio Event Players";
         EditorGUI.LabelField(rect, name, blueStylePreset);
     }
    
     void DrawFooterTracks(Rect rect)
     {
         var  blueStylePreset = new GUIStyle(GUI.skin.label);
         blueStylePreset.normal.textColor = new Color(.1f, .6f, .8f);
         string name = "By @babelgames_es";
         EditorGUI.LabelField(rect, name, blueStylePreset);
     }
    
     void DrawBackgroundNoTracks(Rect rect)
     {
         var greenStylePreset = new GUIStyle(GUI.skin.label);
         greenStylePreset.normal.textColor = new Color(.05f, .9f, .2f);
         string name = "Add Audio Events for attach any function to any track";
         EditorGUI.LabelField(rect, name, greenStylePreset);
     }
     
     public void Space(ref Rect pos, float space = 30f)
     {
         pos.y += space;
     }
     
     public static void DrawUILine(float posX, float posY, float thickness = 38, float padding = 30)
     {
         Rect r = new Rect(posX, posY, thickness, padding);
         r.width = EditorGUIUtility.currentViewWidth;
         r.height = 2;
         r.y+=padding * 0.3f;
         r.x-=70;
         r.width -= thickness;
         EditorGUI.DrawRect(r, Color.cyan);
     }
}