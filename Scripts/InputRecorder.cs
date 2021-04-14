using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utj
{
    /// <summary>
    /// Inputをスクリプトとして記録するClass
    /// 
    /// 基本的にはトリガー的なイベントのみ記録します。(DownとUpは記録するが、押し続けられる状態は記録しない)
    /// 
    /// Programed by Katsumasa.Kimura
    /// </summary>
    public class InputRecorder : MonoBehaviour
    {        
        [SerializeField, Tooltip("記録するAxisの名称")]
        string[] m_axisNames =
        {
            "Horizontal",
            "Vertical",
        };
        
        [SerializeField, Tooltip("記録するボタンの名称")]
        string[] m_buttonNames =
        {
            "Fire1",
            "Fire2",
            "Fire3",
            "Jump",
            "Cancel",
            "Submit",
        };

        [SerializeField, Tooltip("データを圧縮するか否か(圧縮すると再現性が低くなります。)")]
        bool m_isCompress = true;

        [SerializeField, Tooltip("Buttonを記録するか")]
        bool m_isRecordButton = true;

        [SerializeField, Tooltip("AxisRawを記録するか")]
        bool m_isRecordAxisRaw = true;

        [SerializeField, Tooltip("Mouseを記録するか")]
        bool m_isRecordMouse = true;

        [SerializeField, Tooltip("Touchを記録するか")]
        bool m_isRecordTouch = true;


        /// <summary>
        /// 記録中であるか否か
        /// </summary>
        public bool isRecording
        {
            get;
            private set;
        }

        /// <summary>
        /// Scriptを書き出したTextAsset
        /// Runtimeでは煮るなり焼くなりして下さい。
        /// </summary>
        public TextAsset textAsset
        {
            get;
            private set;
        }

        /// <summary>
        /// データを圧縮するか否か
        /// </summary>
        public bool isCompress
        {
            get { return m_isCompress; }
            set { m_isCompress = value; }
        }


        StringWriter m_stringWriter;
        float[] m_axisValues;
        bool m_isAddWait;
        int m_waitCount;
        Vector2 m_mousePosition;


        /// <summary>
        /// 記録を開始する
        /// </summary>
        public void Record()
        {
            if (isRecording)
            {
                //
            }
            else
            {
                // 初期化処理
                m_stringWriter = new StringWriter();                
                isRecording = true;
                m_waitCount = 1;
                m_isAddWait = false;
                m_mousePosition = Vector2.zero;
                m_axisValues = new float[m_axisNames.Length];
                for (var i = 0; i < m_axisValues.Length; i++)
                {
                    m_axisValues[i] = 0f;
                }
            }
        }


        /// <summary>
        /// 記録を終了する
        /// </summary>
        public void Stop()
        {
            if (isRecording)
            {
                textAsset = new TextAsset(m_stringWriter.ToString());
                isRecording = false;
                m_stringWriter.Close();            
            }
        }



        // Start is called before the first frame update
        void Start()
        {            
        }


        // Update is called once per frame
        void Update()
        {
            if (isRecording)
            {
                InputEventRecorder();
            }
        }


        /// <summary>
        /// Inputイベントを記録する
        /// </summary>
        void InputEventRecorder()
        {
            if (m_isAddWait)
            {
                m_stringWriter.WriteLine("wait {0}", m_waitCount);
                m_isAddWait = false;
                m_waitCount = 1;
            }
            else
            {
                m_waitCount++;
            }
            if (m_isRecordAxisRaw)
            {
                m_isAddWait |= AxisRawEventRecorder();
            }
            if (m_isRecordButton)
            {
                m_isAddWait |= ButtonEventRecorder();
            }
            if (m_isRecordTouch)
            {
                m_isAddWait |= TouchEventRecorder();
            }
            if (m_isRecordMouse)
            {
                m_isAddWait |= MouseEventRecorder();
            }
        }


        /// <summary>
        /// ボタンイベントを記録する
        /// </summary>
        bool ButtonEventRecorder()
        {
            bool ret = false;
            foreach(var buttonName in m_buttonNames)
            {
                // BaseInputにはGetButtonUpが存在しない為、Inputを使用する
                if (Input.GetButtonDown(buttonName))
                {
                    m_stringWriter.WriteLine("button \"down\" \"{0}\"", buttonName);
                    ret = true;
                }
                else if (Input.GetButtonUp(buttonName))
                {
                    m_stringWriter.WriteLine("button \"up\" \"{0}\"", buttonName);
                    ret = true;
                }                
            }
            return ret;
        }


        /// <summary>
        /// AxisRawのイベントを記録する
        /// </summary>
        bool AxisRawEventRecorder()
        {
            bool result = false;
            for(var i = 0; i < m_axisNames.Length; i++)
            {
                var name = m_axisNames[i];
                var axis = DummyInput.instance.GetAxisRaw(name);                
                if (axis != m_axisValues[i] || m_isCompress == false)
                {
                    m_axisValues[i] = axis;
                    m_stringWriter.WriteLine("axisraw \"{0}\" {1}", name, axis);
                    result = true;
                }
            }
            return result;
        }


        /// <summary>
        /// Touchイベントを記録する
        /// </summary>
        bool TouchEventRecorder()
        {
            // DummyInputではMouseでTouchをシュミレーションすることもあるので
            // DummyInputを参照する
            bool result = false;
            for(var i = 0; i  < DummyInput.instance.touchCount; i++)
            {
                var touch = DummyInput.instance.GetTouch(i);                
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        {
                            m_stringWriter.WriteLine("touch begin {0} {1} {2] {3} {4} {5} {6} {7}",
                                touch.fingerId,
                                touch.position.x,
                                touch.position.y,
                                touch.pressure,
                                touch.radius,
                                touch.radiusVariance,
                                (int)touch.type,
                                touch.tapCount
                                );
                            result = true;
                        }
                        break;

                    case TouchPhase.Moved:
                        {
                            m_stringWriter.WriteLine("touch move {0} {1} {2} {3} {4} {5}",
                                touch.fingerId,
                                touch.position.x,
                                touch.position.y,
                                touch.pressure,
                                touch.radius,
                                touch.radiusVariance
                                );
                            result = true;
                        }
                        break;

                    case TouchPhase.Ended:
                        {
                            m_stringWriter.WriteLine("touch ended {0}", touch.fingerId);
                            result = true;
                        }
                        break;
                }                
            }
            return result;
        }


        /// <summary>
        /// Mouseイベントを記録する
        /// </summary>
        /// <returns></returns>
        bool MouseEventRecorder()
        {
            bool result = false;

            for (var i = 0; i < DummyInput.kMouseButtonCount; i++)
            {
                if (DummyInput.instance.GetMouseButtonDown(i))
                {
                    m_stringWriter.WriteLine("mousebutton \"down\" {0}", i);
                    result = true;
                }
                else if(DummyInput.instance.GetMouseButtonUp(i))
                {
                    m_stringWriter.WriteLine("mousebutton \"up\" {0}", i);
                    result = true;
                }
            }
                        
            if(DummyInput.instance.mousePosition != m_mousePosition)
            {
                m_stringWriter.WriteLine("mousepos {0} {1}", DummyInput.instance.mousePosition.x, DummyInput.instance.mousePosition.y);
                m_mousePosition = DummyInput.instance.mousePosition;
                result = true;
            }
            return result;
        }

    }





}