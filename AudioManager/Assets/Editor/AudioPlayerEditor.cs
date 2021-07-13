using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AudioEngine;
using UnityEngine;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEditorInternal;

[CustomEditor(typeof(AudioPlayer))]
public class AudioPlayerEditor : Editor
{
     AudioPlayer manager;

     private SerializedProperty s_audioEvents;
     private SerializedProperty s_eventCreators;

     private ReorderableList _reorderableAudioEvents;

     private ReorderableList _reorderableEventCreators;

     private bool setH_Event;

     private void OnEnable()
     {
         manager = target as AudioPlayer;

         s_audioEvents = serializedObject.FindProperty(nameof(manager.audioEvent));
         s_eventCreators = serializedObject.FindProperty(nameof(manager.eventCreator));
         
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

         #region ReorderableListEvents

         _reorderableEventCreators = new ReorderableList(serializedObject, s_eventCreators, true, true, false, false);
         _reorderableEventCreators.drawHeaderCallback = DrawHeaderEvents;
         _reorderableEventCreators.drawElementCallback = DrawEventCreator;
         _reorderableEventCreators.drawFooterCallback = DrawFooterEvents;
         _reorderableEventCreators.drawNoneElementCallback = DrawBackgroundNoEvents;

         setH_Event = false;
         
         _reorderableEventCreators.elementHeightCallback = delegate(int index) {
             var element = _reorderableEventCreators.serializedProperty.GetArrayElementAtIndex(index);
             var margin = EditorGUIUtility.standardVerticalSpacing;
             if (element.isExpanded) return 245 + margin;
             else return 20 + margin;
         };

         #endregion
         
         manager.allEventsSetted = false;
     }

