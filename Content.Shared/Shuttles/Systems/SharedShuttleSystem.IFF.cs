using System.Linq;
using Content.Shared._Mono.Company;
using Content.Shared.Shuttles.Components;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared.Shuttles.Systems;

public abstract partial class SharedShuttleSystem
{
    /*
     * Handles the label visibility on radar controls. This can be hiding the label or applying other effects.
     */

    protected virtual void UpdateIFFInterfaces(EntityUid gridUid, IFFComponent component) {}

    public Color GetIFFColor(EntityUid gridUid, bool self = false, IFFComponent? component = null)
    {
        if (self)
        {
            return IFFComponent.SelfColor;
        }

        if (!Resolve(gridUid, ref component, false))
        {
            return IFFComponent.IFFColor;
        }

        return component.Color;
    }

    public string? GetIFFLabel(EntityUid gridUid, bool self = false, IFFComponent? component = null)
    {
        var entName = MetaData(gridUid).EntityName;

        if (self)
        {
            return entName;
        }

        if (Resolve(gridUid, ref component, false) && (component.Flags & (IFFFlags.HideLabel | IFFFlags.Hide)) != 0x0)
        {
            return null;
        }

        var suffix = component != null ? GetServiceFlagsSuffix(component.ServiceFlags) : string.Empty;

        var labelText = string.IsNullOrEmpty(entName) ? Loc.GetString("shuttle-console-unknown") : entName + suffix;

        Color? companyColor = null;
        string? companyName = null;

        if (TryComp<_Mono.Company.CompanyComponent>(gridUid, out var companyComp) && !string.IsNullOrEmpty(companyComp.CompanyName))
        {
            var noneCompanyLocalized = Loc.GetString("company-none");

            if (IoCManager.Resolve<IPrototypeManager>().TryIndex<CompanyPrototype>(companyComp.CompanyName, out var prototype))
            {
                if (prototype.ID != "None")
                {
                    companyName = prototype.Name;
                    companyColor = prototype.Color;
                }
            }
            else
            {
                if (companyComp.CompanyName != noneCompanyLocalized)
                {
                    companyName = companyComp.CompanyName;
                    companyColor = Color.Yellow;
                }
            }
        }

        if (companyName != null && companyColor != null)
        {
            return $"{labelText}\n{companyName}";
        }

        return labelText;
    }

    /// <summary>
    /// Sets the color for this grid to appear as on radar.
    /// </summary>
    [PublicAPI]
    public void SetIFFColor(EntityUid gridUid, Color color, IFFComponent? component = null)
    {
        component ??= EnsureComp<IFFComponent>(gridUid);

        if (component.ReadOnly) // Frontier: POI IFF protection
            return; // Frontier: POI IFF protection

        if (component.Color.Equals(color))
            return;

        component.Color = color;
        Dirty(gridUid, component);
        UpdateIFFInterfaces(gridUid, component);
    }

    [PublicAPI]
    public void AddIFFFlag(EntityUid gridUid, IFFFlags flags, IFFComponent? component = null)
    {
        component ??= EnsureComp<IFFComponent>(gridUid);

        if (component.ReadOnly) // Frontier: POI IFF protection
            return; // Frontier: POI IFF protection

        if ((component.Flags & flags) == flags)
            return;

        component.Flags |= flags;
        Dirty(gridUid, component);
        UpdateIFFInterfaces(gridUid, component);
    }

    /// <summary>
    /// Frontier: Service flags
    /// Sets the service flags for this grid to appear as on radar.
    /// </summary>
    /// <param name="gridUid">The grid to set the flags for.</param>
    /// <param name="flags">The flags to set.</param>
    /// <param name="component">The IFF component to set the flags for.</param>
    public void SetServiceFlags(EntityUid gridUid, ServiceFlags flags, IFFComponent? component = null)
    {
        component ??= EnsureComp<IFFComponent>(gridUid);

        if (component.ReadOnly) // Frontier: POI IFF protection
            return; // Frontier: POI IFF protection

        if (component.ServiceFlags == flags)
            return;

        component.ServiceFlags = flags;
        Dirty(gridUid, component);
        UpdateIFFInterfaces(gridUid, component);
    }

    /// <summary>
    /// Turns the service flags into a string for display.
    /// IE. [M] for Medical, [R] for Research, etc.
    /// This function also handles duplicate first characters by using the first two characters of the flag name.
    /// </summary>
    /// <param name="flags">The IFF flags to get the suffix for</param>
    /// <returns>The string to display.</returns>
    public string GetServiceFlagsSuffix(ServiceFlags flags)
    {
        if (flags == ServiceFlags.None)
            return string.Empty;

        string outputString = "";
        foreach (var flag in Enum.GetValues<ServiceFlags>())
        {
            if (flag == ServiceFlags.None || !flags.HasFlag(flag))
                continue;

            if (Loc.TryGetString($"shuttle-console-service-flag-{flag}-shortform", out var flagString))
                outputString = string.Concat(outputString, flagString);
            else
                outputString = string.Concat(outputString, flag.ToString()[0]); // Fallback: use first character of string
        }
        return $"[{outputString}]";
    }

    [PublicAPI]
    public void RemoveIFFFlag(EntityUid gridUid, IFFFlags flags, IFFComponent? component = null)
    {
        if (!Resolve(gridUid, ref component, false))
            return;

        if (component.ReadOnly) // Frontier: POI IFF protection
            return; // Frontier: POI IFF protection

        if ((component.Flags & flags) == 0x0)
            return;

        component.Flags &= ~flags;
        Dirty(gridUid, component);
        UpdateIFFInterfaces(gridUid, component);
    }

    // Frontier: POI IFF protection
    [PublicAPI]
    public void SetIFFReadOnly(EntityUid gridUid, bool readOnly, IFFComponent? component = null)
    {
        if (!Resolve(gridUid, ref component, false))
            return;

        if (component.ReadOnly == readOnly)
            return;

        component.ReadOnly = readOnly;
    }
    // End Frontier
}
