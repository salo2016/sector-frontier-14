using Content.Client.Stylesheets;
using Content.Client.UserInterface.Systems.Ghost.Controls;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;
using Robust.Shared.Configuration;
using Content.Client._NF.CryoSleep; // Frontier

namespace Content.Client.UserInterface.Systems.Ghost.Widgets;

[GenerateTypedNameReferences]
public sealed partial class GhostGui : UIWidget
{
    [Dependency] private readonly IGameTiming _gameTiming = default!; // Frontier
    private TimeSpan? _respawnTime; // Frontier

    private TimeSpan? _cryoReturnTime; //Lua

    public GhostTargetWindow TargetWindow { get; }
    public GhostRespawnRulesWindow RulesWindow { get; } // Frontier
    public CryosleepWakeupWindow CryosleepWakeupWindow { get; } // Frontier

    public event Action? RequestWarpsPressed;
    public event Action? ReturnToBodyPressed;
    public event Action? GhostRolesPressed;
    public event Action? GhostRespawnPressed; // Frontier
    private int _prevNumberRoles;

    public GhostGui()
    {
        RobustXamlLoader.Load(this);

        TargetWindow = new GhostTargetWindow();
        RulesWindow = new GhostRespawnRulesWindow(); // Frontier
        CryosleepWakeupWindow = new CryosleepWakeupWindow(); // Frontier
        RulesWindow.RespawnButton.OnPressed += _ => GhostRespawnPressed?.Invoke(); // Frontier

        MouseFilter = MouseFilterMode.Ignore;

        GhostWarpButton.OnPressed += _ => RequestWarpsPressed?.Invoke();
        ReturnToBodyButton.OnPressed += _ => ReturnToBodyPressed?.Invoke();
        GhostRolesButton.OnPressed += _ => GhostRolesPressed?.Invoke();
        GhostRolesButton.OnPressed += _ => GhostRolesButton.StyleClasses.Remove(StyleBase.ButtonCaution);
        GhostRespawnButton.OnPressed += _ => RulesWindow.OpenCentered(); // Frontier
        CryosleepReturnButton.OnPressed += _ => CryosleepWakeupWindow.OpenCentered(); // Frontier
    }

    public void Hide()
    {
        TargetWindow.Close();
        Visible = false;
    }

    public void UpdateRespawn(TimeSpan? respawnTime)
    {
        _respawnTime = respawnTime;
    }

    //Lua start
    public void UpdateCryoReturn(TimeSpan? cryoReturnTime)
    {
        _cryoReturnTime = cryoReturnTime;
    }
    //Lua end

    public void Update(int? roles, bool? canReturnToBody, bool? canUncryo)
    {
        ReturnToBodyButton.Disabled = !canReturnToBody ?? true;

        if (roles != null)
        {
            GhostRolesButton.Text = Loc.GetString("ghost-gui-ghost-roles-button", ("count", roles));

            if (roles > _prevNumberRoles)
            {
                GhostRolesButton.StyleClasses.Add(StyleBase.ButtonCaution);
            }

            _prevNumberRoles = (int)roles;
        }

        TargetWindow.Populate();

        CryosleepReturnButton.Disabled = !canUncryo ?? true; // Frontier
    }

    // Frontier: respawn logic
    protected override void FrameUpdate(FrameEventArgs args)
    {
        if (_respawnTime is null || _gameTiming.CurTime > _respawnTime)
        {
            GhostRespawnButton.Text = Loc.GetString("ghost-gui-respawn-button-allowed");
            GhostRespawnButton.Disabled = false;
        }
        else
        {
            double delta = (_respawnTime.Value - _gameTiming.CurTime).TotalSeconds;
            GhostRespawnButton.Text = Loc.GetString("ghost-gui-respawn-button-denied", ("time", $"{delta:f1}"));
            GhostRespawnButton.Disabled = true;
        }

        // Lua start ������ �������� �� ����
        if (_cryoReturnTime is null || _gameTiming.CurTime > _cryoReturnTime)
        {
            CryosleepReturnButton.Text = Loc.GetString("cryo-wakeup-window-title");
            CryosleepReturnButton.Disabled = false;
        }
        else
        {
            double delta = (_cryoReturnTime.Value - _gameTiming.CurTime).TotalSeconds;
            CryosleepReturnButton.Text = Loc.GetString("cryo-wakeup-window-denied-title", ("time", $"{delta:f1}"));
            CryosleepReturnButton.Disabled = true;
        }
        //Lua end
    }
    // End Frontier: respawn logic

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            TargetWindow.Dispose();
        }
    }
}
