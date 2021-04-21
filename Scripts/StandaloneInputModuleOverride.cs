using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utj.UnityBotKun
{

    public class StandaloneInputModuleOverride : StandaloneInputModule
    {        

        public static StandaloneInputModuleOverride instance
        {
            get;
            private set;
        }

        public GameObject currentSelectGameObject
        {
            get
            {
                //return eventSystem.currentSelectedGameObject;
                return GetCurrentFocusedGameObject();
            }
        }


        protected override void Awake()
        {
               
            if (instance == null)
            {
                instance = this;
                base.Awake();                
            }
            else
            {
                Destroy(this);
            }
        }


        protected override void Start()
        {
            base.Start();
            inputOverride = InputBot.instance;
        }



        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (instance == this)
            {                
                instance = null;
            }
        }

        public override void Process()
        {
#if UNITY_EDITOR
            // eventSystem.isFocusedはvoid OnApplicationFocus(bool hasFocus)によって設定されるが、
            // UnityEditorの場合、GameViewからフォーカスが外れると、
            // フォーカスが外れたと認定される為、Processが実行されなくなるが、
            // それでは困る為、UnityEditor上で実行する場合、フォーカスを無視するようにProcessの処理を変更

            //if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
            //    return;


            bool usedEvent = SendUpdateEventToSelectedObject();

            // case 1004066 - touch / mouse events should be processed before navigation events in case
            // they change the current selected gameobject and the submit button is a touch / mouse button.

            // touch needs to take precedence because of the mouse emulation layer
            if (!ProcessTouchEvents() && input.mousePresent)
                ProcessMouseEvent();

            if (eventSystem.sendNavigationEvents)
            {
                if (!usedEvent)
                    usedEvent |= SendMoveEventToSelectedObject();

                if (!usedEvent)
                    SendSubmitEventToSelectedObject();
            }
#else
            base.Process();
#endif
        }

        private bool ProcessTouchEvents()
        {
            for (int i = 0; i < input.touchCount; ++i)
            {
                Touch touch = input.GetTouch(i);

                if (touch.type == TouchType.Indirect)
                    continue;

                bool released;
                bool pressed;
                var pointer = GetTouchPointerEventData(touch, out pressed, out released);

                ProcessTouchPress(pointer, pressed, released);

                if (!released)
                {
                    ProcessMove(pointer);
                    ProcessDrag(pointer);
                }
                else
                    RemovePointerData(pointer);
            }
            return input.touchCount > 0;
        }
    }
}