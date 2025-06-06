using System.Numerics;
using Content.Client.UserInterface.Controls;
using Content.Shared._Lua.FtlPoints;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;

namespace Content.Client._Lua.FtlPoints;

[GenerateTypedNameReferences]
public sealed partial class StarmapConsole : FancyWindow
{
    private readonly StarmapConsoleBoundUserInterface _owner;
    private Star? _hoveredStar = null;

    public StarmapConsole(StarmapConsoleBoundUserInterface owner, IPrototypeManager protoManager)
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        _owner = owner;

        Title = Loc.GetString("starmap-computer-title");
        Stars.OnStarSelect += StarsOnOnStarSelect;
        StarWarpButton.OnPressed += args =>
        {
            if (_hoveredStar.HasValue)
            {
                _owner.StarWarpButtonOnOnPressed(_hoveredStar.Value);
            }
        };
    }

    private void StarsOnOnStarSelect(Star obj)
    {
        StarName.Text = obj.Name;
        StarCoordinates.Text = Loc.GetString("starmap-star-details-position", ("x", $"{obj.GlobalPosition.X:0.00)}"), ("y", $"{obj.GlobalPosition.Y:0.00}"));
        StarWarpButton.Disabled = CurrentStarName.Text == StarName.Text;
        _hoveredStar = obj;
    }

    public void UpdateState(StarmapConsoleBoundUserInterfaceState state)
    {
        UpdateStars(state.Stars);
        Stars.Range = state.Range;

        var currentStar = state.Stars.Find(star => star.Position == Vector2.Zero);
        CurrentStarName.Text = currentStar.Name;
    }

    private void UpdateStars(List<Star> stars)
    {
        Stars.SetStars(stars);
    }
}
