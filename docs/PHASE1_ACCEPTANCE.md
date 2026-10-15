# Phase 1 acceptance

Phase 1 is the portable desktop foundation only. Implementation and offline host checks were completed on October 15, 2026. Source/build checks, extracted-package checks, interactive workflows, target-OS qualification and owner visual acceptance are separate layers. Phase 1 overall remains In Progress pending acceptance.

| Criterion | Evidence required | Status |
|---|---|---|
| WPF shell preserves dense approved layout and identity | Actual WPF RenderTargetBitmap at 1536x1024 visually inspected; all panels visible after table sizing correction. Offline layout assertions at 1280x800 and 1536x1024. | Host checks passed; File menu/keyboard and normal startup checked; owner review pending |
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
| Windows 11, Server 2019/2022/2025 qualification | Separate environment evidence still required; current-host rendering is not full product qualification. | Needs Testing |
| Final branding/theme and owner acceptance | Provisional light styling; approved name and attribution. Actual assets and final theme unresolved. | Owner review/input pending |

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

Issue #1 is complete for the reusable core/configuration/session task scope, based on the documented source and host checks. Linux execution remains untested. Issue #2 remains open for owner visual review and final branding; the named interactive workflows passed. Issue #3's current-host local and USB package gates passed; clean-machine qualification remains pending. The earlier approved failure/pass/failure comparison demonstrates copy-related USB contention, and the unchanged package subsequently passed all four USB gates with no copy processes present. Issue #4 remains open for target-OS qualification. Later phases remain Todo. Phase 1 overall is not accepted or closed; Phase 2 has not started.

## Owner feedback follow-up

The dashboard now shows connection state and qualified adapter suggestions, with full details collapsed by default. File commands moved into a File menu; configuration is under Tools > Settings. See [follow-up evidence](PHASE1_FEEDBACK.md) and [sanitized package results](validation/PHASE1_FEEDBACK_RESULTS.json). These changes do not establish owner acceptance or another operating system's qualification.



