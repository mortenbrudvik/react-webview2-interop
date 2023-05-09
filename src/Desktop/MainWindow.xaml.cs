using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using EmbedIO;
using EmbedIO.Files;

namespace Desktop;

public partial class MainWindow
{
    private readonly string _url;
    private WebServer _server;
    private readonly string _reactBuildPath;


    public MainWindow()
    {
        InitializeComponent();
            
        _url = "http://localhost:3005/";
            
        var projectPath = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
        var parentDirectory = Directory.GetParent(projectPath).Parent?.FullName;
        _reactBuildPath = Path.Combine(parentDirectory, @"Web\build");            

        webView.NavigationCompleted += WebView_NavigationCompleted;

        try
        {
            InitializeAsync();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async void InitializeAsync()
    {
        await webView.EnsureCoreWebView2Async(null);
        
        
        
        // var server = new SimpleHttpServer(_reactBuildPath, _url);
        //
        // // Start the server
        // await Task.Factory.StartNew(() => server.StartAsync());
        
        // // Create a local server with EmbedIO
        // _server = CreateWebServer(_url, @"path\to\your\react\build\folder");
        //
        // // Start the server
        // await Task.Factory.StartNew(() => StartWebServer(_server));

        // Navigate to the local server
        var indexPath = Path.Combine(_reactBuildPath, "index.html");
        
        //webView.CoreWebView2.Navigate($"file:///{indexPath}");
        webView.CoreWebView2.Navigate("http://localhost:3005/");
    }
    
    private static WebServer CreateWebServer(string url, string rootPath) =>
        new WebServer(o => o
                .WithUrlPrefix(url)
                .WithMode(HttpListenerMode.EmbedIO))
            .WithLocalSessionManager()
            .WithStaticFolder("/", rootPath, true, m => m.WithContentCaching());

    private static async void StartWebServer(IWebServer server) => await server.RunAsync();

    void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        Console.WriteLine(e.IsSuccess
            ? "Navigation to webpage completed successfully."
            : $"Navigation to webpage failed with error code: {e.WebErrorStatus}");
    }
}