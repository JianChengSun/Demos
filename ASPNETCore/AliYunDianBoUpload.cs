using Aliyun.OSS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AliDianBoTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            GetCreditials().GetAwaiter().GetResult();
        }

        static async Task GetCreditials()
        {
            Console.WriteLine(DateTime.Now);
            //获取视频点播上传凭证
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(SignatureURL());
            string content = await response.Content.ReadAsStringAsync();
            Creditial data = JsonConvert.DeserializeObject<Creditial>(content);
            //解析得到OSS上传的凭证
            UploadAddress uploadAddress = JsonConvert.DeserializeObject<UploadAddress>(Base64Decode(data.UploadAddress));
            UploadAuth uploadAuth = JsonConvert.DeserializeObject<UploadAuth>(Base64Decode(data.UploadAuth));
            await UploadImage(uploadAddress, uploadAuth);
            Console.WriteLine(DateTime.Now);
            Console.ReadKey();
        }

        //加密SecrectKey,对URL转码
        private static string SignatureURL()
        {
            const string HTTP_METHOD = "GET";
            IDictionary<String, String> parameterDic = new Dictionary<string, string>();

            // 加入请求公共参数
            parameterDic.Add("Version", "2017-03-21");
            parameterDic.Add("AccessKeyId", "LTAIe6Gy6Aq2xFQc"); //此处请替换成您自己的AccessKeyId
            parameterDic.Add("TimeStamp", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
            parameterDic.Add("SignatureMethod", "HMAC-SHA1");
            parameterDic.Add("SignatureVersion", "1.0");
            parameterDic.Add("SignatureNonce", System.Guid.NewGuid().ToString());
            parameterDic.Add("Format", "JSON");

            // 加入方法特有参数
            parameterDic.Add("Action", "CreateUploadImage");
            parameterDic.Add("ImageType", "default");
            parameterDic.Add("ImageExt", "png");
            parameterDic.Add("Title", "图片");
            parameterDic.Add("Tags", "png");
            //parameterDic.Add("CateId", "jpg");
            parameterDic.Add("Description", "png");

            // 对参数进行排序
            var dicSort = from objDic in parameterDic orderby objDic.Key select objDic;

            // 生成stringToSign字符
            const string SEPARATOR = "&";
            const string EQUAL = "=";
            StringBuilder stringToSign = new StringBuilder();
            stringToSign.Append(HTTP_METHOD).Append(SEPARATOR); ;
            stringToSign.Append(PercentEncode("/")).Append(SEPARATOR);

            StringBuilder canonicalizedQueryString = new StringBuilder();
            foreach (KeyValuePair<string, string> kvp in dicSort)
            {
                // 此处需要对key和value进行编码
                canonicalizedQueryString.Append(SEPARATOR).Append(PercentEncode(kvp.Key)).Append(EQUAL).Append(PercentEncode(kvp.Value));
            }

            // 此处需要对canonicalizedQueryString进行编码
            stringToSign.Append(PercentEncode(canonicalizedQueryString.ToString().Substring(1)));

            string strToSign = stringToSign.ToString();

            byte[] bytKey = Encoding.UTF8.GetBytes("elw8ThdrIyHCSBE71tSb5CZ90wlqcJ&");

            //将StringToSign转化为Byte数组
            byte[] bytToSign = Encoding.UTF8.GetBytes(strToSign);

            //设置HMAC SHA1的密钥（Access Key Secret的Byte数组）
            HMACSHA1 hmac = new HMACSHA1(bytKey);
            //进行哈希运算
            byte[] hashValue = hmac.ComputeHash(bytToSign);
            //进行Base64编码
            string signature = Convert.ToBase64String(hashValue);
            //对生成的签名进行URL编码（UTF8）
            signature = HttpUtility.UrlEncode(signature, Encoding.UTF8);
            //构建完整的URL

            // 生成请求URL
            StringBuilder requestURL = new StringBuilder("http://vod.cn-shanghai.aliyuncs.com?");
            requestURL.Append(PercentEncode("Signature")).Append("=").Append(signature);

            foreach (var item in dicSort)
            {
                requestURL.Append("&").Append(PercentEncode(item.Key)).Append("=").Append(PercentEncode(item.Value));
            }
            return requestURL.ToString();
        }

        //URL转码
        public static string PercentEncode(String value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(value);
            foreach (char c in bytes)
            {
                if (text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                }
                else
                {
                    stringBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
                }
            }
            return stringBuilder.ToString();
        }

        //解密Base64
        public static string Base64Decode(string value)
        {
            byte[] bytes = Convert.FromBase64String(value);
            return Encoding.Default.GetString(bytes);
        }

        //上传图片
        public static async Task UploadImage(UploadAddress uploadAddress, UploadAuth uploadAuth)
        {
            var client = new OssClient(uploadAddress.Endpoint, uploadAuth.AccessKeyId, uploadAuth.AccessKeySecret,uploadAuth.SecurityToken);
            try
            {
                // 上传文件。
                //client.PutObject(uploadAddress.Bucket, uploadAddress.FileName, @"D:\微信截图_20190418172641.png");
                Stream fileStream = File.OpenRead(@"D:\微信截图_20190418172641.png");
                client.PutObject(uploadAddress.Bucket, uploadAddress.FileName, fileStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Put object failed, {0}", ex.Message);
            }
        }
    }

    //视频点播上传凭证
    public class Creditial
    {
        public string RequestId { get; set; }

        public string ImageId { get; set; }

        public string UploadAddress { get; set; }

        public string UploadAuth { get; set; }

        public string ImageURL { get; set; }
    }

    //OSS上传地址
    public class UploadAddress
    {
        public string Bucket { get; set; }

        public string Endpoint { get; set; }

        public string FileName { get; set; }
    }

    //OSS上传验证
    public class UploadAuth
    {
        public string AccessKeyId { get; set; }

        public string AccessKeySecret { get; set; }

        public string SecurityToken { get; set; }

        public string Expiration { get; set; }
    }
}
