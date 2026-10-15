# Phase 1 acceptance

Phase 1 is the portable desktop foundation only. Implementation and offline host checks were completed on October 15, 2026. Source/build checks, extracted-package checks, interactive workflows, target-OS qualification and owner visual acceptance are separate layers. Phase 1 overall remains In Progress pending acceptance.

| Criterion | Evidence required | Status |
|---|---|---|
| WPF shell preserves dense approved layout and identity | Actual WPF RenderTargetBitmap at 1536x1024 visually inspected; offline layout assertions at 1280x800 and 1536x1024; 14 native workflow groups; explicit owner dashboard acceptance. | Phase 1 dashboard accepted by owner |
| Synthetic versus unavailable/live data is explicit | Empty initial state, opt-in demo, unavailable protocol rows, synthetic adapter and banner assertions. | Host checks passed |
| Ethernet inventory and stable selection | Read-only inventory and stable identifiers; explicit selection, refresh, session/configuration restoration and missing-adapter cases checked. | Current-host cases passed; no physical adapter reconfiguration/removal performed |
| No interface-routing or capture guarantee implied | UI/capability text and independent source review. | Source review passed |
| JSON configuration/session metadata round trip | Configuration persistence, limits, failed replacement preservation; native save/load/cancel and export checked. | Current-host storage and interactive cases passed |
| SQLite sessions reopen correctly | Parameterized round trip, replacement, transaction rollback, canceled/future/oversize save preservation, identity mismatch rejection. Packaged native SQLite round trip passed separately. | Offline checks passed |
| Unknown/malformed files do not overwrite valid state | Corrupt SQLite and future versions rejected; rejected overwrites preserve prior database/configuration contents. | Tested cases passed; not an exhaustive hostile-file qualification |
| Writable location explicit; read-only failure clear | Explicit USB data storage; native Save As read-only refusal separately from configuration/SQLite failed-write preservation checks. | Named current-host storage cases and subsequent four USB package gates passed |
| Bounded history and dropped-entry accounting | Smoke checks limit 10, newest-entry retention, demo-aware bound and explicit discarded count. | Offline checks passed |
| Core has no WPF/Windows dependency | net10.0 project graph and independent source review. | Source/build checks passed; Linux execution untested |
| Logs exclude secrets and uncontrolled exception data | Fixed-message memory-only operation diagnostics; sanitized UI errors reviewed. | Source review passed |
| Cancellation/shutdown and storage errors handled | Native canceled dialogs, invalid inputs retaining prior state, unsaved-close Cancel/discard, restart and real-window clean closure checked; busy close guard reviewed. | Named current-host checks passed |
| Build and foundation runner pass | Release build 0 warnings/errors; 41 checks passed with default fixtures and again with explicit USB storage fixtures. | Passed |
| Self-contained win-x64 package executes | Final unchanged package passed four local gates and all four USB gates after no copy processes were present. Each retained a 60-second deadline; USB real-window startup took 2.3335344 seconds. Earlier active/held/resumed smoke failures/pass preserved. | Local and USB package checks passed on current host; clean machine pending |
| Windows 11, Server 2019/2022/2025 qualification | Separate environment evidence still required; isolated Desktop Experience evaluation tests authorized under the Server test plan. Image downloads alone are not execution evidence. | Needs Testing; Server evaluation preparation in progress |
| Phase 1 styling and owner acceptance | Owner explicitly accepted current dashboard and light styling for Phase 1; approved name and attribution. | Phase 1 visual acceptance complete; official brand assets/future theme decisions remain separate |

## Evidence and reproduction

