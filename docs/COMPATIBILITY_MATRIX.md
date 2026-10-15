# Compatibility matrix

Documentation support for a framework is distinct from tested application/package/capture support.

| Operating system | .NET/WPF basis | Portable application | Native capture | Npcap | Privileges and limits |
|---|---|---|---|---|---|
| Windows 11 x64 | Supported by Documentation | Needs Testing | Needs Testing | Needs Testing | Phase 1 standard-user target; capture later |
| Server 2019 x64 | Supported by Documentation | Needs Testing | Needs Testing; highest feature-variation concern | Needs Testing | Desktop Experience baseline proposed; Core not qualified |
| Server 2022 x64 | Supported by Documentation | Needs Testing | Needs Testing | Needs Testing | Desktop Experience baseline proposed; capture privileges later |
| Server 2025 x64 | Supported by Documentation | Needs Testing | Needs Testing | Needs Testing | Exact driver/package combinations unqualified |
| Linux | Core reuse objective | No application implementation | Future backend | Windows Npcap not applicable | Future UI, permissions, packaging, distribution matrix required |
| macOS | No current commitment | Unsupported by current WPF application | Not in scope | Not applicable | No support claim |

No row means application compatibility was verified. Per-OS release acceptance requires actual self-contained package launch, read/write/session workflows, shutdown, and separately qualified capture/tests when implemented. The current host is not all four environments.

References: [official .NET 10 supported OS list](https://github.com/dotnet/core/blob/main/release-notes/10.0/supported-os.md), [WPF Windows desktop overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/), [Server Core overview](https://learn.microsoft.com/windows-server/administration/server-core/what-is-server-core).
