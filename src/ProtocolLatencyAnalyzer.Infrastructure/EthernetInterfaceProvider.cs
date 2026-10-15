using System.Net.NetworkInformation;
using ProtocolLatencyAnalyzer.Core;

namespace ProtocolLatencyAnalyzer.Infrastructure;

public sealed class EthernetInterfaceProvider
{
    // Inventory only. Ethernet type can include virtual adapters; it does not prove physical media or routing.
    public IReadOnlyList<EthernetInterfaceInfo> GetInterfaces() => NetworkInterface.GetAllNetworkInterfaces()
        .Where(n => n.NetworkInterfaceType is NetworkInterfaceType.Ethernet or NetworkInterfaceType.Ethernet3Megabit
            or NetworkInterfaceType.FastEthernetFx or NetworkInterfaceType.FastEthernetT or NetworkInterfaceType.GigabitEthernet)
        .Select(n => new EthernetInterfaceInfo(n.Id, n.Name, n.OperationalStatus.ToString(), n.Speed,
            n.GetIPProperties().UnicastAddresses.Select(a => a.Address.ToString()).ToArray()))
        .OrderBy(n => n.Name, StringComparer.OrdinalIgnoreCase).ToArray();
}
