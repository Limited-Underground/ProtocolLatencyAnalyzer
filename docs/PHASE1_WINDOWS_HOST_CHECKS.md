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

The source/project drive was verified as USB. A fresh extraction of the exact final ZIP (same checksum above; 257 files) was verified at 2026-10-15T13:36:13.8970870Z. Its first model/layout check exceeded the unchanged 60-second deadline with no diagnostic stages emitted; recorded duration including bounded cleanup was 61.6248782 seconds, exit code unavailable, failure `ProcessTimeout`. The remaining three checks were not reached, and no process from that attempt remained. That attempt did not qualify USB execution; the later controlled investigation below records a single smoke pass during the copy hold. Running the portable folder locally requires no installer; actual clean-machine qualification remains pending. No disk, antivirus, network, driver or security settings were changed.

The verifier now uses exact process arguments, drains redirected output asynchronously, saves partial failure results and fixed diagnostic stages, and supports a guarded explicit execution directory. Existing directories are preserved. Normal startup diagnostics require the explicit `PLA_TRACE_STARTUP=1` opt-in. Offline verification modes emit fixed labels without paths, addresses or interface identifiers. The new startup gate exercises a real HWND, ContentRendered, dispatcher responsiveness and closure, rather than relying solely on an offscreen layout.

## USB contention investigation

Read-only host checks identified an active Robocopy job writing to the same USB drive. Its creation preceded the final USB gate. Four correlated one-second samples showed USB writes of 10.4–24.4 MB/s and queue lengths of 3–5, closely tracking the copy's write rate. Six matching package files totaling 32.77 MB took 3.392/2.870 seconds to read and hash from USB versus 0.094/0.099 seconds from the local copy. Cache state was uncontrolled, and read timings include hashing. The volume reported healthy, with no matching storage warning/error events in the preceding 12 hours; these observations do not prove device health. Checked files/folders had no offline, recall, compression, encryption or reparse flags, and no Zone.Identifier stream.

The owner explicitly approved briefly pausing only the identified copy for a guarded comparison. The exact final package and smoke arguments were unchanged, and the deadline remained 60 seconds:

| Copy state | USB model/layout smoke result |
|---|---|
| Active, before pause | Timed out before any stage marker; recorded 61.625 seconds including bounded cleanup |
| Held, after three-second settling interval | Exit 0 in 19.6706534 seconds; all smoke stages completed |
| Resumed, reversal control | Timed out in 60.3377487 seconds; no stage markers; only 0.125 CPU seconds by the 55-second sample |

The hold lasted 22.6998139 seconds, including settling. The watchdog reported successful resumption with no failure. Independent observations confirmed the same copy identity remained alive and advanced its read/write counters; 2,781,557 write bytes advanced over a 4.965-second observation. Copy load varied after resumption, so the comparison is not a constant-load benchmark.

This failure/pass/failure sequence strongly demonstrates copy-related contention on the USB drive as a cause of the observed startup timeout. It does not isolate physical disk latency from filesystem filters, mapped-file loading or caching, nor establish the cause of every historical stall. No application startup fix was adopted; the diagnostic bootstrap was not built and was removed from application source, and the flat package format and dependency locks remain unchanged. There was no timeout increase, antivirus exclusion or disk/security-setting change. Only the separately approved temporary copy hold changed process state, and the copy resumed.

Only the USB model/layout smoke gate passed during the hold; packaged storage, render and real-window startup were not run in that hold. Full USB qualification was therefore pending at that stage. The subsequent post-copy run below completed all four gates. The local four-gate result, USB storage tests and 14 native workflow groups remain valid separate evidence.

## Post-copy USB verification

At 2026-10-15T17:27:48.5143182Z, no Robocopy processes were present; two USB queue samples were zero. A fresh extraction of the exact unchanged ZIP above then passed all four gates directly from the USB drive at 2026-10-15T17:28:48.2330088Z:

| USB executable check | Seconds | Outcome |
|---|---:|---|
| Model and WPF layout | 4.5486286 | Exit 0 |
| Packaged SQLite and configuration | 1.4430404 | Exit 0 |
| Synthetic WPF render | 2.3118562 | Exit 0 |
| Real window, rendering, dispatcher idle and clean closure | 2.3335344 | Exit 0 |

All four retained the 60-second process deadline. Source HEAD at verification was `ad3af21`; the application and immutable package were unchanged. No new copy hold, application fix, timeout increase or settings change was required. The earlier 90-second watchdog limit bounded only the separately approved copy hold; it never changed the application check deadline.

Post-test observations at 2026-10-15T17:29:39.6675829Z again found no Robocopy processes and USB queues of zero in two samples. These are bounded before/after observations, not a claim about copy/migration success or continuous disk inactivity. The four USB executable gates are passed for this Windows host; clean-machine, other-OS and owner acceptance remain separate.

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

Clean-machine execution, Server 2019/2022/2025, broader Windows 11 release qualification, final branding/theme and owner acceptance retain their own gates. Linux execution is future work. Current-host local and USB package gates and the named native workflows passed. Phase 1 remains open and Phase 2 has not started; these results do not close overall acceptance.

Earlier failures and narrower results remain recorded in [the feedback history](PHASE1_FEEDBACK.md) and its historical evidence; this record adds subsequent outcomes without erasing those failures.
