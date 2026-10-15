using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using ProtocolLatencyAnalyzer.Core;

namespace ProtocolLatencyAnalyzer.Infrastructure;

public sealed class EthernetInterfaceProvider
{
    // Local inventory only: gateway presence does not establish a destination route.
    public IReadOnlyList<EthernetInterfaceInfo> GetInterfaces()
    {
        var interfaces = new List<EthernetInterfaceInfo>();
        foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
        {
            try
            {
                if (adapter.NetworkInterfaceType is not (NetworkInterfaceType.Ethernet or NetworkInterfaceType.Ethernet3Megabit
                    or NetworkInterfaceType.FastEthernetFx or NetworkInterfaceType.FastEthernetT or NetworkInterfaceType.GigabitEthernet)) continue;
                string? warning = null;
                IReadOnlyList<string> addresses = [];
                bool hasGateway = false;
                long speed = 0;
                try
                {
                    var properties = adapter.GetIPProperties();
                    addresses = properties.UnicastAddresses.Select(a => a.Address.ToString()).ToArray();
                    hasGateway = properties.GatewayAddresses.Any(g => IsUsableAddress(g.Address.ToString(), allowLinkLocal: true));
                }
                catch (Exception error) when (IsInventoryError(error)) { warning = "IP or gateway details unavailable; recommendation withheld."; }
                try { speed = adapter.Speed; }
                catch (Exception error) when (IsInventoryError(error)) { warning = warning is null ? "Link speed unavailable." : warning + " Link speed unavailable."; }
                interfaces.Add(new(adapter.Id, adapter.Name, adapter.OperationalStatus.ToString(), speed, addresses)
                {
                    Description = adapter.Description,
                    HasGateway = hasGateway,
                    IsLikelyVirtual = HasVirtualHint(adapter.Name, adapter.Description),
                    InventoryWarning = warning
                });
            }
            // An adapter may disappear during enumeration. Preserve the remaining inventory.
            catch (Exception error) when (IsInventoryError(error)) { }
        }
        return RankCandidates(interfaces);
    }

    public static bool HasVirtualHint(string name, string description)
    {
        string text = name + " " + description;
        string[] hints = ["virtual", "vethernet", "hyper-v", "vmware", "virtualbox", "vpn", "tap-windows", "tunnel", "docker", "wsl"];
        return hints.Any(hint => text.Contains(hint, StringComparison.OrdinalIgnoreCase));
    }

    // Pure ranking: no traffic, route probes, configuration changes or implicit selection.
    public static IReadOnlyList<EthernetInterfaceInfo> RankCandidates(IEnumerable<EthernetInterfaceInfo> interfaces)
    {
        var candidates = interfaces.ToArray();
        int Score(EthernetInterfaceInfo adapter) => adapter.OperationalStatus == "Up" && adapter.HasGateway
            && adapter.Addresses.Any(a => IsUsableAddress(a)) ? (adapter.IsLikelyVirtual ? 1 : 2) : 0;
        int best = candidates.Select(Score).DefaultIfEmpty(0).Max();
        int matches = best == 0 ? 0 : candidates.Count(a => Score(a) == best);
        return candidates.Select(adapter => adapter with
        {
            IsRecommended = best > 0 && Score(adapter) == best,
            RecommendationReason = best > 0 && Score(adapter) == best
                ? $"Connected with an IP address and configured gateway.{(matches > 1 ? " Multiple adapters match." : "")}{(adapter.IsLikelyVirtual ? " Name suggests a virtual adapter." : " No virtual name hint detected; hardware type unverified.")} Confirm this is your intended network; destination route is unverified."
                : adapter.OperationalStatus != "Up" ? "Not currently connected."
                : !adapter.Addresses.Any(a => IsUsableAddress(a)) ? "No usable IP address found; local-only or incomplete configuration."
                : !adapter.HasGateway ? "No configured gateway found; may still serve a local network."
                : "Connected with an IP address and gateway; another candidate has no virtual name hint. Confirm your intended network; destination route is unverified."
        }).OrderByDescending(a => a.IsRecommended).ThenByDescending(a => a.OperationalStatus == "Up")
            .ThenBy(a => a.Name, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    private static bool IsUsableAddress(string text, bool allowLinkLocal = false)
    {
        if (!IPAddress.TryParse(text, out var address) || IPAddress.IsLoopback(address)
            || address.Equals(IPAddress.Any) || address.Equals(IPAddress.IPv6Any) || address.IsIPv6Multicast) return false;
        if (address.AddressFamily == AddressFamily.InterNetworkV6) return allowLinkLocal || !address.IsIPv6LinkLocal;
        var bytes = address.GetAddressBytes();
        return bytes[0] is > 0 and < 224 && (allowLinkLocal || bytes[0] != 169 || bytes[1] != 254);
    }

    private static bool IsInventoryError(Exception error) => error is NetworkInformationException or PlatformNotSupportedException
        or InvalidOperationException or UnauthorizedAccessException;
}
