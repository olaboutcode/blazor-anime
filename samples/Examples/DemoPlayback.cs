using BlazorAnime;

namespace Examples;

public static class DemoPlayback
{
    public static bool ShouldAutoPlay(bool? playing) => playing is null;

    public static async Task<bool?> Apply(Animation? animation, bool? playing, bool? applied)
    {
        if (animation is null || playing is null || applied == playing)
            return applied;

        if (playing.Value)
            await animation.Restart();
        else
            await animation.Reset();

        return playing;
    }
}
