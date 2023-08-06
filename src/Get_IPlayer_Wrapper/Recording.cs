using System.Text;
using System.Web;
using CliWrap;
using CliWrap.Buffered;
using CliWrap.EventStream;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Get_IPlayer_Wrapper;

public class Recording : IRecording
{
    private readonly ILogger<Recording> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _appName;
    private readonly string? _proxy;
    private readonly string? _workingDir;

    public Recording(ILogger<Recording> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        
        _appName = configuration.GetValue<string>("AppName");
        _workingDir =  configuration.GetValue<string>("WorkingDir");
        _proxy = configuration.GetValue<string>("Proxy");
    }
    
    public async Task<string> Get(string url, bool subtitles,  string? quality = null)                                                                                                                                                             
    {
        _logger.LogInformation("url: " + url);
        
        var stdOutBuffer = new StringBuilder();

        var iplayerCommand = Cli.Wrap(_appName)
            .WithArguments(args =>
            {
                args.Add(url);
                
                if (quality is not null)
                {
                    args.Add("--quality")
                    .Add(quality);
                }
                    
        
                if(url.ToLower().Contains("episodes"))
                    args.Add($"--pid-recursive");

                if (subtitles)
                    args.Add("--subtitles");

                if (_proxy is not null)
                {
                    args.Add($"--proxy")
                    .Add(_proxy);
                }
                    
            });

        
        
        _logger.LogInformation("IPlayer command: {CommandAsString}",iplayerCommand.ToString());
        
        
        var result = iplayerCommand
            .WithValidation(CommandResultValidation.None)
            .WithWorkingDirectory(_workingDir);

        var processId = string.Empty;

        await foreach(var cmdEvent in iplayerCommand.ListenAsync())
        {
            switch (cmdEvent)
            {
                case StartedCommandEvent started:
                    processId = started.ProcessId.ToString();
                    _logger.LogInformation($"Process started; ID: {started.ProcessId}");
                    break;
                case StandardOutputCommandEvent stdOut:
                    _logger.LogDebug($"Out> {stdOut.Text}");
                    break;
                case StandardErrorCommandEvent stdErr:
                    _logger.LogError($"Err> {stdErr.Text}");
                    break;
                case ExitedCommandEvent exited:
                    _logger.LogInformation($"Process exited; Code: {exited.ExitCode}");
                    break;
            }
        }

        return processId;
    }
    
    
}

public interface IRecording
{
    /// <summary>
    ///  Record the PIDs contained in the specified iPlayer episode URLs
    /// </summary>
    /// <param name="url">URL of episode or episodes to be recored</param>
    /// <param name="subtitles">True to download subtitles</param>
    /// <param name="quality">Select download quality. See TVQuality Constants for available options</param>
    Task<string> Get(string url, bool subtitles, string? quality = null);
}
