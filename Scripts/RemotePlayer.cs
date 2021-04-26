using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;


namespace Utj.UnityBotKun
{
    /// <summary>
    /// EditorとPlayer間で通信を行うクラス
    /// Programed by Katsumasa.Kimura
    /// </summary>
    public class RemotePlayer : MonoBehaviour
    {
        delegate void Task();

        Task m_updateTask;


        private void OnEnable()
        {
            PlayerConnection.instance.Register(RemoteMessageBase.kMsgSendEditorToPlayer, OnMessageEvent);
        }


        private void OnDisable()
        {
            PlayerConnection.instance.Unregister(RemoteMessageBase.kMsgSendEditorToPlayer, OnMessageEvent);
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(m_updateTask != null)
            {
                m_updateTask();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        void OnMessageEvent(MessageEventArgs args)
        {
            var messageBase = new RemoteMessageBase();
            messageBase = RemoteMessageBase.Desirialize<RemoteMessageBase>(args.data);
            Debug.Log("messageId:"+messageBase.m_messageId);
            switch (messageBase.m_messageId)
            {                
                case RemoteMessageBase.MessageId.Reflesh:
                    {
                        var message = RemoteMessageBase.Desirialize<RemoteMessageReflesh>(args.data);
                        message.m_scriptNames = new string[EventScriptSystem.instance.scripts.Count];
                        message.m_scriptNoValues = new int[EventScriptSystem.instance.scripts.Count];
                        for (var i = 0; i < message.m_scriptNames.Length; i++)
                        {
                            message.m_scriptNames[i] = EventScriptSystem.instance.scripts[i].name;
                            message.m_scriptNoValues[i] = i;
                        }
                        message.m_isRecord = InputRecorder.instance.isRecording;
                        message.m_isPlay = EventScriptSystem.instance.isPlay;


                        SendRemoteMessage(message.ToBytes());                        
                    }
                    break;

                case RemoteMessageBase.MessageId.Play:
                    {
                        var message = RemoteMessageBase.Desirialize<RemoteMessagePlay>(args.data);
                        EventScriptSystem.instance.Play(message.m_scriptIdx);
                        message.m_isSuccess = true;
                        SendRemoteMessage(message.ToBytes());
                        m_updateTask += PlayScriptFinishCB;
                    }
                    break;

                case RemoteMessageBase.MessageId.Stop:
                    {
                        EventScriptSystem.instance.Stop();
                        messageBase.m_isSuccess = true;
                        SendRemoteMessage(messageBase.ToBytes());
                    }
                    break;

                case RemoteMessageBase.MessageId.AddEventScript:
                    {
                        var message = RemoteMessageBase.Desirialize<RemoteMessageAddEventScript>(args.data);
                        var textAsset = new TextAsset(message.m_text);
                        textAsset.name = message.m_name;
                        message.m_setIndex = EventScriptSystem.instance.SetScript(textAsset);
                        message.m_scriptNames = new string[EventScriptSystem.instance.scripts.Count];
                        message.m_scriptNoValues = new int[EventScriptSystem.instance.scripts.Count];
                        for (var i = 0; i < message.m_scriptNames.Length; i++)
                        {
                            message.m_scriptNames[i] = EventScriptSystem.instance.scripts[i].name;
                            message.m_scriptNoValues[i] = i;
                        }

                        message.m_isSuccess = true;
                        SendRemoteMessage(message.ToBytes());
                    }
                    break;

                case RemoteMessageBase.MessageId.Record:
                    {
                        InputRecorder.instance.Record();
                        messageBase.m_isSuccess = true;
                        SendRemoteMessage(messageBase.ToBytes());
                    }
                    break;

                case RemoteMessageBase.MessageId.StopRecord:
                    {
                        var message = RemoteMessageBase.Desirialize<RemoteMessageStopRecord>(args.data);
                        InputRecorder.instance.Stop();
                        message.m_text = InputRecorder.instance.textAsset.ToString();
                        message.m_isSuccess = true;
                        SendRemoteMessage(message.ToBytes());
                    }
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        void SendRemoteMessage(byte[] bytes)
        {
            PlayerConnection.instance.Send(RemoteMessageBase.kMsgSendPlayerToEditor,bytes);
        }


        void PlayScriptFinishCB()
        {
            if(EventScriptSystem.instance.isPlay == false)
            {
                var message = new RemoteMessagePlayFinish();
                SendRemoteMessage(message.ToBytes());

                m_updateTask -= PlayScriptFinishCB;
            }
        }
    }
}