     public override void OnInspectorGUI()
     {
         serializedObject.Update();

         using (new EditorGUILayout.VerticalScope("Box"))
         {
             var settingsButtonStyle = new GUIStyle(GUI.skin.button)
             {
                 normal = new GUIStyleState() {textColor = new Color(.2f,.6f,.8f)}, fontSize = 20,
                 font = (Font) Resources.Load("customFont"), alignment = TextAnchor.MiddleCenter
             };
             
             if (GUILayout.Button("SETTINGS",settingsButtonStyle,GUILayout.Width(30), GUILayout.Height(30), GUILayout.ExpandWidth(true)))
             {
                 manager.configuration = !manager.configuration;
             }
             
             if (!manager.configuration)
             {
                 if (!manager.automatic)
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

                         if (!manager.allEventsSetted)
                         {
                             manager.allEventsNames.Clear();
                             manager.allEventsTypes.Clear();
                         
                             foreach (var mEvent in matchEvents)
                             {
                                 manager.allEventsNames.Add(mEvent.Name);
                                 manager.allEventsTypes.Add(mEvent.DeclaringType.AssemblyQualifiedName);
                             }

                             manager.allEventsSetted = true;
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
                 else
                 {
                     using (new EditorGUILayout.VerticalScope("Box"))
                     {
                         
                         if (GUILayout.Button("Create"))
                         {
                             if (AudioManager.Instance.tracks.Count > 0)
                             {
                                 CreateAllEvents();

                                 for (int i = 0; i < manager.eventCreator.Count; i++)
                                 {
                                     //Set the type of the event for subscribing it later
                                     EventInfo ev = manager.eventCreator[i].selectedScript.GetType().GetEvent(manager.eventCreator[i].eventName);
                                     Debug.Log("Event: " + ev.Name + " | " + ev.DeclaringType.AssemblyQualifiedName);
                                     if(ev != null) manager.eventCreator[i].eventType = ev.DeclaringType.AssemblyQualifiedName;
                                 }
                             }
                         }
                         
                         using (new EditorGUILayout.HorizontalScope())
                         {
                             if (GUILayout.Button("Add Event"))
                             {
                                 manager.eventCreator.Add(new AudioPlayer.EventCreator());
                             }
                             
                             if (GUILayout.Button("Remove Event"))
                             {
                                 if(manager.eventCreator.Count > 0) manager.eventCreator.RemoveAt(manager.eventCreator.Count-1);
                             }
                         }
                         
                         using (new EditorGUILayout.VerticalScope("Box"))
                         {
                             //Despliegue de cada evento
                             _reorderableEventCreators.DoLayoutList();
                         }
                     }
                 }
             }
             else
             {
                 using (new EditorGUILayout.HorizontalScope("Box"))
                 {
                     EditorGUILayout.LabelField("Automatic Invoking");
                 
                     EditorGUILayout.Space();
                 
                     manager.automatic = EditorGUILayout.Toggle(manager.automatic);
             
                     EditorGUILayout.Space();
                 }
             }

         }

         serializedObject.ApplyModifiedProperties();
     }

     void CreateAllEvents()
     {
         //Crea todos los eventos y sus invocaciones
         if (manager.eventCreator.Count > 0)
         {
             for (int i = 0; i < manager.eventCreator.Count; i++)
             {
                 string path = "";
                 string[] res = System.IO.Directory.GetFiles(Application.dataPath, $"{manager.eventCreator[i].selectedScript.GetType().Name}.cs", SearchOption.AllDirectories);
                 if (res.Length != 0)
                 {
                     path = res[0].Replace("\\", "/");
                     //Debug.Log(path);
                }

                StreamReader scriptReader = new StreamReader(path);

                 string refScriptReader = scriptReader.ReadToEnd();
                 scriptReader.Close();

                 if (refScriptReader.Contains(manager.eventCreator[i].eventName))
                 {
                     continue;
                 }
                 else
                 {
                     string indent = "    ";
                     string indentMethod = indent + indent;
                     
                     string declaringEvent = indent + $"public event Action<string> {manager.eventCreator[i].eventName};";
                     string trackReference = $"\"{manager.eventCreator[i].selectedTrack}\"";
                     string invokingEvent = indentMethod + $"{manager.eventCreator[i].eventName}?.Invoke({trackReference});";
                     
                     if (refScriptReader.Contains("#region EventsRegion"))
                     {
                         string scriptRefresh = DeclaringInvokeWithRegion(refScriptReader, i, declaringEvent);

                         string overwriteScript = InvokeInMethod(scriptRefresh, i, invokingEvent);
                         Debug.Log(overwriteScript);

                         StreamWriter scriptOverWriter = new StreamWriter(path);
                         scriptOverWriter.Write(overwriteScript);
                         scriptOverWriter.Close();
                     }
                     else
                     {
                         string scriptRefresh = DeclaringInvokeWithoutRegion(refScriptReader, i, declaringEvent, indent);
                         
                         string overwriteScript = InvokeInMethod(scriptRefresh, i, invokingEvent);
                         Debug.Log(overwriteScript);
                         
                         StreamWriter scriptOverWriter = new StreamWriter(path);
                         scriptOverWriter.Write(overwriteScript);
                         scriptOverWriter.Close();
                     }
                 }
             }
         }
     }

     void CreateEvent(int i)
     {
         string path = "";
         string[] res = System.IO.Directory.GetFiles(Application.dataPath, $"{manager.eventCreator[i].selectedScript.GetType().Name}.cs", SearchOption.AllDirectories);
         if (res.Length != 0)
         {
             path = res[0].Replace("\\", "/");
             //Debug.Log(path);
         }

         StreamReader scriptReader = new StreamReader(path);

         string refScriptReader = scriptReader.ReadToEnd();
         scriptReader.Close();

         if (!refScriptReader.Contains(manager.eventCreator[i].eventName))
         {
             string indent = "    ";
             string indentMethod = indent + indent;
             
             string declaringEvent = indent + $"public event Action<string> {manager.eventCreator[i].eventName};";
             string trackReference = $"\"{manager.eventCreator[i].selectedTrack}\"";
             string invokingEvent = indentMethod + $"{manager.eventCreator[i].eventName}?.Invoke({trackReference});";
             
             if (refScriptReader.Contains("#region EventsRegion"))
             {
                 string scriptRefresh = DeclaringInvokeWithRegion(refScriptReader, i, declaringEvent);

                 string overwriteScript = InvokeInMethod(scriptRefresh, i, invokingEvent);
                 Debug.Log(overwriteScript);

                 StreamWriter scriptOverWriter = new StreamWriter(path);
                 scriptOverWriter.Write(overwriteScript);
                 scriptOverWriter.Close();
             }
             else
             {
                 string scriptRefresh = DeclaringInvokeWithoutRegion(refScriptReader, i, declaringEvent, indent);
                 
                 string overwriteScript = InvokeInMethod(scriptRefresh, i, invokingEvent);
                 Debug.Log(overwriteScript);
                 
                 StreamWriter scriptOverWriter = new StreamWriter(path);
                 scriptOverWriter.Write(overwriteScript);
                 scriptOverWriter.Close();
             }
         }
     }

     #region OverwritingScript
     
     string DeclaringInvokeWithRegion(string refScript, int i, string declaringInvoke)
     {
         //hasta la region
         string eventFirstPart = refScript.Split(new string[] {"#region EventsRegion"}, StringSplitOptions.None)[0] + "#region EventsRegion\n";

         //el resto del script
         string restEventScriptPart = refScript.Split(new string[] {"#region EventsRegion"}, StringSplitOptions.None)[1];
                                             
         eventFirstPart += $"\n{declaringInvoke}";

         string firstPartScript = eventFirstPart + restEventScriptPart;

         return firstPartScript;
     }

     string DeclaringInvokeWithoutRegion(string refScript, int i, string declaringInvoke, string indent)
     {
         //Priemera parte hasta el nombre de la clase (Añado la clase que se pierde al splitear)
         string s0 = refScript.Split(new string[] {$"{manager.eventCreator[i].selectedScript.GetType().Name}"}, StringSplitOptions.None)[0] + manager.eventCreator[i].selectedScript.GetType().Name;
         
         //El resto
         string s1 = refScript.Split(new string[] {$"{manager.eventCreator[i].selectedScript.GetType().Name}"}, StringSplitOptions.None)[1];
         
         //A partir de la primera llave justo despues de la definicion de la clase
         string t0 = s1.Split('{')[0] + "{";
         
         //join the first part
         string firstPartToFirstQuotes = s0 + t0;
         
         //El resto
         string t1 = refScript.Substring(firstPartToFirstQuotes.Length);
         

         // Set at the first { the region for the events
         string regionPart = firstPartToFirstQuotes + $"\n{indent}#region EventsRegion" + $"\n\n{indent}#endregion\n";

         string eventSettingPart = regionPart.Split(new string[] {"#region EventsRegion"}, StringSplitOptions.None)[0] + "#region EventsRegion";
         string restEventSettingPart = regionPart.Split(new string[] {"#region EventsRegion"}, StringSplitOptions.None)[1];

         eventSettingPart += $"\n{declaringInvoke}";
         
         //join the event region
         string eventDeclaredResult = eventSettingPart + restEventSettingPart;

         string firstPartScript = eventDeclaredResult + t1;

         return firstPartScript;
     }
     
     string InvokeInMethod(string scriptRefresh, int i, string invokingEvent)
     {
         //find the method and invoke the event
         string methodName = manager.eventCreator[i].selectedMethod.Name;
         string methodFirstPart = scriptRefresh.Split(new string[] {$"{methodName}"}, StringSplitOptions.None)[0] + methodName;
         
         string restMethodPart = scriptRefresh.Split(new string[] {$"{methodName}"}, StringSplitOptions.None)[1];
         
         string firstQuoteInMethodPart = restMethodPart.Split('{')[0] + '{';
         
         string firstPartToFirstMethod = methodFirstPart + firstQuoteInMethodPart;
                                             
         //El resto
         string restQuoteInMethodPart = scriptRefresh.Split(new string[] {firstPartToFirstMethod}, StringSplitOptions.None)[1];

         firstPartToFirstMethod += $"\n{invokingEvent}";

         string finalScriptResult = firstPartToFirstMethod + restQuoteInMethodPart;

         return finalScriptResult;
     }
     
     #endregion

     #region DrawingReorderableLists
     
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
                     
                         manager.audioEvent[index].eventIndex = EditorGUI.Popup(fieldRect, "Event Player", manager.audioEvent[index].eventIndex, methodNames);
                         if (manager.audioEvent[index].eventIndex >= 0)
                         {
                             //Debug.Log(manager.audioEvent[index].eventIndex);
                             manager.audioEvent[index].TypeName = matchEvents[manager.audioEvent[index].eventIndex].DeclaringType.AssemblyQualifiedName;
                             manager.audioEvent[index].SelectedEventName = matchEvents[manager.audioEvent[index].eventIndex].Name;
                         }
                         else
                         {
                             manager.audioEvent[index].SelectedEventName = matchEvents[0].Name;
                             manager.audioEvent[index].TypeName = matchEvents[0].DeclaringType.AssemblyQualifiedName;
                         }
                         
                         
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

     public void DrawEventCreator(Rect position, int index, bool isActive, bool isFocused)
     {
         if (manager.eventCreator.Count - 1 >= index)
         {
             SerializedProperty property = _reorderableEventCreators.serializedProperty.GetArrayElementAtIndex(index);
             
             position.width -= 34;
             position.height = 18;
            
             Rect dropdownRect = new Rect(position);
             dropdownRect.width = 10;
             dropdownRect.height = 10;
             dropdownRect.x += 10;
             dropdownRect.y += 5;
             
             property.isExpanded = EditorGUI.Foldout(dropdownRect, property.isExpanded, "Event Creator");
             
             position.x += 10;
             position.width -= 15;
            
             Rect fieldRect = new Rect(position);
             
             Space(ref fieldRect, 30);

             List<MonoBehaviour> scripts = manager.GetComponents<MonoBehaviour>()
                 .Where(s => s.GetType().Name != manager.GetType().Name).ToList();

             if (property.isExpanded)
             {
                 float x = fieldRect.x;
                 if (scripts.Count > 0)
                 {
                     string[] scriptsNames = scripts.Select(s => s.GetType().Name).ToArray();

                     EditorGUI.LabelField(fieldRect, "Script");

                     fieldRect.x += 100;

                     int scriptIndex =
                         EditorGUI.Popup(
                             new Rect(fieldRect.position,
                                 new Vector2(EditorGUIUtility.currentViewWidth - fieldRect.x - 50, 20)),
                             scripts.ToList().IndexOf(manager.eventCreator[index].selectedScript), scriptsNames);
                     if (scriptIndex >= 0) manager.eventCreator[index].selectedScript = scripts[scriptIndex];
                     else if (scripts.Count > 0) manager.eventCreator[index].selectedScript = scripts[0];

                     Space(ref fieldRect);

                     List<MethodInfo> methods = manager.eventCreator[index].selectedScript.GetType()
                         .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(m =>
                             m.DeclaringType == manager.eventCreator[index].selectedScript.GetType() &&
                             !m.IsSpecialName).ToList();

                     if (methods.Count > 0)
                     {
                         string[] methodsNames = methods.Select(m => m.Name).ToArray();

                         fieldRect.x = x;
                         EditorGUI.LabelField(fieldRect, "Method");

                         fieldRect.x += 100;

                         int methodIndex =
                             EditorGUI.Popup(
                                 new Rect(fieldRect.position,
                                     new Vector2(EditorGUIUtility.currentViewWidth - fieldRect.x - 50, 20)),
                                 methods.ToList().IndexOf(manager.eventCreator[index].selectedMethod), methodsNames);
                         if (methodIndex >= 0) manager.eventCreator[index].selectedMethod = methods[methodIndex];
                         else manager.eventCreator[index].selectedMethod = methods[0];

                         Space(ref fieldRect);
                     }

                 }

                 if (AudioManager.Instance.tracks.Count > 0)
                 {
                     if(setH_Event) GetEventHeight();
                     
                     string[] tracksNames = AudioManager.Instance.tracks.Select(t => t.name).ToArray();

                     fieldRect.x = x;
                     EditorGUI.LabelField(fieldRect, "Track");

                     fieldRect.x += 100;

                     int trackIndex = EditorGUI.Popup(new Rect(fieldRect.position, new Vector2(EditorGUIUtility.currentViewWidth - fieldRect.x - 50, 20)), tracksNames.ToList().IndexOf(manager.eventCreator[index].selectedTrack), tracksNames);
                     if (trackIndex >= 0) manager.eventCreator[index].selectedTrack = tracksNames[trackIndex];
                     else manager.eventCreator[index].selectedTrack = tracksNames[0];

                     Space(ref fieldRect);

                     fieldRect.x = x;
                     EditorGUI.LabelField(fieldRect, "Auto name");

                     fieldRect.x += 100;

                     manager.eventCreator[index].autoName =
                         EditorGUI.Toggle(fieldRect, manager.eventCreator[index].autoName);

                     if (manager.eventCreator[index].autoName)
                     {
                         Space(ref fieldRect);

                         fieldRect.x = x;

                         EditorGUI.LabelField(fieldRect, "Event Name");

                         fieldRect.x += 100;

                         if (tracksNames.Length - 1 >= trackIndex && trackIndex >= 0)
                         {
                             manager.eventCreator[index].eventName = $"{tracksNames[trackIndex]}Event";
                             EditorGUI.LabelField(
                                 new Rect(fieldRect.position,
                                     new Vector2(EditorGUIUtility.currentViewWidth - fieldRect.x - 50, 20)),
                                 manager.eventCreator[index].eventName, new GUIStyle(GUI.skin.box));
                         }
                     }
                     else
                     {
                         Space(ref fieldRect);

                         fieldRect.x = x;

                         EditorGUI.LabelField(fieldRect, "Event Name");

                         fieldRect.x += 100;

                         manager.eventCreator[index].eventName = EditorGUI.TextField(
                             new Rect(fieldRect.position,
                                 new Vector2(EditorGUIUtility.currentViewWidth - fieldRect.x - 50, 20)),
                             manager.eventCreator[index].eventName);

                     }
                 }
                 else
                 {
                     if(!setH_Event) GetEventHeight();

                     fieldRect.x = x;
                     EditorGUI.LabelField(fieldRect, "You must have any track for use this component".ToUpper(), new GUIStyle(GUI.skin.box));
                 }
                 
                 Space(ref fieldRect, 30);
                 Rect createButtonRect = new Rect(fieldRect.position, new Vector2(EditorGUIUtility.currentViewWidth-70, fieldRect.height));
                 createButtonRect.x = x - 20;
                 if (GUI.Button(createButtonRect, "Create Event"))
                 {
                     if (AudioManager.Instance.tracks.Count > 0)
                     {
                         CreateEvent(index);
                         
                         //Set the type of the event for subscribing it later
                         EventInfo ev = manager.eventCreator[index].selectedScript.GetType().GetEvent(manager.eventCreator[index].eventName);
                         Debug.Log("Event: " + ev.Name + " | " + ev.DeclaringType.AssemblyQualifiedName);
                         if(ev != null) manager.eventCreator[index].eventType = ev.DeclaringType.AssemblyQualifiedName;
                     }
                 }

                 Space(ref fieldRect, 40);

                 Rect buttonRect = new Rect(fieldRect.position, new Vector2(50, fieldRect.height));
                 buttonRect.x += (EditorGUIUtility.currentViewWidth * 0.5f) - buttonRect.x;
                 if (GUI.Button(buttonRect, "X"))
                 {
                     manager.eventCreator.Remove(manager.eventCreator.ElementAt(index));
                 }

                 Space(ref fieldRect, 15);

                 fieldRect.x = x + 40;

                 DrawUILine(fieldRect.x, fieldRect.y, manager.eventCreator.Count);

                 Space(ref fieldRect);

             }
             else
             {
                 Rect buttonRect = new Rect(dropdownRect.position, new Vector2(50, 20));
                 buttonRect.y -= 5;
                 buttonRect.x += (EditorGUIUtility.currentViewWidth - (buttonRect.x * 2.5f));
                 if (GUI.Button(buttonRect, "X"))
                 {
                     manager.eventCreator.Remove(manager.eventCreator.ElementAt(index));
                 }
             }
         }
     }
     
     #endregion

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

     void GetEventHeight()
     {
         _reorderableEventCreators.elementHeightCallback = delegate(int index) {
             var element = _reorderableEventCreators.serializedProperty.GetArrayElementAtIndex(index);
             var margin = EditorGUIUtility.standardVerticalSpacing;
             if (element.isExpanded) return (AudioManager.Instance.tracks.Count > 0 ? 245 : 185) + margin;
             else return 20 + margin;
         };

         setH_Event = !setH_Event;
     }
     
     void DrawHeaderTracks(Rect rect)
     {
         var  blueStylePreset = new GUIStyle(GUI.skin.label);
         blueStylePreset.normal.textColor = new Color(.1f, .6f, .8f);
         string name = "Audio Event Players";
         EditorGUI.LabelField(rect, name, blueStylePreset);
     }
     
     void DrawHeaderEvents(Rect rect)
     {
         var  blueStylePreset = new GUIStyle(GUI.skin.label);
         blueStylePreset.normal.textColor = new Color(.1f, .6f, .8f);
         string name = "Event Creator";
         EditorGUI.LabelField(rect, name, blueStylePreset);
     }
    
     void DrawFooterTracks(Rect rect)
     {
         var  blueStylePreset = new GUIStyle(GUI.skin.label);
         blueStylePreset.normal.textColor = new Color(.1f, .6f, .8f);
         string name = "By @babelgames_es";
         EditorGUI.LabelField(rect, name, blueStylePreset);
     }
     
     void DrawFooterEvents(Rect rect)
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
     
     void DrawBackgroundNoEvents(Rect rect)
     {
         var greenStylePreset = new GUIStyle(GUI.skin.label);
         greenStylePreset.normal.textColor = new Color(.05f, .9f, .2f);
         string name = "Add any event creator";
         EditorGUI.LabelField(rect, name, greenStylePreset);
     }
     
     public void Space(ref Rect pos, float space = 30f)
     {
         pos.y += space;
     }
     
     public static void DrawUILine(float posX, float posY, int actualEvents = 0, float thickness = 38, float padding = 30)
     {
         Rect r = new Rect(posX, posY, thickness, padding);
         r.width = EditorGUIUtility.currentViewWidth - (actualEvents > 1 ? 21 : 10);
         r.height = 2;
         r.y+=padding * 0.3f;
         r.x-=70;
         r.width -= thickness;
         EditorGUI.DrawRect(r, Color.cyan);
     }
}