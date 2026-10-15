# Phase 1 Windows host checks

This record covers the current Windows 11 Pro build 26200 host. Phase 2 has not started. A successful local portable launch, USB data storage, direct USB executable launch, clean-machine qualification and owner acceptance are separate outcomes.

## Portable startup and verification

The final flat self-contained package uses .NET/WPF 10.0.13. Its checksum-verified contents were extracted to an explicitly selected new local temporary directory. All four executable checks exited 0 under the unchanged 60-second deadline:

| Check | Seconds |
|---|---:|
| Model and WPF layout | 2.45 |
| Packaged SQLite and configuration | 6.66 |
| Synthetic WPF render | 3.36 |
| Real window, rendering, dispatcher idle and clean closure | 2.30 |

The final package checksum is `89b17ee349f72ee4698f52d5ec976043917f1bf7a48f1101aac120ee4535b0d9`. This is local artifact verification, not a published release. See [sanitized results](validation/PHASE1_WINDOWS_HOST_RESULTS.json).

The prior timeout was not established as an adapter-enumeration defect: synthetic mode bypasses enumeration. Identical previously failing binaries later passed. Normal USB-hosted startup was also delayed; managed stacks sampled initial WPF layout and later an idle dispatcher, without establishing a permanent deadlock. An identical diagnostic bundle completed a full local-disk window/render/close check in 2.87 seconds but exceeded the same deadline from USB. Bundling did not resolve that USB case and was not adopted. The original package format and application dependency locks remain in use.

The source/project drive was verified as USB. A fresh extraction of the exact final ZIP (same checksum above; 257 files) was verified at 2026-10-15T13:36:13.8970870Z. Its first model/layout check exceeded the unchanged 60-second deadline with no diagnostic stages emitted; recorded duration including bounded cleanup was 61.6248782 seconds, exit code unavailable, failure `ProcessTimeout`. The remaining three checks were not reached, and no process from that attempt remained. Direct executable startup from USB therefore remains unresolved and is not counted as passed. Running the portable folder locally requires no installer; actual clean-machine qualification remains pending. No disk, antivirus, network, driver or security settings were changed.

The verifier now uses exact process arguments, drains redirected output asynchronously, saves partial failure results and fixed diagnostic stages, and supports a guarded explicit execution directory. Existing directories are preserved. Normal startup diagnostics require the explicit `PLA_TRACE_STARTUP=1` opt-in. Offline verification modes emit fixed labels without paths, addresses or interface identifiers. The new startup gate exercises a real HWND, ContentRendered, dispatcher responsiveness and closure, rather than relying solely on an offscreen layout.

## Storage and source checks

Release build: zero warnings and errors. The 41-check runner passed with its default fixtures and again with an explicit fixture root on the USB drive. Added Windows read-only configuration and SQLite checks reject writes, preserve original file bytes and reopened state, restore only their own fixture attributes and clean up only the generated child directory. These are storage-layer checks; native error-dialog checks are recorded separately.

The launcher reported medium integrity (`S-1-16-8192`), and the app was launched without an elevation request. No standard-user claim is inferred merely from an absent UAC prompt.

## Interactive workflows

The native helper's earlier error was `Computer Use app approval timed out`. That result did not establish an application hang. A subsequent app-control request succeeded and returned the portable dashboard.

| Workflow | Current evidence |
|---|---|
| Session save/new/reopen on USB data directory | Passed; synthetic name and identity preserved |
| Invalid session | Passed; explicit error and prior session retained |
| Configuration save/load | Passed; selected synthetic configuration restored |
| Malformed/future configuration | Passed; error and prior state retained |
| Adapter selection and refresh persistence | Passed; same explicit choice retained and restored on session reopen |
| Missing-adapter configuration | Passed; unavailable choice cleared without substituting another adapter; saved choice restored on configuration load |
| Demo-enabled metadata export | Passed; privately verified exactly the eight expected metadata fields and zero events, without synthetic readings |
| Native read-only destination refusal | Passed at Windows Save As layer; cancel retained state. Separate 41-check runner verifies application storage failures and preservation |
| Unsaved-close Cancel/discard and restart | Passed; Cancel retained session and dirty state, discard closed own test changes, exact final package reopened |

Fourteen native workflow groups completed at 2026-10-15T13:31:35.7725579Z. Configuration-load cancellation also retained state. Export/configuration contents were verified privately; actual adapter names, addresses and stable identifiers remain in ignored fixture files only. Published previews use synthetic data.

The native workflow candidate had the same MainWindow, XAML and infrastructure source as the final candidate. The only subsequent application change made normal startup tracing opt-in; the final package then passed all four executable gates. At 2026-10-15T13:33:01.2233959Z its normal dashboard window title appeared in 2.569 seconds, with observed CoreCLR and WPF modules inside the extracted package folder. That observation complements the real-window readiness gate; a title alone does not establish responsiveness. The launcher reported medium integrity. App-control approval enabled the test helper; it does not establish owner visual acceptance.

## Remaining qualification

Direct USB executable startup, clean-machine execution, Server 2019/2022/2025, broader Windows 11 release qualification, final branding/theme and owner acceptance retain their own gates. Linux execution is future work. Local host checks do not establish overall Phase 1 acceptance or authorize Phase 2.

Earlier failures and narrower results remain recorded in [the feedback history](PHASE1_FEEDBACK.md) and its historical evidence; this record adds subsequent outcomes without erasing those failures.
