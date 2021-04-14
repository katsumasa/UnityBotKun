using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Utj
{
    [CustomEditor(typeof(DummyInput))]
    public class DummyInputEditor :Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var dummyInput = target as DummyInput;

            if (Application.isPlaying && dummyInput.isActiveAndEnabled)
            {
                EditorGUILayout.LabelField("Touch Infomation");
                using (new EditorGUI.IndentLevelScope())
                {

                    var sb = new StringBuilder();
                    sb.AppendFormat("phase:{0}\n", dummyInput.mouseTouch.phase);
                    sb.AppendFormat("position:{0}\n", dummyInput.mouseTouch.position);
                    sb.AppendFormat("delta:{0}\n",dummyInput.mouseTouch.deltaPosition);
                    GUILayout.Label(sb.ToString());
                    
                }
            }
        }
    }
}