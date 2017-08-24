using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TokenUtils.Entity
{
    public class thirdParty
    {
        /// <summary>
        /// 第三方渠道ID
        /// </summary>
        public string clientId { get; set; }
        /// <summary>
        /// 第三方渠道秘钥
        /// </summary>
        public string clientSecret { get; set; }
        /// <summary>
        /// 第三方对接账户用户名
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// 第三方对接账户密码
        /// </summary>
        public string passWord { get; set; }
    }
}
