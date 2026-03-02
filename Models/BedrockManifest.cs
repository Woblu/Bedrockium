namespace AmethystLauncher.Models;

public class BedrockManifest
{
    public Header? Header { get; init; }
    public Module[] Modules { get; init; } = [];
}

public class Header
{
    public string Name { get; init; } = string.Empty;
}

public class Module
{
    public string Type { get; init; } = string.Empty;
}
