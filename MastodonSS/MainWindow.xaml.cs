using MastodonSS.Utility;
using MastodonSS.Utility.File;
using MastodonSS.Utility.OneWri;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MastodonSS
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        FileUtility fUtl;
        OneWriUtility oneUtl;
        BackupClass backupList;

        public TimeSpan OneWriSpan;

        private Timer backupTimer;
        private DispatcherTimer oneWriTimer;
        private DateTime timeLimit;
        private EventHandler eHander;

        private bool blnCanCopy;
        private bool blnHashtag;
        private bool blnForTwitter;
        private string strArticle;
        private string strBackArticle;
        private string strTitle;

        public MainWindow()
        {
            InitializeComponent();
        }

        private enum TimerMode{
            Hour1,
            Off
        }

        #region プロパティ
        private int ArticleLength
        {
            get
            {
                return strArticle.Replace("\r\n", "\n").Length;
            }
        }
        #endregion

        #region イベント
        /// <summary>
        /// 初期化メソッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fUtl = new FileUtility();
            backupList = new BackupClass();

            backupTimer = new Timer(60000);
            backupTimer.Elapsed += BackupTimer_Elapsed;
            backupTimer.Start();

            #region DispatcherTimer
            oneWriTimer = new DispatcherTimer(DispatcherPriority.Normal);
            oneWriTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            oneWriTimer.Tick += new EventHandler((s, ev) =>
            {
                
                if (oneUtl == null)
                {
                    SetEnabled(TimerMode.Off);
                    StsLblOneWri.Content = string.Empty;
                }
                else if (oneUtl.GetTimerEnabled == false)
                {
                    oneUtl = null;
                    SetEnabled(TimerMode.Off);
                    StsLblOneWri.Content = string.Empty;
                }
                else
                {
                    StsLblOneWri.Content = oneUtl.GetLeftTime;
                }

                if (DateTime.Now >= timeLimit)
                {
                    StsLblStatus.Content = string.Empty;
                }
            });
            oneWriTimer.Start();
            #endregion

            tblkCount.Text = "0";
            strArticle = "";
            strTitle = "";

            blnCanCopy = true;
            blnForTwitter = false;
            blnHashtag = true;
            ckbHashtag.IsChecked = true;
            
            MenuOneWriEnd.IsEnabled = false;

            cmbMaxChar.Items.Add(Properties.Settings.Default.MastodonLength);
            cmbMaxChar.Items.Add(Properties.Settings.Default.TwitterLength);
            cmbMaxChar.Items.Add(Properties.Settings.Default.TwitterLongLength);
            cmbMaxChar.SelectedIndex = 0;
          
            getArticle();
        }

        #region タイマーイベント
        private void BackupTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string strException = "ファイルのバックアップに失敗しました";

            // エラーチェック
            if (string.IsNullOrEmpty(strBackArticle))
            {
                return;
            }

            if (backupList.AutoBackup(strBackArticle) == false)
            {
                MessageBox.Show(strException, "失敗", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //if (fUtl.AutoBackup(new StringBuilder(strBackArticle), out strException) == false)
            //{
            //    MessageBox.Show(strException, "失敗", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
        #endregion

        /// <summary>
        /// 現在の本文を記録
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbArticle_TextChanged(object sender, TextChangedEventArgs e)
        {
            getArticle();
        }

        /// <summary>
        /// 現在のタイトルを記録
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            getTitle();
        }

        /// <summary>
        /// タイトルコピー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopyTitle_Click(object sender, RoutedEventArgs e)
        {
            getTitle();

            // タイトルが空ならコピーしない
            if (CommonUtility.IsNoArticle(txbTitle.Text)) return;
            Clipboard.SetText(strTitle);
        }

        /// <summary>
        /// 本文コピー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopyArticle_Click(object sender, RoutedEventArgs e)
        {
            getArticle();

            // 文字数オーバー・不足時はコピー不可
            if (blnCanCopy == false) return;
            // Twitter用は、ハッシュタグ付きでない、タイトル無しまたは本文無しではコピー不可
            if (blnForTwitter)
            {
                if (!blnHashtag || !CanCopyArticle()) return;
            }
            // Mastodon用は、タイトルまたは本文無しではコピー不可
            else
            {
                if (!CanCopyArticle()) return;
            }

            Clipboard.SetText(strArticle);
        }

        /// <summary>
        /// ハッシュタグの切り替え
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbHashtag_Click(object sender, RoutedEventArgs e)
        {
            getArticle();
            blnHashtag = (bool)ckbHashtag.IsChecked;
        }

        /// <summary>
        /// 最大文字数でモード切替
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbMaxChar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((int)cmbMaxChar.SelectedValue != Properties.Settings.Default.MastodonLength)
            {
                blnForTwitter = true;
                blnHashtag = true;
                ckbHashtag.IsChecked = true;
            }
            else
            {
                blnForTwitter = false;
            }

            ProgBar.Maximum = (int)cmbMaxChar.SelectedValue;
            setTextCount();
        }

        /// <summary>
        /// テキストファイルの読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuReadText_Click(object sender, RoutedEventArgs e)
        {
            string strContent = "";
            string strException = "";

            if (fUtl.ReadText(out strContent, out strException) == false)
            {
                MessageBox.Show(strException, "失敗", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                StsLblStatus.Content = "読み込みに成功しました";
                timeLimit = DateTime.Now.AddSeconds(5);
                txbArticle.Text = strContent;
            }
        }

        /// <summary>
        /// バックアップファイルの読み込み
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuReadBackup_Click(object sender, RoutedEventArgs e)
        {
            string strContent = "";
            string strException = "";

            if (fUtl.ReadBackupFile(out strContent, out strException) == false)
            {
                MessageBox.Show(strException, "失敗", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                StsLblStatus.Content = "読み込みに成功しました";
                timeLimit = DateTime.Now.AddSeconds(5);
                txbArticle.Text = strContent;
            }
        }

        /// <summary>
        /// 保存メニュークリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb;
            string strException = "";
            string strTitle = txbTitle.Text;
            int intSeq = 0;

            // エラーチェック
            if (CommonUtility.IsNoArticle(txbArticle.Text))
            {
                return;
            }
            else if (CommonUtility.IsInt(txbNo.Text) == false)
            {
                return;
            }
            else if(!CommonUtility.IsNoArticle(txbNo.Text))
            {
                intSeq = int.Parse(txbNo.Text);
            }

            fUtl.setTitle(strTitle, intSeq);

            if (fUtl.SaveFile(new StringBuilder(txbArticle.Text), out strException) == true)
            {
                StsLblStatus.Content = "保存に成功しました";
                timeLimit = DateTime.Now.AddSeconds(5);
            }
            else
            {
                MessageBox.Show(strException, "失敗", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

        /// <summary>
        /// 終了メニュークリック時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuQuit_Click(object sender, RoutedEventArgs e)
        {
            EndAppProcess();
            this.Close();
        }

        /// <summary>
        /// アプリを終了するとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        /// <summary>
        /// ワンライ開始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuOneHourStart_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("開始します", "ワンライ", 
                MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                oneUtl = new OneWriUtility();
                SetEnabled(TimerMode.Hour1);
            }
        }

        /// <summary>
        /// ワンライ終了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuOneWriEnd_Click(object sender, RoutedEventArgs e)
        {
            if(oneUtl.Stop())
            {
                SetEnabled(TimerMode.Off);
                oneUtl = null;
            }
        }
        #endregion

        #region 画面連携
        /// <summary>
        /// コピーが可能か判定
        /// </summary>
        /// <returns></returns>
        private bool CanCopyArticle()
        {
            return !(CommonUtility.IsNoArticle(txbTitle.Text) || CommonUtility.IsNoArticle(txbArticle.Text));
        }

        /// <summary>
        /// 本文を取得し、文字数を反映
        /// </summary>
        private void getArticle()
        { 
            // ハッシュタグの切り替えを反映
            if(ckbHashtag.IsChecked == true)
            {
                strArticle = string.Format("#{0}\r\n{1}", txbTitle.Text, txbArticle.Text);
            }
            else
            {
                strArticle = txbArticle.Text;
            }
            strBackArticle = txbArticle.Text;

            // 文字数を反映
            setTextCount();
            return;
        }

        /// <summary>
        /// タイトルを取得し、文字数を反映
        /// </summary>
        private void getTitle()
        {
            // 本文を取得
            getArticle();
            
            // ナンバリングが有効な数字でない場合、タイトルのみにする
            if (CommonUtility.IsInt(txbNo.Text) == false)
            {
                strTitle = txbTitle.Text;
            }
            else
            {
                strTitle = txbTitle.Text + " " + txbNo.Text;
            }

            // 文字数を反映
            setTextCount();
            return;
        }

        /// <summary>
        /// 文字数を反映
        /// </summary>
        /// <param name="txtLen"></param>
        private void setTextCount()
        {
            int txtLen = ArticleLength;

            // 文字数を反映
            tblkCount.Text = txtLen.ToString().PadLeft(4);
            ProgBar.Value = txtLen;

            // 文字数オーバー時は赤で表示
            if (txtLen > (int)cmbMaxChar.SelectedValue)
            {
                tblkCount.Foreground = Brushes.Red;
                blnCanCopy = false;
            }
            // 本文がないときは灰色で表示
            else if (txbArticle.Text.Length == 0)
            {
                tblkCount.Foreground = Brushes.Gray;
                blnCanCopy = false;
            }
            // 問題が無いときは黒で表示
            else
            {
                tblkCount.Foreground = Brushes.Black;
                blnCanCopy = true;
            }
        }

        /// <summary>
        /// 終了プロセス
        /// </summary>
        private void EndAppProcess()
        {
            string strException = "バックアップの削除に失敗しました。";
            backupTimer.Stop();

            if (backupList.AllDelete() == false)
            {
                MessageBox.Show(strException, "失敗", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            //if (fUtl.DeleteBackup(out strException) == false)
            //{
            //    MessageBox.Show(strException, "失敗", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }

        private void SetEnabled(TimerMode mode)
        {
            switch(mode)
            {
                case TimerMode.Hour1:
                    MenuOneHourStart.IsEnabled = false;
                    MenuOneWriEnd.IsEnabled = true;
                    break;
                default:
                    MenuOneHourStart.IsEnabled = true;
                    MenuOneWriEnd.IsEnabled = false;
                    break;
            }
        }
        #endregion
    }
}
