using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastodonSS
{
    /// <summary>
    /// ファイル入出力クラス
    /// </summary>
    class FileUtility
    {
        static List<string> backupList;

        string strToday;
        string strNowTime;

        string fileName;
        string defDirPath;
        string filePath;
        string backupFileName;

        #region コンストラクタ
        /// <summary>
        /// ファイル入出力クラス
        /// </summary>
        public FileUtility()
        {
            getDefault();
            fileName = string.Format("{0}.txt", strToday);
        }

        /// <summary>
        /// ファイル入出力クラス
        /// </summary>
        /// <param name="strTitle">タイトル</param>
        public FileUtility(string strTitle)
        {
            getDefault();
            fileName = string.Format("{0}-{1}.txt", strTitle, strToday);
        }

        /// <summary>
        /// ファイル入出力クラス
        /// </summary>
        /// <param name="strTitle">タイトル</param>
        /// <param name="seqNo">番号</param>
        public FileUtility(string strTitle, int seqNo)
        {
            string seqFormat = Properties.Settings.Default.SequenceFormat;

            getDefault();
            fileName = string.Format("{0}-{1}-{2}.txt", strTitle, seqNo.ToString(seqFormat), strToday);
        }

        /// <summary>
        /// コンストラクタ共通の初期変数を取得
        /// </summary>
        private void getDefault()
        {
            getBackup();

            backupList = new List<string>();
            strToday = DateTime.Today.ToString("yyyyMMdd");
            defDirPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private void getBackup()
        {
            strNowTime = DateTime.Now.ToString("yyyyMMddhhmmss");
            backupFileName = string.Format("{0}.bak", strNowTime);
        }
        #endregion

        #region ファイル保存系統
        /// <summary>
        /// ファイル保存メソッド
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="strException"></param>
        /// <returns></returns>
        public bool SaveFile(StringBuilder sb, out string strException)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = defDirPath;
            sfd.FileName = fileName;
            sfd.Filter = "txtファイル(*.txt)|*.txt";
            sfd.Title = "保存先のファイル名を指定してください";

            if (sfd.ShowDialog() == true)
            {
                filePath = sfd.FileName;
            }
            else
            {
                strException = "ファイルが選択されませんでした。";
                return true;
            }

            try
            {
                File.WriteAllText(filePath, sb.ToString());
            }
            #region catch句
            catch(FileNotFoundException ex)
            {
                strException = "ファイルが見つかりませんでした。";
                return false;
            }
            catch(ObjectDisposedException ex)
            {
                strException = "ファイルの保存に失敗しました。";
                return false;
            }
            catch(NotSupportedException ex)
            {
                strException = "ファイルの保存に失敗しました。";
                return false;
            }
            catch(IOException ex)
            {
                strException = "その他のファイル保存エラーです。\r\n" + ex.Message.ToString();
                return false;
            }
            catch(Exception ex)
            {
                strException = "その他のエラーです。\r\n" + ex.Message.ToString();
                return false;
            }
            #endregion

            strException = "正常に保存しました。";
            return true;
        }

        /// <summary>
        /// ファイルバックアップ
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="strException"></param>
        /// <returns></returns>
        public bool AutoBackup(StringBuilder sb, out string strException)
        {
            getBackup();

            string backDirPath = Path.Combine(defDirPath, "MastodonSS_bak");
            string backFilePath = Path.Combine(backDirPath, backupFileName);
            strException = "";

            try
            {
                if (!Directory.Exists(backDirPath))
                {
                    Directory.CreateDirectory(backDirPath);
                }

                File.WriteAllText(backFilePath, sb.ToString());

                backupList.Add(backFilePath);
            }
            #region catch句
            catch (FileNotFoundException ex)
            {
                strException = "ファイルが見つかりませんでした。";
                return false;
            }
            catch (ObjectDisposedException ex)
            {
                strException = "ファイルの保存に失敗しました。";
                return false;
            }
            catch (NotSupportedException ex)
            {
                strException = "ファイルの保存に失敗しました。";
                return false;
            }
            catch (IOException ex)
            {
                strException = "その他のファイル保存エラーです。\r\n" + ex.Message.ToString();
                return false;
            }
            catch (Exception ex)
            {
                strException = "その他のエラーです。\r\n" + ex.Message.ToString();
                return false;
            }
            #endregion

            return true;
        }

        /// <summary>
        /// （終了時）バックアップの削除
        /// </summary>
        /// <param name="strException"></param>
        /// <returns></returns>
        public bool DeleteBackup(out string strException)
        {
            strException = "";

            try
            {
                foreach (string backFilePath in backupList)
                {
                    if (File.Exists(backFilePath))
                    {
                        File.Delete(backFilePath);
                    }
                }
            }
            #region catch句
            catch (FileNotFoundException ex)
            {
                strException = "ファイルが見つかりませんでした。";
                return false;
            }
            catch (ObjectDisposedException ex)
            {
                strException = "ファイルの保存に失敗しました。";
                return false;
            }
            catch (NotSupportedException ex)
            {
                strException = "ファイルの保存に失敗しました。";
                return false;
            }
            catch (IOException ex)
            {
                strException = "その他のファイル保存エラーです。\r\n" + ex.Message.ToString();
                return false;
            }
            catch (Exception ex)
            {
                strException = "その他のエラーです。\r\n" + ex.Message.ToString();
                return false;
            }
            #endregion

            return true;
        }
        #endregion

        /// <summary>
        /// ファイル読み込み
        /// </summary>
        /// <param name="content"></param>
        /// <param name="strException"></param>
        /// <returns></returns>
        public bool ReadText(out string content, out string strException)
        {
            string OpenFilePath = "";
            content = "";
            strException = "";

            OpenFileDialog pfd = new OpenFileDialog();
            pfd.InitialDirectory = defDirPath;
            pfd.FileName = fileName;
            pfd.Filter = "txtファイル(*.txt)|*.txt";
            pfd.Title = "読み込み先のファイル名を指定してください";

            if (pfd.ShowDialog() == true)
            {
                OpenFilePath = pfd.FileName;
            }

            try
            {
                if (ReadTextAll(OpenFilePath, out content, out strException))
                {
                    if (File.Exists(OpenFilePath))
                    {
                        File.Delete(OpenFilePath);
                    }
                }
                else
                {
                    strException = string.Concat("ファイルの読み込みに失敗しました。\r\n", strException);
                    return false;
                }
            }
            #region catch句
            catch (FileNotFoundException ex)
            {
                strException = "ファイルが見つかりませんでした。";
                return false;
            }
            catch (IOException ex)
            {
                strException = "その他のファイル保存エラーです。\r\n" + ex.Message.ToString();
                return false;
            }
            catch (Exception ex)
            {
                strException = "その他のエラーです。\r\n" + ex.Message.ToString();
                return false;
            }
            #endregion

            strException = "正常に読み込みました。";
            return true;
        }

        /// <summary>
        /// バックアップファイルの読み込み
        /// </summary>
        /// <param name="content"></param>
        /// <param name="strException"></param>
        /// <returns></returns>
        public bool ReadBackupFile(out string content, out string strException)
        {
            string backDirPath = Path.Combine(defDirPath, "MastodonSS_bak");
            string OpenFilePath = "";
            content = "";
            strException = "";

            OpenFileDialog pfd = new OpenFileDialog();
            pfd.InitialDirectory = backDirPath;
            pfd.FileName = fileName;
            pfd.Filter = "bakファイル(*.bak)|*.bak";
            pfd.Title = "読み込み先のファイル名を指定してください";

            if (pfd.ShowDialog() == true)
            {
                OpenFilePath = pfd.FileName;
            }

            try
            {
                if(ReadTextAll(OpenFilePath, out content, out strException))
                {
                    if (File.Exists(OpenFilePath))
                    {
                        File.Delete(OpenFilePath);
                    }
                }
                else
                {
                    strException = string.Concat("ファイルの読み込みに失敗しました。\r\n", strException);
                    return false;
                }
            }
            #region catch句
            catch (FileNotFoundException ex)
            {
                strException = "ファイルが見つかりませんでした。";
                return false;
            }
            catch (IOException ex)
            {
                strException = "その他のファイル読込エラーです。\r\n" + ex.Message.ToString();
                return false;
            }
            catch (Exception ex)
            {
                strException = "その他のエラーです。\r\n" + ex.Message.ToString();
                return false;
            }
            #endregion


            strException = "正常に読み込みました。";
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="readFilePath"></param>
        /// <param name="content"></param>
        /// <param name="strException"></param>
        /// <returns></returns>
        private bool ReadTextAll(string readFilePath, out string content, out string strException)
        {
            strException = "";
            content = "";

            try
            {
                using (StreamReader sr = new StreamReader(readFilePath))
                {
                    content = sr.ReadToEnd();
                }
            }
            #region catch句
            catch (FileNotFoundException ex)
            {
                strException = "ファイルが見つかりませんでした。";
                return false;
            }
            catch (IOException ex)
            {
                strException = "その他のファイル読込エラーです。\r\n" + ex.Message.ToString();
                return false;
            }
            catch (Exception ex)
            {
                strException = "その他のエラーです。\r\n" + ex.Message.ToString();
                return false;
            }
            #endregion

            return true;
        }
    }
}
