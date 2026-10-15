# Phase 1 isolated Server evaluation plan

The owner authorized official Microsoft trial-image downloads, isolated Windows Server 2019/2022/2025 Desktop Experience VM qualification, and deletion of only newly created project evaluation resources afterward. Phase 1 remains open; Phase 2 has not started. The current dashboard and light styling are owner-accepted for Phase 1, without a final official brand-identity or future theme commitment.

## Current checkpoint and prerequisites

Official Microsoft image downloads are in progress, approximately 18.85 GB total, with Server 2019 transferring and 2022/2025 queued. Download completion is preparation only. Official download provenance is validated. The creation attempt exited before recording intent; a subsequent complete elevated inventory confirms no new evaluation VM exists. No guest OS has started or passed qualification.

Hyper-V is installed and running; an elevated read-only inventory succeeded and recorded five protected preexisting VMs. No host setting was changed. The management access method is validated; lifecycle commands still use ordinary Windows UAC consent under the existing authorization. No feature installation, host reboot, or host networking change is planned.

The host has about 15.9 GiB RAM. The owner independently shut down their Home Assistant VM; available memory briefly reached 5.25 GiB. The latest elevated inventory found all five preexisting VMs Off, no new evaluation VM, and 2.69 GiB available RAM. The selected guest uses 3 GiB fixed RAM, with a start guard of at least 4.5 GiB available to retain 1.5 GiB host headroom. Recheck before creation and each start; the guard is currently unmet, so wait and report the resource constraint. Do not close owner applications or stop/start any preexisting VM. The project volume has approximately 686 GiB free on USB storage; recheck disk availability and expect USB contention to affect timings. Resource constraints are distinct from application failures.

Run one VM at a time: 2 vCPUs, 3 GiB fixed RAM, one 64 GiB dynamic VHDX, Generation 2, Microsoft Windows Secure Boot template, automatic checkpoints disabled. Desktop Experience's 2 GiB minimum/4 GiB recommendation makes this a constrained functional test, not a performance baseline. Include one disconnected virtual Ethernet NIC for adapter coverage, without host/guest switch connection. Do not create/change host switches, routes, adapters, features, or preexisting VM settings. If the official image cannot boot/install under this configuration, record the exact blocker and review the plan rather than silently weakening it.

## Inventory and ownership controls

Before any VM mutation, inventory existing VM identities, attached disks, and resolved storage paths. Retain that protected inventory privately. Put only newly created evaluation resources beneath this project's `.artifacts/server-evaluations` directory, with a dedicated per-OS child directory. Refuse collisions or paths outside that root. Retain a private creation receipt containing the exact new VM ID, resolved directory/VHDX paths, creation time, and artifact ownership. Never record credentials, private paths, or VM IDs in public results.

Use the same immutable self-contained application ZIP for each OS and record its SHA-256. No SDK or separately installed .NET runtime is allowed in the guest. Transfer only the package and synthetic fixtures through a project-owned offline medium; keep the guest NIC disconnected. The application package may contain runtimes/native libraries as published. Do not import owner sessions, captures, credentials, or other project data.

## Four qualification gates per operating system

| Gate | Required evidence | Pass condition |
|---|---|---|
| 1. Image and clean OS baseline | Official Microsoft source/final URL, downloaded bytes and SHA-256, ISO edition/language/build, installed Desktop Experience edition/build, architecture, patch inventory, evaluation/activation state, clock, CPU/RAM/disk configuration | Clean x64 installation reaches the desktop; exact baseline recorded; no SDK/separate runtime added |
| 2. Immutable package validation | ZIP hash, extraction result, packaged smoke/storage/render modes and actual window/dispatcher/closure check, exit codes and timings | All four existing executable gates pass with their unchanged 60-second deadlines; actual launch is observed and errors retained |
| 3. Standard-user interactive workflows | Standard-user/elevation state and sanitized results for the checklist below | Named workflows pass as standard user without installing a runtime, sending traffic, capturing, or elevating the application |
| 4. Evidence and cleanup | Private raw receipts, sanitized summary, failure details, exact owned-resource cleanup receipt and protected-inventory comparison | Results distinguish passes/failures/untested; only receipt-owned VM/artifacts removed; existing resources unchanged |

Offline evaluation guests are not assumed patched or activated. Record their actual state; do not describe an unpatched evaluation image as a current production baseline. Any later update/network/activation requirement needs a bounded plan, since this plan does not attach guests to a network or provide credentials.

## Standard-user interactive checklist

1. Launch the extracted executable, observe an empty initial state, identity, readable layout, and unavailable capture/protocol actions. Record actual display resolution/scaling.
2. Inspect File menu and Ctrl+N/O/S/E while the session-name field has focus; cancel dialogs and verify state remains intact.
3. Toggle demonstration data and verify its label; save/export excludes synthetic measurements. No active tests or capture can start.
4. Rename/save/new/open a synthetic session; verify metadata and selection restoration. A disconnected guest may have no Ethernet inventory: verify truthful empty/unavailable state instead of requiring a physical interface or changing networking.
5. Save/load configuration; reject invalid, future-version, and missing-adapter fixtures safely. An absent adapter remains unavailable and does not become a routing claim.
6. Export session metadata; check expected fields and absence of secrets, payloads, and synthetic measurements.
7. Test a project-owned read-only destination and malformed session. Distinguish an operating-system dialog refusal from an application write failure; verify previous valid state survives. Do not change host security/ACL settings.
8. Edit the synthetic session, close/Cancel, verify retention, then discard only that test session and reopen. Verify clean shutdown and no acquisition resume.

Retain all failures, deadlines and environment limitations; do not lengthen deadlines to create a pass. A failed capture/protocol action is not part of Phase 1 because those capabilities are not implemented. Do not infer their compatibility from foundation checks.

## Cleanup and reporting

Export evidence before cleanup. Resolve and verify every cleanup target remains beneath the dedicated evaluation root; match each VM by its exact creation-receipt ID and confirm attached disks are exclusively owned by it. Remove only that VM and its allowlisted disks/media/test artifacts after use. Never delete preexisting VMs/disks, shared directories, owner data, or artifacts with uncertain ownership. If ownership or deletion is uncertain, stop cleanup and report retained resources rather than broadening deletion. Preserve evidence and cleanup receipts outside the disposable per-VM directories.

Public results include OS/edition/build/patch/evaluation baseline, package hash, standard-user status, gate timings/outcomes, unresolved limitations, and cleanup confirmation without private paths/VM IDs/credentials. Download, installation, test execution, acceptance, and deletion are separate states. Keep issue #3 clean-machine qualification pending until relevant evidence exists; issue #4 can show evaluation preparation In Progress with OS results still Needs Testing. A fresh Server installation establishes clean-machine evidence for that Server baseline only. Fresh Windows 11 VM coverage is unestablished, not an added mandatory gate: current Windows 11 host evidence remains valid. Bounded Phase 1 acceptance may combine that evidence with qualified clean Servers; broader Windows releases and production configurations remain distinct.
