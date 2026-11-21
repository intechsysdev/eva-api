using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Migracion.Api.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Migracion.Api.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        FileSettings _settings;
        IWebHostEnvironment _hostEnvironment;

        public FileUploadController(IOptions<FileSettings> settings, IWebHostEnvironment hostEnvironment) {
            _settings = settings.Value;
            _hostEnvironment = hostEnvironment;
        }

        [HttpPost]
        [Route("Upload")]
        public  string Upload(IFormFile file,string folder = "") {

            string result = "0";
            try
            {
                if (file != null)
                {
                    
                    //string uploads = Path.Combine(_hostEnvironment.ContentRootPath, "FilesEva");
                    if (file.Length > 0)
                    {
                        string filePath = "";
                         
                        if (folder == string.Empty)
                        {
                            filePath = Path.Combine(_settings.UrlPath, file.FileName);
                        }
                        else {
                            filePath = Path.Combine($"{_settings.UrlPath}/{folder}", file.FileName);
                        }
                       
                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            
                             file.CopyTo(fileStream);
                        }

                    }

                    // result = "Carga Exitosa";
                    result = "1";
                }
            }
            catch (Exception ex)
            {

                result = ex.Message;
                //result = "-1";
            }

            return result;
        }

        [HttpPost]
        [Route("UploadWeb")]
        public string UploadWeb(IFormFile file, string folder = "")
        {

            string result = "";
            try
            {
                if (file != null)
                {

                    String hostName = Dns.GetHostName();


                    string uploads = Path.Combine(_hostEnvironment.ContentRootPath, "FilesEva");
                    if (file.Length > 0)
                    {
                        //string filePathWeb = "https://sync1.recamier.com:8452/FilesEva/";
                        string filePathWeb = "/FilesEva/";


                        string filePath = Path.Combine(uploads, file.FileName);
                        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                        {

                            file.CopyTo(fileStream);
                            
                        }
                        WebClient myWebClient = new WebClient();
                        if (folder == string.Empty)
                        {
                            // filePath = Path.Combine(_settings.UrlPath, file.FileName);
                            myWebClient.UploadFile(filePathWeb, filePath);
                        }
                        else
                        {
                            myWebClient.UploadFile($"{filePathWeb}/{folder}", filePath);
                        }

                    }

                    // result = "Carga Exitosa";
                    result = "1";
                }
            }
            catch (Exception ex)
            {

                result = ex.Message;
                // result = "-1";
            }

            return result;
        }

        [HttpPost]
        [Route("UploadWebV2")]
        public Task<HttpResponseMessage> UploadWebV2(FileInfo fileInfo)
        {
            try
            {

                


                var httpClient = new HttpClient();
                var multipartFormDataContent = new MultipartFormDataContent();
                httpClient.BaseAddress = new Uri(_settings.UrlPath);
                var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(fileInfo.FullName));
                multipartFormDataContent.Add(fileContent, "file", fileInfo.Name);
                return httpClient.PostAsync("upload", multipartFormDataContent);
            }
            catch
            {
                return null;
            }
        }

    }
}
