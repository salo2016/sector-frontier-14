using Content.Server.Administration.Logs;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.Radio.EntitySystems; // Frontier
using Content.Server.Station.Systems;
using Content.Server.StationEvents.Components;
using Content.Shared.Database;
using Content.Shared.GameTicking.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Map;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.StationEvents.Events;

/// <summary>
///     An abstract entity system inherited by all station events for their behavior.
/// </summary>
public abstract class StationEventSystem<T> : GameRuleSystem<T> where T : IComponent
{
    [Dependency] protected readonly IAdminLogManager AdminLogManager = default!;
    [Dependency] protected readonly IPrototypeManager PrototypeManager = default!;
    [Dependency] protected readonly ChatSystem ChatSystem = default!;
    [Dependency] protected readonly SharedAudioSystem Audio = default!;
    [Dependency] protected readonly StationSystem StationSystem = default!;
    [Dependency] protected readonly RadioSystem RadioSystem = default!; // Frontier
    [Dependency] protected readonly MapSystem MapSystem = default!; // Frontier

    protected ISawmill Sawmill = default!;

    public override void Initialize()
    {
        base.Initialize();

        Sawmill = Logger.GetSawmill("stationevents");
    }

    protected virtual MapId GetRelevantMapId()
    {
        return GameTicker.DefaultMap;
    }

    /// <inheritdoc/>
    protected override void Added(EntityUid uid, T component, GameRuleComponent gameRule, GameRuleAddedEvent args)
    {
        base.Added(uid, component, gameRule, args);

        if (!TryComp<StationEventComponent>(uid, out var stationEvent))
            return;

        AdminLogManager.Add(LogType.EventAnnounced, $"Event added / announced: {ToPrettyString(uid)}");

        // we don't want to send to players who aren't in game (i.e. in the lobby)
        Filter allPlayersInGame = Filter.Empty().AddWhere(GameTicker.UserHasJoinedGame);

        // Lua
        if (stationEvent.StartAnnouncement != null)
        {
            var mapUid = MapSystem.GetMap(GetRelevantMapId()); //Lua
            var mapName = MetaData(mapUid).EntityName; //Lua
            var message = Loc.GetString(stationEvent.StartAnnouncement, ("mapdestination", mapName));
            ChatSystem.DispatchFilteredAnnouncement(allPlayersInGame, message, playSound: false, colorOverride: stationEvent.StartAnnouncementColor);
        }
        // Lua

        // Frontier
        if (stationEvent.StartRadioAnnouncement != null)
        {
            var mapUid = MapSystem.GetMap(GetRelevantMapId()); //Lua
            var mapName = MetaData(mapUid).EntityName; // Lua
            var message = Loc.GetString(stationEvent.StartRadioAnnouncement, ("mapdestination", mapName));
            RadioSystem.SendRadioMessage(uid, message, stationEvent.StartRadioAnnouncementChannel, mapUid, escapeMarkup: false);
        }

        Audio.PlayGlobal(stationEvent.StartAudio, allPlayersInGame, true);
    }

    /// <inheritdoc/>
    protected override void Started(EntityUid uid, T component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        if (!TryComp<StationEventComponent>(uid, out var stationEvent))
            return;

        AdminLogManager.Add(LogType.EventStarted, LogImpact.High, $"Event started: {ToPrettyString(uid)}");

        if (stationEvent.Duration != null)
        {
            var duration = stationEvent.MaxDuration == null
                ? stationEvent.Duration
                : TimeSpan.FromSeconds(RobustRandom.NextDouble(stationEvent.Duration.Value.TotalSeconds,
                    stationEvent.MaxDuration.Value.TotalSeconds));
            stationEvent.EndTime = Timing.CurTime + duration;
        }
    }

