
using System;

namespace Mx.Util
{
    /// <summary>字符串扩展</summary>
    public class StringEx
    {
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="bflag"></param>
        /// <returns></returns>
        public static long GetTimeStamp(bool bflag = false)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long ret;
            if (bflag) ret = Convert.ToInt64(ts.TotalSeconds);
            else ret = Convert.ToInt64(ts.TotalMilliseconds);
            return ret;
        }
    }
}