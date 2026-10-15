# Phase 1 acceptance

Phase 1 is the portable desktop foundation only. Implementation and offline host checks were completed on October 15, 2026. Source/build checks, extracted-package checks, interactive workflows, target-OS qualification and owner visual acceptance are separate layers. Phase 1 overall remains In Progress pending acceptance.

| Criterion | Evidence required | Status |
|---|---|---|
| WPF shell preserves dense approved layout and identity | Actual WPF RenderTargetBitmap at 1536x1024 visually inspected; all panels visible after table sizing correction. Offline layout assertions at 1280x800 and 1536x1024. | Host checks passed; owner/interactive review pending |
| Synthetic versus unavailable/live data is explicit | Empty initial state, opt-in demo, unavailable protocol rows, synthetic adapter and banner assertions. | Host checks passed |
| Ethernet inventory and stable selection | Read-only implementation and identifier persistence reviewed; verification modes intentionally bypass real adapter enumeration. | Interactive adapter/reorder/removal checks pending |
| No interface-routing or capture guarantee implied | UI/capability text and independent source review. | Source review passed |
| JSON configuration/session metadata round trip | Actual configuration persistence, limit validation, failed replacement preservation. Explicit configuration file dialogs in UI. | Offline checks passed; dialog workflow pending |
| SQLite sessions reopen correctly | Parameterized round trip, replacement, transaction rollback, canceled/future/oversize save preservation, identity mismatch rejection. Packaged native SQLite round trip passed separately. | Offline checks passed |
| Unknown/malformed files do not overwrite valid state | Corrupt SQLite and future versions rejected; rejected overwrites preserve prior database/configuration contents. | Tested cases passed; not an exhaustive hostile-file qualification |
| Writable location explicit; read-only failure clear | Source review, storage failure and locked-target tests; no implicit persistence directory. | Host checks passed; interactive read-only/USB cases pending |
| Bounded history and dropped-entry accounting | Smoke checks limit 10, newest-entry retention, demo-aware bound and explicit discarded count. | Offline checks passed |
| Core has no WPF/Windows dependency | net10.0 project graph and independent source review. | Source/build checks passed; Linux execution untested |
| Logs exclude secrets and uncontrolled exception data | Fixed-message memory-only operation diagnostics; sanitized UI errors reviewed. | Source review passed |
| Cancellation/shutdown and storage errors handled | Canceled save preserves state; offscreen process shutdown succeeds; busy close guard reviewed. | Host/source checks passed; interactive close/error workflow pending |
| Build and foundation runner pass | scripts/Test.ps1: Release build 0 warnings/errors; 25 offline checks passed. | Passed |
| Self-contained win-x64 package executes | Extracted ZIP: --smoke-check, --verify-storage, --render-preview all exit 0; included .NET/WPF 10.0.13; no SDK-based launch. | Offline current-host checks passed |
| Windows 11, Server 2019/2022/2025 qualification | Separate environment evidence still required; current-host rendering is not full product qualification. | Needs Testing |
| Final branding/theme and owner acceptance | Provisional light styling; approved name and attribution. Actual assets and final theme unresolved. | Owner review/input pending |

## Evidence and reproduction

- `scripts/Test.ps1` performs locked normal restore, Release build, and 25 storage/model checks.
- `scripts/Publish.ps1 -OutputName win-x64-review1` produced a self-contained runtime-version-pinned ZIP. All normal and RID-specific lockfiles are retained.
- A subsequent `dotnet restore ProtocolLatencyAnalyzer.sln --locked-mode` succeeded, establishing that publish does not invalidate normal-build locks.
- `scripts/Verify-Package.ps1 -OutputName win-x64-review1` checked the ZIP hash, extracted a fresh copy, and ran all three offline validation modes. Each exited 0. Detailed results and synthetic PNG are beneath `.artifacts/validation/win-x64-review1`.
- The delivery package is `ProtocolLatencyAnalyzer-0.1.0-win-x64-phase1.zip`, containing the same reviewed binaries with refreshed README/acceptance/GitHub documentation. Its checksum and final verification evidence are stored alongside the package and under `.artifacts/validation/win-x64-phase1`.
- Independent source review resolved API integration, UI blocking, configuration limit, session-ignore, and bounded logging findings. Visual review found and corrected narrow table columns and excessive spacing before the review candidate.
- Historical intake files remained byte-identical locally and are excluded from source publication. Sanitized package results are committed in [PHASE1_HOST_RESULTS.json](validation/PHASE1_HOST_RESULTS.json); local paths, caches, ZIPs and database fixtures are not published.

No protocol traffic, packet capture, driver operations, privileged helper, or offline capture-file analysis was implemented or tested. GitHub repository/Project creation and this Phase 1 source publication were separately authorized by the owner; see GITHUB_SETUP.md. No release or website publication occurred. Local portable packaging and source publication do not establish product or operating-system acceptance.

## Tracker interpretation

Issue #1 is complete for the reusable core/configuration/session task scope, based on the documented source and host checks. Linux execution remains untested. Issue #2 remains open for dashboard/interactive/owner review; issue #3 remains open for interactive read-only/USB, dialog and close/error workflow checks; issue #4 remains open for target-OS qualification. Later phases remain Todo. Phase 1 overall is not accepted or closed.
