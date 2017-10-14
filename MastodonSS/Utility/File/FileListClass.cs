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
        Queue<FileClass> _queue = new Queue<FileClass>();
        string _subDir = "";

        /// <summary>
        /// コンストラクタ（空）
        /// </summary>
        public FileListClass()
        {

        }

        /// <summary>
        /// ファイル名が決まっている場合、キューに追加
        /// </summary>
        /// <param name="fileName"></param>
        public FileListClass(string fileName)
        {
            _queue.Enqueue(new FileClass(_subDir, fileName));
        }

        /// <summary>
        /// ファイル名が決まっている場合、キューに追加
        /// </summary>
        /// <param name="fileName"></param>
        public FileListClass(string subDir, string fileName)
        {
            _subDir = subDir;
        }

        /// <summary>
        /// キューの残数
        /// </summary>
        public int Count => _queue.Count;

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
            _queue.Enqueue(new FileClass(_subDir, fileName));
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

            if (_queue.Count == 0)
            {
                fn = new FileClass(_subDir, fileName);   // インスタンス化
                fn.CreateText(content);                  // テキストの作成
                _queue.Enqueue(fn);                      // キューに追加
            }
            else
            {
                // 最新ファイルとの比較準備
                fn = new FileClass(_subDir, fileName);
                FileClass before = _queue.Peek();

                // 最新ファイルとの比較（ハッシュ比較）
                if (before.Compare(content) == false)
                {
                    before = _queue.Dequeue();      // 古いファイルをキューから出す
                    if (Remove(before) == false)    // 古いファイルを削除
                    {
                        _queue.Enqueue(before);     // 古いファイルが削除出来ない場合、キューに戻す
                    }

                    fn.CreateText(content);         // テキストの作成
                    _queue.Enqueue(fn);             // キューに追加
                }
            }
        }

        /// <summary>
        /// キューへ追加（FileClass）
        /// </summary>
        /// <param name="item"></param>
        public void Add(FileClass item)
        {
            _queue.Enqueue(item);
        }

        /// <summary>
        /// キューを空にする
        /// </summary>
        public void Clear()
        {
            foreach (FileClass fc in _queue)
            {
                Remove(fc);
            }
            _queue.Clear();
        }

        /// <summary>
        /// キューにファイルがあるか確認（ファイル名）
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool Contains(string fileName)
        {
            foreach(FileClass fc in _queue)
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
            if (item.Extension.Contains(".bak"))
            {
                if(Contains(item.FileName))
                {
                    item.Delete();
                    item.Dispose();
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        #region 使用用途の無いメソッド
        public void CopyTo(FileClass[] array, int arrayIndex)
        {
            _queue.CopyTo(array, arrayIndex);
        }

        public IEnumerator<FileClass> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }

}
