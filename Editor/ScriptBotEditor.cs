using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Utj
{
    /// <summary>
    /// ScriptBotのInspector表示をカスタマイズするClass
    /// </summary>
    [CustomEditor(typeof(ScriptBot))]
    public class ScriptBotEditor : Editor
    {
        

        public void Awake()
        {
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();

            var scriptIdxProperty = serializedObject.FindProperty("m_currentScriptIndex");
            var scriptsProperty = serializedObject.FindProperty("m_scripts");
            var isPlayProperty = serializedObject.FindProperty("m_isPlay");


            var scriptBot = target as ScriptBot;
            
            var index = scriptIdxProperty.intValue;                     
            var n = scriptsProperty.arraySize;
            var scriptNames = new GUIContent[n];
            var scriptNumbers = new int[n];

            for (var i = 0; i < n; i++)
            {
                var item = scriptsProperty.GetArrayElementAtIndex(i);
                if (item.objectReferenceValue != null)
                {
                    var text = new SerializedObject(item.objectReferenceValue);
                    var nameProperty = text.FindProperty("m_Name");
                    scriptNames[i] = new GUIContent(nameProperty.stringValue);
                }
                else
                {
                    scriptNames[i] = new GUIContent("None (Text Asset)");
                }
                scriptNumbers[i] = i;
            }
            
            var isPlay = isPlayProperty.boolValue;
            GUI.enabled = !isPlay;
            EditorGUI.BeginChangeCheck();
            index =  EditorGUILayout.IntPopup(new GUIContent("Play Script","実行するスクリプトを選択します"), index, scriptNames, scriptNumbers);
            if (EditorGUI.EndChangeCheck()) {
                scriptIdxProperty.intValue = index;
                serializedObject.ApplyModifiedProperties();
            }

            GUILayout.BeginHorizontal();
            GUI.enabled = !isPlay && Application.isPlaying;
            if (GUILayout.Button(new GUIContent("Play", "スクリプトを実行します")))
            {
                scriptBot.Play(index);
            }

            GUI.enabled = isPlay && Application.isPlaying;
            if (GUILayout.Button(new GUIContent("Stop","スクリプトの実行を停止します")))
            {
                scriptBot.Stop();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            EditorGUILayout.Slider(scriptBot.currentPosition,0, scriptBot.maxPosition);
        }
    }
}