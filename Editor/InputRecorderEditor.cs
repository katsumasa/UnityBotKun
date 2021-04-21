using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
namespace Utj.UnityBotKun
{
    [CustomEditor(typeof(InputRecorder))]
    public class InputRecorderEditor : Editor
    {
        Vector2 m_scrollPosition;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var inputRecorder = target as InputRecorder;


            GUILayout.BeginHorizontal();
            GUI.enabled = !inputRecorder.isRecording && Application.isPlaying;
            if (GUILayout.Button(new GUIContent("Record", "レコーディングを開始します。")))
            {
                inputRecorder.Record();
            }
            GUI.enabled = inputRecorder.isRecording;
            if (GUILayout.Button(new GUIContent("Stop", "レコーディングを終了します。")))
            {
                inputRecorder.Stop();
            }

            GUI.enabled = !inputRecorder.isRecording && inputRecorder.textAsset != null;
            if (GUILayout.Button(new GUIContent("Save", "スクリプトを保存します。")))
            {
                string fpath;

                fpath = EditorUtility.SaveFilePanel("Save Script", "", "", "txt");
                if (!string.IsNullOrEmpty(fpath))
                {
                    using (StreamWriter sw = File.CreateText(fpath))
                    {
                        sw.Write(inputRecorder.textAsset.ToString());
                        //AssetDatabase.CreateAsset(inputRecorder.textAsset, fpath);
                    }

                }
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;
#if false
            GUILayout.Label("Script:");
            using (var scope = new GUILayout.ScrollViewScope(m_scrollPosition,GUILayout.Height(100))) {
                m_scrollPosition = scope.scrollPosition;
                GUILayout.TextArea(inputRecorder.script);
            }
#endif   


            
        }
    }
}