    /// <inheritdoc/>
    protected override void Ended(EntityUid uid, T component, GameRuleComponent gameRule, GameRuleEndedEvent args)
    {
        base.Ended(uid, component, gameRule, args);

        if (!TryComp<StationEventComponent>(uid, out var stationEvent))
            return;

        AdminLogManager.Add(LogType.EventStopped, $"Event ended: {ToPrettyString(uid)}");

        // we don't want to send to players who aren't in game (i.e. in the lobby)
        Filter allPlayersInGame = Filter.Empty().AddWhere(GameTicker.UserHasJoinedGame);

        // Lua
        if (stationEvent.EndAnnouncement != null)
        {
            var mapUid = MapSystem.GetMap(GetRelevantMapId()); //Lua
            var mapName = MetaData(mapUid).EntityName; //Lua
            var message = Loc.GetString(stationEvent.EndAnnouncement, ("mapdestination", mapName));
            ChatSystem.DispatchFilteredAnnouncement(allPlayersInGame, message, playSound: false, colorOverride: stationEvent.EndAnnouncementColor);
        }
        // Lua

        // Frontier: radio announcements
        if (stationEvent.EndRadioAnnouncement != null)
        {
            var mapUid = MapSystem.GetMap(GetRelevantMapId()); //Lua
            var mapName = MetaData(mapUid).EntityName; // Lua
            var message = Loc.GetString(stationEvent.EndRadioAnnouncement, ("mapdestination", mapName));
            RadioSystem.SendRadioMessage(uid, message, stationEvent.EndRadioAnnouncementChannel, mapUid, escapeMarkup: false);
        }
        // End Frontier

        Audio.PlayGlobal(stationEvent.EndAudio, allPlayersInGame, true);
    }

    /// <summary>
    ///     Called every tick when this event is running.
    ///     Events are responsible for their own lifetime, so this handles starting and ending after time.
    /// </summary>
    /// <inheritdoc/>
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<StationEventComponent, GameRuleComponent>();
        while (query.MoveNext(out var uid, out var stationEvent, out var ruleData))
        {
            if (!GameTicker.IsGameRuleAdded(uid, ruleData))
                continue;

            if (!GameTicker.IsGameRuleActive(uid, ruleData) && !HasComp<DelayedStartRuleComponent>(uid))
            {
                GameTicker.StartGameRule(uid, ruleData);
            }
            else if (stationEvent.EndTime != null && Timing.CurTime >= stationEvent.EndTime && GameTicker.IsGameRuleActive(uid, ruleData))
            {
                GameTicker.EndGameRule(uid, ruleData);
            }
            // Frontier: Added Warning for events ending soon
            else if (!stationEvent.WarningAnnounced && stationEvent.EndTime != null && (stationEvent.EndTime.Value - Timing.CurTime).TotalSeconds <= stationEvent.WarningDurationLeft && GameTicker.IsGameRuleActive(uid, ruleData))
            {
                Filter allPlayersInGame = Filter.Empty().AddWhere(GameTicker.UserHasJoinedGame); // we don't want to send to players who aren't in game (i.e. in the lobby)
                // Lua
                if (stationEvent.WarningAnnouncement != null)
                {
                    var mapUid = MapSystem.GetMap(GetRelevantMapId()); //Lua
                    var mapName = MetaData(mapUid).EntityName; // Lua
                    var message = Loc.GetString(stationEvent.WarningAnnouncement, ("mapdestination", mapName));
                    ChatSystem.DispatchFilteredAnnouncement(allPlayersInGame, message, playSound: false, colorOverride: stationEvent.WarningAnnouncementColor);
                }
                // Lua
                if (stationEvent.WarningRadioAnnouncement != null)
                {
                    var mapUid = MapSystem.GetMap(GetRelevantMapId()); //Lua
                    var mapName = MetaData(mapUid).EntityName; // Lua
                    var message = Loc.GetString(stationEvent.WarningRadioAnnouncement, ("mapdestination", mapName));
                    RadioSystem.SendRadioMessage(uid, message, stationEvent.WarningRadioAnnouncementChannel, mapUid, escapeMarkup: false);
                }
                Audio.PlayGlobal(stationEvent.WarningAudio, allPlayersInGame, true);
                stationEvent.WarningAnnounced = true;
            }
            // End Frontier
        }
    }
}
