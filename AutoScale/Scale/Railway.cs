using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComPort.Scale
{
    /// <summary>
    /// Железнодорожные весы
    /// </summary>
    public class Railway : IScale
    {
        public int SetWeight(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                    return 0;

                var massString = str.Split('\n');
                if (massString.Length == 1)
                {
                    return Weight(massString[0]);
                }
                else
                {
                    return Weight(massString[massString.Length - 2]);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private int Weight(string str)
        {
            try
            {
                int res = 0;
                str = str.Trim();
                str = str.ToUpper().Replace("ST,GS,1,", "");
                str = str.Replace("ST,GS,1,", "");
                str = str.Replace("S,1,", "");
                str = str.Replace("ST,G", "");
                str = str.Replace("T,", "");
                str = str.Replace("S,1", "");
                str = str.Replace("K", "").Replace("G", "").Replace("\r", "").Replace(",", "").Replace("", "");
                str = str.Trim();
                if (string.IsNullOrEmpty(str))
                    return 0;

                string[] st = str.Split(' ');

                for (int i = st.Length - 1; i >= 0; i--)
                {
                    if (st[i] != "")
                    {
                        str = st[i];
                        break;
                    }
                }

                if (str[0] == '-')
                {
                    str = str.Replace("-", "");
                    res = -int.Parse(str);
                    return res;
                }

                if (!int.TryParse(str, out res))
                {
                    throw new Exception("Failed to convert string to int! string:" + str);
                }

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
