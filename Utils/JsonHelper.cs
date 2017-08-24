using Newtonsoft.Json;
using System;
using System.IO;

namespace TokenUtils
{
    public class JsonHelper
    {
        /// <summary>
        /// 序列化数据
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(string inputStr)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(inputStr);
            }
            catch (Exception ex)
            {

            }
            return default(T);
        }

        /// <summary>
        /// 序列化数据
        /// </summary>
        /// <param name="inputStr"></param>
        /// <returns></returns>
        public static string Serialize(object inputStr)
        {
            try
            {
                return JsonConvert.SerializeObject(inputStr);
            }
            catch (Exception ex)
            {

            }
            return "";
        }

        /// <summary>
        /// 将实例对象转换为JSON字符串
        /// </summary>
        /// <param name="objReq"></param>
        /// <returns></returns>
        public static string getJsonString<T>(T objReq)
        {
            JsonSerializer serializer = new JsonSerializer();
            StringWriter sw = new StringWriter();
            serializer.Serialize(new JsonTextWriter(sw), objReq);
            String json = sw.GetStringBuilder().ToString();
            Console.WriteLine(json);
            sw.Close();
            return json;
        }
    }
}