- `scripts/Test.ps1` performs locked normal restore, Release build, and 41 storage/model/adapter checks on Windows. The same runner also passed with an explicitly selected USB fixture root.
- `scripts/Publish.ps1 -OutputName win-x64-review1` produced a self-contained runtime-version-pinned ZIP. All normal and RID-specific lockfiles are retained.
- A subsequent `dotnet restore ProtocolLatencyAnalyzer.sln --locked-mode` succeeded, establishing that publish does not invalidate normal-build locks.
- `scripts/Verify-Package.ps1 -OutputName win-x64-review1` checked the ZIP hash, extracted a fresh copy, and ran all three offline validation modes. Each exited 0. Detailed results and synthetic PNG are beneath `.artifacts/validation/win-x64-review1`.
- The original delivery package was `ProtocolLatencyAnalyzer-0.1.0-win-x64-phase1.zip`. Subsequent feedback and timeout investigation are retained as historical evidence. The final local host candidate is `ProtocolLatencyAnalyzer-0.1.0-win-x64-phase1-validated-local.zip`, checksum `89b17ee349f72ee4698f52d5ec976043917f1bf7a48f1101aac120ee4535b0d9`. It passed all four executable gates; real-window startup/render/dispatcher/closure took 2.296 seconds. See [current host checks](PHASE1_WINDOWS_HOST_CHECKS.md) and [sanitized current results](validation/PHASE1_WINDOWS_HOST_RESULTS.json).
- Independent source review resolved API integration, UI blocking, configuration limit, session-ignore, and bounded logging findings. Visual review found and corrected narrow table columns and excessive spacing before the review candidate.
- Historical intake files remained byte-identical locally and are excluded from source publication. Sanitized package results are committed in [PHASE1_HOST_RESULTS.json](validation/PHASE1_HOST_RESULTS.json); local paths, caches, ZIPs and database fixtures are not published.

No protocol traffic, packet capture, driver operations, privileged helper, or offline capture-file analysis was implemented or tested. GitHub repository/Project creation and this Phase 1 source publication were separately authorized by the owner; see GITHUB_SETUP.md. No release or website publication occurred. Local portable packaging and source publication do not establish product or operating-system acceptance.

## Tracker interpretation

Issue #1 is complete for the reusable core/configuration/session task scope; Linux execution remains untested. Issue #2's Phase 1 dashboard scope is complete following explicit owner acceptance of the current dashboard/light styling and the prior 14 native workflow groups. This does not approve a final official brand identity. Issue #3's current-host local and USB package gates passed; clean-machine qualification remains pending. The earlier approved failure/pass/failure comparison demonstrates copy-related USB contention, and the unchanged package subsequently passed all four USB gates with no copy processes present. Issue #4's authorized Server evaluation preparation is In Progress with official download provenance validated; actual OS execution remains Needs Testing. Later phases remain Todo. Phase 1 remains open; Phase 2 has not started.

## Owner feedback follow-up

The dashboard now shows connection state and qualified adapter suggestions, with full details collapsed by default. File commands moved into a File menu; configuration is under Tools > Settings. See [follow-up evidence](PHASE1_FEEDBACK.md) and [sanitized package results](validation/PHASE1_FEEDBACK_RESULTS.json). The owner subsequently accepted the current dashboard/light styling for Phase 1. Other operating systems still require qualification.

## Isolated Server evaluation gate

The owner has now directed **keep preparing; test later**. Official downloads and offline preparation continue; VM creation, startup and qualification are deferred until the owner resumes testing. This does not change prior host passes or create a Server execution pass.

The owner approved Desktop Experience and official Microsoft trial downloads for Server 2019/2022/2025, isolated VM qualification, and deletion of only this project's newly created evaluation resources afterward. Follow [PHASE1_SERVER_TEST_PLAN.md](PHASE1_SERVER_TEST_PLAN.md). Elevated read-only inventory succeeded and protected five preexisting VMs. The owner independently shut down Home Assistant; available RAM briefly reached 5.25 GiB but the latest elevated inventory measured 2.69 GiB, below the 4.5 GiB creation/start guard for the selected 3 GiB guest. All five preexisting VMs were Off. Lifecycle commands still require ordinary Windows UAC consent; no host-feature/reboot change is needed. The preparation attempt exited before recording intent; complete inventory confirms no evaluation VM was created and no Server guest OS has started or passed. Downloads are preparation only. Fresh Server tests can establish clean-machine evidence for those baselines, alongside valid current Windows 11 host checks. Fresh Windows 11 VM coverage remains an unestablished limitation rather than a new mandatory gate; broader production/release qualification is separate.
