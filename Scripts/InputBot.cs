using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Utj
{    
    /// <summary>
    /// BaseInputを偽装するClass
    /// Programed by Katsumasa Kimura
    /// </summary>
    public class InputBot : BaseInput
    {
        //
        // staticプロパティの定義
        //

        /// <summary>
        /// マウスのボタンの最大数
        /// </summary>
        public static readonly int kMouseButtonCount = 7;

        
        public enum ButtonState{
            None, // 押されていない
            Begin,// 押されたフレーム
            Down, // 押されている
            Up,   // 放されたフレーム         
        }


        /// <summary>
        /// BotInput Classのインスタンス
        /// </summary>
        public static InputBot instance
        {
            get;
            private set;
        }
                                            

        [SerializeField,Tooltip("Inputをスクリプトから制御することを許可します。")]
        bool m_isOverrideInput;
        public bool isOverrideInput
        {
            get {return m_isOverrideInput; }
            set { m_isOverrideInput = value; }
        }


        [SerializeField, Tooltip("Mouseの入力をTouchへ変換します。")]
        bool m_isEnableTouchSimulation;
        

        /// <summary>
        /// MouseをTouchとしてシュミレーションするか
        /// </summary>
        public bool isEnableTouchSimulation
        {
            get { return m_isEnableTouchSimulation && Application.isEditor; }
            set { m_isEnableTouchSimulation = value; }
        }

        
        /// <summary>
        /// Mouseの入力をTouchへ変換した内容
        /// ※Debug目的で参照可能
        /// </summary>
        public Touch mouseTouch {
            get { return m_mouseTouch; }
            private set { m_mouseTouch = value; }
        }


        
        /// <summary>
        /// タップした数を返す
        /// </summary>
        public override int touchCount
        {
            get
            {
                if (isEnableTouchSimulation)
                {
                    return (m_mouseTouch.phase != TouchPhase.Canceled) ? 1 : 0;
                }
                else if (isOverrideInput)
                {                                        
                    return m_touchEvents.Count;                    
                }
                else
                {
                    return base.touchCount;
                }
            }
        }


        public class TouchEvent
        {
            public Touch touch;


            public TouchEvent()
            {
                touch = new Touch();
            }
        }



        //=========================================================================================
        // private変数の定義
        //=========================================================================================

        List<TouchEvent> m_touchEvents;
        Dictionary<string, float> m_axisEvents;
        Dictionary<string, bool> m_buttonDownEvents;        
        Touch m_mouseTouch;
        ButtonState[] m_mouseButtonEvents;
        Vector2 m_mousePosition;
        Vector2 m_mousePrevPosition;
        Vector2 m_mouseScrollDelta;


        //=========================================================================================
        // public関数の定義
        //=========================================================================================

        /// <summary>
        /// Tochを返します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override Touch GetTouch(int index)
        {
            if (isEnableTouchSimulation)
            {
                return m_mouseTouch;
            }
            else if (!isOverrideInput)
            {
                return base.GetTouch(index);
            }            
            else
            {
                return m_touchEvents[index].touch;
            }                        
        }


        /// <summary>
        /// axisName で識別される仮想軸の平滑化フィルターが適用されていない値を返します
        /// </summary>
        /// <param name="axisName"></param>
        /// <returns></returns>
        public override float GetAxisRaw(string axisName)
        {
            if (!isOverrideInput)
            {
                return base.GetAxisRaw(axisName);
            }
            
            if (m_axisEvents.ContainsKey(axisName))
            {
                return m_axisEvents[axisName];
            }
            else
            {
                return 0f;
            }            
        }


        /// <summary>
        /// Interface to Input.GetButtonDown. Can be overridden to provide custom input instead of using the Input class.
        /// </summary>
        /// <param name="buttonName"></param>
        /// <returns></returns>
        public override bool GetButtonDown(string buttonName)
        {
            if (!isOverrideInput)
            {
                var result = base.GetButtonDown(buttonName);
                if (result)
                {
                    return true;
                }
                return false;
            }

            if (m_axisEvents.ContainsKey(buttonName))
            {
                return true;
            }
            if (m_buttonDownEvents.ContainsKey(buttonName))
            {
                return m_buttonDownEvents[buttonName];                    
            }
            return false;            
        }


        /// <summary>
        /// マウスボタンが押されているかどうかを返します
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public override bool GetMouseButton(int button)
        {
            if (!isOverrideInput)
            {
                return base.GetMouseButton(button);
            }
            if(m_mouseButtonEvents[button] != ButtonState.Begin)
            {
                return true;
            }
            return false;            
        }


        /// <summary>
        /// ユーザーがマウスボタンを押したフレームの間だけ true を返します
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public override bool GetMouseButtonDown(int button)
        {
            if (!isOverrideInput)
            {
                return base.GetMouseButtonDown(button);
            }
            if (m_mouseButtonEvents[button] == ButtonState.Down)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// マウスボタンを離したフレームの間だけ true を返します
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public override bool GetMouseButtonUp(int button)
        {
            if (!isOverrideInput)
            {
                return base.GetMouseButtonUp(button);
            }
            if(m_mouseButtonEvents[button] == ButtonState.Up)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Mouseの現在位置
        /// </summary>
        public override Vector2 mousePosition {
            get
            {
                if (!isOverrideInput)
                {
                    return base.mousePosition;
                }
                return m_mousePosition;
            }
        }


        /// <summary>
        /// Mouseの移動量
        /// </summary>
        public override Vector2 mouseScrollDelta {
            get
            {
                if (!isOverrideInput)
                {
                    return base.mouseScrollDelta;
                }
                return m_mouseScrollDelta;
            }
        }


        /// <summary>
        /// Mouseが有効であるか否か
        /// </summary>
        public override bool mousePresent {
            get
            {
                // Touchシュミレーションが有効な場合は、Mouseは無効とする
                if (isEnableTouchSimulation)
                {
                    return false;
                }
                return base.mousePresent;
            }
        }

        
        /// <summary>
        /// ボタンを押す
        /// </summary>
        /// <param name="buttonName">Buttonの名称</param>
        public void SetButtonDown(string buttonName,bool isDown)
        {
            if (m_buttonDownEvents.ContainsKey(buttonName))
            {
                m_buttonDownEvents[buttonName] = isDown;
            }
            else
            {                
                m_buttonDownEvents.Add(buttonName, isDown);
            }            
        }


        /// <summary>
        /// AxisRawの疑似情報を発生させる
        /// </summary>
        /// <param name="name">Axis名</param>
        /// <param name="axisRaw">Axis Raw値</param>
        /// <param name="time">発生時間</param>
        public void SetAxisRaw(string name, float axisRaw)
        {
            if (m_axisEvents.ContainsKey(name))
            {
                m_axisEvents[name] = axisRaw;
            }
            else
            {
                m_axisEvents.Add(name, axisRaw);
            }
        }


        /// <summary>
        /// Touchの疑似情報を発生させる
        /// </summary>
        /// <param name="fingerId">フィンガーID</param>
        /// <param name="target">GameObject名</param>
        public void SetTouchBegin(int fingerId, string target)
        {
            SetTouchBegin(fingerId, GameObject.Find(name));
        }


        /// <summary>
        /// Touchを発生させる
        /// </summary>
        /// <param name="fingerId"></param>
        /// <param name="position"></param>
        /// <param name="maximumPossiblePressure"></param>
        /// <param name="pressure"></param>
        /// <param name="radius"></param>
        /// <param name="radiusVariance"></param>
        /// <param name="type"></param>
        /// <param name="tapCount"></param>
        public void SetTouchBegin(
            int fingerId,
            Vector2 position, 
            float maximumPossiblePressure = 1.0f, 
            float pressure = 1.0f, 
            float radius = 0f, 
            float radiusVariance = 0f, 
            TouchType type = TouchType.Direct, 
            int tapCount = 1)
        {
            var touchEvent = new TouchEvent();
            touchEvent.touch.phase = TouchPhase.Began;
            touchEvent.touch.position = position;
            touchEvent.touch.rawPosition = position;
            touchEvent.touch.maximumPossiblePressure = maximumPossiblePressure;
            touchEvent.touch.pressure = pressure;
            touchEvent.touch.radius = radius;
            touchEvent.touch.radiusVariance = radiusVariance;
            touchEvent.touch.type = type;
            touchEvent.touch.tapCount = tapCount;
            touchEvent.touch.fingerId = fingerId;
            m_touchEvents.Add(touchEvent);
        }


        /// <summary>
        /// スワイプ
        /// </summary>
        /// <param name="fingerId"></param>
        /// <param name="position"></param>
        public void SetTouchMove(int fingerId, 
            Vector2 position,
            float maximumPossiblePressure = 1.0f,
            float pressure = 1.0f,
            float radius = 0f,
            float radiusVariance = 0f,
            TouchType type = TouchType.Direct,
            int tapCount = 1)
        {
            for (var i = 0; i < m_touchEvents.Count; i++)
            {                
                if (m_touchEvents[i].touch.fingerId == fingerId)
                {
                    m_touchEvents[i].touch.maximumPossiblePressure = maximumPossiblePressure;
                    m_touchEvents[i].touch.pressure = pressure;
                    m_touchEvents[i].touch.radius = radius;
                    m_touchEvents[i].touch.radiusVariance = radiusVariance;
                    m_touchEvents[i].touch.deltaPosition = position - m_touchEvents[i].touch.position;
                    m_touchEvents[i].touch.position = position;
                    m_touchEvents[i].touch.deltaTime += Time.deltaTime;
                    m_touchEvents[i].touch.phase = TouchPhase.Moved;
                    return;
                }
            }
        }


        /// <summary>
        /// Touchを放す
        /// </summary>
        /// <param name="fingerId"></param>
        public void SetTouchEnded(int fingerId)
        {
            for (var i = 0; i < m_touchEvents.Count; i++)
            {                
                if (m_touchEvents[i].touch.fingerId == fingerId)
                {
                    m_touchEvents[i].touch.phase = TouchPhase.Ended;
                    return;
                }
            }
        }


        /// <summary>
        /// マウスのボタンを押す
        /// </summary>
        /// <param name="no">マウスのボタン番号</param>
        public void SetMouseButtonDown(int no = 0)
        {
            m_mouseButtonEvents[no] = ButtonState.Begin;
        }


        /// <summary>
        /// マウスのボタンを放す
        /// </summary>
        /// <param name="no">マウスのボタン番号</param>
        public void SetMouseButtonUp(int no = 0)
        {
            m_mouseButtonEvents[no] = ButtonState.Up;
        }


        /// <summary>
        /// マウスの座標を指定する
        /// </summary>
        /// <param name="position"></param>
        public void SetMousePosition(Vector2 position)
        {
            m_mousePosition = position;
        }


        //=========================================================================================
        // private関数の定義
        //=========================================================================================

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            if (instance == null)
            {
                instance = this;
                base.Awake();
                m_touchEvents = new List<TouchEvent>();
                m_axisEvents = new Dictionary<string, float>();                
                m_buttonDownEvents = new Dictionary<string, bool>();
                m_mouseTouch = new Touch();
                m_mouseButtonEvents = new ButtonState[kMouseButtonCount];
                m_mousePosition = Vector2.zero;
                m_mousePrevPosition = Vector2.zero;
                m_mouseScrollDelta = Vector2.zero;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        /// <summary>
        /// OnDestoroy
        /// </summary>
        protected override void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
            m_touchEvents = null;
            m_axisEvents = null;
            m_buttonDownEvents = null;
            base.OnDestroy();
             
        }

        
        /// <summary>
        /// Update
        /// </summary>
        void Update()
        {
            var deltaTime = Time.deltaTime;                        
            if (isEnableTouchSimulation)
            {
                UpdateMouseTouch(deltaTime);
            }
            else
            {
                UpdateTouchEvent(deltaTime);
                UpdateMouseEvent(deltaTime);
            }            
        }


        /// <summary>
        /// Touch Eventのアップデート
        /// </summary>
        /// <param name="deltaTime"></param>
        void UpdateTouchEvent(float deltaTime)
        {
            if (m_touchEvents != null)
            {
                for (var i = 0; i < m_touchEvents.Count; i++)
                {
                    
                    switch (m_touchEvents[i].touch.phase)
                    {
                        case TouchPhase.Began:
                            {
                                m_touchEvents[i].touch.phase = TouchPhase.Stationary;                                
                            }
                            break;

                        case TouchPhase.Moved:
                            {
                                m_touchEvents[i].touch.deltaPosition = Vector2.zero;
                                m_touchEvents[i].touch.deltaTime = deltaTime;
                                m_touchEvents[i].touch.phase = TouchPhase.Stationary;
                            }
                            break;

                        case TouchPhase.Ended:
                            {
                                m_touchEvents.Remove(m_touchEvents[i]);
                            }
                            break;
                    }

                }
            }
        }


        /// <summary>
        /// MouseをTouchとして動作させる
        /// </summary>
        /// <param name="deltaTime"></param>
        void UpdateMouseTouch(float deltaTime)
        {
            if (base.GetMouseButtonDown(0))
            {
                m_mouseTouch.phase = TouchPhase.Began;
                m_mouseTouch.position = base.mousePosition;
                m_mouseTouch.rawPosition = base.mousePosition;
                m_mouseTouch.pressure = 1.0f;
                m_mouseTouch.radius = 0;
                m_mouseTouch.radiusVariance = 0;
                m_mouseTouch.tapCount = 1;
                m_mouseTouch.type = TouchType.Direct;
                m_mouseTouch.altitudeAngle = 0;
                m_mouseTouch.azimuthAngle = 0;
                m_mouseTouch.deltaPosition = Vector2.zero;
                m_mouseTouch.deltaTime = 0;
                m_mouseTouch.maximumPossiblePressure = 1.0f;
                m_mouseTouch.fingerId = StandaloneInputModuleOverride.kFakeTouchesId;
            }
            else if (base.GetMouseButton(0))
            {
                m_mouseTouch.deltaTime += deltaTime;                
                m_mouseTouch.deltaPosition = base.mousePosition - m_mouseTouch.position;
                m_mouseTouch.position = base.mousePosition;                
                if(m_mouseTouch.deltaPosition.magnitude >= 0.5f)
                {
                    m_mouseTouch.phase = TouchPhase.Moved;
                }
                else
                {
                    m_mouseTouch.phase = TouchPhase.Stationary;
                }
            } 
            else if (base.GetMouseButtonUp(0))
            {                
                m_mouseTouch.deltaTime += deltaTime;
                m_mouseTouch.deltaPosition = base.mousePosition - m_mouseTouch.position;
                m_mouseTouch.position = base.mousePosition;
                m_mouseTouch.phase = TouchPhase.Ended;
            }
            else
            {
                m_mouseTouch.position = Vector2.zero;
                m_mouseTouch.deltaPosition = Vector2.zero;
                m_mouseTouch.phase = TouchPhase.Canceled;
            }
        }


        /// <summary>
        /// マウスを更新する
        /// </summary>
        /// <param name="deltaTime"></param>
        void UpdateMouseEvent(float deltaTime)
        {
            // ボタンを更新する
            for (var i = 0; i < m_mouseButtonEvents.Length; i++)
            {
                switch (m_mouseButtonEvents[i])
                {
                    case ButtonState.Begin:
                        m_mouseButtonEvents[i] = ButtonState.Down;
                        break;
                    case ButtonState.Up:
                        m_mouseButtonEvents[i] = ButtonState.None;
                        break;
                }
            }
            // ポジションを更新する
            m_mouseScrollDelta = m_mousePrevPosition - m_mousePosition;
            m_mousePrevPosition = m_mousePosition;
        }
            




        /// <summary>
        /// Touchの疑似情報を発生させる
        /// </summary>
        /// <param name="fingerId">フィンガーID</param>
        /// <param name="target"></param>        
        void SetTouchBegin(int fingerId, GameObject target)
        {
            if (target != null && target.GetComponent<UIBehaviour>() != null)
            {
                var canvas = GetCanvas(target);
                if (canvas)
                {
                    SetTouchBegin(fingerId, target, canvas);
                }
            }
        }


        /// <summary>
        /// Touchの疑似情報を発生させる
        /// </summary>
        /// <param name="fingerId">フィンガーID</param>
        /// <param name="target"></param>
        /// <param name="canvas"></param>
        void SetTouchBegin(int fingerId, GameObject target, Canvas canvas)
        {
            if (target != null && canvas != null)
            {
                switch (canvas.renderMode)
                {
                    case RenderMode.ScreenSpaceOverlay:
                        {
                            var position = RectTransformUtility.WorldToScreenPoint(null, target.transform.position);

                            SetTouchBegin(fingerId, position);
                        }
                        break;

                    case RenderMode.ScreenSpaceCamera:
                        {
                            var position = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, target.transform.position);
                            SetTouchBegin(fingerId, position);
                        }
                        break;

                    case RenderMode.WorldSpace:
                        {
                            var position = canvas.worldCamera.WorldToScreenPoint(target.transform.position);
                            SetTouchBegin(fingerId, position);
                        }
                        break;
                }
            }
        }


        /// <summary>
        /// GameObjectが所属するCanvasを取得する
        /// </summary>
        /// <param name="obj">GameObject</param>
        /// <returns>Canvas</returns>
        Canvas GetCanvas(GameObject obj)
        {
            var canvas = obj.GetComponent<Canvas>();
            if (obj.activeInHierarchy && canvas != null)
            {
                return canvas;
            }
            else if (obj.transform.parent != null)
            {
                return GetCanvas(obj.transform.parent.gameObject);
            }
            return null;            
        }       
    }
}
