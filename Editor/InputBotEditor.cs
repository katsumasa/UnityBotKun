using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace Utj.UnityBotKun
{
    /// <summary>
    /// InputBotのInspectorの表示Class
    /// Programrd By Katsumasa.Kimura
    /// 
    /// 注意事項
    /// OnInspectorGUIの実行されるタイミングの都合上、GetDownxxx()やGetUpxxx()のトリガーを取得出来ません。
    /// </summary>
    [CustomEditor(typeof(InputBot))]
    public class InputBotEditor :Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Input Infomation");
            EditorGUI.indentLevel++;
            var inputBot = target as InputBot;
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField("Axis(" +
                    inputBot.GetAxisRaw(StandaloneInputModuleOverride.instance.horizontalAxis) + "," +
                    inputBot.GetAxisRaw(StandaloneInputModuleOverride.instance.verticalAxis) + ")");
                                
                EditorGUILayout.LabelField("Mouse Present:" + inputBot.mousePresent);                                
                if (inputBot.mousePresent)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.LabelField("position:" + inputBot.mousePosition);
                    for (var i = 0; i < InputBot.kMouseButtonCount; i++)
                    {
                        EditorGUILayout.LabelField("button[" + i + "]:" + inputBot.GetMouseButton(i));
                    }
                    EditorGUI.indentLevel--;
                }                

                EditorGUILayout.LabelField("Touch Count" + inputBot.touchCount);
                EditorGUI.indentLevel++;                
                for (var i = 0; i < inputBot.touchCount; i++)
                {
                    TouchLabel(inputBot.GetTouch(i));
                }                
                EditorGUI.indentLevel--;                
            }
            EditorGUI.indentLevel--;





        }



        void TouchLabel(Touch touch)
        {
            EditorGUILayout.LabelField("fingerId:"+touch.fingerId);
            EditorGUILayout.LabelField("phase:"+ touch.phase);
            EditorGUILayout.LabelField("position:"+touch.position);
            EditorGUILayout.LabelField("delta:"+touch.deltaPosition);
            EditorGUILayout.Separator();
        }
    }
}