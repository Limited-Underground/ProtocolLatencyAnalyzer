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
| #2 WPF dashboard foundation | Open / In Progress | Named native interactive workflows passed; owner visual review and final branding remain |
| #3 Portable packaging and host validation | Open / In Progress | Local four gates and native workflows passed; copy-related USB contention demonstrated by approved failure/pass/failure smoke comparison; full USB gates after copy completion and clean-machine qualification pending |
| #4 Target operating-system acceptance | Open / Todo | Needs target OS testing: Windows 11 and Server 2019/2022/2025 |
| #5-#10 Later phases | Open / Todo | Existing scope and approval gates retained |

Phase 1 milestone remains open. A closed implementation task does not establish overall Phase 1 acceptance, compatibility on another OS, or owner acceptance.

Published contents are source, tests, build/verification scripts, configuration/lockfiles, licenses/notices, curated documentation, sanitized host evidence and a synthetic dashboard preview. Historical intake files, caches, generated build outputs, operational configuration/sessions, captures and local ZIPs are excluded.

Current host evidence: Release build with zero warnings/errors; 41 core/storage/adapter checks, repeated with USB storage fixtures; four extracted local package gates including real-window/dispatcher/closure; 14 native workflow groups. Included runtime remains .NET/WPF 10.0.13. See [current host checks](PHASE1_WINDOWS_HOST_CHECKS.md), [current sanitized results](validation/PHASE1_WINDOWS_HOST_RESULTS.json), [acceptance evidence](PHASE1_ACCEPTANCE.md), [original results](validation/PHASE1_HOST_RESULTS.json) and [historical 36-check follow-up results](validation/PHASE1_FEEDBACK_RESULTS.json). No broader target-OS or owner acceptance is inferred.

The exact organization handle is Limited-Underground. Existing repositories and Trail Project #1 are outside this project's scope. Release publication, website deployment, live captures and device actions retain separate authorization requirements.

The owner-feedback follow-up adds qualified adapter suggestions and the File menu. The [feedback history](PHASE1_FEEDBACK.md) retains earlier failures; [current Windows host checks](PHASE1_WINDOWS_HOST_CHECKS.md) record subsequent verification. Implementation, interactive host checks and owner acceptance remain separate.



Historical feedback candidates exceeded the unchanged 60-second startup gate, and early native-helper approval/observation failures limited interactive checks. Subsequent testing passed all four local packaged executable gates and the named native workflows; those results do not erase earlier failures. The exact unchanged USB smoke failed while a concurrent copy ran, passed in 19.671 seconds during the separately approved brief hold, and failed again after resumption. Copy resumption and continued progress were independently verified. Copy-related USB contention is demonstrated; the underlying physical/filter/mapped-I/O mechanism is not fully isolated. Full USB four-gate verification after the copy finishes remains pending, and no application workaround or package-format change was adopted. Issues #2 and #3 remain open, overall Phase 1 acceptance is not claimed, and Phase 2 is held. App-control approval is permission to operate the test helper, not owner acceptance of the dashboard.

