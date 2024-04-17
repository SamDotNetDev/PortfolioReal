using Microsoft.AspNetCore.Mvc;
using Portfolio.Data;
using Portfolio.Statuses;
using System.Net.Http.Headers;

namespace Portfolio.Area.User
{
    [ApiController]
    [Route("api/User")]
    public class UserController : ControllerBase
    {
        private readonly PortfolioDbContext _context;
        public UserController(PortfolioDbContext context)
        {
            _context = context;
        }

        #region About Me
        [HttpGet("GetAboutMe")]
        public IActionResult GetInfo()
        {
            var model = _context.InfoAboutMe.FirstOrDefault();
            if (model != null)
            {
                return Ok(new Status() { Message = nameof(StatusMessage.Success), Data = model });
            }
            return Ok(new Status() { Message = nameof(StatusMessage.NotFound) });
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
        #endregion

        #region Recent Works
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
                    Url = Url.Action(nameof(GetImage), new { id = i.Id, Request.Scheme }) // Generating URL for downloading each image
                }).ToList();

                return Ok(new Status() { Message = nameof(StatusMessage.Success), Data = images });
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
        #endregion
    }
}
