using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TokenUtils.Entity;

namespace TokenUtils
{
    /// <summary>
    /// Uzai安全验证
    /// </summary>
    public interface IUzaiSecurityVerificationLogicService
    {
        /// <summary>
        /// 申请token
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TokenInfoEntity GetToken(IdentityCheckEntity entity);
        /// <summary>
        /// 申请token（客户端测试）
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        string GetToken(string appid, string username, string password, string appSecret);
        /// <summary>
        /// 校验token
        /// </summary>
        /// <param name="token">身份令牌</param>
        /// <returns></returns>
        TokenInfoEntity ValidateToken(string token);
    }
}
