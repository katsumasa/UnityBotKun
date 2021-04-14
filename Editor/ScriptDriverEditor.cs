using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Utj
{
    [CustomEditor(typeof(ScriptDriver))]
    public class ScriptDriverEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var scriptDriver = target as ScriptDriver;

            GUI.enabled = !scriptDriver.isPlay && Application.isPlaying;
            if(GUILayout.Button(new GUIContent("Play", "スクリプトを実行します")))
            {
                scriptDriver.Play();
            }
            GUI.enabled = true;
        }
    }
}