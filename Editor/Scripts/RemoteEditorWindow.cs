using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.Profiling;
using UnityEditor.Networking.PlayerConnection;
#if UNITY_2020_1_OR_NEWER
using ConnectionUtility = UnityEditor.Networking.PlayerConnection.PlayerConnectionGUIUtility;
using ConnectionGUILayout = UnityEditor.Networking.PlayerConnection.PlayerConnectionGUILayout;
using UnityEngine.Networking.PlayerConnection;
#else
using ConnectionUtility = UnityEditor.Experimental.Networking.PlayerConnection.EditorGUIUtility;
using ConnectionGUILayout = UnityEditor.Experimental.Networking.PlayerConnection.EditorGUILayout;
using UnityEngine.Experimental.Networking.PlayerConnection;
#endif


namespace Utj.UnityBotKun
{
    /// <summary>
    /// 
    /// </summary>
    public class RemoteEditorWindow : EditorWindow
    {
        private static class Styles
        {
            
            public static readonly GUIContent TitleContents   = new GUIContent("UnityBotKun RemoteClient");            
            public static readonly GUIContent PlayContents = new GUIContent((Texture2D)EditorGUIUtility.Load("d_PlayButton@2x"), "Play Script");
            public static readonly GUIContent RecordContents  = new GUIContent((Texture2D)EditorGUIUtility.Load("d_Record On@2x"),"Record Script");           
            public static readonly GUIContent FolderContents = new GUIContent((Texture2D)EditorGUIUtility.Load("d_OpenedFolder Icon"), "Set Save Location");
            public static readonly GUIContent ScriptSelectContens = new GUIContent("Play Script","実行するスクリプトを選択します。");
            public static readonly GUIContent ReloadContens = new GUIContent((Texture2D)EditorGUIUtility.Load("Refresh@2x"),"Refresh");
            public static readonly GUIContent UploadContens = new GUIContent((Texture2D)EditorGUIUtility.Load("d_ol_plus"),"Add Script to Player");
            public static readonly GUIContent EventScriptContens = new GUIContent("Upload","Playerに追加するEventScript");
        }



        IConnectionState m_connectionState;
        [SerializeField] bool m_isPlay;
        [SerializeField] bool m_isRec;
        [SerializeField] bool m_isWait;
        [SerializeField] static RemoteEditorWindow m_window;
        [SerializeField] int m_scriptIdx;
        [SerializeField] List<int> m_scriptNoValues;
        [SerializeField] List<GUIContent> m_scriptNames;
        [SerializeField] TextAsset m_uploadTextAsset;


        [MenuItem("Window/UnityBotKun/RemoteClient")]
        static void OpenWindow()
        {
            m_window = (RemoteEditorWindow)EditorWindow.GetWindow(typeof(RemoteEditorWindow));
            m_window.titleContent = Styles.TitleContents;
        }


        private void Awake()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        private void OnEnable()
        {
            if(m_scriptNames == null)
            {
                m_scriptNames = new List<GUIContent>();
            }

            if(m_scriptNoValues == null)
            {
                m_scriptNoValues = new List<int>();
            }

            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.Initialize();
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.Register(RemoteMessageBase.kMsgSendPlayerToEditor, OnMessageEvent);
            
#if UNITY_2020_1_OR_NEWER
            m_connectionState = ConnectionUtility.GetConnectionState(this);
#else
            m_connectionState = ConnectionUtility.GetAttachToPlayerState(this);
#endif

        }


        /// <summary>
        /// 
        /// </summary>
        private void OnDisable()
        {
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.Unregister(RemoteMessageBase.kMsgSendPlayerToEditor, OnMessageEvent);
            UnityEditor.Networking.PlayerConnection.EditorConnection.instance.DisconnectAll();
            m_connectionState.Dispose();
        }


        private void OnDestroy()
        {                        
        }


        /// <summary>
        /// 
        /// </summary>
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayoutConnect();
            ReloadGUI();
            PlayButtonGUI();
            RecButtonGUI();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayoutConnectInfo();
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            SelectScriptGUI();
            EditorGUILayout.EndHorizontal();

