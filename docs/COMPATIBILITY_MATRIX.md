# Compatibility matrix

Documentation support for a framework is distinct from tested application/package/capture support.

| Operating system | .NET/WPF basis | Portable application | Native capture | Npcap | Privileges and limits |
|---|---|---|---|---|---|
| Windows 11 x64 | Supported by Documentation | Build 26200 local package/workflows passed; USB smoke passed during approved copy hold, full USB gates after copy completion pending; clean machine pending | Needs Testing | Needs Testing | Medium-integrity launcher observed; copy-related USB contention demonstrated; broader release qualification pending; capture later |
| Server 2019 x64 | Supported by Documentation | Needs Testing | Needs Testing; highest feature-variation concern | Needs Testing | Desktop Experience baseline proposed; Core not qualified |
| Server 2022 x64 | Supported by Documentation | Needs Testing | Needs Testing | Needs Testing | Desktop Experience baseline proposed; capture privileges later |
| Server 2025 x64 | Supported by Documentation | Needs Testing | Needs Testing | Needs Testing | Exact driver/package combinations unqualified |
| Linux | Core reuse objective | No application implementation | Future backend | Windows Npcap not applicable | Future UI, permissions, packaging, distribution matrix required |
| macOS | No current commitment | Unsupported by current WPF application | Not in scope | Not applicable | No support claim |

The Windows 11 row records bounded checks on one Windows 11 Pro build 26200 x64 host; it is not broad release qualification. The self-contained local package passed four executable gates and the named native session/configuration/export/error/close workflows. USB data storage passed separately. An unchanged USB model/layout smoke failed while a concurrent copy ran, passed during an owner-approved brief hold, and timed out again after resumption. That demonstrates copy-related USB contention without qualifying the other three USB gates or isolating every storage mechanism. Full USB verification after copy completion remains pending. Other OS rows have no application execution evidence. See [current host checks](PHASE1_WINDOWS_HOST_CHECKS.md). Per-OS release acceptance requires actual self-contained package launch, read/write/session workflows, shutdown, and separately qualified capture/tests when implemented. The current host is not all four environments.

References: [official .NET 10 supported OS list](https://github.com/dotnet/core/blob/main/release-notes/10.0/supported-os.md), [WPF Windows desktop overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/), [Server Core overview](https://learn.microsoft.com/windows-server/administration/server-core/what-is-server-core).
