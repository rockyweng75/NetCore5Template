using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NetCore5_Domain.Constants
{
    /// <summary>
    /// 大寫為一般使用；小寫為簽核者專用
    /// </summary>
    public class StatusCodeConstants
    {
        public static readonly string 送出 = "S";

        public static readonly string 同意 = "A";

        public static readonly string 簽核完成 = "Z";

        public static readonly string 未處理 = null;

        public static readonly string 待簽核 = "C";

        public static readonly string 退回 = "B";

        public static readonly string 不同意 = "D";

    }
}