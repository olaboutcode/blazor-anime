using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace BlazorAnime.UiTests;

[Collection(SampleServerCollection.Name)]
public sealed class HarnessTests : PageTest
{
    public HarnessTests(SampleServer server)
    {
        _ = server;
    }

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = SampleServer.BaseUrl,
        ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
    };

    [Fact]
    public async Task SeekingTheBoxMovesItAndReportsCompletion()
    {
        await OpenHarness();
        await Page.Locator("#seek-box").ClickAsync();

        await Until(async () =>
            Assert.Equal(120, await TranslateX(Page.Locator("#box")), precision: 0));
        await Expect(Page.Locator("#progress")).ToHaveTextAsync("100");
    }

    [Fact]
    public async Task PartialStaggerSeekMovesTheFirstDotBeforeTheLast()
    {
        await OpenHarness();
        await Page.Locator("#seek-stagger").ClickAsync();
        var dots = Page.Locator("#dots .dot");

        await Until(async () =>
        {
            Assert.True(await TranslateX(dots.Nth(0)) > 40);
            Assert.True(await TranslateX(dots.Nth(2)) < 1);
        });
    }

    private static async Task Until(Func<Task> assertion)
    {
        var deadline = DateTime.UtcNow.AddSeconds(15);
        Exception? last = null;
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                await assertion();
                return;
            }
            catch (Exception exception)
            {
                last = exception;
                await Task.Delay(100);
            }
        }

        throw last ?? new TimeoutException("The condition was not met.");
    }

    private async Task OpenHarness()
    {
        await Page.GotoAsync("/harness");
        await Expect(Page.Locator("#harness")).ToHaveAttributeAsync("data-ready", "true");
    }

    private static async Task<double> TranslateX(ILocator locator)
    {
        var transform = await locator.EvaluateAsync<string>(
            "element => getComputedStyle(element).transform");
        if (string.IsNullOrEmpty(transform) || transform == "none")
            return 0;

        const string matrixPrefix = "matrix(";
        if (transform.StartsWith(matrixPrefix, StringComparison.Ordinal))
        {
            var parts = transform[matrixPrefix.Length..].TrimEnd(')').Split(',');
            return double.Parse(parts[4], System.Globalization.CultureInfo.InvariantCulture);
        }

        const string translatePrefix = "translateX(";
        if (transform.StartsWith(translatePrefix, StringComparison.Ordinal) && transform.EndsWith("px)", StringComparison.Ordinal))
        {
            var number = transform[translatePrefix.Length..^3];
            return double.Parse(number, System.Globalization.CultureInfo.InvariantCulture);
        }

        return 0;
    }
}
