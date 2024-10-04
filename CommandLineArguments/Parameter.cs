namespace MacroscopCamMapper.CommandLineArguments;

public class Parameter
{
    public string[] Prefixes;
    public string Format;
    public bool IsRequired;
    public int SortingOrder;

    private readonly Func<string> _descriptionFormatter;
    public string Description() => _descriptionFormatter();

    private readonly Func<string[], string[]> _argsParser;
    internal string[] ParseArgs(string[] args)
        => _argsParser(args);

    /// <summary>
    /// Iinitializes the new Parameter.
    /// </summary>
    /// <param name="prefixes"><see cref="Array"/> of command line prefixies of parameter.</param>
    /// <param name="format">
    /// <para>Human-like description of type of values of parameter.</para>
    /// <para>Used just to describe what type of values user should pass as values to this parameter.</para>
    /// </param>
    /// <param name="parser">
    /// <para>Function that parse <see cref="Array"/> of command line arguments.</para>
    /// <para>Returns: <see cref="Array"/> of command line arguments without <paramref name="prefixes"/> and any parsed values.</para>
    /// </param>
    /// <param name="descriptionFormatter">Function that return param description with values in runtime.</param>
    /// <param name="isRequired">Should be true if program can not do work while user not specify this parameter.</param>
    /// <param name="sortingOrder">Оrder of sorting. Used to order params on help and set priority of params while handle command line arguments. Biggest is later.</param>
#pragma warning disable IDE0290 // Use primary constructor
    public Parameter(string[] prefixes, string format, Func<string[], string[]> parser,
#pragma warning restore IDE0290 // Use primary constructor
        Func<string> descriptionFormatter, bool isRequired = false, int sortingOrder = 1)
    {
        Prefixes = prefixes;
        Format = format;
        _descriptionFormatter = descriptionFormatter;
        IsRequired = isRequired;
        _argsParser = parser;
        SortingOrder = sortingOrder;
    }
}

public class Parameter<T>(string[] prefixes, T value, string format, Func<string[], string[]> parser,
    Func<string> descriptionFormatter, bool isRequired = false, int sortingOrder = 1)
    : Parameter(prefixes, format, parser, descriptionFormatter, isRequired, sortingOrder) where T : notnull
{
    public T Value = value;
}
