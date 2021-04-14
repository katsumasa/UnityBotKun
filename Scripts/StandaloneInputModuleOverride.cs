using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utj
{

    public class StandaloneInputModuleOverride : StandaloneInputModule
    {        

        public static StandaloneInputModuleOverride instance
        {
            get;
            private set;
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
            m_InputOverride = DummyInput.instance;
        }


        public override void UpdateModule()
        {            
            base.UpdateModule();            
        }


        protected override void OnDestroy()
        {
            if(instance == this)
            {
                instance = null;
            }
        }
    }
}