using System.Diagnostics;
using System.Text;
using Xunit;

namespace BlazorAnime.UiTests;

public sealed class SampleServer : IAsyncLifetime
{
    public const string BaseUrl = "http://127.0.0.1:5161";

    private Process? _process;
    private readonly StringBuilder _output = new();

    public async Task InitializeAsync()
    {
        if (await IsReady())
            return;

        var repoRoot = FindRepoRoot();
        var project = Path.Combine(repoRoot, "samples", "Examples.WebAssembly", "Examples.WebAssembly.csproj");
        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{project}\" --no-launch-profile --urls {BaseUrl}",
                WorkingDirectory = repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };
        _process.OutputDataReceived += (_, eventArgs) => Append(eventArgs.Data);
        _process.ErrorDataReceived += (_, eventArgs) => Append(eventArgs.Data);
        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        var deadline = DateTime.UtcNow.AddMinutes(3);
        while (DateTime.UtcNow < deadline)
        {
            if (_process.HasExited)
                throw new InvalidOperationException($"The sample server exited early.{Environment.NewLine}{_output}");
            if (await IsReady())
                return;
            await Task.Delay(500);
        }

        throw new TimeoutException($"The sample server did not answer {BaseUrl}/harness.{Environment.NewLine}{_output}");
    }

    public Task DisposeAsync()
    {
        if (_process is { HasExited: false })
            _process.Kill(entireProcessTree: true);
        _process?.Dispose();
        return Task.CompletedTask;
    }

    private void Append(string? line)
    {
        if (line is null)
            return;
        lock (_output)
            _output.AppendLine(line);
    }

    private static async Task<bool> IsReady()
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
            using var response = await client.GetAsync($"{BaseUrl}/harness");
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "BlazorAnime.slnx")))
                return directory.FullName;
            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not find BlazorAnime.slnx above the test assembly.");
    }
}

[CollectionDefinition(Name)]
public sealed class SampleServerCollection : ICollectionFixture<SampleServer>
{
    public const string Name = "sample-server";
}
