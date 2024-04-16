using Microsoft.AspNetCore.Mvc;
using Portfolio.Data;
using Portfolio.Models;
using Portfolio.Statuses;
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
        public IActionResult AddAboutMe(Informations aboutMe)
        {
            var existInfo = _context.InfoAboutMe.FirstOrDefault();
            if (existInfo != null)
            {
                _context.InfoAboutMe.Remove(existInfo);
            }

            Informations model = new()
            {
                AboutMe = aboutMe.AboutMe
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
            var file = _context.CvLink.FirstOrDefault();
            if (file == null)
                return Ok(new Status() { Message = nameof(StatusMessage.NotFound) });

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "CV"
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

            if (file == null || file.PDF.Length == 0)
                return Ok(new Status() { Message = nameof(StatusMessage.Error) });

            using (var ms = new MemoryStream())
            {
                file.PDF.CopyTo(ms);
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
            var recentWorks = _context.Works;
            return model!=null 
                ? Ok(new Status() { Message = nameof(StatusMessage.Success), Data = model })
                : Ok(new Status() { Message = nameof(StatusMessage.NotFound) });
        }

        [HttpPost("RecentWokrs")]
        public IActionResult AddRecentWorks(RecentWorks resentWorks)
        {
            if (_context.Works.Count() < 10)
            {
                RecentWorks model = new()
                {
                    AboutProject = resentWorks.AboutProject
                };
                _context.Works.Add(model);
                _context.SaveChanges();
                return Ok(new Status() { Message = nameof(StatusMessage.Success), Data = model });
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
