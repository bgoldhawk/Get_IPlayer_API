using System.Text;
using System.Web;
using CliWrap;
using Microsoft.AspNetCore.Mvc;

namespace Get_IPlayer_API.Controllers;

[ApiController]
[Route("[controller]")]
public class RecordingController : Controller
{
    private readonly ILogger<RecordingController> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _appName;
    private readonly string _saveDir;

    public RecordingController(ILogger<RecordingController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        _appName = configuration.GetValue<string>("AppName");
        _saveDir = configuration.GetValue<string>("SaveDir");
    }
    
    // GET
    [HttpGet("{url}")]

    public async Task<IActionResult> Index(string url, bool subtitles, string quality = "default")
    {
        var episode = url.Substring(37, 8);
        
        _logger.LogInformation("url: " + url);
        
        var stdOutBuffer = new StringBuilder();
        
        var result = await Cli.Wrap(_appName)
            .WithArguments($"{HttpUtility.UrlDecode(url)} --tv-quality=\"fhd\"")
            .WithWorkingDirectory(_saveDir)
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .ExecuteAsync();

        Console.WriteLine(result.ToString());
            
        //https://www.bbc.co.uk/iplayer/episodes/p0fy65w8/reframed-marilyn-monroe
        //https://www.bbc.co.uk/iplayer/episode/p0fy6732/reframed-marilyn-monroe-series-1-episode-1
        return Ok(stdOutBuffer.ToString());
    }
}