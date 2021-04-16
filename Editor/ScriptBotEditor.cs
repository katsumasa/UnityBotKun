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
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var scriptBot = target as ScriptBot;

            var serializedProperty = serializedObject.FindProperty("m_currentScriptIndex");
            var index = serializedProperty.intValue;

            var scriptNames = new GUIContent[scriptBot.scripts.Count];
            var scriptNumbers = new int[scriptBot.scripts.Count];
            for(var i = 0; i < scriptBot.scripts.Count; i++)
            {
                if (scriptBot.scripts != null)
                {
                    scriptNames[i] = new GUIContent(scriptBot.scripts[i].name);
                }
                else
                {
                    scriptNames[i] = new GUIContent("None");
                }
                scriptNumbers[i] = i;
            }
            index = System.Math.Min(index, scriptNames.Length);


            var isPlay = serializedObject.FindProperty("m_isPlay").boolValue;


            GUI.enabled = !isPlay;
            EditorGUI.BeginChangeCheck();
            index =  EditorGUILayout.IntPopup(new GUIContent("Play Script","実行するスクリプトを選択します"), index, scriptNames, scriptNumbers);
            if (EditorGUI.EndChangeCheck()) {                         
                serializedProperty.intValue = index;
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