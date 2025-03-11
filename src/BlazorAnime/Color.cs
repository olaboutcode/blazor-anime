namespace BlazorAnime;

public class Color
{
    /// <summary>
    /// Defines the color by Hex value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Color Hex(string value) =>
        new(value.StartsWith('#') ? value : $"#{value}");
    
    /// <summary>
    /// Defines the color by RGA values
    /// </summary>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="blue"></param>
    /// <returns></returns>
    public static Color Rgb(int red, int green, int blue) =>
        new($"rgb({red}, {green}, {blue})");
    
    /// <summary>
    /// Defines the color by RGBA values
    /// </summary>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="blue"></param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static Color Rgba(int red, int green, int blue, double alpha) =>
        new($"rgb({red}, {green}, {blue}, {alpha})");
    
    /// <summary>
    /// Defines the color by HSL values
    /// </summary>
    /// <param name="hue"></param>
    /// <param name="saturation"></param>
    /// <param name="lightness"></param>
    /// <returns></returns>
    public static Color Hsl(int hue, string saturation, string lightness) =>
        new($"hsl({hue}, {saturation}, {lightness})");
    
    /// <summary>
    /// Defines the color by HSLA values
    /// </summary>
    /// <param name="hue"></param>
    /// <param name="saturation"></param>
    /// <param name="lightness"></param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    public static Color Hsla(int hue, string saturation, string lightness, double alpha) =>
        new($"hsla({hue}, {saturation}, {lightness}, {alpha})");

    public string GetValue() => Value;

    private string Value { get; }

    private Color(string value) => Value = value;
}