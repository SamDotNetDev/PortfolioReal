using Microsoft.AspNetCore.Mvc;
using Portfolio.Data;
using Portfolio.Models;
using Portfolio.Statuses;
using System.Globalization;
using System.Net.Http.Headers;

namespace Portfolio.Area.Admin
{
    [ApiController]
    [Route("api/Admin")]
    public class AdminController : ControllerBase
    {
        private readonly PortfolioDbContext _context;
        public AdminController(PortfolioDbContext context)
        {
            _context = context;
        }

        #region AboutMe

        [HttpGet("GetAboutMe")]
        public IActionResult GetInfo()
        {
            var model = _context.InfoAboutMe.FirstOrDefault();
            if (model != null)
            {
                return Ok(new Status() { Message = nameof(StatusMessage.Success), Data = model});
            }
            return Ok(new Status() { Message = nameof(StatusMessage.NotFound)});
        }

        [HttpPost("AboutMe")]
        public IActionResult AddAboutMe([FromForm]string AboutMe)
        {
            var existInfo = _context.InfoAboutMe.FirstOrDefault();
            if (existInfo != null)
            {
                _context.InfoAboutMe.Remove(existInfo);
            }

            Informations model = new()
            {
                AboutMe = AboutMe
            };
            _context.InfoAboutMe.Add(model);
            _context.SaveChanges();
            return Ok(new Status() { Message = nameof(StatusMessage.Success), Data = model });
        }
        #endregion

        #region CV

        [HttpGet("GetCV")]
        public IActionResult GetCVLink()
        {
            var model = _context.Works.FirstOrDefault();
            var file = _context.Works;
            if (model != null)
            {
                var pfd = _context.CvLink.Select(i => new
                {
                    Url = Url.Action(nameof(GetCV), new { id = i.Id, Request.Scheme })
                }).ToList();

                return Ok(new Status() { Message = nameof(StatusMessage.Success), Data = pfd });
            }

            return Ok(new Status() { Message = nameof(StatusMessage.NotFound) });
        }

        [HttpGet("GetCVToDownload")]
        public IActionResult GetCV()
        {
            var file = _context.CvLink.FirstOrDefault();
            if (file == null)
                return Ok(new Status() { Message = nameof(StatusMessage.NotFound) });

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "CV.pdf"
            };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");

            return File(file.CVLink, "application/pdf");
        }

        [HttpPost("UploadCV")]
        public IActionResult UploadCV([FromForm] Files file)
        {
            var existCV = _context.CvLink.FirstOrDefault();
            if (existCV != null)
                _context.CvLink.Remove(existCV);

            if (file == null || file.File.Length == 0)
                return Ok(new Status() { Message = nameof(StatusMessage.Error) });

            using (var ms = new MemoryStream())
            {
                file.File.CopyTo(ms);
                var fileBytes = ms.ToArray();
                var pdfFile = new CV { CVLink = fileBytes };
                _context.CvLink.Add(pdfFile);
                _context.SaveChanges();
                return Ok(new Status() { Message = nameof(StatusMessage.Success)});
            }
        }
        #endregion

        #region RecentWorks

        [HttpGet("RecentWorks")]
        public IActionResult GetAllRecentWorks()
        {
            var model = _context.Works.FirstOrDefault();
            var file = _context.Works;
            if (model != null)
            {
                var images = _context.Works.Select(i => new
                {
                    i.Id,
                    i.AboutProject,
                    Url = Url.Action(nameof(GetImage), new { id = i.Id ,Request.Scheme}) // Generating URL for downloading each image
                }).ToList();

                return Ok(new Status() { Message = nameof(StatusMessage.Success),Data = images});
            } 

            return Ok(new Status() { Message = nameof(StatusMessage.NotFound) });
        }

        [HttpGet("GetImageToDownload/{id}")]
        public IActionResult GetImage(int id)
        {
            var image = _context.Works.Find(id);
            if (image == null)
            {
                return Ok(new Status() { Message = nameof(StatusMessage.NotFound) });
            }

            // Return the image as a file
            return File(image.Image, "image/jpeg", $"{id}.jpg");
        }

        [HttpPost("RecentWokrs")]
        public IActionResult AddRecentWorks([FromForm] Files file, string About)
        {
            if (_context.Works.Count() < 10)
            {
                using (var ms = new MemoryStream())
                {
                    file.File.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    var jpgFile = new RecentWorks { Image = fileBytes,AboutProject = About };
                    _context.Works.Add(jpgFile);
                    _context.SaveChanges();
                    return Ok(new Status() { Message = nameof(StatusMessage.Success) });
                }
            }
            return Ok(new Status() { Message = nameof(StatusMessage.IndexOutOfBounds) });
        }

        [HttpDelete("DeleteRecentWork/{id}")]
        public IActionResult DeleteWork(int id)
        {
            var model = _context.Works.FirstOrDefault(x => x.Id == id);
            if (model == null)
                return Ok(new Status() { Message = nameof(StatusMessage.NotFound) });

            _context.Works.Remove(model);
            _context.SaveChanges();
            return Ok(new Status() { Message = nameof(StatusMessage.Success) });
        }
        #endregion
    }
}
