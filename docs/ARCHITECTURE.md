# Protocol Latency Analyzer architecture

This is the current architecture reading path for the **frozen November 2, 2026
foundation**, using the twelve [arc42 sections](https://arc42.org/overview/).
It describes two different source identities: published desktop foundation and
the preserved, unpublished TCP preview. It does not integrate that preview into
`main`. Read [the current handoff](../CONTRIBUTING.md) and
[build and recovery](BUILD_AND_RECOVERY.md) before executing anything.

Claim labels below mean **implemented** in the named source, **demonstrated** by
named retained evidence, **intended** but unimplemented, or **unresolved**.
The [lineage record](validation/FOUNDATION_LINEAGE_20261102.json) pins source,
locks, package and historical patches. Current GitHub task status belongs to
[Project #2](https://github.com/orgs/Limited-Underground/projects/2).

## 1. Introduction and Goals

Protocol Latency Analyzer, by Limited Underground, is a portable Windows network
diagnostics application. Its implemented foundation provides Ethernet inventory,
explicit selection, session/configuration storage and an information-dense WPF
dashboard. The unpublished preview adds one explicitly requested TCP IPv4 connect
measurement at a time. No passive capture, ICMP, DNS or HTTP engine exists in this
scope. A reusable core does not imply a working Linux application.

| Stakeholder | Concern and observable goal |
|---|---|
| Operator | Know which source and destination were requested, whether a connection succeeded, and what was actually measured. Never interpret a failure as zero latency. |
| Maintainer | Recover the exact source/overlay and binary separately; reproduce packaging without overwriting accepted output. |
| Reviewer | Distinguish simulation, loopback, physical Ethernet, OS qualification and owner acceptance. |
| Data owner | Explicit local storage, bounded diagnostics, no silent telemetry or credential collection. |

Product behavior is frozen indefinitely. Documentation/preservation approval does
not authorize a measurement, code correction, release or deployment.

## 2. Architecture Constraints

Implemented platform: .NET 10, custom lightweight MVVM, WPF `net10.0-windows`,
Core/Infrastructure `net10.0`, SQLite sessions and JSON configuration. WPF requires
Windows. Windows 11 and Server 2019/2022/2025 Desktop Experience are target
environments, not a completed support matrix. Server Core and macOS have no
accepted GUI implementation. See [decisions D-002 through D-007](DECISION_LOG.md).

`global.json` selects SDK `10.0.303`, `latestPatch`, no previews. Portable publishing
pins .NET/WPF runtime `10.0.13`, `win-x64`, self-contained, untrimmed and not
single-file. Normal and RID-specific lockfiles are distinct. Apache-2.0 applies to
the application; supplier licenses remain separate in
[dependencies](DEPENDENCIES.md), `LICENSE`, `NOTICE` and `THIRD-PARTY-NOTICES.txt`.
This is a recorded environment, not a recommendation to upgrade or a security
certification.

## 3. System Scope and Context

The operator controls a single desktop process. Windows supplies read-only
Ethernet metadata and socket APIs. Operator-selected files hold JSON
configuration, SQLite sessions and metadata-only exports. In the TCP preview,
one authorized listening endpoint is external to the application; it needs no
Analyzer agent. Standard TCP is used, without an application message protocol,
DNS resolution, TLS or payload exchange.

The version-controlled [Structurizr model](architecture/workspace.dsl) contains
C4 `SystemContext` and `Containers` views. Core and Infrastructure are assemblies
inside the desktop runtime, not independent C4 containers. The endpoint relationship
is explicitly labelled preview-only; it is absent from published foundation code.
Generated JSON/presentation is derived from this model, not a second specification.
The [generated workspace JSON](architecture/workspace.json) passed official
Structurizr `2026.09.19` validation and JSON/static export on November 2; exact
tool/model/output hashes and the limited claim are in the lineage record.

## 4. Solution Strategy

Implemented separation keeps domain/storage models independent of WPF. Windows
dialogs and dispatcher ownership stay in Desktop; Infrastructure implements OS
inventory, JSON/SQLite storage, and the preview socket boundary. Stable adapter
IDs survive enumeration reorder. Connection/gateway/virtual-name hints help
selection but establish neither route nor physical hardware identity.

The preview uses injected transport, inventory and `TimeProvider` for deterministic
failure/race tests, plus a separate opt-in local socket fixture. One executor owns
one attempt; cancellation observes actual completion before reuse. Existing
session/configuration contracts are reused unchanged. These choices preserve
measurement honesty without adding a scheduler, service or persistence schema.

## 5. Building Block View

| Block | Owning source and responsibility |
|---|---|
| Core | `src/ProtocolLatencyAnalyzer.Core/Models.cs`: schema-v1 session/configuration and Ethernet metadata. Preview `TcpConnectModels.cs`: immutable request/result, outcomes, endpoints and transport contracts; no WPF dependency. |
| Infrastructure | `EthernetInterfaceProvider.cs`: Ethernet-family inventory and pure ranking. `ConfigurationStore.cs`: validated JSON and temporary-file replacement. `SessionRepository.cs`: version/identity-checked SQLite transactions. Preview `TcpConnectProbe.cs` and `SocketTcpConnectTransport.cs`: validation, ownership, binding, monotonic connect and cleanup. |
| Desktop | `App.xaml.cs`: application/validation entrypoints. `MainWindow.xaml(.cs)`: navigation, dialogs, inventory, command/lifecycle guards. `DashboardViewModel.cs`: bounded presentation, explicit source/input state, generation/session checks. |
| Tests | Console `tests/ProtocolLatencyAnalyzer.Tests/Program.cs`; preview `TcpConnectTests.cs`; actual STA/WPF controls in preview `TcpDesktopTests.cs`. Synthetic executor/inventory are fixtures, not Ethernet evidence. |
| Packaging | `scripts/Build.ps1`, `Test.ps1`, `Publish.ps1`, `Verify-Package.ps1`; preview preservation guards and separate `Test-PublishPreflight.ps1`. Scripts in public main and the overlay are different inputs. |

Desktop references Core and Infrastructure; Infrastructure references Core and
Microsoft.Data.Sqlite; Core does not reference either other project. The only
direct third-party application package is Microsoft.Data.Sqlite `10.0.13`; its
resolved transitives and hashes are in locks. See [the source map](validation/FOUNDATION_LINEAGE_20261102.json).

## 6. Runtime View

**Startup/inventory — implemented.** Startup creates an empty session and the
window. Inventory runs off the UI thread. Refresh retains selection by stable ID
when present; disappearance is visible. Construction, selection, session load and
demo mode never start a TCP connection. Generation and window-close guards
discard obsolete inventory completion; a timed-out OS query is not claimed stopped.

**TCP preview — implemented in the overlay.** Operator explicitly chooses an Up
Ethernet adapter and its IPv4 address, then enters a numeric target, port and timeout.
Start validates first, takes executor ownership, refreshes source inventory, binds
the chosen address with local port zero, and invokes `ConnectAsync` once. Success
requires observed endpoints matching the request. Result selection is immutable;
the owned socket and pending task settle before Busy is released. A cleanup failure
leaves the executor unavailable rather than allowing a replacement operation.

**Cancel/context/close — implemented in the overlay.** Cancel remains usable while
running and requests socket cancellation/abort. Source, target, session or demo
changes invalidate a generation and suppress late results. A late Success cannot
replace cancellation. First close during a run requests cancellation and defers
closure; after cleanup, close again through the existing unsaved-session flow.
Storage commands and refresh cannot overlap the measurement. A noncancellable
inventory query can delay completion; the connect timeout is not a whole-operation
deadline. No bounded cancellation-latency promise follows from these guards.

**Save/open/configuration — implemented foundation.** Explicit dialogs select the
data location. A valid snapshot is written transactionally; schema/version/identity
errors preserve prior state. Configuration loads only on explicit request, not
startup. Metadata export serializes session metadata/events, not a restorable
database or a measurement report. TCP input/result state never enters these stores.

## 7. Deployment View

One portable directory contains the executable, managed/native dependencies,
self-contained runtime, licenses and supporting documents. There is no installed
service, destination agent, privileged helper or capture driver in this scope.
Use a writable, deliberately chosen data location; executable portability and
data-directory portability are separate acceptance questions.

The immutable TCP candidate is
`ProtocolLatencyAnalyzer-0.1.0-win-x64-tcp-preview-20261029-r2.zip`, 66,743,668 bytes,
439 entries, SHA-256
`66bb169b749de0ee3f70339a0798c7edea4f49c2d9035b86a01c06d7ceea6c13`.
It is an unpublished preview, not the output of a plain clone of public main.
Server 2019 evidence belongs to disconnected Standard Evaluation Desktop Experience
`10.0.17763.3650`, with 2 vCPUs/3 GiB fixed RAM. The recorded initial grace was
not an activated 180-day evaluation; its time window is historical, not reusable
current license proof. Existing VM lifecycle/network access is outside this task.

## 8. Crosscutting Concepts

### TCP measurement contract — overlay only

| Input/concept | Implemented semantics |
|---|---|
| Source | Explicit stable adapter ID and exact inventory IPv4 address; fresh inventory must contain exactly one matching Up adapter. No Any-address or alternate-adapter fallback. |
| Target | One canonical four-part numeric IPv4 address, explicit port 1–65535. No hostname/URL/range, shortened or leading-zero form, 0-net, multicast or class-E address. Directed subnet broadcast detection is not established. |
| Defaults | Source/target/port unset. Timeout 3000 ms, integer 100–10000 ms. One application attempt, no retries; OS retransmissions are not an extra application attempt. |
| Timing | Monotonic elapsed connect invocation through observed completion; excludes validation, inventory, bind, UI and cleanup. UTC timestamps are context. This is socket-connect duration, not packet RTT or application health. |
| Evidence | Requested identity and observed local ephemeral/remote endpoints remain distinct. Contradictory/missing success evidence becomes `InternalError / EndpointEvidenceUnavailable`. Egress stays **egress interface unverified**. |
| Failure | Qualified `ConnectDuration` is null; attempt elapsed may be present. UI shows **Unavailable**, never failed zero milliseconds. |
| Persistence | In-memory only. No target, source address, timeout or result is automatically saved/exported. |

Outcomes are `InvalidRequest`, `Busy`, `SourceInventoryUnavailable`,
`SourceUnavailable`, `Success`, `Cancelled`, `TimedOut`, `ConnectionRefused`,
`NetworkUnreachable`, `HostUnreachable`, `AccessDenied`, `SocketError`, and
`InternalError`. Native categories are reported conservatively, not upgraded to
claims of packet loss or service failure. Timeout origin distinguishes application
and OS. Invalid input precedes pre-cancellation; at result selection caller
cancellation precedes elapsed-deadline expiry, which precedes connect completion.
`CompletedUtc` records selection, not cleanup quiescence.

### Storage and privacy

Schema-v1 sessions have GUID identity, name, creation time, selected interface ID,
origin (`UserSession`/`Simulation`) and bounded events (maximum 10,000). JSON config
holds selected interface and a 10–10,000 visible-event limit; input is at most
65,536 bytes. Session open caps file size at 128 MiB and rejects unsupported
versions/identity mismatch. SQLite uses parameterized values, no connection pool
and a 5-second database timeout. These storage limits differ from TCP timeout.

Diagnostics use fixed messages in a bounded memory-only queue with discarded-entry
accounting. User-selected session files/exports can contain interface identifiers
and require appropriate custody. No automatic cloud telemetry exists. Credentials,
guest disks, infrastructure records and private captures never belong in repository
or handbook inputs.

## 9. Architectural Decisions

Existing decision identities remain in [DECISION_LOG.md](DECISION_LOG.md):
D-001/D-003 naming/license; D-002/D-007 platform/MVVM/storage;
D-005/D-006 OS/packaging; D-008/D-009/D-023 approved foundation layout/light theme;
D-020/D-022 retained flat-package/USB evidence. D-011's early broader protocol
proposal is historical: the separately approved TCP-only implementation is recorded
by [issue #5](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/issues/5)
and the lineage evidence. Other protocols and measurement persistence remain
unimplemented; the freeze does not make their design decisions necessary now.

The earlier [architecture proposal](ARCHITECTURE_PROPOSAL.md),
[protocol/capture assessment](PROTOCOL_CAPABILITIES.md), [roadmap](ROADMAP.md)
and [progress log](PROGRESS_LOG.md) remain historical/planning references. They
are not updated to imply the TCP overlay is merged. The reading-path and
[document map](BUILD_AND_RECOVERY.md#document-ownership-map) explain precedence.

## 10. Quality Requirements

| Scenario | Retained demonstration | Remaining boundary |
|---|---|---|
| Foundation model/storage/inventory safety | 41 foundation regressions; recorded Windows host workflows | No new clean reconstruction in this foundation task |
| Single TCP ownership, exact input/source, timing/errors/races | 186 deterministic checks in accepted implementation; eight opt-in loopback checks | Injected behavior/loopback do not qualify physical egress |
| UI Start/Cancel, unavailable failure, stale context/close, storage unchanged | 86 view-model/actual WPF-control checks with synthetic inventory/executor | No current operator or actual Ethernet acceptance |
| Package integrity and offline OS execution | Five Server 2019 gates; all 439 accepted files hash-matched; two repeated startup/clean-exit cycles | Partial Server qualification, not non-admin complete workflows or Server 2022/2025 |
| Existing output preservation | Seven publish-preflight cases and five verifier/receipt guards | No concurrent-publisher atomicity guarantee |
| Recoverability | Separate source/Git/package checkpoint with membership and SHA verification | See five independent claims in recovery guide; no byte-identical/remote TCP recovery claim |

The [November 1 sanitized result](validation/TCP_PREVIEW_SERVER2019_20261101.json)
records exact gates and limits. Historical counts are reused, not rerun or added
together as new coverage. An independent documentation review concerns navigation,
claim fidelity and privacy; it does not reaccept product behavior.

## 11. Risks and Technical Debt

* The TCP/code/packaging overlays are local-only; public main cannot reproduce the
  current preview without the separately retained source checkpoint.
* SQLite session paths passed at 240 characters but failed with error 14 at tested
  259/260/266/280 lengths on host and guest, while managed creation passed. Exact
  native stage and cutoff remain unproved. The short-path pass is not a fix; no
  silent relocation is authorized. Product correction is frozen.
* Full non-admin interactive workflows, operator review, physical Ethernet/source
  qualification, realistic timeout/cancel trials and Server 2022/2025 remain open.
* Binary checksum equality proves artifact identity, not clean-source rebuild or
  deterministic ZIP output. SDK/lock pins do not preserve an unavailable supplier.
* VM/credential private custody and off-laptop protection remain separate from the
  ordinary-source checkpoint. Do not copy a live VHDX as a consistent backup.
* Later protocols, capture backend/privilege rights, Linux UI and typed measurement
  storage remain intended work, not hidden current implementation.

## 12. Glossary

| Term | Meaning |
|---|---|
| Foundation | Published Windows desktop/storage/inventory baseline; also distinguish the November 2 preservation/documentation work package. |
| TCP preview | Unpublished overlay plus exact retained ZIP, distinct from public main and a release. |
| Socket-connect duration | Monotonic duration of a single connect observation, including OS/socket scheduling. |
| RTT | Packet round-trip interval; not what the preview measures. |
| Stable adapter ID | OS-provided interface identity, not list position or proven physical egress. |
| Synthetic | Explicit demonstration or injected fixture data; never acquired network evidence. |
| Quiescence | Real inventory/connect/disposal settled; canceling a wait alone does not establish it. |

Viewpoints address operator semantics, maintainer dependencies/recovery and reviewer
evidence/privacy. Context/container views relate to source and runtime sections
above. [ISO/IEC/IEEE 42010:2022](https://www.iso.org/standard/74393.html) is a review
reference only; no conformance or certification is claimed.
