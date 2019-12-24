using OfficeApp.App_Start;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace OfficeApp.Controllers
{
    [RoutePrefix("api/file")]
    public class FileController : ApiController
    {
        private readonly string _physicalPath;
        private readonly string _virtualPath;
        private readonly Random _random;

        FileController()
        {
            var host = 
                HttpContext.Current.Request.Url.Scheme + 
                Uri.SchemeDelimiter + 
                HttpContext.Current.Request.Url.Host + ":" + 
                HttpContext.Current.Request.Url.Port;

            _physicalPath = HttpContext.Current.Server.MapPath(Constants.UPLOADS_PATH);
            _virtualPath = Constants.UPLOADS_PATH.Replace("~", host);
            _random = new Random();
        }

        [HttpGet]
        public IHttpActionResult GetFiles()
        {
            return Ok(new
            {
                Status = true,
                Files = new DirectoryInfo(_physicalPath)
                            .GetFiles()
                            .Select(i => new
                            {
                                i.Name,
                                Url = GetVirtualFilePath(i.Name)
                            })
            });
        }

        [HttpGet]
        public HttpResponseMessage GetPhysicalFile(string fileName)
        {
            //Create HTTP Response.
            var response = Request.CreateResponse(HttpStatusCode.OK);

            //Set the File Path.
            string filePath = GetPhysicalFilePath(fileName);

            //Check whether File exists.
            if (!File.Exists(filePath))
            {
                //Throw 404 (Not Found) exception if File not found.
                response.StatusCode = HttpStatusCode.NotFound;
                response.ReasonPhrase = string.Format("File not found: {0} .", fileName);
                throw new HttpResponseException(response);
            }

            //Read the File into a Byte Array.
            byte[] bytes = File.ReadAllBytes(filePath);

            //Set the Response Content.
            response.Content = new ByteArrayContent(bytes);

            //Set the Response Content Length.
            response.Content.Headers.ContentLength = bytes.LongLength;

            //Set the Content Disposition Header Value and FileName.
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = fileName;

            //Set the File Content Type.
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
            return response;
        }

        [HttpPost]
        public IHttpActionResult UploadFile()
        {
            //var napTime = _random.Next(1000, 5000);
            //Thread.Sleep(napTime);

            try
            {
                var number = _random.Next(0, 9);
                if (number % 2 == 0)
                {
                    return Ok(new
                    {
                        Status = false,
                        Message = "Something went wrong",
                        Url = ""
                    });
                }

                var file = HttpContext.Current.Request.Files.Count > 0 ?
                HttpContext.Current.Request.Files[0] : null;

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    var path = Path.Combine(
                        _physicalPath,
                        fileName
                    );

                    file.SaveAs(path);
                }

                return Ok(new
                {
                    Status = true,
                    Message = "File uploaded successfully",
                    Url = GetVirtualFilePath(file.FileName),
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Status = false,
                    Message = "Something went wrong",
                    Url = "",
                    Exception = ex
                });
            }
        }

        [HttpDelete]
        public IHttpActionResult DeleteFile(string fileName)
        {
            try
            {
                if (!File.Exists(GetPhysicalFilePath(fileName)))
                {
                    return Ok(new
                    {
                        Status = false,
                        Message = "File doesn't exist",
                        Url = GetVirtualFilePath(fileName),
                    });
                }

                File.Delete(GetPhysicalFilePath(fileName));

                return Ok(new
                {
                    Status = true,
                    Message = "File deleted successfully",
                    Url = GetVirtualFilePath(fileName),
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Status = false,
                    Url = "",
                    Exception = ex
                });
            }
        }

        [HttpDelete]
        public IHttpActionResult DeleteAllFile()
        {
            try
            {
                var fileNames = new DirectoryInfo(_physicalPath).GetFiles().Select(i => i.Name);
                foreach (var fileName in fileNames)
                {
                    var filePath = GetPhysicalFilePath(fileName);
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }

                return Ok(new
                {
                    Status = true,
                    Message = "File deleted successfully"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Status = false,
                    Url = "",
                    Exception = ex
                });
            }
        }

        private string GetVirtualFilePath(string fileName)
        {
            return _virtualPath + "/" + fileName;
        }

        private string GetPhysicalFilePath(string fileName)
        {
            return _physicalPath + "\\" + fileName;
        }
    }
}
