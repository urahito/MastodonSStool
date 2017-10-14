using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MastodonSS.Utility.File
{
    public class FileClass : IDisposable
    {        
        FileInfo fi;
        private string _dirPath;
        private string _filepath;
        private string _hash;

        public FileClass(string subDir, string fileName)
        {
            _dirPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _dirPath = Path.Combine(_dirPath, subDir);
            _filepath = Path.Combine(_dirPath, fileName);
            fi = new FileInfo(_filepath);
            _hash = "";
        }

        public void Dispose()
        {
            fi = null;
            return;
        }

        public bool Exists => fi.Exists;
        public bool Contains(string fileName) => _filepath.Contains(fileName);
        public string DirPath => fi.DirectoryName;
        public string FileName => fi.Name;
        public string Extension => fi.Extension;


        #region 基本動作
        public bool Compare(string content)
        {
            string thisContent = "";
            string newHash = "";

            if(_hash.Length == 0)
            {
                // 既存のテキストを読み込み
                using (StreamReader sr = fi.OpenText())
                {
                    thisContent = sr.ReadToEnd();
                    _hash = GetHash(thisContent);
                }
            }

            newHash = GetHash(content);

            return (_hash.CompareTo(newHash) == 0);
        }

        private string GetHash(string content)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            byte[] bArray = md5.ComputeHash(Encoding.UTF8.GetBytes(content));
            StringBuilder sb = new StringBuilder();

            for(int idx = 0; idx < bArray.Length; idx++)
            {
                sb.Append(bArray[idx].ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// ファイルの存在を確認してからDelete
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            if (!fi.Exists)
            {
                return true;
            }

            try
            {
                fi.Delete();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 新規ファイルの作成
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool CreateText(string content)
        {
            try
            {
                // 新規のテキストへ書き込み
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.Write(content);
                    sw.Flush();
                }
            }
            catch
            {
                return false;
            }

            _hash = GetHash(content);
            return true;
        }

        /// <summary>
        /// 既存ファイルへの追記
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool AppendText(string content)
        {
            StringBuilder sb = new StringBuilder();

            try
            {        
                // 既存のテキストを読み込み
                using (StreamReader sr = fi.OpenText())
                {
                    sb.AppendLine(sr.ReadToEnd());
                    sb.Append(content);
                }

                // 既存のテキストへ書き込み
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.Write(sb.ToString());
                    sw.Flush();
                }
            }
            catch
            {
                return false;
            }

            _hash = GetHash(content);
            return true;
        }
        #endregion
    }
}
