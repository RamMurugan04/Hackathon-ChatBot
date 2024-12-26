using ChatBot.Server.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {
        private readonly IModelHelper _modelHelper;

        public ChatBotController(IModelHelper modelHelper)
        {
            _modelHelper = modelHelper;
        }

        [HttpPost("upload")]
        public IActionResult UploadDocument(IFormFile file)
        {
            // Process and store the file
            return Ok(new { Message = "File uploaded successfully" });
        }

        [HttpGet("query")]
        public IActionResult QueryDocument(string query)
        {
            // Use LLM to process the query and return results
            return Ok(new { Answer = "Sample response" });
        }
    }
}
