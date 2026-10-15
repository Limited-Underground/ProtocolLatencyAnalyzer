# Roadmap

| Phase | Deliverables | Acceptance gate / dependencies |
|---|---|---|
| 0: Assessment | Twelve-part feasibility/architecture report | Completed read-only assessment; owner subsequently approved Phase 1 |
| 1: Foundation | WPF shell, read-only Ethernet selection, capabilities, configuration, logs, sessions, portable build | Foundation tests, host workflow/package evidence; target OS gaps explicit |
| 2: Active measurements | Proposed ICMP/TCP/DNS/HTTP(S), bounded scheduler, profiles and reports | Approve exact subset/test plan; validate responder timing/failure/cancellation/source behavior |
| 3: Native passive | Backend proof, bounded capture/reassembly, qualified protocol analysis | Authorized captures; per-OS bytes/timestamp/drop/overhead/recovery validation |
| 4: Custom and specialized | Dedicated custom editor/templates/preview/dry-run/matching milestone, then ordered protocol modules | Approve order; exact bytes and matched responses, malformed/partial/late/duplicate tests |
| 5: Advanced/optional | Evidence-based correlation/discovery; separately approved capture extensions | Earlier metric validity, licensing, lifecycle and scope decisions resolved |
| 6: Release/publication | Notices, contribution/security docs, build/package checksums, release and website materials | Owner acceptance, actual OS qualification, dependency audit and separate publication approval |

GitHub repository/Project creation follows results review and a separate authorized setup plan. Website work and existing migration/source trees remain outside this project work.

Linux is future work: preserve reusable contracts and models now; later select Linux UI, capture adapter, privileges and packaging with its own feasibility/acceptance plan. macOS has no current commitment.
