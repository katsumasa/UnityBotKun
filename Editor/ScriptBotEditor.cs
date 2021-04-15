using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Utj
{
    [CustomEditor(typeof(ScriptBot))]
    public class ScriptBotEditor : Editor
    {
        int m_scriptIndex = 0;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var scriptBot = target as ScriptBot;

            

            var scriptNames = new GUIContent[scriptBot.scripts.Count];
            var scriptNumbers = new int[scriptBot.scripts.Count];
            for(var i = 0; i < scriptBot.scripts.Count; i++)
            {
                scriptNames[i] = new GUIContent(scriptBot.scripts[i].name);
                scriptNumbers[i] = i;
            }

            
            
            GUI.enabled = !scriptBot.isPlay;
            m_scriptIndex =  EditorGUILayout.IntPopup(new GUIContent("Play Script","実行するスクリプトを選択します"), m_scriptIndex, scriptNames, scriptNumbers);

            GUILayout.BeginHorizontal();
            GUI.enabled = !scriptBot.isPlay && Application.isPlaying;
            if (GUILayout.Button(new GUIContent("Play", "スクリプトを実行します")))
            {
                    scriptBot.Play(m_scriptIndex);
            }

            GUI.enabled = scriptBot.isPlay;
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