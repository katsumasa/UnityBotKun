using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utj.UnityBotKun
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

        [Header("Axes")]

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

        [Header("Recorder Settings")]
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


        public static InputRecorder instance {
            get;
            private set;
        }


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

#if false
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
#else
        [TextArea(3,5)]
        public string script;
#endif

        /// <summary>
        /// スクリプト書き込み用
        /// </summary>
        StringBuilder m_scripttAll;
        StringBuilder m_scriptBuffer;


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
                m_scriptBuffer = new StringBuilder();
                m_scripttAll = new StringBuilder();
                isRecording = true;
                m_waitCount = 0;
                m_waitTime = 0f;
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
                if (m_waitType == Wait.Sec)
                {
                    m_scripttAll.AppendFormat("wait sec {0}\n", m_waitTime);
                }
                else
                {
                    m_scripttAll.AppendFormat("wait frame {0}\n", m_waitCount);
                }

                textAsset = new TextAsset(m_scripttAll.ToString());
                isRecording = false;
                m_scripttAll = null;
                m_scriptBuffer = null;
            }
        }

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        // Start is called before the first frame update
        void Start()
        {            
        }


        
        void Update()
        {
            if (isRecording)
            {
                InputEventRecorder(Time.unscaledDeltaTime);
            }
        }


        private void OnDestroy()
        {
            if(instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Inputイベントを記録する
        /// </summary>
        void InputEventRecorder(float deltaTime)
        {
            m_waitCount++;
            m_waitTime += deltaTime;


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

            if (m_isAddWait)
            {
                if (m_waitType == Wait.Sec)
                {
                    m_scripttAll.AppendFormat("wait sec {0}\n", m_waitTime);
                }
                else
                {
                    m_scripttAll.AppendFormat("wait frame {0}\n", m_waitCount);                    
                }
                m_scripttAll.Append(m_scriptBuffer);
                m_scriptBuffer.Clear();

                m_isAddWait = false;
                m_waitCount = 0;
                m_waitTime = 0f;
            }
            script = m_scripttAll.ToString();
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
                    m_scriptBuffer.AppendFormat("button {0} down\n", buttonName);
                    ret = true;
                }
                else if (Input.GetButtonUp(buttonName))
                {
                    m_scriptBuffer.AppendFormat("button {0} up\n", buttonName);
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
                var axis = BaseInputOverride.instance.GetAxisRaw(name);                
                if (axis != m_axisValues[i] || m_isCompress == false)
                {
                    m_axisValues[i] = axis;
                    m_scriptBuffer.AppendFormat("axisraw {0} {1}\n", name, axis);
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
            for(var i = 0; i  < BaseInputOverride.instance.touchCount; i++)
            {
                var touch = BaseInputOverride.instance.GetTouch(i);                
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
                                    m_scriptBuffer.AppendFormat("touch begin {0} {1}\n",
                                        touch.fingerId,
                                        raycastResults[0].gameObject.name
                                        );

                                    result = true;
                                    break;
                                }
                            }

                            m_scriptBuffer.AppendFormat(
                                "touch begin {0} {1} {2} {3} {4} {5} {6} {7}\n",
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
                            m_scriptBuffer.AppendFormat("touch move {0} {1} {2} {3} {4} {5}\n",
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
                            m_scriptBuffer.AppendFormat("touch ended {0}\n", touch.fingerId);
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

            if (BaseInputOverride.instance.mousePresent)
            {

                // Mouseのボタン
                for (var i = 0; i < BaseInputOverride.kMouseButtonCount; i++)
                {
                    if (BaseInputOverride.instance.GetMouseButtonDown(i))
                    {
                        m_scriptBuffer.AppendFormat("mousebutton {0} down\n", i);
                        result = true;
                    }
                    else if (BaseInputOverride.instance.GetMouseButtonUp(i))
                    {
                        m_scriptBuffer.AppendFormat("mousebutton {0} up\n", i);
                        result = true;
                    }
                }


                // Mouseの位置
                if (BaseInputOverride.instance.mousePosition != m_mousePosition || m_isCompress == false)
                {
                    m_scriptBuffer.AppendFormat("mousepos {0} {1}\n", BaseInputOverride.instance.mousePosition.x, BaseInputOverride.instance.mousePosition.y);
                    m_mousePosition = BaseInputOverride.instance.mousePosition;
                    result = true;
                }
            }
            return result;
        }

    }





}