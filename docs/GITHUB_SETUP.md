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
| #1 Reusable core, configuration and sessions | Closed / Done | Accepted for this task scope using source/build and 36 offline checks; Linux execution untested |
| #2 WPF dashboard foundation | Open / In Progress | Implementation and offscreen checks complete; owner visual/interactive review and final branding remain |
| #3 Portable packaging and host validation | Open / In Progress | Host checks only; interactive read-only/USB, dialogs, close/error workflows and clean-machine acceptance remain |
| #4 Target operating-system acceptance | Open / Todo | Needs target OS testing: Windows 11 and Server 2019/2022/2025 |
| #5-#10 Later phases | Open / Todo | Existing scope and approval gates retained |

Phase 1 milestone remains open. A closed implementation task does not establish overall Phase 1 acceptance, compatibility on another OS, or owner acceptance.

Published contents are source, tests, build/verification scripts, configuration/lockfiles, licenses/notices, curated documentation, sanitized host evidence and a synthetic dashboard preview. Historical intake files, caches, generated build outputs, operational configuration/sessions, captures and local ZIPs are excluded.

Host evidence: Release build with zero warnings/errors; 36 offline core/storage/adapter checks; inspected WPF layout; extracted ZIP view-model/layout, packaged SQLite/configuration and synthetic render checks all exit 0; included .NET/WPF 10.0.13. See [acceptance evidence](PHASE1_ACCEPTANCE.md) and [original sanitized results](validation/PHASE1_HOST_RESULTS.json) and [36-check follow-up results](validation/PHASE1_FEEDBACK_RESULTS.json). No interactive or target-OS acceptance is inferred.

The exact organization handle is Limited-Underground. Existing repositories and Trail Project #1 are outside this project's scope. Release publication, website deployment, live captures and device actions retain separate authorization requirements.

The owner-feedback follow-up adds qualified adapter suggestions and the File menu. Current validation details are recorded in [PHASE1_FEEDBACK.md](PHASE1_FEEDBACK.md); implementation, interactive host checks and owner acceptance remain separate.



The latest feedback candidate has an unresolved portable startup gate: its synthetic layout mode exceeded the unchanged 60-second limit. Development-build normal startup, 46-adapter inventory, File menu and focused-field Ctrl+N/O/S/E checks passed. Remaining interactive checks were limited by native accessibility observation stalls. Issues #2 and #3 remain open; no current portable or overall Phase 1 acceptance is claimed.

