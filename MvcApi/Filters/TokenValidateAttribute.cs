using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;
using System.Net.Http;
using TokenUtils;

namespace MvcApi.Filters
{
    /// <summary>
    /// token过滤器:校验身份令牌,不通过则拒绝请求
    /// </summary>
    public class TokenValidateAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        /// <summary>
        /// 对第三方请求进行令牌校验：401->令牌校验失败 提醒第三方重新获取
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            try
            {
                base.OnActionExecuting(filterContext);
                #region 获取客户端身份令牌
                //定义身份令牌
                string accesstoken = string.Empty;
                //读取请求参数->优先取Headers下的Authorization，其次取入参
                var Authorheader = filterContext.Request.Headers.Authorization;
                if (Authorheader != null && !string.IsNullOrEmpty(Authorheader.Parameter) && !string.IsNullOrEmpty(Authorheader.Scheme))
                {
                    //将token存储在头部一起传输至服务器校验
                    if (Authorheader.Scheme.ToLower() == "bearer")
                        accesstoken = Authorheader.Parameter;
                }
                #endregion
                #region 客户端身份令牌校验
                if (string.IsNullOrEmpty(accesstoken))
                {
                    //未取到身份令牌
                    filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "未检测到令牌");
                }
                else
                {
                    //取到身份令牌，则进行身份校验
                    IUzaiSecurityVerificationLogicService _Security = new UzaiSecurityVerificationLogicService();
                    //校验token值
                    var result = _Security.ValidateToken(accesstoken);
                    if (!result.success)
                    {
                        filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, result.errmsg);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("令牌校验失败:", ex);
                filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized, "令牌检查报错");
            }
        }
    }
}