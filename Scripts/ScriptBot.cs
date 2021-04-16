using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


namespace Utj
{
    /// <summary>
    /// イベントスクリプト実行システム
    /// Programed by Katsumasa Kimura
    /// </summary>
    public class ScriptBot : MonoBehaviour
    {
        //readonly string scriptRoot = Path.Combine(Application.streamingAssetsPath, "UnityAutoTesterKun");

        /// <summary>
        /// コマンドファンクション
        /// </summary>
        /// <param name="args">引数</param>
        /// <returns>true:続行 fale:中断</returns>
        delegate bool CommandFunction(string[] args);

        
        /// <summary>
        /// コマンド毎に割り当てられたファンクションのDictionary
        /// </summary>
        Dictionary<string, CommandFunction> m_commandFunctons;

        
        /// <summary>
        /// Scriptの配列
        /// </summary>
        [SerializeField,Tooltip("実行するスクリプトの配列")] List<TextAsset> m_scripts;

        
        /// <summary>
        /// TextAssetReader
        /// </summary>
        TextAssetReader m_textAssetReader;

        /// <summary>
        /// 現在選択されているスクリプト番号(Debug用)
        /// </summary>
        [SerializeField,HideInInspector]
        int m_currentScriptIndex;

        /// <summary>
        /// スクリプトの実行位置(Debug用)
        /// </summary>
        public long currentPosition
        {
            get
            {
                if (m_textAssetReader == null)
                {
                    return 0;
                }
                else
                {
                    return m_textAssetReader.Position;
                }
            }
        }


        /// <summary>
        /// スクリプトの最大位置(デバック用)
        /// </summary>
        public long maxPosition
        {
            get;
            private set;            
        }


        /// <summary>
        /// ジャンプ先のオフセットを保管場所
        /// </summary>
        Dictionary<string, long> m_labelOfsts;


        /// <summary>
        /// ユーザー変数の保管場所
        /// key:変数名
        /// value: 変数値
        /// </summary>
        Dictionary<string, int> m_variableInts;


        /// <summary>
        /// ユーザー変数の保管場所
        /// key:変数名
        /// value: 変数値
        /// </summary>
        Dictionary<string, float> m_variableFloats;


        /// <summary>
        /// string型変数のコレクション
        /// </summary>
        Dictionary<string, string> m_variableStrings;


        /// <summary>
        /// スクリプトの実行を待機するフレーム数
        /// </summary>
        int m_waitFrame;


        /// <summary>
        /// スクリプトの実行を待機する時間
        /// </summary>
        float m_waitTime;


        /// <summary>
        /// InputBot.instance.isOverrideInputの値の退避先
        /// </summary>
        bool m_overrideInputBackup;


        /// <summary>
        /// 実行を停止する
        /// </summary>
        bool m_isStop;

        [SerializeField,HideInInspector]
        bool m_isPlay;

        /// <summary>
        /// スクリプトが再生中であるか否か
        /// </summary>
        public bool isPlay {
            get { return m_isPlay; }
            private set { m_isPlay = value; }
        }
        
        
        /// <summary>
        /// エラーが発生したか否か
        /// </summary>
        public bool isError
        {
            get;
            private set;
        }


        /// <summary>
        /// 自動的に再生を実行するか否か
        /// </summary>
        [SerializeField, Tooltip("Auto Playを実行する")]
        bool m_isAutoPlay;
        

        public bool isAutoPlay
        {
            get { return m_isAutoPlay; }
            set { m_isAutoPlay = value; }
        }


        /// <summary>
        /// イベントスクリプトのコレクション
        /// </summary>
        public List<TextAsset> scripts
        {
            get
            {
                if(m_scripts == null)
                {
                    m_scripts = new List<TextAsset>();
                }
                return m_scripts;
            }

            private set
            {
                m_scripts = value;
            }
        }


        /// <summary>
        /// ラベルオフセットのコレクション
        /// </summary>
        Dictionary<string, long> labelOfsts
        {
            get {
                if(m_labelOfsts == null)
                {                                
                    m_labelOfsts = new Dictionary<string, long>();                                        
                }
                return m_labelOfsts;
            }

            set
            {
                m_labelOfsts = value;
            }
        }


