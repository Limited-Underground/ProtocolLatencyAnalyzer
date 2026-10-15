# Compatibility matrix

Documentation support for a framework is distinct from tested application/package/capture support.

| Operating system | .NET/WPF basis | Portable application | Native capture | Npcap | Privileges and limits |
|---|---|---|---|---|---|
| Windows 11 x64 | Supported by Documentation | Build 26200 local and USB four-gate package checks and named workflows passed; clean machine pending | Needs Testing | Needs Testing | Medium-integrity launcher observed; copy-related USB contention demonstrated; broader release qualification pending; capture later |
| Server 2019 x64 | Supported by Documentation | Needs Testing | Needs Testing; highest feature-variation concern | Needs Testing | Desktop Experience baseline proposed; Core not qualified |
| Server 2022 x64 | Supported by Documentation | Needs Testing | Needs Testing | Needs Testing | Desktop Experience baseline proposed; capture privileges later |
| Server 2025 x64 | Supported by Documentation | Needs Testing | Needs Testing | Needs Testing | Exact driver/package combinations unqualified |
| Linux | Core reuse objective | No application implementation | Future backend | Windows Npcap not applicable | Future UI, permissions, packaging, distribution matrix required |
| macOS | No current commitment | Unsupported by current WPF application | Not in scope | Not applicable | No support claim |

The Windows 11 row records bounded checks on one Windows 11 Pro build 26200 x64 host; it is not broad release qualification. The self-contained package passed all four executable gates from local and USB folders, and the named native session/configuration/export/error/close workflows passed. USB data storage passed separately. Historically, unchanged USB smoke failed with a concurrent copy, passed during an approved hold, then timed out after resumption. Subsequently, with no copy processes observed before/after, a fresh extraction of the same ZIP passed all four USB gates. Copy-related contention was demonstrated without isolating every storage mechanism or proving copy/migration success. Other OS rows have no application execution evidence. See [current host checks](PHASE1_WINDOWS_HOST_CHECKS.md). Per-OS release acceptance requires actual self-contained package launch, read/write/session workflows, shutdown, and separately qualified capture/tests when implemented. The current host is not all four environments.

References: [official .NET 10 supported OS list](https://github.com/dotnet/core/blob/main/release-notes/10.0/supported-os.md), [WPF Windows desktop overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/), [Server Core overview](https://learn.microsoft.com/windows-server/administration/server-core/what-is-server-core).
