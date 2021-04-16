using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Utj
{
    /// <summary>
    /// DontDestroyOnLoadを実行するだけのClass
    /// Programed by Katsumasa.Kimura
    /// </summary>
    public class DontDestory : MonoBehaviour
    {

        static GameObject instance;

        [SerializeField, Tooltip("Sceneを跨いで使用する場合は、有効にする")]
        bool m_isDontDestroyOnLoad;

        private void Awake()
        {
            if (instance == null)
            {
                instance = gameObject;
                if (m_isDontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                GameObject.Destroy(gameObject);
            }
        }


        private void OnDestroy()
        {
            if (instance == gameObject)
            {
                instance = null;
            }
        }        
    }
}