        /// <summary>
        /// int型変数のコレクション
        /// </summary>
        Dictionary<string, int> variableInts
        {
            get
            {
                if (m_variableInts == null)
                {
                    m_variableInts = new Dictionary<string, int>();
                }
                return m_variableInts;
            }

            set
            {
                m_variableInts = value;
            }
        }


        /// <summary>
        /// float型変数のコレクション
        /// </summary>
        Dictionary<string, float> variableFloats
        {
            get
            {
                if (m_variableFloats == null)
                {
                    m_variableFloats = new Dictionary<string, float>();
                }
                return m_variableFloats;
            }

            set
            {
                m_variableFloats = value;
            }
        }


        /// <summary>
        /// string型の変数のコレクション
        /// </summary>
        Dictionary<string,string> variableStrings
        {
            get
            {
                if(m_variableStrings == null)
                {
                    m_variableStrings = new Dictionary<string, string>();
                }
                return m_variableStrings;
            }

            set
            {
                m_variableStrings = value;
            }
        }


        /// <summary>
        /// ScriptBotのインスタンス
        /// </summary>
        public static ScriptBot instance
        {
            get;
            private set;
        }


        /// <summary>
        /// スクリプトを設定する
        /// </summary>
        /// <param name="textAsset">script</param>
        /// <returns>インデックス</returns>
        public int SetScript(TextAsset textAsset)
        {
            if(scripts == null)
            {
                scripts = new List<TextAsset>();
            }
            scripts.Add(textAsset);
            return scripts.Count - 1;
        }
        

        /// <summary>
        /// スクリプトを再生する
        /// </summary>
        public void Play(int index = 0)
        {
            m_textAssetReader = new TextAssetReader(scripts[index]);
            m_textAssetReader.Position = 0;
            m_waitFrame = 0;
            isPlay = true;
            m_isStop= false;
            isError = false;
            m_overrideInputBackup = InputBot.instance.isOverrideInput;
            InputBot.instance.isOverrideInput = true;



            PreProcessor();            
        }


        /// <summary>
        /// スクリプトの再生を停止する
        /// </summary>
        public void Stop()
        {
            m_isStop = true;
        }


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                m_labelOfsts = new Dictionary<string, long>();
                m_variableInts = new Dictionary<string, int>();
                m_variableFloats = new Dictionary<string, float>();
                m_variableStrings = new Dictionary<string, string>();
                isError = false;
                isPlay = false;


