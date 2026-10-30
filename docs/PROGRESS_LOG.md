# Protocol Latency Analyzer Progress Log

Historical October 15 milestones were reconstructed from published Git history.
Later entries explicitly distinguish local unpublished candidates, recorded host
checks, target-OS baselines and release acceptance. Entries are newest first.
Current acceptance belongs to [Phase 1 acceptance](PHASE1_ACCEPTANCE.md), with
decisions in the [decision log](DECISION_LOG.md). Historical tests are not rerun
by this log reconciliation.

## 2026-10-29

### Issue #3 — Validate an unpublished TCP preview package with a retained path limitation

The local self-contained TCP preview passed seven publish-preservation cases,
five verifier/receipt guards, four extracted-package executable gates and 86
actual STA/WPF checks using an explicitly shorter synthetic storage root.
Independent security/integration/test review passed within that current-host
scope. ZIP SHA-256: `66bb169b749de0ee3f70339a0798c7edea4f49c2d9035b86a01c06d7ceea6c13`;
439 extracted file hashes matched. The original default-path storage failure at
266 characters remains preserved; the short-path pass neither accepts long paths
nor proves the precise native cause. Matching selected binaries across publishes
does not establish deterministic ZIP or independent clean-build reproducibility.
[Issue #3](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/issues/3)
remained open at this checkpoint. No source publication, release, physical
Ethernet or qualification on another OS followed from this local package.

### Issue #4 — Establish a fresh offline Server 2019 qualification baseline

A separate clean Server 2019 Standard Evaluation Desktop Experience guest reached
completed Setup/OOBE and build 17763.3650, with disconnected Ethernet and usable
PowerShell Direct access. Initial evaluation grace was about ten days, not an
activated 180-day evaluation. No PLA package was transferred or executed on this
new guest at this checkpoint; prior guest results do not transfer to it. See the
[fresh-baseline evidence and limits](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/issues/4).
Remaining application, standard-user, interactive/export and cleanup gates remain
under [issue #4](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/issues/4).

## 2026-10-28

### PLA-0016 and PLA-0017 — Complete bounded TCP engine and desktop host candidates

The approved headless TCP IPv4 slice passed a zero-warning/error Release build,
41 foundation regressions, 186 deterministic TCP checks and eight authorized
loopback checks. The separately approved WPF integration added explicit source
and target selection, bounded Start/Cancel, truthful failure timing and stale-
result/lifecycle refusal. Its 86 actual WPF checks and affected smoke gate passed;
independent reviews passed. These are uncommitted isolated candidates, not merged
source, measurement persistence, external Ethernet acceptance or a release.
[Headless child PLA-0016](https://github.com/orgs/Limited-Underground/projects/2?pane=issue&itemId=286342858)
and [desktop child PLA-0017](https://github.com/orgs/Limited-Underground/projects/2?pane=issue&itemId=286342988)
record those bounded completions; parent issue #5 retains owner/operational gates.

### Issue #3 — Correct publish-output preservation before SDK execution

A separate uncommitted packaging fix moved publish/ZIP/checksum collision refusal
before SDK execution. Seven focused cases passed and independent review found no
open actionable findings. Existing foundation, USB and Server results were reused
within their original scope; this fix did not requalify an application or OS.
See the [development-resume checkpoint](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/issues/3).

## 2026-10-15

### Preserve Server 2019 passes and the unfinished acceptance boundary

Recorded four unchanged 60-second executable gates passing on the clean,
disconnected Server 2019 Standard Evaluation Desktop Experience baseline as a
standard user without an SDK/global runtime. Narrow later GUI observations and
canceled dialogs remain distinct from full native-workflow acceptance. The owner
paused screen control; exported-content verification, configuration equivalence,
complete interactive acceptance, evidence export and cleanup remained pending.
Server 2022/2025 were unqualified, Phase 1 remained In Progress and Phase 2 had not
started. [Commit](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/commit/5e3a144);
[acceptance and limits](PHASE1_ACCEPTANCE.md);
[sanitized Server results](validation/PHASE1_SERVER2019_RESULTS.json).

### Prepare internal storage and relocate the isolated evaluation trial

Recorded supported disposable-cache/component cleanup, hash-verified media copying
and relocation of the receipt-owned Server trial while it was Off. All five
preexisting VMs were verified unchanged. Preserved the initial parameter-conversion
failure and guarded retry, the paused Server 2022 partial and the unstarted 2025
preparation. The earlier low-RAM checkpoint blocked guest preparation; relocation
itself established no application qualification.
[Commit](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/commit/9532e03);
[preparation decisions and guards](DECISION_LOG.md);
[Server test plan](PHASE1_SERVER_TEST_PLAN.md).

### Defer guest qualification while offline preparation continues

Recorded the owner's temporary deferral of VM creation/startup/qualification while
permitted download and offline preparation continued. This checkpoint was later
superseded by explicit resumed qualification recorded in the decision log; it was
not a completed OS test. [Commit](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/commit/caeb477);
[decision history](DECISION_LOG.md).

### Accept the Phase 1 dashboard and plan isolated Server evaluation

The owner accepted the current dense dashboard and light styling for Phase 1.
Official branding and future theme choices remained separate. Defined guarded,
isolated Desktop Experience evaluation for Server 2019/2022/2025, protecting
existing VMs and requiring receipt-owned cleanup. Visual acceptance and test
planning did not close OS or full Phase 1 acceptance.
[Commit](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/commit/9c8581a);
[Server plan](PHASE1_SERVER_TEST_PLAN.md);
[acceptance record](PHASE1_ACCEPTANCE.md).

### Pass all four USB executable gates after copy completion

Fresh extraction of the unchanged immutable package passed model/layout, packaged
storage, synthetic rendering and real-window/dispatcher/closure gates directly
from USB after no copy processes were present. All retained the 60-second deadline;
no application fix or timeout increase was required. These current-host USB passes
did not establish other-OS or overall acceptance.
[Commit](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/commit/0675a44);
[post-copy verification](PHASE1_WINDOWS_HOST_CHECKS.md);
[sanitized results](validation/PHASE1_WINDOWS_HOST_RESULTS.json).

### Diagnose USB startup contention and verify copy resumption

A separately authorized, watchdog-bounded copy hold produced a fail/pass/fail USB
smoke sequence with the copy active/held/resumed. Independent observations verified
resumption and progress. The evidence demonstrated copy-related contention without
isolating every storage/filter/cache cause or explaining every historical stall.
Only the smoke gate passed during the hold; full USB qualification followed later.
The diagnostic bundle was not adopted and no application workaround was claimed.
[Commit](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/commit/ad3af21);
[controlled comparison and limits](PHASE1_WINDOWS_HOST_CHECKS.md).

### Validate current Windows workflows and retain startup failures

The warning-free Release build and 41-check runner passed with default and explicit
USB storage fixtures. Four local extracted-package executable gates and 14 named
native workflow groups passed, including save/reopen, configuration, invalid-input
preservation, explicit adapter selection and close/cancel behavior. Added precise
process arguments, asynchronous output draining, partial failure receipts and a
real-window readiness gate; normal startup tracing became opt-in. Exact-final USB
startup still timed out at this checkpoint, with remaining modes not reached.
Earlier failures remained evidence rather than being replaced by narrower passes.
[Commit](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/commit/ff45ecd);
[Windows host checks](PHASE1_WINDOWS_HOST_CHECKS.md).

### Improve adapter guidance and organize session commands

Added qualified connection/recommendation hints and explicit selection, with no
automatic adapter choice or route guarantee. Moved session commands into File and
routed shortcuts while preserving state/busy guards. The revised build passed at
zero warnings/errors with 36 offline checks. Native inventory was deferred from
the constructor to a guarded worker; visible development startup and keyboard cases
passed, while portable synthetic startup failures remained unresolved at that stage.
[Commit](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/commit/fb2c218);
[feedback and startup history](PHASE1_FEEDBACK.md).

### Build the Phase 1 portable Windows foundation

Implemented the .NET 10 WPF shell, reusable core, explicit one-Ethernet selection,
opt-in synthetic data, JSON configuration, SQLite sessions, bounded history and
portable self-contained packaging. Recorded locked dependencies, third-party
licenses, offline host validation and synthetic dashboard evidence. The read-only
Phase 0 assessment and architecture separated passive capture, active protocols,
drivers, future Linux reuse and later phases from the authorized foundation.
Source/build and initial package evidence did not qualify every OS or establish
live capture/network behavior.
[Commit](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/commit/d4c3aa6);
[Phase 0 assessment](PHASE0_ASSESSMENT.md);
[foundation host results](validation/PHASE1_HOST_RESULTS.json);
[capability boundaries](PROTOCOL_CAPABILITIES.md).

### Establish Apache-2.0 project licensing

Created the repository's application license. Application licensing remained
separate from third-party dependency terms and any package/release acceptance.
[Initial commit](https://github.com/Limited-Underground/ProtocolLatencyAnalyzer/commit/8f2940c);
[license](../LICENSE).
