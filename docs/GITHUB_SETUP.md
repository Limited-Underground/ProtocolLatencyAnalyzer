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
| #1 Reusable core, configuration and sessions | Closed / Done | Accepted for this task scope using source/build and 25 offline checks; Linux execution untested |
| #2 WPF dashboard foundation | Open / In Progress | Implementation and offscreen checks complete; owner visual/interactive review and final branding remain |
| #3 Portable packaging and host validation | Open / In Progress | Host checks only; interactive read-only/USB, dialogs, close/error workflows and clean-machine acceptance remain |
| #4 Target operating-system acceptance | Open / Todo | Needs target OS testing: Windows 11 and Server 2019/2022/2025 |
| #5-#10 Later phases | Open / Todo | Existing scope and approval gates retained |

Phase 1 milestone remains open. A closed implementation task does not establish overall Phase 1 acceptance, compatibility on another OS, or owner acceptance.

Published contents are source, tests, build/verification scripts, configuration/lockfiles, licenses/notices, curated documentation, sanitized host evidence and a synthetic dashboard preview. Historical intake files, caches, generated build outputs, operational configuration/sessions, captures and local ZIPs are excluded.

Host evidence: Release build with zero warnings/errors; 25 offline core/storage checks; inspected WPF layout; extracted ZIP view-model/layout, packaged SQLite/configuration and synthetic render checks all exit 0; included .NET/WPF 10.0.13. See [acceptance evidence](PHASE1_ACCEPTANCE.md) and [sanitized results](validation/PHASE1_HOST_RESULTS.json). No interactive or target-OS acceptance is inferred.

The exact organization handle is Limited-Underground. Existing repositories and Trail Project #1 are outside this project's scope. Release publication, website deployment, live captures and device actions retain separate authorization requirements.
