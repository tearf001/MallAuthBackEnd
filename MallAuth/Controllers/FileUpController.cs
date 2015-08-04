using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Data;
using MallAuth.extensions;

namespace MallAuth.Controllers
{
    [RoutePrefix("api/FileUp")]

    public class FileUpController : ApiController
    {


        [Authorize]
        [Route("")]
        public async Task<Object> Post()
        {

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotAcceptable, "Invalid Request!"));
            }
   
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var keys = httpRequest.Form;
                //keysArray = (FileMark[])Newtonsoft.Json.JsonConvert.DeserializeObject(keys, typeof(FileMark[]));

            }
            catch (Exception)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            var url = ConfigurationManager.AppSettings["uploadUrl"];
            if (string.IsNullOrWhiteSpace(url)) url = "http://hb.10000shequ.com/UploadImg/Home/UpImg";
            try
            {
                var streamProvider = new MyMultipartFormDataStreamProvider("uploads",Request.RequestUri.Authority);
                //Request.Content.LoadIntoBufferAsync().Wait();
                return await Request.Content.ReadAsMultipartAsync(streamProvider)
                     .ContinueWith<Object>(res =>
                     {
                         var fileInfo = res.Result.FileData.Select(i =>
                         {
                             var filename = i.LocalFileName.Replace("\"", "");
                             var rf =  res.Result.fnmap[i.Headers.ContentDisposition.FileName];
                             return new { url = DoCorsUpload(url, filename, i.Headers.ContentType.MediaType), midUrl = rf };
                         });
                         return fileInfo;// new { files = fileInfo };
                     });

            }
            catch (Exception)
            {
                //Console.log()
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
        /// <summary>
        /* ---------------------------acebdf13572468
        Content-Disposition: form-data; name="fieldNameHere"; filename="popup_2.png"
        Content-Type: image/png

        <@INCLUDE *C:\Users\Administrator\Desktop\Android 1080P\pic\popup_2.png*@>
        ---------------------------acebdf13572468
        Content-Disposition: form-data; name="fieldNameHere"; filename="popup_3.png"
        Content-Type: image/png

        <@INCLUDE *C:\Users\Administrator\Desktop\Android 1080P\pic\popup_3.png*@>
        //---------------------------acebdf13572468--*/
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public string DoCorsUpload(string url, string filepath, string mediaType)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["upload-middler-for-test"]))
                return "in test";

            var timeBoundary = DateTime.Now.Ticks.ToString("x");

            //Identificate separator
            string boundary = "---------------------------" + timeBoundary;
            //Encoding
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            //Creation and specification of the request
            HttpWebRequest httpwebreq = (HttpWebRequest)WebRequest.Create(url); //sVal is id for the webService
            httpwebreq.ContentType = "multipart/form-data; boundary=" + boundary;
            httpwebreq.Method = "POST";
            httpwebreq.KeepAlive = true;
            httpwebreq.Credentials = System.Net.CredentialCache.DefaultCredentials;

            //如果需要鉴权..
            //string sAuthorization = "login:password";//AUTHENTIFICATION BEGIN
            //byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(sAuthorization);
            //string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            //wr.Headers.Add("Authorization: Basic " + returnValue); //AUTHENTIFICATION END
            //--------------------------------------end of headers--------------------------

            //拼接body流
            using (Stream rs = httpwebreq.GetRequestStream())
            {

                ////非文件域,非文件格式
                //---------------------------acebdf13572468
                //string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                //foreach (string key in nvc.Keys)
                //{
                //    rs.Write(boundarybytes, 0, boundarybytes.Length);
                //    string formitem = string.Format(formdataTemplate, key, nvc[key]);
                //    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                //    rs.Write(formitembytes, 0, formitembytes.Length);
                //}

                //写body,以 boundary开始.
                //---------------------------acebdf13572468
                rs.Write(boundarybytes, 0, boundarybytes.Length);



                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, "file", timeBoundary, mediaType);
                byte[] header_bytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(header_bytes, 0, header_bytes.Length);

                FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);

                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();
                //---------------------------acebdf13572468--
            }
            WebResponse httpwresp = null;
            try
            {
                //Get the response
                httpwresp = httpwebreq.GetResponse();
                using (Stream stream2 = httpwresp.GetResponseStream())
                using (StreamReader responseRead = new StreamReader(stream2))
                    return responseRead.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    public class MyMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        public readonly Dictionary<string, string> fnmap;
        private string Authority;
        private string serverBase;

        public MyMultipartFormDataStreamProvider(string uploadFolder, string Authority)
            : base(HttpContext.Current.Server.MapPath("~/"+uploadFolder))
        {
            fnmap = new Dictionary<string, string>();
            this.Authority = Authority + "/" + uploadFolder+"/";
            this.serverBase = HttpContext.Current.Server.MapPath("~/" + uploadFolder+"/");
        }     

        // Summary:
        //     Gets the name of the local file which will be combined with the root path
        //     to create an absolute file name where the contents of the current MIME body
        //     part will be stored.
        //     默认是guid作为文件名的.如果重载,那么可以制定一个有意义的文件名
        // Parameters:
        //   headers:
        //     The headers for the current MIME body part.
        // Returns:
        //     A relative filename with no path component.
        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            var oldname = headers.ContentDisposition.FileName;
            var posible_full_name = (!string.IsNullOrWhiteSpace(oldname) ? oldname : "NoName").Replace("\"", string.Empty);
            var nicename = Path.GetFileNameWithoutExtension(posible_full_name);
            var nicenameExt = Path.GetExtension(posible_full_name);
            var hn = serverBase + posible_full_name;
            Random r = new Random();
            int i = 0;
            while (File.Exists(hn)) { 
                i = r.Next(100,999);
                posible_full_name = string.Format(nicename + "({0})" + nicenameExt, i);
                hn = serverBase + posible_full_name;
            }
            if (!fnmap.ContainsKey(oldname)) {
                fnmap.Add(oldname, Authority + posible_full_name);
            }
            return posible_full_name;
        }

    }
}
