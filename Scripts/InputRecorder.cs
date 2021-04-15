using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
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
        enum Wait
        {
            Frame,
            Sec,
        };

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

        [SerializeField, Tooltip("waitをどの方式で記録するか")]
        Wait m_waitType = Wait.Frame;

        [SerializeField, Tooltip("Touch Begin時に可能であればPositionをGameObject名に変換する")]
        bool m_isEnablePositionToGameObject;

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
        /// </summary>
        public TextAsset textAsset
        {
            get;
            private set;
        }

        /// <summary>
        /// データを圧縮するか否か
        /// axisとmousePositionを毎フレーム出力するようになります。
        /// </summary>
        public bool isCompress
        {
            get { return m_isCompress; }
            set { m_isCompress = value; }
        }

        public string script
        {
            get
            {
                if (m_stringWriter != null)
                {
                    return m_stringWriter.ToString();
                } else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// スクリプト書き込み用
        /// </summary>
        StringWriter m_stringWriter;

        /// <summary>
        /// 軸の値が前回のフレームと異なる場合に出力する為の保存用
        /// </summary>
        float[] m_axisValues;

        /// <summary>
        /// waitを出力するか否か
        /// イベントが何も発生しない場合、waitを出力せず、waitCountをインクリメントします。
        /// </summary>
        bool m_isAddWait;

        /// <summary>
        /// waitを実行するフレーム数
        /// </summary>
        int m_waitCount;

        /// <summary>
        /// waitを実行する時間[sec]
        /// </summary>
        float m_waitTime;


        /// <summary>
        /// マウスのポジションが前回と異なった場合に出力する為の保存用
        /// </summary>
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
                m_waitTime = Time.fixedDeltaTime;
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
                if (m_waitType == Wait.Sec)
                {
                    m_stringWriter.WriteLine("wait \"sec\" {0}", m_waitTime);
                } else
                {
                    m_stringWriter.WriteLine("wait \"frame\" {0}", m_waitCount);
                }
                m_isAddWait = false;
                m_waitCount = 1;
                m_waitTime = Time.fixedDeltaTime;
            }
            else
            {
                m_waitCount++;
                m_waitTime += Time.fixedDeltaTime;
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
                var axis = InputBot.instance.GetAxisRaw(name);                
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
            for(var i = 0; i  < InputBot.instance.touchCount; i++)
            {
                var touch = InputBot.instance.GetTouch(i);                
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        {
                            if (m_isEnablePositionToGameObject)
                            {
                                var pointer = new PointerEventData(EventSystem.current);
                                pointer.position = touch.position;
                                pointer.pointerId = touch.fingerId;
                                var raycastResults = new List<RaycastResult>();
                                EventSystem.current.RaycastAll(pointer, raycastResults);
                                if (raycastResults.Count > 0)
                                {
                                    m_stringWriter.WriteLine("touch begin {0} {1}",
                                        touch.fingerId,
                                        raycastResults[0].gameObject.name
                                        );

                                    result = true;
                                    break;
                                }
                            }                            
                            
                            m_stringWriter.WriteLine(
                                "touch begin {0} {1} {2} {3} {4} {5} {6} {7}",
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

            if (InputBot.instance.mousePresent)
            {

                // Mouseのボタン
                for (var i = 0; i < InputBot.kMouseButtonCount; i++)
                {
                    if (InputBot.instance.GetMouseButtonDown(i))
                    {
                        m_stringWriter.WriteLine("mousebutton \"down\" {0}", i);
                        result = true;
                    }
                    else if (InputBot.instance.GetMouseButtonUp(i))
                    {
                        m_stringWriter.WriteLine("mousebutton \"up\" {0}", i);
                        result = true;
                    }
                }


                // Mouseの位置
                if (InputBot.instance.mousePosition != m_mousePosition || m_isCompress == false)
                {
                    m_stringWriter.WriteLine("mousepos {0} {1}", InputBot.instance.mousePosition.x, InputBot.instance.mousePosition.y);
                    m_mousePosition = InputBot.instance.mousePosition;
                    result = true;
                }
            }
            return result;
        }

    }





}