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

        [HttpGet("RecentWorks")]
        public IActionResult GetAllRecentWorks()
        {
            var model = _context.Works.FirstOrDefault();
            var recentWorks = _context.Works;
            return model != null
                ? Ok(new Status() { Message = nameof(StatusMessage.Success), Data = model })
                : Ok(new Status() { Message = nameof(StatusMessage.NotFound) });
        }
    }
}
