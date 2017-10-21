using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastodonSS.Utility.File
{
    public class FileListClass : ICollection<FileClass>
    {
        LinkedList<FileClass> _list = new LinkedList<FileClass>();
        string _subDir = "";
        private int Capacity;

        /// <summary>
        /// コンストラクタ（空）
        /// </summary>
        public FileListClass(int intCapacity)
        {
            Capacity = intCapacity;
        }

        /// <summary>
        /// ファイル名が決まっている場合、キューに追加
        /// </summary>
        /// <param name="fileName"></param>
        public FileListClass(string subDir, int intCapacity)
        {
            _subDir = subDir;
            Capacity = intCapacity;
        }

        /// <summary>
        /// キューの残数
        /// </summary>
        public int Count => _list.Count;

        /// <summary>
        /// 読み取り専用フラグ
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// キューへ追加（ファイル名のみ）
        /// </summary>
        /// <param name="fileName"></param>
        public void Add(string fileName)
        {
            _list.AddLast(new FileClass(_subDir, fileName));
        }

        /// <summary>
        /// キューへ追加（テキストも追加）
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        public void Add(string fileName, string content)
        {
            FileClass fn;

            // 空の文章はファイルの管理のみ
            if (content.Length == 0)
            {
                Add(fileName);
                return;
            }

            if (_list.Count == 0)
            {
                fn = new FileClass(_subDir, fileName);   // インスタンス化
                fn.CreateText(content);                  // テキストの作成
                _list.AddLast(fn);                      // キューに追加
            }
            else
            {
                // 最新ファイルとの比較準備
                fn = new FileClass(_subDir, fileName);
                FileClass before = _list.Last();

                // 最新ファイルとの比較（ハッシュ比較）
                if (before.Compare(content) == true)    // 最新ファイルと現在のテキストが同じ
                {
                    before = _list.Last();      
                    if (Remove(before) == true)         // 最新ファイルを削除
                    {
                        _list.RemoveLast();             // 新しい名前で保存するためリストから削除
                    }                    
                }

                fn.CreateText(content);         // テキストの作成
                _list.AddLast(fn);              // リストに追加（リネーム）

                // 最新数世代までの保存（世代数より古いものは削除）
                while (_list.Count > Capacity)
                {
                    FileClass fc = _list.First();
                    if (Remove(fc) == true)       // 古いファイルを削除出来たとき
                    {
                        _list.RemoveFirst();      // 古いファイルをリストから削除する
                    }
                }
            }
        }

        /// <summary>
        /// キューへ追加（FileClass）
        /// </summary>
        /// <param name="item"></param>
        public void Add(FileClass item)
        {
            _list.AddLast(item);
        }

        /// <summary>
        /// キューを空にする
        /// </summary>
        public void Clear()
        {
            foreach (FileClass fc in _list)
            {
                Remove(fc);
            }
            _list.Clear();
        }

        /// <summary>
        /// キューにファイルがあるか確認（ファイル名）
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool Contains(string fileName)
        {
            foreach(FileClass fc in _list)
            {
                if(fc.Contains(fileName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// キューにファイルがあるか確認（FileClass）
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(FileClass item)
        {
            return Contains(item.FileName);
        }

        /// <summary>
        /// 指定ファイルを削除（ファイル名）
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool Remove(string fileName)
        {
            return Remove(new FileClass(_subDir, fileName));
        }

        /// <summary>
        /// 指定ファイルを削除（FileClass）
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(FileClass item)
        {
            bool result = false;

            if (item.Extension.Contains(".bak"))
            {
                if(item.Exists)
                {
                    result = item.Delete();
                    item.Dispose();
                }
            }
            else
            {
                return false;
            }

            return result;
        }

        #region 使用用途の無いメソッド
        public void CopyTo(FileClass[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<FileClass> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }

}
