// Мёртвый Космос, Licensed under custom terms with restrictions on public hosting and commercial use, full text: https://raw.githubusercontent.com/dead-space-server/space-station-14-fobos/master/LICENSE.TXT
//This content is sourced from Мёртвый Космос and is used with explicit permission for use in Sector Frontier(LuaWorld) https://github.com/HacksLua/sector-frontier-14.
// Мёртвый Космос - This file is licensed under AGPLv3
// Copyright (c) 2025 Мёртвый Космос Contributors
// See AGPLv3.txt for details.

using Content.Shared._DeadSpace.Photocopier;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Client._DeadSpace.Photocopier.UI;

[UsedImplicitly]
public sealed class PhotocopierBoundUi : BoundUserInterface
{
    [ViewVariables]
    private PhotocopierWindow? _window;

    public PhotocopierBoundUi(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = new PhotocopierWindow(IoCManager.Resolve<IPrototypeManager>());
        _window.OpenCentered();

        _window.OnClose += Close;
        _window.FormButtonPressed += OnFormButtonPressed;
        _window.PrintButtonPressed += OnPrintButtonPressed;
        _window.CopyModeButtonPressed += OnCopyModeButtonPressed;
        _window.PrintModeButtonPressed += OnPrintModeButtonPressed;
    }

    private void OnFormButtonPressed(PaperworkFormPrototype formPrototype)
    {
        SendMessage(new PhotocopierChoseFormMessage(formPrototype));
    }

    private void OnPrintButtonPressed(int amount, PhotocopierMode mode)
    {
        SendMessage(new PhotocopierPrintMessage(amount, mode));
    }

    private void OnCopyModeButtonPressed()
    {
        SendMessage(new PhotocopierCopyModeMessage());
    }

    private void OnPrintModeButtonPressed()
    {
        SendMessage(new PhotocopierPrintModeMessage());
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (_window == null || state is not PhotocopierUiState cast)
            return;

        _window.PopulateCategories();
        _window.PopulateForms();
        _window.UpdateState(cast);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _window?.Dispose();
    }
}
