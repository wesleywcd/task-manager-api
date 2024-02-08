using System.Text;

namespace TaskAPI.Middleware;

public class LogsMiddleware
{
    private readonly RequestDelegate _req;

    public LogsMiddleware(RequestDelegate req)
    {
        _req = req;
    }
    
    public async Task Invoke(HttpContext context)
    {
        string logMessage = $"{DateTime.UtcNow.ToString("yyyy-dd-MM HH:mm:ss")} - {context.Request.Method}: {context.Request.Path}{context.Request.QueryString}";
        LogToFile(logMessage);

        await _req(context);
    }

    private void LogToFile(string message)
    {
        using (StreamWriter writer = new StreamWriter("log.txt", true, Encoding.UTF8))
        {
            writer.WriteLine(message);
        }
    }
}