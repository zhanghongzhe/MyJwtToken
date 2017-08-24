using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenUtils.Entity
{
    /// <summary>
    /// 申请令牌实体
    /// </summary>
    public class IdentityCheckEntity
    {
        /// <summary>
        /// 算法签名结果
        /// </summary>
        public string signature { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp { get; set; }


        /// <summary>
        /// 随机数
        /// </summary>
        public string nonce { get; set; }

        /// <summary>
        /// 渠道ID
        /// </summary>
        public string clienid { get; set; }

        /// <summary>
        /// 渠道账号
        /// </summary>
        public string username { get; set; }


        /// <summary>
        /// 渠道密码
        /// </summary>
        public string password { get; set; }
    }
}
