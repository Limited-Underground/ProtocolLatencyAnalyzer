# Decision log

Direct owner instructions supersede earlier proposals and queued-state text in the preserved intake handoff.

| ID | Decision | Status / authority |
|---|---|---|
| D-001 | Protocol Latency Analyzer, By Limited Underground; general-purpose open source | Decided by owner |
| D-002 | Windows-first WPF application; reusable core for future Linux application | Decided by owner in this chat |
| D-003 | Apache-2.0 application license | Decided by owner; does not license third-party components |
| D-004 | Begin Phase 1 portable application foundation | Decided by owner; later functional phases remain separate |
| D-005 | Windows 11 and Server 2019/2022/2025 targets; one Ethernet interface first | Decided by owner; application compatibility needs testing |
| D-006 | Portable self-contained ZIP and win-x64 development artifact | Implementation baseline; owner approved Server Desktop Experience baseline; Server Core is outside this GUI qualification |
| D-007 | .NET 10; custom lightweight MVVM; JSON configuration and SQLite through Microsoft.Data.Sqlite | Phase 1 implementation selections within approved architecture; resolved packages require license/version review |
| D-008 | Preserve approved dense dashboard layout | Decided by owner; mockup sample values/copy are not live or final |
| D-009 | Initial light styling | Owner accepted current light styling for Phase 1; no final official brand identity or future theme/default commitment |
| D-010 | Official brand assets, fonts, and palette | Pending owner input; do not describe provisional colors as verified brand identity |
| D-011 | First active protocol subset | Proposed ICMP/TCP/DNS/HTTP(S); pending before Phase 2 |
| D-012 | Native-first capture; optional existing Npcap | Architectural direction; exact backend needs per-OS proof |
| D-013 | Managed temporary Npcap install | Blocked by rights, ownership, cleanup/recovery validation and separate authorization |
| D-014 | Offline files, agents, combined correlation, additional adapters | Proposed extensions; not Phase 1 scope |
| D-015 | Separate GitHub repository and Project | Decided under separate owner setup authorization; public Limited-Underground/ProtocolLatencyAnalyzer and private organization Project #2 created; source publication remains separate |
| D-016 | macOS | No current commitment |
| D-017 | Publish Phase 1 source and reconcile Project completion/testing | Explicitly authorized by owner on October 15, 2026; source-only push, no release/website or elevated capability claim |
| D-018 | Clear adapter guidance and File menu | Owner feedback during Windows dashboard review; implement within Phase 1, retaining explicit selection and qualified connection/routing hints |
| D-019 | Finish Phase 1 Windows host checks before Phase 2 | Explicitly requested by owner; local and USB packaged startup and named workflows passed on the current host; preserve separate clean-machine, OS, branding and owner acceptance gates |
| D-020 | Retain flat self-contained package after bundling experiment | Diagnostic single-file bundle did not resolve the USB startup case and was not adopted; original application dependency lock contexts remain unchanged |
| D-021 | Guarded USB startup comparison during a brief hold of the identified copy | Explicit owner approval; hold lasted 22.6998139 seconds including settling, watchdog resumed successfully, same-copy progress independently verified. USB smoke failed/passed/failed with active/held/resumed copy; copy-related contention demonstrated, no application workaround adopted. Full USB gates were pending at that stage and subsequently passed under D-022; Phase 2 not started |
| D-022 | Repeat all four USB package gates once no concurrent copy is running | Explicit owner request; exact immutable ZIP passed all four unchanged 60-second gates on the current host at 2026-10-15T17:28:48.2330088Z. Update GitHub Project evidence before Phase 2; clean-machine, other-OS and owner acceptance remain separate |
| D-023 | Accept current dashboard and light styling for Phase 1 | Explicit owner acceptance; dashboard task #2 complete with the prior 14 native workflow groups. Official brand assets remain a separate future input |
| D-024 | Isolated Server evaluation qualification | Owner authorized official Microsoft trial downloads and isolated Server 2019/2022/2025 Desktop Experience VM tests, followed by deletion of only newly created evaluation VMs/artifacts. Inventory/protected identifiers and exact cleanup receipts required; see PHASE1_SERVER_TEST_PLAN.md |
| D-025 | Constrained evaluation guest and protected host state | Elevated read-only inventory succeeded with five preexisting VMs protected. Owner independently shut down Home Assistant; RAM briefly reached 5.25 GiB, then latest elevated inventory found 2.69 GiB/all five existing VMs Off/no evaluation VM. Selected 3 GiB fixed/2 vCPU/64 GiB dynamic guest with a disconnected Ethernet NIC requires a 4.5 GiB creation/start guard, currently unmet. No authority to stop/start or delete preexisting VMs; no guest OS execution pass yet |

Phase 1 does not authorize network/capture tests, driver installation, device changes, publication, or neighboring-project changes. Linux reuse is a design objective, not a claim that this Windows shell runs on Linux.

The isolated VM authorization permits the bounded evaluation work in D-024, including its scoped downloads and cleanup; it does not permit changing Hyper-V features, host networking, preexisting VMs/disks, or shared directories. Phase 1 remains open and Phase 2 has not started.