            UploadtTextAssetGUI();
        }


        /// <summary>
        /// 
        /// </summary>
        private void PlayButtonGUI()
        {
            bool isDisable = (EditorConnection.instance.ConnectedPlayers.Count == 0) || m_isWait || m_isRec;

            EditorGUI.BeginDisabledGroup(isDisable);
            var contentSize = EditorStyles.label.CalcSize(Styles.PlayContents);
            EditorGUI.BeginChangeCheck();
            m_isPlay = GUILayout.Toggle(m_isPlay, Styles.PlayContents, EditorStyles.miniButtonMid,GUILayout.MaxWidth(25));
            if (EditorGUI.EndChangeCheck())
            {
                m_isWait = true;
                if (m_isPlay)
                {
                    var message = new RemoteMessagePlay(m_scriptIdx);
                    SendMessage(message.ToBytes());
                }
                else
                {
                    var message = new RemoteMessageStop();
                    SendMessage(message.ToBytes());
                }
            }
            EditorGUI.EndDisabledGroup();
        }


        private void RecButtonGUI()        
        {
            bool isDisable = (EditorConnection.instance.ConnectedPlayers.Count == 0) || m_isWait || m_isPlay;

            EditorGUI.BeginDisabledGroup(isDisable);
            var contentSize = EditorStyles.label.CalcSize(Styles.RecordContents);
            EditorGUI.BeginChangeCheck();
            m_isRec = GUILayout.Toggle(m_isRec, Styles.RecordContents, EditorStyles.miniButtonRight,GUILayout.MaxWidth(25));
            if (EditorGUI.EndChangeCheck())
            {
                if (m_isRec)
                {
                    var message = new RemoteMessageRecord();
                    SendMessage(message.ToBytes());
                    m_isWait = true;
                }
                else
                {
                    var message = new RemoteMessageStopRecord();
                    SendMessage(message.ToBytes());
                    m_isWait = true;
                }
            }
            EditorGUI.EndDisabledGroup();
        }


        private void SelectScriptGUI()
        {
            bool isDisable = (EditorConnection.instance.ConnectedPlayers.Count == 0) || m_isWait || m_isPlay || m_isRec;
            EditorGUI.BeginDisabledGroup(isDisable);
            m_scriptIdx = EditorGUILayout.IntPopup(Styles.ScriptSelectContens, m_scriptIdx, m_scriptNames.ToArray(), m_scriptNoValues.ToArray());
            EditorGUI.EndDisabledGroup();
        }


        private void ReloadGUI()
        {
            bool isDisable = (EditorConnection.instance.ConnectedPlayers.Count == 0) || m_isWait || m_isPlay || m_isRec;
            EditorGUI.BeginDisabledGroup(isDisable);
            if (GUILayout.Button(Styles.ReloadContens, EditorStyles.miniButtonLeft, GUILayout.MaxWidth(25)))
            {
                m_isWait = true;
                var message = new RemoteMessageReflesh();
                SendMessage(message.ToBytes());
            }            
            EditorGUI.EndDisabledGroup();
        }


