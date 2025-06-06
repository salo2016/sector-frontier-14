using System.Diagnostics.CodeAnalysis;
using Content.Server.DeviceNetwork.Components;
using Content.Server.Medical.CrewMonitoring;
using Content.Server.Station.Systems;
using Content.Shared.Power;
using Content.Shared.DeviceNetwork.Components;
using Robust.Shared.Map; // Frontier

namespace Content.Server.DeviceNetwork.Systems;

/// <summary>
/// Keeps one active server entity per station. Activates another available one if the currently active server becomes unavailable
/// Server in this context means an entity that manages the devicenet packets like the <see cref="Content.Server.Medical.CrewMonitoring.CrewMonitoringServerSystem"/>
/// </summary>
public sealed class SingletonDeviceNetServerSystem : EntitySystem
{
    [Dependency] private readonly DeviceNetworkSystem _deviceNetworkSystem = default!;
    // [Dependency] private readonly StationSystem _stationSystem = default!; // Frontier: map-wide singletons

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SingletonDeviceNetServerComponent, PowerChangedEvent>(OnPowerChanged);
    }

    /// <summary>
    /// Returns whether the given entity is an active server or not
    /// </summary>
    public bool IsActiveServer(EntityUid serverId, SingletonDeviceNetServerComponent? serverComponent = default)
    {
        return Resolve(serverId, ref serverComponent) && serverComponent.Active;
    }

    // Frontier: map-wide servers
    /// <summary>
    /// Returns the address of the currently active server for the given map (instead of station id) if there is one.<br/>
    /// What kind of server you're trying to get the active instance of is determined by the component type parameter TComp.<br/>
    /// <br/>
    /// Setting TComp to <see cref="CrewMonitoringServerComponent"/>, for example, gives you the address of an entity containing the crew monitoring server component.<br/>
    /// </summary>
    /// <param name="stationId">The entityUid of the station</param>
    /// <param name="address">The address of the active server if it exists</param>
    /// <typeparam name="TComp">The component type that determines what type of server you're getting the address of</typeparam>
    /// <returns>True if there is an active serve. False otherwise</returns>
    public bool TryGetActiveServerAddress<TComp>(MapId map, [NotNullWhen(true)] out string? address) where TComp : IComponent
    {
        var servers = EntityQueryEnumerator<
            SingletonDeviceNetServerComponent,
            DeviceNetworkComponent,
            TComp,
            TransformComponent // Frontier
        >();

        (EntityUid id, SingletonDeviceNetServerComponent server, DeviceNetworkComponent device)? last = default;

        while (servers.MoveNext(out var uid, out var server, out var device, out _, out var xform))
        {
            // Frontier PR 1053 QoL tweaks to displayed coordinates
            //if (!_stationSystem.GetOwningStation(uid)?.Equals(stationId) ?? true)
            if (xform.MapID != map) //Frontier
                continue;

            if (!server.Available)
            {
                DisconnectServer(uid,server, device);
                continue;
            }

            last = (uid, server, device);

            if (!server.Active || string.IsNullOrEmpty(device.Address))
                continue;

            address = device.Address;
            return true;
        }

        //If there was no active server for the station make the last available inactive one active
        if (last.HasValue)
        {
            ConnectServer(last.Value.id, last.Value.server, last.Value.device);
            address = last.Value.device.Address;
            return true;
        }

        address = null;
        return address != null;
    }
    // End Frontier

    // Lua start
    public bool TryGetActiveServerAddressGlobal<TComp>([NotNullWhen(true)] out string? address)
    where TComp : IComponent
    {
        var servers = EntityQueryEnumerator<
            SingletonDeviceNetServerComponent,
            DeviceNetworkComponent,
            TComp
        >();

        (EntityUid id, SingletonDeviceNetServerComponent server, DeviceNetworkComponent device)? lastAvailable = null;

        while (servers.MoveNext(out var uid, out var server, out var device, out _))
        {
            if (!server.Available)
            {
                DisconnectServer(uid, server, device);
                continue;
            }

            lastAvailable = (uid, server, device);

            if (server.Active)
            {
                address = device.Address;
                return true;
            }
        }

        if (lastAvailable.HasValue)
        {
            var (id, serv, dev) = lastAvailable.Value;
            ConnectServer(id, serv, dev);
            address = dev.Address;
            return true;
        }
        address = null;
        return false;
    }
    // Lua end

    /// <summary>
    /// Disconnects the server losing power
    /// </summary>
    private void OnPowerChanged(EntityUid uid, SingletonDeviceNetServerComponent component, ref PowerChangedEvent args)
    {
        component.Available = args.Powered;

        if (!args.Powered && component.Active)
            DisconnectServer(uid, component);
    }

    private void ConnectServer(EntityUid uid, SingletonDeviceNetServerComponent? server = null, DeviceNetworkComponent? device = null)
    {
        if (!Resolve(uid, ref server, ref device))
            return;

        server.Active = true;

        var connectedEvent = new DeviceNetServerConnectedEvent();
        RaiseLocalEvent(uid, ref connectedEvent);

        if (_deviceNetworkSystem.IsDeviceConnected(uid, device))
            return;

        _deviceNetworkSystem.ConnectDevice(uid, device);
    }

    /// <summary>
    /// Disconnects a server from the device network and clears the currently active server
    /// </summary>
    private void DisconnectServer(EntityUid uid, SingletonDeviceNetServerComponent? server = null, DeviceNetworkComponent? device = null)
    {
        if (!Resolve(uid, ref server, ref device))
            return;

        server.Active = false;

        var disconnectedEvent = new DeviceNetServerDisconnectedEvent();
        RaiseLocalEvent(uid, ref disconnectedEvent);

        _deviceNetworkSystem.DisconnectDevice(uid, device, false);
    }
}

/// <summary>
/// Raised when a server gets activated and connected to the device net
/// </summary>
[ByRefEvent]
public record struct DeviceNetServerConnectedEvent;

/// <summary>
/// Raised when a server gets disconnected
/// </summary>
[ByRefEvent]
public record struct DeviceNetServerDisconnectedEvent;
