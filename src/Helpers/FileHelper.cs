using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace Labelissimo.Helpers;

public class FileHelper
{
    private readonly ITelegramBotClient _client;

    public FileHelper(ITelegramBotClient client) => _client = client;

    public async Task<string> DownloadFileAsync(string fileId)
    {
        var path = Path.GetTempFileName();
        using var stream = System.IO.File.OpenWrite(path);
        await _client.GetInfoAndDownloadFileAsync(fileId, stream);
        return path;
    }

    public InputOnlineFile PrepareFile(byte[] file)
    {
        using var stream = new MemoryStream(file);
        return new InputOnlineFile(stream, "output.pdf"); //stream?
    }
}