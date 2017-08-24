using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using TokenUtils;
using TokenUtils.Entity;

namespace MyJwtToken
{
    public class TokenClass
    {
        public static readonly DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private string requestUrl = "http://localhost:7257/api/Token/Get";

        public void Test()
        {
            var result = this.GetToken("Tisp0001", "UtourTispCruise", "tisp123456", "dGlzcDEyMzQ1Ng==");
            var tokenInfoEntity = JsonHelper.DeserializeObject<TokenInfoEntity>(result);

            var r = RequestValue(tokenInfoEntity.accesstoken);
            Console.WriteLine(r);
        }

        //正常请求需要验证的接口
        private string RequestValue(string tokenKey)
        {
            string s = string.Empty;
            //请求线路html
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:7257/api/Values/Get?id=1");
                byte[] bs = Encoding.UTF8.GetBytes("");
                request.Method = "POST";
                request.ContentLength = bs.Length;
                request.ContentType = "application/json";
                if (!string.IsNullOrWhiteSpace(tokenKey))
                    request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + tokenKey);
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        return readStream.ReadToEnd();
                    }
                }
            }
            catch (Exception exception)
            {
                return exception.Message;
            }
        }
        #region 客户端
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
                    URL = this.requestUrl,//URL     必需项  
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
