namespace BlazorAnime;

public static class TextPropsBuilderExtensions
{
    /// <summary>Sets <c>innerHTML</c> to a <c>scrambleText</c> function.</summary>
    public static PropsBuilder InnerHtml(this PropsBuilder builder, TextScramble scramble)
    {
        ArgumentNullException.ThrowIfNull(scramble);
        return builder.Prop("innerHTML", scramble.Reference);
    }
}
