using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace Utj.UnityBotKun
{
    /// <summary>
    /// TextAssetをStreamReaderライクに扱うクラス
    /// Programed by Katsumasa.Kimura
    /// </summary>
    public class TextAssetReader
    {
        List<string> lines;

        /// <summary>
        /// ストリーム内の現在位置
        /// </summary>
        public long Position
        {
            get;
            set;            
        }


        public bool EndOfStream
        {
            get
            {
                if (lines != null)
                {
                    if (Position >= lines.Count)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
        }


        /// <summary>
        /// 指定したTextAssetから読み取るTextAssetReaderクラスの新しいインスタンスを初期化します
        /// </summary>
        /// <param name="textAsset"></param>
        public TextAssetReader(TextAsset textAsset)
        {
            Position = 0;
            lines = new List<string>();

            using (var ms = new MemoryStream(textAsset.bytes))
            {
                using (var sr = new StreamReader(ms))
                {                    
                    while (!sr.EndOfStream)
                    {
                        lines.Add(sr.ReadLine());
                    }
                }
            }
        }


        /// <summary>
        /// 現在の文字列から 1 行分の文字を読み取り、そのデータを文字列として返します。
        /// </summary>
        /// <returns>現在の文字列の次の行。文字列の末尾に到達した場合は null。</returns>
        public string ReadLine()
        {
            if (EndOfStream)
            {
                return null;
            }
            else
            {
                if(lines == null)
                {
                    throw new System.ObjectDisposedException(this.GetType().FullName);
                }
                return lines[(int)Position++];
            }            
        }


        /// <summary>
        /// TextAssetReaderを閉じます
        /// </summary>
        public void Close()
        {            
            lines = null;
        }


        /// <summary>
        /// リーダーや文字の読み取り元の状態を変更せずに、次の文字を読み取ります。 リーダーから実際に文字を読み取らずに次の文字を返します。
        /// </summary>
        /// <returns>読み取り対象の次の文字を表す整数。使用できる文字がないか、リーダーがシークをサポートしていない場合は -1。</returns>
        public int Peek()
        {
            if(lines == null)
            {
                throw new System.ObjectDisposedException(this.GetType().FullName);
            }
            else if(Position < lines.Count)
            {
                var line = lines[(int)Position];
                return line[0];
            }
            return -1;
        }



        public long Seek(long offset,System.IO.SeekOrigin loc)
        {
            if (lines != null)
            {
                switch (loc)
                {
                    case SeekOrigin.Begin:
                        Position = offset;
                        break;
                    case SeekOrigin.Current:
                        Position += offset;
                        break;
                    case SeekOrigin.End:             
                        Position = lines.Count + offset;             
                        break;
                }
                Position = System.Math.Min(Position, lines.Count);
                Position = System.Math.Max(Position, 0);
            }
            return Position;
        }
    }
}