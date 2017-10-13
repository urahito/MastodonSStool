using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastodonSS.Utility
{
    public class CommonUtility
    {
        /// <summary>
        /// 整数または数値か判定
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(object value)
        {
            try
            {
                double parseValue = double.Parse(value.ToString());
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 整数か判定
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsInt(object value)
        {
            try
            {
                int parseValue = int.Parse(value.ToString());
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 内容があるか判定
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNoArticle(string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
        }
    }
}