        private void UploadtTextAssetGUI()
        {
            bool isDisable = (EditorConnection.instance.ConnectedPlayers.Count == 0) || m_isWait || m_isPlay || m_isRec || (m_uploadTextAsset == null);

            EditorGUILayout.BeginHorizontal();            
            m_uploadTextAsset = (TextAsset)EditorGUILayout.ObjectField(Styles.EventScriptContens, m_uploadTextAsset, typeof(TextAsset), true);
            EditorGUI.BeginDisabledGroup(isDisable);
            if (GUILayout.Button(Styles.UploadContens,EditorStyles.miniButton,GUILayout.MaxWidth(25)))
            {
                var message = new RemoteMessageAddEventScript();
                message.m_name = m_uploadTextAsset.name;
                message.m_text = m_uploadTextAsset.text;
                SendMessage(message.ToBytes());
                m_isWait = true;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 
        /// </summary>
        private void GUILayoutConnect()
        {
            EditorGUILayout.BeginHorizontal();
            var contents = new GUIContent("Connect To");
            var v2 = EditorStyles.label.CalcSize(contents);
            EditorGUILayout.LabelField(contents, GUILayout.Width(v2.x));
            if (m_connectionState != null)
            {
#if UNITY_2020_1_OR_NEWER
                ConnectionGUILayout.ConnectionTargetSelectionDropdown(m_connectionState, EditorStyles.toolbarDropDown);
#else
                ConnectionGUILayout.AttachToPlayerDropdown(m_connectionState, EditorStyles.toolbarDropDown);
#endif                
            }
            EditorGUILayout.EndHorizontal();

        
        }


        private void GUILayoutConnectInfo()
        {
            var playerCount = EditorConnection.instance.ConnectedPlayers.Count;
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(string.Format("{0} players connected.", playerCount));
            int i = 0;
            foreach (var p in EditorConnection.instance.ConnectedPlayers)
            {
                builder.AppendLine(string.Format("[{0}] - {1} {2}", i++, p.name, p.playerId));
            }
            EditorGUILayout.HelpBox(builder.ToString(), MessageType.Info);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        void SendMessage(byte[] bytes)
        {
            EditorConnection.instance.Send(RemoteMessageBase.kMsgSendEditorToPlayer, bytes);
        }


        /// <summary>
        /// Playerから受信したメッセージのCB
        /// </summary>
        /// <param name="args"></param>
        private void OnMessageEvent(UnityEngine.Networking.PlayerConnection.MessageEventArgs args)
        {
            var messageBase = RemoteMessageBase.Desirialize<RemoteMessageBase>(args.data);
            switch (messageBase.m_messageId)
            {
                case RemoteMessageBase.MessageId.Reflesh:
                    {
                        var message = RemoteMessageBase.Desirialize<RemoteMessageReflesh>(args.data);
                        m_scriptNames = new List<GUIContent>();
                        m_scriptNoValues = new List<int>(message.m_scriptNoValues);
                        for(var i = 0; i < message.m_scriptNames.Length; i++)
                        {
                            m_scriptNames.Add(new GUIContent(message.m_scriptNames[i]));                            
                        }
                        m_isPlay = message.m_isPlay;
                        m_isRec = message.m_isRecord;
                        m_isWait = false;
                    }
                    break;

                case RemoteMessageBase.MessageId.Play:
                    {
                        m_isWait = false;
                        m_isPlay = messageBase.m_isSuccess;
                    }
                    break;

                case RemoteMessageBase.MessageId.Stop:
                    {
                        m_isWait = false;                        
                    }
                    break;

                case RemoteMessageBase.MessageId.AddEventScript:
                    {
                        var message = RemoteMessageAddEventScript.Desirialize<RemoteMessageAddEventScript>(args.data);

                        m_scriptNames = new List<GUIContent>();
                        m_scriptNoValues = new List<int>(message.m_scriptNoValues);
                        for (var i = 0; i < message.m_scriptNames.Length; i++)
                        {
                            m_scriptNames.Add(new GUIContent(message.m_scriptNames[i]));
                        }
                        m_scriptIdx = message.m_setIndex;

                        m_isWait = false;
                    }
                    break;

                case RemoteMessageBase.MessageId.PlayFinish:
                    {
                        m_isPlay = false;
                    }
                    break;

                case RemoteMessageBase.MessageId.Record:
                    {
                        m_isRec = messageBase.m_isSuccess;
                        m_isWait = false;
                    }
                    break;

                case RemoteMessageBase.MessageId.StopRecord:
                    {
                        var message = RemoteMessageStopRecord.Desirialize<RemoteMessageStopRecord>(args.data);
                        var path = EditorUtility.SaveFilePanel(
                                    "Save Event Script Data as TXT",
                                    "",
                                    "",
                                    "txt");
                        if(path.Length != 0)
                        {
                            using (StreamWriter sw = new StreamWriter(path))
                            {
                                sw.Write(message.m_text);
                                sw.Close();
                            }
                        }
                        m_isWait = false;
                    }
                    break;
            }
        }
    }
}