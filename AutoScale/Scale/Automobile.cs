using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComPort.Scale
{
    /// <summary>
    /// Автомобильные весы
    /// </summary>
    public class Automobile : IScale
    {
        public int SetWeight(string str)
        {
            try
            {
                if (str.Length >= 12)
                {
                    str = str.Substring(str.Length - 11, 11);
                    return Weight(str);
                }
                return 0;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        private int Weight(string str)
        {
            try
            {
                int res;
                str = str.Replace("", "").Replace("", "");
                string copy = str.Substring(1, 6);
                if (str[0] == '+')
                {
                    if (!int.TryParse(copy, out res))
                    {
                        throw new Exception("Failed to convert string to int! string:" + str);
                    }
                }
                else
                {
                    if (!int.TryParse(copy, out res))
                    {
                        throw new Exception("Failed to convert string to int! string:" + str);
                    }


                    res = -res;
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
