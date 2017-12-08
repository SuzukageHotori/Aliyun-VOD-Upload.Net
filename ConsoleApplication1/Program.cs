using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.vod.Model.V20170321;
using Aliyun.OSS;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            IClientProfile clientProfile = DefaultProfile.GetProfile("cn-shanghai", "", "");
            DefaultAcsClient client = new DefaultAcsClient(clientProfile);
            var r =  CreateUploadVideo(client);
            
            byte[] bar1 = Convert.FromBase64String(r.UploadAddress);
            var uploadAddress = Encoding.Default.GetString(bar1);
            var m1 = JsonConvert.DeserializeObject<UploadAddress>(uploadAddress);


            byte[] bar2 = Convert.FromBase64String(r.UploadAuth);
            var uploadAuth = Encoding.Default.GetString(bar2);
            var m2 = JsonConvert.DeserializeObject<UploadAuth>(uploadAuth);

            var client2 = new OssClient(m1.Endpoint, m2.AccessKeyId, m2.AccessKeySecret,m2.SecurityToken);
            
            try
            {
                string fileToUpload = @"D:\video\1.mp4";
                var r2 = client2.PutObject(m1.Bucket, m1.FileName, fileToUpload);
                Console.WriteLine("Put object succeeded");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Put object failed, {0}", ex.Message);
            }
        }


        private static CreateUploadVideoResponse CreateUploadVideo(DefaultAcsClient client)
        {
            CreateUploadVideoRequest request = new CreateUploadVideoRequest();
            request.Title = "测试OSS上传";
            request.FileName = "1.mp4";
            request.Description = "视频描述";
            //request.CoverURL = "http://cover.sample.com/sample.jpg";
            //request.Tags = "标签1,标签2";
            //request.CateId = 0;
            try
            {
                CreateUploadVideoResponse response = client.GetAcsResponse(request);
                return response;
            }
            catch (ServerException e)
            {
                Console.WriteLine(e.ErrorCode);
                Console.WriteLine(e.ErrorMessage);
                throw;
            }
            catch (ClientException e)
            {
                Console.WriteLine(e.ErrorCode);
                Console.WriteLine(e.ErrorMessage);
                throw;
            }
        }
    }

    public class UploadAddress
    {
        public string Bucket { get; set; }
        public string Endpoint { get; set; }
        public string FileName { get; set; }
    }

    public class UploadAuth
    {
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string SecurityToken { get; set; }
        public string Expiration { get; set; }
    }
}
