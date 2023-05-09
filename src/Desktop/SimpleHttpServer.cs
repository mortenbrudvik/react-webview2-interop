using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Desktop;

public class SimpleHttpServer
{
    private readonly HttpListener _listener = new HttpListener();
    private readonly string _folder;

    public SimpleHttpServer(string folder, string url)
    {
        _folder = folder;

        _listener.Prefixes.Add(url);
        _listener.Start();
    }

    public async Task StartAsync()
    {
        while (_listener.IsListening)
        {
            var context = await _listener.GetContextAsync();

            var path = context.Request.Url.AbsolutePath;
            var filename = Path.Combine(_folder, path.TrimStart('/'));

            if (File.Exists(filename))
            {
                try
                {
                    var content = await File.ReadAllBytesAsync(filename);
                    await context.Response.OutputStream.WriteAsync(content, 0, content.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                context.Response.StatusCode = 404;
            }

            context.Response.Close();
        }
    }

    public void Stop()
    {
        _listener.Stop();
        _listener.Close();
    }
}