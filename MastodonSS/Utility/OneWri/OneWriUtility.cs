using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace MastodonSS.Utility.OneWri
{
    class OneWriUtility
    {
        Timer tm = new Timer(500);
        DateTime timeLimit;
        TimeSpan leftTime;

        #region プロパティ
        public string GetLeftTime
        {
            get
            {
                leftTime = timeLimit.Subtract(DateTime.Now);
                return string.Format("終了予定→ {0} / 残り→ {1}", timeLimit.ToShortTimeString(), leftTime.ToString("hh\\:mm\\:ss"));
            }
        }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// 標準：1時間でタイマーセット
        /// </summary>
        public OneWriUtility()
        {
            setDefault(new TimeSpan(1, 0, 0));
        }

        /// <summary>
        /// 時間を指定してタイマーセット
        /// </summary>
        /// <param name="ts"></param>
        public OneWriUtility(TimeSpan ts)
        {
            setDefault(ts);
        }

        /// <summary>
        /// 共通初期設定
        /// </summary>
        /// <param name="ts"></param>
        private void setDefault(TimeSpan ts)
        {
            timeLimit = DateTime.Now.Add(ts);
            tm.Elapsed += Tm_Elapsed;
            tm.Start();
        }
        #endregion

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~OneWriUtility()
        {
            if(tm.Enabled)
            {
                tm.Stop();
            }
        }

        /// <summary>
        /// タイマーイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tm_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(DateTime.Now >= timeLimit)
            {
                tm.Stop();
                MessageBox.Show("タイムリミットです", "終了", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Stop()
        {
            if(MessageBox.Show("終了しますか？", "確認", MessageBoxButton.OKCancel, MessageBoxImage.Question)
                == MessageBoxResult.OK)
            {
                tm.Stop();
                return true;
            }

            return false;
        }

        
    }
}
