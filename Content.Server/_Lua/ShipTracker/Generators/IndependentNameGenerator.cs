using Content.Server.Maps.NameGenerators;
using JetBrains.Annotations;
using Robust.Shared.Random;

namespace Content.Server._Lua.FTLPoints.Generators;

[UsedImplicitly]
public sealed partial class IndependentNameGenerator : StationNameGenerator
{
    /// <summary>
    ///     Where the map comes from. Should be a two or three letter code, for example "VG" for Packedstation.
    /// </summary>
    [DataField("prefixCreator")] public string PrefixCreator = default!;

    private string Prefix => "ISV";
    private string[] SuffixCodes => new []{ "LV", "NX", "EV", "QT", "PR" };

    public override string FormatName(string input)
    {
        var random = IoCManager.Resolve<IRobustRandom>();

        // No way in hell am I writing custom format code just to add nice names. You can live with {0}
        return string.Format(input, $"{Prefix}-{PrefixCreator}", $"{random.Pick(SuffixCodes)}-{random.Next(0, 999):D3}");
    }
}
