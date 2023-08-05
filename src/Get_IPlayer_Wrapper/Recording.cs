using System.Text;
using System.Web;
using CliWrap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Get_IPlayer_Wrapper;

public class Recording
{
    private readonly ILogger<Recording> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _appName;
    private readonly string _saveDir;

    public Recording(ILogger<Recording> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        
        _appName = configuration.GetValue<string>("AppName");
        _saveDir = configuration.GetValue<string>("SaveDir");
    }

    /// <summary>
    ///  Record the PIDs contained in the specified iPlayer episode URLs
    /// </summary>
    /// <param name="url">URL of episode or episodes to be recored</param>
    /// <param name="subtitles">True to download subtitles</param>
    /// <param name="quality">Select download quality. See TVQuality Constants for available options</param>
    public async Task Get(string url, bool subtitles,  string? quality = null)
    {
        _logger.LogInformation("url: " + url);
        
        var stdOutBuffer = new StringBuilder();

        var iplayerCommand = Cli.Wrap($"{_appName} {url}");

        if (!string.IsNullOrEmpty(quality))
            iplayerCommand.WithArguments($"--quality=\"{quality}\"");
        
        if(url.ToLower().Contains("episodes"))
            iplayerCommand.WithArguments($"--pid-recursive");

        if (subtitles)
            iplayerCommand.WithArguments("--subtitles");
        
        _logger.LogInformation("IPlayer command: {CommandAsString}",iplayerCommand.ToString());
        
        
        var result = await iplayerCommand
            .WithValidation(CommandResultValidation.None)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .ExecuteAsync();

        Console.WriteLine(result.ToString());
            
        //https://www.bbc.co.uk/iplayer/episodes/p0fy65w8/reframed-marilyn-monroe
        //https://www.bbc.co.uk/iplayer/episode/p0fy6732/reframed-marilyn-monroe-series-1-episode-1
        return Ok(stdOutBuffer.ToString());
    }
    
    
}
