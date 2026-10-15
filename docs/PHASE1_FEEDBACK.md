# Phase 1 owner feedback and host follow-up

The owner reviewed the original Windows dashboard and requested clearer adapter selection for machines with many interfaces, and a File menu to keep the main screen uncluttered. These changes remain within the Phase 1 foundation. Phase 2 is not started.

## Implemented changes

- Ethernet entries show connected/disconnected state and suggested candidates before an explicit user selection.
- Suggestions use local link/IP/gateway metadata. A configured gateway does not establish the destination route or Internet access.
- Likely virtual adapters are identified using qualified name/description hints. An absence of those hints does not prove physical hardware.
- Equal candidates remain ambiguous; no adapter is automatically selected. Refresh preserves a selected stable identifier when available.
- Individual adapter inventory failures are isolated; unknown speed is labeled unavailable.
- File contains New/Open/Save/Export/Exit, with Ctrl+N/O/S/E shortcuts. Tools contains Settings. Existing unsaved-change and busy-operation protections remain.

## Evidence boundaries

The revised Release build passed with zero warnings/errors. An explicitly audited locked restore succeeded. The offline runner passed 36 checks: the prior 25 storage/model checks plus 11 synthetic adapter-ranking checks. A ranking-test failure was corrected to compare stable adapter identity and recommendation data instead of separately allocated address-array references; the ranking algorithm itself did not change for that test correction.

Independent source review found no blocking issue after adding narrow window-level routing for Ctrl+N/O/S/E before focused text controls. The self-contained candidate uses the existing .NET/WPF 10.0.13 runtime and dependency set. Interactive checks on the owner's Windows 11 Pro build 26200 x64 host are tracked separately from deterministic fixtures, owner acceptance, USB use, clean-machine validation and the other target operating systems.

Actual adapter names, addresses and identifiers must not be included in published screenshots or evidence. The documentation preview uses the synthetic validation mode only. No capture, active protocol operation, driver installation or network configuration change is part of this follow-up.

## Interactive checklist

Initial native launch stalled without a targetable window. After the background-inventory correction, a normal development build opened successfully and inventoried 46 Ethernet adapters on the current host. The File menu was usable. Ctrl+N/O/S/E were exercised while the session-name text field had focus: New created a fresh empty session, Open/Save/Export reached the expected dialogs, and canceling those dialogs retained the session. These results establish only the named cases; remaining workflows below need their own evidence.

- Passed: File menu and Ctrl+N/O/S/E from the session-name text field; canceled Open/Save/Export dialogs.
- Actual local Ethernet inventory passed (46 entries); explicit selection/refresh persistence remains pending.
- Session save/new/open with selection round trip.
- Configuration save/load/cancel and missing-adapter behavior.
- Metadata export with demonstration enabled, excluding illustrative samples.
- Invalid/future configuration and invalid session handling, preserving the prior state.
- Read-only destination behavior, distinguishing file-dialog refusal from application error handling.
- Unsaved-close Cancel and discard for synthetic test changes, followed by restart.

Owner review of the revised appearance, final branding, actual USB execution and the remaining Windows target matrix require their own evidence.


## Startup and adapter refresh follow-up

Normal startup originally called adapter enumeration inside the window constructor, before `Show()`. On this host, a portable review process remained running without an exposed dashboard window. The same portable candidate also timed out in a synthetic mode, so native enumeration is not established as the cause of that process stall. Synchronous constructor inventory was a separate source-level UI readiness risk. Synthetic validation bypasses adapter enumeration, so those passing modes did not establish normal startup readiness.

The correction moves local inventory off the UI thread. Its UI deadline does not claim cancellation of an operating-system query. Only one inventory task may remain outstanding; session selection is resolved against the current session when results are applied. Closing and late completion require separate guards. Actual post-correction launch evidence is recorded separately from source review.

A superseded shortcut-only portable candidate timed out in the 60-second synthetic layout check; it is not recorded as a successful final package. The subsequent startup-corrected candidate is verified separately. No timeout threshold was extended.


| Transition | Source review outcome |
|---|---|
| Startup to visible window | Native inventory is deferred until Loaded and runs on a worker; normal host window observed. |
| Refresh to pending | One native task is shared; the current session selection remains available. |
| Pending to timeout | UI deadline is ten seconds; retains list/task without claiming cancellation. |
| New/Open/config load during pending | Selection is reconciled against the current session ID at application time. |
| Success/failure | Success applies metadata without dirtying the session; failure retains list and allows retry. |
| Superseded request or closed window | Generation and closed checks prevent stale application. |


## Current portable gate

The startup-corrected self-contained candidate also exceeded the unchanged 60-second synthetic layout timeout, including one bounded diagnostic retry. Its other offline executable modes were not reached, and this candidate is not accepted as a portable package. The development build's visible dashboard and keyboard checks do not override that failure. Sanitized evidence distinguishes the current failure from the earlier successful candidate. The exact runtime/WPF loading cause remains unresolved.


The follow-up native accessibility observation also stalled before returning state. No additional session/configuration/error/export-content/close tests completed in that attempt. The app was not changed by that observation, and the untested checklist remains open.


An isolated Rebuild publish also exceeded the same smoke deadline. The earlier immutable compact-menu package still passed a bounded rerun. Runtime/native package files matched between successful and failing candidates; the desktop assembly differs. Source review found no enumeration path in synthetic mode. No unverified root-cause claim or timeout extension is made.

