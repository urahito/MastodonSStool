using MastodonSS.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastodonSS.Utility.File
{
    class BackupClass
    {
        FileListClass fList;

        public BackupClass()
        {
            
        }

        /// <summary>
        /// 自動バックアップ
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool AutoBackup(string content)
        {
            string strNowTime = DateTime.Now.ToString("yyyyMMddhhmmss");
            string fileName = string.Format("{0}.bak", strNowTime);

            if (fList == null)
            {
                fList = new FileListClass("MastodonSS_bak", Settings.Default.LeftFile);
            }

            try
            {
                fList.Add(fileName, content);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 指定ファイル削除
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool Delete(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);

            if (fList == null)
            {
                fList = new FileListClass("\\MastodonSS_bak", 0);
            }

            try
            {
                fList.Remove(fi.Name);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 全バックアップ削除
        /// </summary>
        /// <returns></returns>
        public bool AllDelete()
        {
            if (fList == null)
            {
                return false;
            }

            try
            {
                fList.Clear();
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
