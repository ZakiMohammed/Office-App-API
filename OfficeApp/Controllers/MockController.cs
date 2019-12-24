using OfficeApp.App_Start;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace OfficeApp.Controllers
{
    [RoutePrefix("api/mock")]
    public class MockController : ApiController
    {
        private string _apiUrl;
        private readonly string _physicalPath;
        private readonly string _virtualPath;

        MockController()
        {
            _apiUrl = 
                HttpContext.Current.Request.Url.Scheme +
                Uri.SchemeDelimiter +
                HttpContext.Current.Request.Url.Host + ":" +
                HttpContext.Current.Request.Url.Port + "/";
            _physicalPath = HttpContext.Current.Server.MapPath(Constants.UPLOADS_MOCK_PATH);
            _virtualPath = Constants.UPLOADS_MOCK_PATH.Replace("~", _apiUrl);
        }

        [HttpPost]
        public IHttpActionResult Post(dynamic document)
        {
            return Ok(new
            {
                requestId = GetRandomKey(),
                presignedURL = _apiUrl + "api/mock/" + document.DocumentId
            });
        }

        [HttpPut]
        [Route("{documentId}")]
        public IHttpActionResult Put(string documentId)
        {
            var file = 
                HttpContext.Current.Request.Files.Count > 0 ?
                HttpContext.Current.Request.Files[0] : null;

            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);

                var path = Path.Combine(
                    _physicalPath,
                    documentId + "." + fileName.Split('.')[1]
                );

                file.SaveAs(path);
            }

            return Ok();
        }

        [HttpGet]
        [Route("{documentId}")]
        public IHttpActionResult Get(string documentId)
        {
            return Ok(new
            {
                requestId = "d4792a05-cf69-4cf9-9194-133794d23b00",
                document = new
                {
                    AssessmentId = "assessment-101010",
                    OriginalFileExtension = "PDF",
                    UploadedOn = new { },
                    DocumentId = "e24f5b60-257e-11ea-a12a-b392253d3ab8",
                    LastAccessedOn = new { },
                    UploadedBy = 10,
                    ChapterId = "EP-101",
                    FileStatus = "ready",
                    LastAccessedBy = 10,
                    OriginalFileName = "SanctionPolicy",
                    FileSize = 34617
                },
                presignedURL = _apiUrl + "api/mock/" + documentId
            });
        }

        private string GetRandomKey()
        {
            var random = new Random();

            string RandomString(int length)
            {                
                const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
                return new string(Enumerable.Repeat(chars, length)
                  .Select(s => s[random.Next(s.Length)]).ToArray());
            }

            return 
                RandomString(8) + "-" + 
                RandomString(4) + "-" + 
                RandomString(4) + "-" + 
                RandomString(12);
        }
    }
}
