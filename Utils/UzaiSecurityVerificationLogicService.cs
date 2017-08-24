using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using JWT;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Web.Security;
using TokenUtils.Entity;

namespace TokenUtils
{
    /// <summary>
    /// JSON Web Token工具类
    /// </summary>
    public class UzaiSecurityVerificationLogicService : IUzaiSecurityVerificationLogicService
    {
        public static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        private int tokenExpiredDay = 7; //TOKEN有效时间 单位天
        private int timspanExpiredMinutes = 10; //时间戳有效时间 单位分钟
        private string sharedKey = "abc"; //公钥key
        private List<thirdParty> thirdPartys = new List<thirdParty>();
        public UzaiSecurityVerificationLogicService()
        {
            //配置可以访问的用户
            thirdPartys.Add(new thirdParty() { clientId = "Tisp0001", clientSecret = "dGlzcDEyMzQ1Ng==", userName = "UtourTispCruise", passWord = "tisp123456" });
        }

        #region 服务端
        /// <summary>
        /// 获取通讯令牌
        /// </summary>
        /// <param name="Eitity"></param>
        /// <returns></returns>
        public TokenInfoEntity GetToken(IdentityCheckEntity Eitity)
        {
            var Eitityinfo = new TokenInfoEntity();
            Eitityinfo.errmsg = "数据完整性检查不通过";
            Eitityinfo.success = false;
            if (Eitity != null)
            {
                var Thirdparty = this.thirdPartys.First(o => o.clientId == Eitity.clienid);
                if (Thirdparty == null) return Eitityinfo;//第三方请求对象不存在，返回请求
                //根据入参获取身份签名，并进行身份签名对比
                string tmpStr = SignatureString(Thirdparty.clientSecret, Eitity.timestamp, Eitity.nonce);
                if (tmpStr != Eitity.signature.ToLower()) return Eitityinfo;                //签名验证不通过，返回请求
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(_unixEpoch);
                long lTime = long.Parse(Eitity.timestamp + "0000000");
                TimeSpan toNow = new TimeSpan(lTime);
                dtStart = dtStart.Add(toNow);

                double minutes = DateTime.Now.Subtract(dtStart).TotalMinutes;
                if (minutes > this.timspanExpiredMinutes)
                {
                    Eitityinfo.errmsg = "签名时间戳失效";
                    Eitityinfo.success = false;
                    return Eitityinfo;
                }
                if (Eitity.username.ToLower() != Thirdparty.userName.ToLower() || Eitity.password != Thirdparty.passWord)
                {
                    Eitityinfo.success = false;
                    Eitityinfo.errmsg = "用户名或密码错误";
                    return Eitityinfo;
                }
                TimeSpan t = (DateTime.UtcNow - _unixEpoch);
                int timestamp = (int)t.TotalDays;
                var tokenload = new Dictionary<string, object>
                    {
                        {"iss",Eitity.clienid},//issuer     请求对象
                        {"iat",timestamp},//时间戳,当前单位天
                        {"username",Thirdparty.userName},//请求账户名   
                    };
                Eitityinfo.accesstoken = JsonWebToken.Encode(tokenload, this.sharedKey, JwtHashAlgorithm.HS256);
                Eitityinfo.expiresin = this.tokenExpiredDay * 24 * 3600;
                Eitityinfo.errmsg = "";
                Eitityinfo.success = true;
            }
            return Eitityinfo;
        }

        /// <summary>
        /// 检查用户的Token有效性,并核实用户是否有访问接口的权限
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public TokenInfoEntity ValidateToken(string token)
        {
            //返回的结果对象
            TokenInfoEntity result = new TokenInfoEntity();
            result.errmsg = "令牌检查不通过";
            result.success = false;
            try
            {
                if (string.IsNullOrEmpty(token)) return result;
                //解密TOKEN，获取真实身份，进行身份判定
                string decodedJwt = JsonWebToken.Decode(token, this.sharedKey);
                if (string.IsNullOrEmpty(decodedJwt)) return result;
                #region 检查令牌对象内容
                dynamic root = JObject.Parse(decodedJwt);
                string clientid = root.iss;
                string username = root.username;
                int jwtcreated = (int)root.iat;
                
                //检查令牌的有效期，7天内有效
                TimeSpan t = (DateTime.UtcNow - _unixEpoch);
                int timestamp = (int)t.TotalDays;
                if (timestamp - jwtcreated > this.tokenExpiredDay)
                {
                    result.errmsg = "用户令牌失效";
                    result.success = false;
                    return result;
                }
                //成功通过校验校验
                result.success = true;
                result.errmsg = "";
                result.clientid = clientid;
                result.username = username;
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("令牌校验失败->token:{0}", token), ex);
            }
            return result;
        }
        #endregion

        #region 客户端（测试用）
        /// <summary>
        /// 从服务端获取身份令牌
        /// </summary>
        /// <returns></returns>
        public string GetToken(string clienid, string username, string password, string appSecret)
        {
            try
            {
                IdentityCheckEntity identity = new IdentityCheckEntity()
                {
                    clienid = clienid,
                    nonce = new Random().NextDouble().ToString(),
                    timestamp = Convert.ToInt64((DateTime.UtcNow - _unixEpoch).TotalSeconds).ToString(),
                    username = username,
                    password = password,
                };
                identity.signature = SignatureString(appSecret, identity.timestamp, identity.nonce);
                HttpItem item = new HttpItem()
                {
                    URL = "http://localhost:8375/api/token/Get",//URL     必需项  
                    Method = "post",//URL     可选项 默认为Get   
                    IsToLower = false,//得到的HTML代码是否转成小写     可选项默认转小写  
                    ContentType = "application/json",
                    Accept = "application/json",
                    Postdata = JsonHelper.Serialize(identity)
                };
                var result = new HttpHelper().GetHtml(item);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine(string.Format("获取Token失败:请求参数->clienid:{0};nonce:{1};timestamp:{2};username:{3};password:{4};appSecret:{5};signature:{6},错误内容：{7}", identity.clienid, identity.nonce, identity.timestamp, identity.username, identity.password, identity.signature, result.Html));
                }
                return result.Html;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("请求异常->获取Token失败"), ex);
                throw ex;
            }
        }
        #endregion

        #region 公共action
        /// <summary>
        /// 生成签名字符串
        /// </summary>
        /// <param name="appSecret">接入秘钥</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        public string SignatureString(string appSecret, string timestamp, string nonce)
        {
            string[] ArrTmp = { appSecret, timestamp, nonce };

            Array.Sort(ArrTmp);
            string tmpStr = string.Join("", ArrTmp);

            tmpStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tmpStr, "SHA1");
            return tmpStr.ToLower();
        }
        #endregion
    }
}
