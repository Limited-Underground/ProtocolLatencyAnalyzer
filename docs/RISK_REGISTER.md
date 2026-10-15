# Risk register

| ID | Risk | Likelihood | Impact | Mitigation | Blocking? |
|---|---|---|---|---|---|
| R-001 | Untested OS/package combinations presented as supported | High currently | High | Keep evidence matrix; actual per-OS package checks | Before support/release claims |
| R-002 | Native capture lacks needed live bytes/features | Medium–High | High | Provider proof and OS/build qualification | Before passive commitment |
| R-003 | Temporary Npcap violates rights or disturbs existing installs | Medium–High | High | Defer; license/ownership/recovery plan | Before managed installation |
| R-004 | Misclassified timing/loss or incomplete observations | Medium | High | Explicit metric contracts, clock domains, validity and reference tests | Before measurement acceptance |
| R-005 | Parser/retention/backlog exhausts resources | Medium | High | Bounds, fuzzing, drop counters, sustained tests | Before capture/custom acceptance |
| R-006 | Local logs/session/export exposes sensitive metadata/secrets | Medium | High | Sanitized logs, explicit paths, no secrets, synthetic fixtures | Before relevant feature acceptance |
| R-007 | Unintended active state changes | Medium | High | Explicit finite starts, previews, authorized targets | Before active tests |
| R-008 | Native SQLite/package graph fails deployment or notices | Medium | High | Inspect pinned graph; self-contained package test and notices | Before distribution |
| R-009 | Read-only/removable media or failed writes corrupt state | Medium | Medium–High | Explicit storage, transactions, atomic replace, error/recovery tests | Before foundation acceptance |
| R-010 | Provisional styling treated as final branding/theme approval | High without labeling | Medium | Decision log; obtain official assets | Before visual acceptance |
| R-011 | Windows dependencies leak into reusable core | Medium | Medium–High | Project boundaries and dependency checks | Before core acceptance |
| R-012 | Scope expands before dependable foundation | High | High | Independently accepted phases | Ongoing control |
| R-013 | Bundled runtime becomes unserviced | Medium over time | High | Rebuild/review servicing for releases | Before each release |

Owner has approved WPF/Phase 1 and selected Apache-2.0. Final architecture/brand/platform details still have explicit provisional or pending states; do not silently upgrade them through implementation.
