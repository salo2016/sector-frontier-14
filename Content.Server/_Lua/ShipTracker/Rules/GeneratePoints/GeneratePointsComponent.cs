namespace Content.Server._Lua.ShipTracker.Rules.GeneratePoints;

[RegisterComponent, Access(typeof(GeneratePointsSystem))]
public sealed partial class GeneratePointsComponent : Component
{
    public bool Generated;
}
