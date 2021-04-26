using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

using UnityEngine;

namespace Utj.UnityBotKun
{
    [System.Serializable]
    public class RemoteMessageBase
    {
        public static readonly System.Guid kMsgSendEditorToPlayer = new System.Guid("5cbcd5c1de1944c28a84c868044c8444");
        public static readonly System.Guid kMsgSendPlayerToEditor = new System.Guid("a36cc6ceedc84b36af660f12cb6155c8");

        public enum MessageId
        {
            Reflesh,
            Play,
            Stop,
            PlayFinish,
            AddEventScript,
            Record,
            StopRecord,
        }

        [SerializeField] public MessageId m_messageId;
        [SerializeField] public bool m_isSuccess;



        public byte[] ToBytes()
        {
            return Serialize(this);
        }


        public static byte[] Serialize(object obj)
        {            
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                return ms.ToArray();                                   
            }
        }


        public static T Desirialize<T>(byte[] srcs)
        {
            using (var ms = new MemoryStream(srcs))
            {
                var bf = new BinaryFormatter();
                return (T)bf.Deserialize(ms);
            }
        }
    }


    [System.Serializable]
    public class RemoteMessageReflesh : RemoteMessageBase
    {
        [SerializeField] public string[] m_scriptNames;
        [SerializeField] public int[] m_scriptNoValues;
        [SerializeField] public bool m_isPlay;
        [SerializeField] public bool m_isRecord;
        public RemoteMessageReflesh() : base()
        {
            m_messageId = MessageId.Reflesh;
        }
               
    }


    [System.Serializable]
    public class RemoteMessagePlay : RemoteMessageBase
    {
        [SerializeField ]public int m_scriptIdx;

        public RemoteMessagePlay():this(-1)
        {        
        }


        public RemoteMessagePlay(int scriptIdx):base()
        {
            m_messageId = MessageId.Play;
            m_scriptIdx = scriptIdx;
        }
    }

    [System.Serializable]
    public class RemoteMessageStop : RemoteMessageBase
    {
        public RemoteMessageStop():base()
        {
            m_messageId = MessageId.Stop;
        }
    }

    [System.Serializable]
    public class RemoteMessagePlayFinish:RemoteMessageBase
    {
        public RemoteMessagePlayFinish():base()
        {
            m_messageId = MessageId.PlayFinish;
        }
    }

    [System.Serializable]
    public class RemoteMessageAddEventScript : RemoteMessageReflesh
    {
        [SerializeField]public string m_name;
        [SerializeField]public string m_text;
        [SerializeField] public int m_setIndex;

        public RemoteMessageAddEventScript():base()
        {
            m_messageId = MessageId.AddEventScript;
        }
    }


    [System.Serializable]
    public class RemoteMessageRecord : RemoteMessageBase
    {
        public RemoteMessageRecord() : base()
        {
            m_messageId = MessageId.Record;
        }        
    }


    [System.Serializable]
    public class RemoteMessageStopRecord : RemoteMessageBase
    {
        [SerializeField]public string m_text;

        public RemoteMessageStopRecord():base()
        {
            m_messageId = MessageId.StopRecord;
        }
    }
 
}
