# GitHub setup and Phase 1 source publication

Owner-authorized source publication and tracker reconciliation: October 15, 2026.

- Public Apache-2.0 repository: https://github.com/Limited-Underground/ProtocolLatencyAnalyzer
- Default branch: main; source history preserves the existing repository license commit.
- Private Project: https://github.com/orgs/Limited-Underground/projects/2
- Six phase milestones, ten repository issues and ten linked Project items.
- Status options: Todo, In Progress, Blocked, Done.
- Phase and Validation fields distinguish implementation, host checks, target-OS testing and owner review.

| Issue | Status | Validation / remaining work |
|---|---|---|
| #1 Reusable core, configuration and sessions | Closed / Done | Accepted for this task scope using source/build and 41 checks, repeated with explicit USB storage fixtures; Linux execution untested |
| #2 WPF dashboard foundation | Complete / Done scope | Owner accepted current dashboard/light styling for Phase 1; 14 native workflow groups passed; final official brand identity remains separate |
| #3 Portable packaging and host validation | Open / In Progress | All four local and USB package gates and native workflows passed on current host; clean-machine qualification remains pending |
| #4 Target operating-system acceptance | Open / In Progress preparation | Authorized isolated Server Desktop Experience evaluation pipeline; OS execution still Needs Testing; fresh Windows 11 VM coverage unestablished, current host results valid |
| #5-#10 Later phases | Open / Todo | Existing scope and approval gates retained |

Phase 1 milestone remains open. A closed implementation task does not establish overall Phase 1 acceptance, compatibility on another OS, or owner acceptance.

The owner resumed bounded Phase 1 Server qualification, then requested internal-drive cleanup and relocation of evaluation resources to internal SSDs before continuing. Internal relocation is complete; qualification is held with the trial Off until the live memory guard passes. Issue #4 remains In Progress with OS execution Needs Testing; no prior host result is superseded.

Server evaluation preparation has validated elevated read-only Hyper-V access and protected five preexisting VMs, which remain unchanged. The receipt-owned Server 2019 trial is now confirmed Off. Its configuration remains 3 GiB fixed RAM, 2 vCPUs, a 64 GiB dynamic disk and one disconnected Ethernet NIC. Its successful guarded start recorded 4.70 GiB available RAM; the latest 3.7 GiB snapshot does not pass the unchanged 4.5 GiB creation/start guard. Server 2019's official ISO completed length/hash validation and inspection selected Standard Evaluation Desktop Experience 10.0.17763.3650. After the owner manually restarted the trial and pressed a key, installation reached 21% preparing files. No desktop-ready state or guest application qualification was established. Server 2022 is paused at 3,189,714,944 of 5,044,094,976 bytes with its partial preserved; Server 2025 has not started. Supported cache and Windows component cleanup completed, including a separately allowlisted 23.85 GiB build-download cache cleanup. Media copy to internal NVMe completed with full source/destination SHA-256 verification and the original retained. The VM relocation to internal SATA SSD completed with the trial Off, internal installation/answer media attached and all five preexisting VMs verified unchanged. The initial parameter-conversion failure occurred before storage copying and was resolved by a guarded retry; its receipt is retained. This preparation issue did not involve application execution. Qualification awaits a fresh passing memory check. No OS/application qualification has passed. Clean Server evidence will apply only to those baselines; a fresh Windows 11 VM is not an added mandatory gate. See [the evaluation plan](PHASE1_SERVER_TEST_PLAN.md).

Published contents are source, tests, build/verification scripts, configuration/lockfiles, licenses/notices, curated documentation, sanitized host evidence and a synthetic dashboard preview. Historical intake files, caches, generated build outputs, operational configuration/sessions, captures and local ZIPs are excluded.

Current host evidence: Release build with zero warnings/errors; 41 core/storage/adapter checks, repeated with USB storage fixtures; all four executable gates from local and USB package folders including real-window/dispatcher/closure; 14 native workflow groups. Included runtime remains .NET/WPF 10.0.13. See [current host checks](PHASE1_WINDOWS_HOST_CHECKS.md), [current sanitized results](validation/PHASE1_WINDOWS_HOST_RESULTS.json), [acceptance evidence](PHASE1_ACCEPTANCE.md), [original results](validation/PHASE1_HOST_RESULTS.json) and [historical 36-check follow-up results](validation/PHASE1_FEEDBACK_RESULTS.json). Owner acceptance of the current dashboard/light styling is explicit; broader target-OS acceptance is not inferred.

The exact organization handle is Limited-Underground. Existing repositories and Trail Project #1 are outside this project's scope. Release publication, website deployment, live captures and device actions retain separate authorization requirements.

The owner-feedback follow-up adds qualified adapter suggestions and the File menu. The [feedback history](PHASE1_FEEDBACK.md) retains earlier failures; [current Windows host checks](PHASE1_WINDOWS_HOST_CHECKS.md) record subsequent verification. Implementation, interactive host checks and owner acceptance remain separate.



Historical feedback candidates exceeded the unchanged 60-second startup gate, and early native-helper approval/observation failures limited interactive checks. Subsequent testing passed all four local packaged executable gates and the named native workflows; those results do not erase earlier failures. The exact unchanged USB smoke failed while a concurrent copy ran, passed in 19.671 seconds during the separately approved brief hold, and failed again after resumption. Copy resumption and continued progress were independently verified. Copy-related USB contention is demonstrated; the underlying physical/filter/mapped-I/O mechanism is not fully isolated. After no Robocopy processes were present, a fresh extraction of the same immutable ZIP passed all four USB gates at 2026-10-15T17:28:48.2330088Z, including real-window startup in 2.3335344 seconds. No application workaround, deadline increase or package-format change was adopted; clean-machine qualification remains pending. Issue #2 is complete for the accepted Phase 1 dashboard scope; #3 remains open for clean-machine qualification. Phase 1 remains open and Phase 2 is held. The owner separately accepted the current dashboard/light styling; app-control approval alone did not establish that acceptance.
