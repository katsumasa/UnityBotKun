using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Utj.UnityBotKun
{
    public static class Input2
    {

        public static bool touchSupported
        {
            get
            {
                return BaseInputOverride.instance.touchSupported;
            }
        }


        public static int touchCount
        {
            get
            {
                return BaseInputOverride.instance.touchCount;
            }
        }

        public static Vector2 mousePosition
        {
            get
            {
                return BaseInputOverride.instance.mousePosition;
            }
        }

        public static Vector2 mouseScrollDelta
        {
            get
            {
                return BaseInputOverride.instance.mouseScrollDelta;
            }
        }
        public static bool mousePresent
        {
            get
            {
                return BaseInputOverride.instance.mousePresent;
            }

        }


        public static Touch GetTouch(int index)
        {
            return BaseInputOverride.instance.GetTouch(index);
        }

        public static float GetAxisRaw(string axisName)
        {
            return BaseInputOverride.instance.GetAxisRaw(axisName);
        }

        public static bool GetButtonDown(string buttonName)
        {
            return BaseInputOverride.instance.GetButtonDown(buttonName);
        }


        public static bool GetMouseButton(int button)
        {
            return BaseInputOverride.instance.GetMouseButton(button);
        }

        public static bool GetMouseButtonDown(int button)
        {
            return BaseInputOverride.instance.GetMouseButtonDown(button);
        }

        public static bool GetMouseButtonUp(int button)
        {
            return BaseInputOverride.instance.GetMouseButtonUp(button);
        }
    }        
}