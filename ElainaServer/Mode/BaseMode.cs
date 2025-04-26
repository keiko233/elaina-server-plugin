using CounterStrikeSharp.API.Core.Translations;
using CounterStrikeSharp.API;

namespace ElainaServer;

public abstract class BaseMode
{
    abstract public string ModeName { get; }
    abstract public string ModeDescription { get; }

    public BaseMode(ElainaServer plugin)
    {
        RegisterCommand(plugin);
    }

    public BaseMode(BaseMode plugin) { }

    public abstract void OnModeLoad(ElainaServer plugin);

    public abstract void OnModeUnload(ElainaServer plugin);

    public virtual void RegisterCommand(ElainaServer plugin) { }

    public virtual void PrintModeDescriptionToChatAll(ElainaServer plugin)
    {
        Server.PrintToChatAll(StringExtensions.ReplaceColorTags($"{{GREEN}}[{plugin.ModuleName}] {{RED}}[{ModeName}]{{WHITE}}: {ModeDescription}"));
    }
}
