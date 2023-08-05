using System.Text;
using System.Web;
using CliWrap;
using Get_IPlayer_Wrapper;
using Get_IPlayer_Wrapper.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Get_IPlayer_API.Controllers;

[ApiController]
[Route("[controller]")]
public class RecordingController : Controller
{
    private readonly IRecording _recording;
    private readonly ILogger<RecordingController> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _appName;
    private readonly string _saveDir;

    public RecordingController(IRecording recording)
    {
        _recording = recording;
    }
    
    // GET
    [HttpGet("{url}/{subtitles:bool}/{quality}")]
    public async Task<IActionResult> Index(string url, bool subtitles = false, string quality = "default")
    {
        var result = await _recording.Get(HttpUtility.UrlDecode(url), subtitles, quality); 
        return Ok(result);
    }
}