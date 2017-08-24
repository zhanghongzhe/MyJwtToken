using System;
using Newtonsoft.Json;

namespace TokenUtils.Entity
{
    /// <summary>
    /// 身份令牌实体
    /// </summary>
    public class TokenInfoEntity
    {
        /// <summary>
        /// 通讯令牌
        /// </summary>
        public string accesstoken { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public int expiresin { get; set; }
        /// <summary>
        /// 令牌类型一般为bearer
        /// </summary>
        public string tokentype { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 错误提示信息
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 渠道ID
        /// </summary>
        [JsonIgnoreAttribute]
        public string clientid { get; set; }
        /// <summary>
        /// 渠道账户
        /// </summary>
        [JsonIgnoreAttribute]
        public string username { get; set; }
    }
}
