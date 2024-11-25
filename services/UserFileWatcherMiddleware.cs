using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Cargohub.models;
using AuthProvider = Cargohub.services.AuthProvider;

public class UserFileWatcherMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _filePath;
    private FileSystemWatcher _fileWatcher;

    public UserFileWatcherMiddleware(RequestDelegate next, string filePath)
    {
        _next = next;
        _filePath = filePath;
        InitializeFileWatcher();
    }

    private void InitializeFileWatcher()
    {
        _fileWatcher = new FileSystemWatcher(Path.GetDirectoryName(_filePath))
        {
            Filter = Path.GetFileName(_filePath),
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
        };

        _fileWatcher.Changed += OnChanged;
        _fileWatcher.Created += OnChanged;
        _fileWatcher.Deleted += OnChanged;
        _fileWatcher.Renamed += OnRenamed;

        _fileWatcher.EnableRaisingEvents = true;
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        // Reload user data
        AuthProvider.ReloadUsers();
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        // Reload user data
        AuthProvider.ReloadUsers();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        var msg = $"{context.Request.Path} was handled with status code {context.Response.StatusCode}\n";
        await File.AppendAllTextAsync("log.txt", msg);
    }
}