                // CommandFunctionのテーブルを作成する
                m_commandFunctons = new Dictionary<string, CommandFunction>()
                {
                    {"wait", CommandWait},
                    {"goto", CommandGoto},
                    {"print",CommandPrint},
                    {"set",CommandSet },
                    {"add",CommandAdd },
                    {"sub",CommandSub },
                    {"mul",CommandMul },
                    {"div",CommandDiv },
                    {"ifeq",CommandIFEQ },
                    {"ifno", CommandIFNOTEQ},
                    {"ifls", CommandIFLS},
                    {"ifle",CommandIFLE },
                    {"ifgr", CommandIFGR},
                    {"ifge", CommandIFGE},
                    {"touch",CommandTouch },
                    {"button",CommandButton },
                    {"axisraw",CommandAxisRaw },
                    {"mousebutton",CommandMouseButton },
                    {"mousepos", CommandMousePosition},
                };
            }
            else
            {
                Destroy(this);
            }
        }


        // Start is called before the first frame update
        IEnumerator Start()
        {
            if (m_isAutoPlay)
            {
                while (true)
                {
                    if (InputBot.instance != null)
                    {                        
                        Play();
                        break;
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.16f);
                    }
                }
            }
        }


        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;                
            }
        }


        // Update is called once per frame
        void Update()
        {                        
            Interpreter();
        }


        /// <summary>
        /// 変数として存在するか否か
        /// </summary>
        /// <param name="name">変数名</param>
        /// <returns>true:存在する</returns>
        bool IsVariable(string name)
        {
            if (variableInts != null && variableInts.ContainsKey(name))
            {
                return true;
            }
            else if(variableFloats != null && variableFloats.ContainsKey(name))
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// int型の取得
        /// </summary>
        /// <param name="name">変数名or数値</param>
        /// <returns>int型の数値</returns>
        int GetInt(string name)
        {
            if (variableInts != null && variableInts.ContainsKey(name))
            {
                return variableInts[name];
            }
            else if (variableFloats != null && variableFloats.ContainsKey(name))
            {
                return (int)variableFloats[name];
            }
            else
            {
                return System.Convert.ToInt32(name);
            }            
        }


        /// <summary>
        /// 少数の取得
        /// </summary>
        /// <param name="name">変数名or数値</param>
        /// <returns></returns>
        float GetFloat(string name)
        {
            if (variableInts != null && variableInts.ContainsKey(name))
            {
                return (float)variableInts[name];
            }
            else if (variableFloats != null && variableFloats.ContainsKey(name))
            {
                return variableFloats[name];
            }
            else
            {
                return System.Convert.ToSingle(name);
            }            
        }
        

        /// <summary>
        /// 文字列を取得する
        /// </summary>
        /// <param name="arg">引数</param>
        /// <returns>文字列</returns>
        string GetString(string arg)
        {
            if (variableStrings.ContainsKey(arg))
            {
                return variableStrings[arg];
            }
            else if(variableInts.ContainsKey(arg)){
                return variableInts[arg].ToString();
            }
            else if (variableFloats.ContainsKey(arg))
            {
                return variableFloats[arg].ToString();
            }
            else if (arg.StartsWith("\""))
            {
                return arg.Trim(new char[] { '"' });
            }
            return arg;
        }


        /// <summary>
        /// 変数の代入 A = B
        /// </summary>
        /// <typeparam name="T">int or float</typeparam>
        /// <param name="name">変数名</param>
        /// <param name="value">値</param>
        void SetValue<T>(string name,T value) where T : IConvertible
        {
            if (variableInts != null && variableInts.ContainsKey(name))
            {
                variableInts[name] = (int)Convert.ChangeType(value,typeof(int));
            }
            else if (variableFloats != null && variableFloats.ContainsKey(name))
            {
                variableFloats[name] = (float)Convert.ChangeType(value, typeof(float));
            }
            else if (variableStrings.ContainsKey(name))
            {
                var word = (string)Convert.ChangeType(value, typeof(string));
                variableStrings[name] = word.Trim(new char[] {'"'});
            }
        }


        /// <summary>
        /// Waitの処理
        /// </summary>
        /// <param name="args[0]">wait</param>
        /// <param name="args[1]">"frame" or "sec"</param>
        /// <param name="args[1]">フレーム数 or 待機時間</param>
        /// <returns></returns>
        bool CommandWait(string[] args)
        {
            if(args.Length == 1)
            {
                m_waitFrame = 0;
            }
            else
            {
                var type = GetString(args[1]);
                if (string.Compare(type, "frame") == 0)
                {
                    m_waitFrame = GetInt(args[2]);
                    m_waitTime = 0f;
                }
                else if (string.Compare(type, "sec") == 0)
                {
                    m_waitTime = GetFloat(args[2]);
                    m_waitFrame = 0;
                }                
            }            
            return false;
        }


        /// <summary>
        /// GOTOコマンド
        /// </summary>
        /// <param name="args">args[0]:goto args[1]:ジャンプ先のラベル</param>
        /// <returns></returns>
        bool CommandGoto(string[] args)
        {
            long ofst;
            if (labelOfsts.TryGetValue(args[1], out ofst))
            {
                m_textAssetReader.Seek(ofst, SeekOrigin.Begin);
            }
            else
            {
                Debug.Log("Invalid Label :" + args);
                isError = true;
                return false;
            }
            return true;
        }


        /// <summary>
        /// コンソールに引数を出力する
        /// </summary>
        /// <param name="args">args[0]:print [args[1...*]:変数名or"任意の文字列"]</param>
        /// <returns></returns>
        bool CommandPrint(string[] args)
        {
            var sb = new StringBuilder();
            for(var i = 1; i < args.Length; i++)
            {                
                sb.AppendFormat("{0} ", GetString(args[i]));
            }
            Debug.Log(sb.ToString());                            
            return true;                        
        }


        /// <summary>
        /// 変数の代入
        /// set 変数名 数値
        /// set 変数名1 変数名2         
        /// </summary>
        /// <param name="args">args[1]代入先の変数名 arg[2]代入元の変数名or数値</param>
        /// <returns></returns>
        bool CommandSet(string[] args)
        {
            if (variableInts.ContainsKey(args[1]))
            {                
                variableInts[args[1]] = GetInt(args[2]);                
            }
            else if (variableFloats.ContainsKey(args[1]))
            {
                variableFloats[args[1]] = GetFloat(args[2]);                                
            }
            else if (variableStrings.ContainsKey(args[1]))
            {
                variableStrings[args[1]] = GetString(args[2]);
            }
            else
            {
                Debug.Log("Invalid variable name : " + args[1]);
                isError = true;
                return false;
            }
            return true;
        }


        /// <summary>
        /// 加算 A = B + C
        /// </summary>
        /// <param name="args">args[0]:add args[1]:変数名 args[2]:変数名or数値 args[3]:変数名or数値</param>
        /// <returns></returns>
        bool CommandAdd(string[] args)
        {
            if (variableInts.ContainsKey(args[1]))
            {
                variableInts[args[1]] = GetInt(args[2]) + GetInt(args[3]);
            }
            else if (variableFloats.ContainsKey(args[1]))
            {
                variableFloats[args[1]] = GetFloat(args[2]) + GetFloat(args[3]);
            }
            else if (variableStrings.ContainsKey(args[1]))
            {
                variableStrings[args[1]] = GetString(args[2]) + GetString(args[3]);
            }
            else
            {
                isError = true;
                Debug.Log("Invalid args in add "+ args);
            }
            return true;
        }


        /// <summary>
        /// 減算 A = B - C
        /// </summary>
        /// <param name="args">args[0]:sub args[1]:変数名 args[2]:変数名or数値 args[3]:変数名or数値</param>
        /// <returns></returns>
        bool CommandSub(string[] args)
        {
            if (variableInts.ContainsKey(args[1]))
            {
                variableInts[args[1]] = GetInt(args[2]) - GetInt(args[3]);
            }
            else if (variableFloats.ContainsKey(args[1]))
            {
                variableFloats[args[1]] -= GetFloat(args[2]) - GetFloat(args[3]);
            }
            return true;
        }


        /// <summary>
        /// 乗算 A = B * C
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        bool CommandMul(string[] args)
        {
            if (variableInts.ContainsKey(args[1]))
            {
                variableInts[args[1]] = GetInt(args[2]) * GetInt(args[3]);
            }
            else if (variableFloats.ContainsKey(args[1]))
            {
                variableFloats[args[1]] = GetFloat(args[2]) * GetInt(args[3]);
            }
            else
            {
                isError = true;
                Debug.Log("Invalid args in Mul " + args);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 除算 A = B / C
        /// </summary>
        /// <param name="args">args[0]:div args[1]:変数名 args[2]:変数名or数値 args[3]:変数名or数値</param>
        /// <returns></returns>
        bool CommandDiv(string[] args)
        {
            if (variableInts.ContainsKey(args[1]))
            {
                variableInts[args[1]] = GetInt(args[2]) / GetInt(args[3]);
            }
            else if (variableFloats.ContainsKey(args[1]))
            {
                variableFloats[args[1]] = GetFloat(args[2]) / GetFloat(args[3]);
            }
            else
            {
                isError = true;
                Debug.Log("Invalid args in div " + args);
                return false;
            }
            return true;
        }


        /// <summary>
        /// if A == B then xxx
        /// </summary>
        /// <param name="args">args[0]:ifeq args[1]:変数名 args[2]:変数名or数値</param>
        /// <returns></returns>
        bool CommandIFEQ(string[] args)
        {
            bool result = false;
            if (variableInts.ContainsKey(args[1]))
            {
                if (variableInts[args[1]] == GetInt(args[2]))
                {
                    result = true;
                }
            }
            else if (variableFloats.ContainsKey(args[1]))
            {
                if (variableFloats[args[1]] == GetFloat(args[2]))
                {
                    result = true;
                }
            }

            if (result)
            {
                var dsts = new string[args.Length - 3];
                Array.Copy(args, 3, dsts, 0, dsts.Length);
                return SyntaxAnalysis(dsts);
            }
            return true;
        }


        /// <summary>
        /// if A != B then xxxx
        /// </summary>
        /// <param name="args">args[0]:ifno args[1]:変数名 args[2]:変数名 or 数値</param>
        /// <returns></returns>
        bool CommandIFNOTEQ(string[] args)
        {
            bool result = false;
            if (variableInts.ContainsKey(args[1]))
            {
                if (variableInts[args[1]] != GetInt(args[2]))
                {
                    result = true;
                }
            }
            else if (variableFloats.ContainsKey(args[1]))
            {
                if (variableFloats[args[1]] != GetFloat(args[2]))
                {
                    result = true;
                }
            }

            if (result)
            {
                var dsts = new string[args.Length - 3];
                Array.Copy(args, 3, dsts, 0, dsts.Length);
                return SyntaxAnalysis(dsts);
            }
            return true;
        }


        /// <summary>
        /// if A < B then...
        /// </summary>
        /// <param name="args">args[0]:ifls args[1]:変数名 args[2]:変数名or数値</param>
        /// <returns></returns>
        bool CommandIFLS(string[] args)
        {
            bool result = false;
            if (variableInts.ContainsKey(args[1]))
            {
                if (variableInts[args[1]] < GetInt(args[2]))
                {
                    result = true;
                }
            }
            else if (variableFloats.ContainsKey(args[1]))
            {
                if (variableFloats[args[1]] < GetFloat(args[2]))
                {
                    result = true;
                }
            }

            if (result)
            {
                var dsts = new string[args.Length - 3];
                Array.Copy(args, 3, dsts, 0, dsts.Length);
                return SyntaxAnalysis(dsts);
            }
            return true;
        }


        /// <summary>
        /// if A <= B then xxx
        /// </summary>
        /// <param name="args">args[0]:ifle args[1]:変数名 args[2]:変数名or数値</param>
        /// <returns></returns>
        bool CommandIFLE(string[] args)
        {
            bool result = false;
            if (variableInts.ContainsKey(args[1]))
            {
                if (variableInts[args[1]] <= GetInt(args[2]))
                {
                    result = true;
                }
            }
            else if (variableFloats.ContainsKey(args[1]))
            {
                if (variableFloats[args[1]] <= GetFloat(args[2]))
                {
                    result = true;
                }
            }

            if (result)
            {
                var dsts = new string[args.Length - 3];
                Array.Copy(args, 3, dsts, 0, dsts.Length);
                return SyntaxAnalysis(dsts);
            }
            return true;
        }


        /// <summary>
        /// if A > B then xxx
        /// </summary>
        /// <param name="args">args[0]:ifgr args[1]:変数名 args[2]:変数名or数値</param>
        /// <returns></returns>
        bool CommandIFGR(string[] args)
        {
            bool result = false;
            if (variableInts.ContainsKey(args[1]))
            {
                if (variableInts[args[1]] > GetInt(args[2]))
                {
                    result = true;
                }
            }
            else if (variableFloats.ContainsKey(args[1]))
            {
                if (variableFloats[args[1]] > GetFloat(args[2]))
                {
                    result = true;
                }
            }

            if (result)
            {
                var dsts = new string[args.Length - 3];
                Array.Copy(args, 3, dsts, 0, dsts.Length);
                return SyntaxAnalysis(dsts);
            }
            return true;
        }


        /// <summary>
        /// if A >= B then xxxx.
        /// </summary>
        /// <param name="args">args[0]:ifge args[1]:変数名 args[2]:数値or変数名</param>
        /// <returns></returns>
        bool CommandIFGE(string[] args)
        {
            bool result = false;
            if (variableInts.ContainsKey(args[1]))
            {
                if (variableInts[args[1]] >= GetInt(args[2]))
                {
                    result = true;
                }
            }
            else if (variableFloats.ContainsKey(args[1]))
            {
                if (variableFloats[args[1]] >= GetFloat(args[2]))
                {
                    result = true;
                }
            }

            if (result)
            {
                var dsts = new string[args.Length - 3];
                Array.Copy(args, 3, dsts, 0, dsts.Length);
                return SyntaxAnalysis(dsts);
            }
            return true;
        }


        /// <summary>
        /// Touchの入力
        /// </summary>
        /// <param name="args[0]">touch</param>
        /// <param name="args[1]">begin,move,ended</param>
        /// <param name="args[2]">fingerId</param>
        /// <param name="args[3]">"オブジェクト名" or x座標</param>
        /// <param name="args[4]">y座標</param>
        /// <returns></returns>
        bool CommandTouch(string[] args)
        {
            var type = GetString(args[1]);
            var fingerId = GetInt(args[2]);
            // begin
            if (string.Compare(type, "begin") == 0)
            {
                if (args[3].StartsWith("\""))
                {
                    var name = args[3].Trim(new char[] { '"' });
                    InputBot.instance.SetTouchBegin(fingerId, name);
                }
                else
                {
                    var position = new Vector2(GetFloat(args[3]), GetFloat(args[4]));
                    if (args.Length == 5)
                    {
                        InputBot.instance.SetTouchBegin(fingerId, position);
                    }
                    else if(args.Length == 6)
                    {
                        InputBot.instance.SetTouchBegin(fingerId, position,
                            GetFloat(args[5])
                            );
                    }
                    else if(args.Length == 7)
                    {
                        InputBot.instance.SetTouchBegin(fingerId, position,
                            GetFloat(args[5]),
                            GetFloat(args[6])
                            );
                    }
                    else if(args.Length == 8)
                    {
                        InputBot.instance.SetTouchBegin(fingerId, position, 
                            GetFloat(args[5]), 
                            GetFloat(args[6]),
                            GetFloat(args[7])
                            );
                    }
                    else if(args.Length == 9)
                    {
                        InputBot.instance.SetTouchBegin(fingerId, position,
                            GetFloat(args[5]),
                            GetFloat(args[6]),
                            GetFloat(args[7]),
                            GetFloat(args[8])
                            );
                    }
                    else if(args.Length == 10)
                    {
                        InputBot.instance.SetTouchBegin(fingerId, position,
                            GetFloat(args[5]),
                            GetFloat(args[6]),
                            GetFloat(args[7]),
                            GetFloat(args[8]),
                            (TouchType)GetInt(args[9])
                            );
                    }
                    else
                    {
                        InputBot.instance.SetTouchBegin(fingerId, position,
                            GetFloat(args[5]),
                            GetFloat(args[6]),
                            GetFloat(args[7]),
                            GetFloat(args[8]),
                            (TouchType)GetInt(args[9]),
                            GetInt(args[10])
                            );
                    }

                }
            }
            // Move
            else if(string.Compare(type,"move") == 0)
            {
                var position = new Vector2(GetFloat(args[3]), GetFloat(args[4]));
                InputBot.instance.SetTouchMove(fingerId,position);
                if (args.Length == 5)
                {
                    InputBot.instance.SetTouchMove(fingerId, position);
                }
                else if (args.Length == 6)
                {
                    InputBot.instance.SetTouchMove(fingerId, position,
                        GetFloat(args[5])
                        );
                }
                else if (args.Length == 7)
                {
                    InputBot.instance.SetTouchMove(fingerId, position,
                        GetFloat(args[5]),
                        GetFloat(args[6])
                        );
                }
                else if (args.Length == 8)
                {
                    InputBot.instance.SetTouchMove(fingerId, position,
                        GetFloat(args[5]),
                        GetFloat(args[6]),
                        GetFloat(args[7])
                        );
                }
            }
            // Ended
            else if (string.Compare(type,"ended") == 0)
            {
                InputBot.instance.SetTouchEnded(fingerId);
            }
            return true;
        }


        /// <summary>
        /// Buttonの入力
        /// </summary>
        /// <param name="args[0]">button</param>
        /// <param name="args[1]">down or up</param>
        /// <param name="args[2]">"buttonName"</param>
        /// <returns></returns>
        bool CommandButton(string[] args)
        {
            var type = GetString(args[1]);
            var name = GetString(args[2]);
            bool isDown = string.Compare(type,"down") == 0;
            InputBot.instance.SetButtonDown(name, isDown);
            return true;
        }


        /// <summary>
        /// Axisの入力
        /// </summary>
        /// <param name="args[0]">axisraw</param>
        /// <param name="args[1]">"軸の名前"</param>
        /// <param name="args[2]">"値"</param>
        /// <returns></returns>
        bool CommandAxisRaw(string[] args)
        {
            string name = GetString(args[1]);
            InputBot.instance.SetAxisRaw(name, GetFloat(args[2]));
            return true;
        }


        /// <summary>
        /// Mouseボタンの入力
        /// </summary>
        /// <param name="args[0]">mousebutton</param>        
        /// <param name="args[1]">down or up</param>
        /// <param name="args[2]">ボタン番号</param>
        /// <returns></returns>
        bool CommandMouseButton(string[] args)
        {
            var type = GetString(args[1]);
            if (string.Compare(type,"down") == 0)
            {
                InputBot.instance.SetMouseButtonDown(GetInt(args[2]));
            }
            else
            {
                InputBot.instance.SetMouseButtonUp(GetInt(args[2]));
            }
            return true;
        }


        /// <summary>
        /// Mouseの座標を入力
        /// </summary>
        /// <param name="args[0]">mousepos</param>
        /// <param name="args[1]">x</param>
        /// <param name="args[2]">y</param>
        /// <returns></returns>
        bool CommandMousePosition(string[] args)
        {
            var position = new Vector2(GetFloat(args[1]), GetFloat(args[2]));
            InputBot.instance.SetMousePosition(position);
            return true;
        }


        /// <summary>
        /// 構文解析
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        bool SyntaxAnalysis(string[] args)
        {
            CommandFunction function;

            if(m_commandFunctons.TryGetValue(args[0], out function))
            {
                return function(args);
            }
            else
            {
                Debug.Log("Invalid Command Name "+ args[0]);
                isError = true;
                return false;
            }            
        }


        /// <summary>
        /// スクリプトの実行
        /// </summary>
        void Interpreter()
        {            
            if (!isPlay || isError)
            {
                return;
            }            
            
            if ((m_isStop) ||(m_textAssetReader == null) || (m_textAssetReader.EndOfStream))
            {
                isPlay = false;
                InputBot.instance.isOverrideInput = m_overrideInputBackup;
                return;
            }

            m_waitFrame--;
            m_waitTime -= Time.deltaTime;
            if (m_waitFrame > 0 || (m_waitTime > 0f))
            {
                return;
            }

            while (!m_textAssetReader.EndOfStream)
            {
                var line = m_textAssetReader.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                
                // ラベルはスキップ
                if (line.StartsWith("#"))
                {
                    continue;
                }
                
                // コメントはスキップ
                if (line.StartsWith(":"))
                {
                    continue;
                }
                
                // 変数はスキップ
                if (line.StartsWith("int") || line.StartsWith("float") || line.StartsWith("string"))
                {
                    continue;
                }

                // 字句解析            
                //var args = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string[] args;
                LexicalAnalysis(line, out args);

                // 構文解析
                if (SyntaxAnalysis(args) == false) {
                    break;
                }
            }
        }


        /// <summary>
        /// 字句解析
        /// </summary>
        /// <param name="line">１行分の文字列</param>
        /// <param name="args">字句に分割された文字列の配列</param>
        void LexicalAnalysis(string line,out string[] args)
        {
#if true
            // スクリプト内で文字列が使えるようになったので、字句解析が複雑になった為、
            // 単純にTrimとSplitを行うという訳には行かなくなった
            StringBuilder sb = new StringBuilder();
            var list = new List<string>();
            int step = 0;


            foreach(var ch in line)
            {
                switch (step)
                {                    
                    case 0:
                        // 冒頭の処理
                        {
                            // 冒頭の空白文字を削除
                            if (ch == ' ')
                            {
                                continue;
                            }
                            // ダブルクォーテーション
                            else if (ch == '"')
                            {
                                sb.Clear();
                                sb.Append(ch);
                                step = 1;
                            }
                            // 通常の文字
                            else
                            {
                                sb.Clear();
                                sb.Append(ch);
                                step = 2;
                            }
                        }
                        break;

                    case 1:
                        // ダブルクォーテーションで括られてた部分の処理
                        {
                            sb.Append(ch);
                            if(ch == '"')
                            {                                
                                list.Add(sb.ToString());
                                sb.Clear();
                                step = 0;
                            }
                        }
                        break;

                    case 2:
                        // 通常の字句として処理
                        {
                            if (ch == ' ')
                            {
                                list.Add(sb.ToString());
                                sb.Clear();
                                step = 0;
                            }
                            else
                            {
                                sb.Append(ch);
                            }
                        }
                        break;
                }
            }

            // 最後の字句が空白で無い場合は追加
            var word = sb.ToString();
            if(!string.IsNullOrEmpty(word) && !string.IsNullOrWhiteSpace(word)){
                list.Add(word);
            }
            args = list.ToArray();
#else
            // スペースで分割する
            // var words = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);            
            // args = list.ToArray();
#endif
        }


        /// <summary>
        /// プリプロセス処理
        /// ラベルや変数を処理する
        /// </summary>
        void PreProcessor()
        {
            if (m_textAssetReader != null)
            {
                m_textAssetReader.Seek(0, SeekOrigin.Begin);
                labelOfsts.Clear();
                variableInts.Clear();
                variableFloats.Clear();
                variableStrings.Clear();

                // 予約語
                variableStrings.Add("begin","begin");
                variableStrings.Add("move", "move");
                variableStrings.Add("ended", "ended");
                variableStrings.Add("down", "down");
                variableStrings.Add("jump","Jump");
                variableStrings.Add("fire1", "Fire1");
                variableStrings.Add("fire2", "Fire2");
                variableStrings.Add("fire3", "Fire3");
                variableStrings.Add("horizontal", StandaloneInputModuleOverride.instance.horizontalAxis);
                variableStrings.Add("vertical", StandaloneInputModuleOverride.instance.verticalAxis);
                variableStrings.Add("submit", StandaloneInputModuleOverride.instance.submitButton);
                variableStrings.Add("cancel", StandaloneInputModuleOverride.instance.cancelButton);
                variableStrings.Add("frame", "frame");
                variableStrings.Add("sec", "sec");

                while (!m_textAssetReader.EndOfStream)
                {
                    var line = m_textAssetReader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    if (line.StartsWith("#"))
                    {
                        //var args = line.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                        var args = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        labelOfsts.Add(args[1], m_textAssetReader.Position);
                    }
                    else if (line.StartsWith("int"))
                    {
                        var args = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        int v;
                        if (args.Length >= 3 && int.TryParse(args[2], out v))
                        {
                            variableInts.Add(args[1], v);
                        }
                        else
                        {
                            variableInts.Add(args[1], 0);
                        }
                    }
                    else if (line.StartsWith("float"))
                    {
                        var args = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        float v;
                        if (args.Length >= 3 && float.TryParse(args[2], out v))
                        {
                            variableFloats.Add(args[1], v);
                        }
                        else
                        {
                            variableFloats.Add(args[1], 0);
                        }
                    }
                    else if (line.StartsWith("string"))
                    {
                        string[] args;
                        LexicalAnalysis(line, out args);
                        if(args.Length >= 3)
                        {
                            variableStrings.Add(args[1], args[2].Trim(new char[] { '"' }));
                        }
                    }
                }
                maxPosition = m_textAssetReader.Position;
                m_textAssetReader.Seek(0, SeekOrigin.Begin);
            }
        }
    }    
}