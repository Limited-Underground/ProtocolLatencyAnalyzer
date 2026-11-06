# Build, packaging and recovery

Read the [frozen current handoff](../CONTRIBUTING.md) and
[architecture](ARCHITECTURE.md). These procedures explain declared inputs; they
do not start a product build/restore/launch without the foundation resource slot
or grant network, VM, installation, release or deployment authority.

## Choose the source profile first

| Profile | Exact input | What a fresh reader receives |
|---|---|---|
| Published foundation | Public repository `Limited-Underground/ProtocolLatencyAnalyzer`, main `3c8367f3d4a6780061046c551df0d4a6f40e8447` | WPF inventory/session/configuration foundation; no TCP engine. Only progress documentation differs from application base `5abeb7f`. |
| Frozen TCP preview | Implementation base `5abeb7f67cb46a53263acdce25f86661b91213f0` plus the separately preserved full TCP/desktop/packaging overlay | Exact source files pinned in [lineage](validation/FOUNDATION_LINEAGE_20261102.json). Not remotely published; a plain Git clone is insufficient. |
| Accepted binary | `ProtocolLatencyAnalyzer-0.1.0-win-x64-tcp-preview-20261029-r2.zip` | Exact unpublished self-contained candidate; not a rebuild or release. |

Do not apply headless, desktop and later packaging patches blindly in sequence:
some patch contexts are historical and overlap. Prefer the manifest-verified full
source snapshot for the final preview; retain individual patches as provenance.
The documentation candidate changes no product code and does not integrate this
snapshot into main.

## Environment and dependencies

* Windows with PowerShell 7 for repository scripts and exact process arguments.
* .NET SDK `10.0.303`, `rollForward=latestPatch`, previews disabled in `global.json`.
  Record the actual selected SDK/host before a future build; do not upgrade pins.
* `net10.0` Core/Infrastructure/tests, WPF `net10.0-windows` Desktop.
* Self-contained `win-x64`, .NET/WPF runtime `10.0.13`, version `0.1.0`, no trimming
  or single-file bundling.
* Microsoft.Data.Sqlite `10.0.13`, SQLitePCLRaw `2.1.13` transitives; exact hashes
  in normal `packages.lock.json` and app `packages.win-x64.lock.json` files.
* `NuGet.config` clears inherited sources and uses `https://api.nuget.org/v3/index.json`.
  Default package cache is project-local `.artifacts/packages`.

See [dependencies/notices](DEPENDENCIES.md). SDK packs, NuGet availability and
the Windows host are external recovery prerequisites. Locks prove selected
dependency identities, not continued supplier availability. A copied warm cache
must be declared as an input; it is not independent remote reconstruction.

## Fresh source retrieval

In a new approved C: task directory, never the dirty canonical checkout:

```powershell
git clone https://github.com/Limited-Underground/ProtocolLatencyAnalyzer.git pla-foundation
Set-Location pla-foundation
git switch --detach 3c8367f3d4a6780061046c551df0d4a6f40e8447
git rev-parse HEAD
git status --short
git remote -v
```

That retrieves the published profile only. November 2 foundation work actually
cloned this public source; no submodule or Git LFS pointer was found in the selected
source tree. The canonical checkout and its historical worktree registrations
were not reset or retired. Offline history is also preserved as an all-refs bundle
and raw Git files in private custody; neither makes the unpublished overlay public.

## Locked build and focused verification

After an explicit resource-slot grant, from the chosen isolated profile:

```powershell
dotnet --info
.\scripts\Build.ps1 -Configuration Release
.\scripts\Test.ps1 -Configuration Release
```

`Build.ps1` sets a project-local CLI home, performs locked solution restore and
Release build without another restore. `Test.ps1` calls that build, then the console
runner. For an independent reconstruction, start with an empty task-owned
`NUGET_PACKAGES`, `TEMP` and `TMP`, record them, and retain restore/network failures.
Do not quietly fall back to the original checkout, user cache or an unrecorded SDK.

The public profile has the 41-check foundation runner. The preserved TCP profile
adds 186 deterministic checks without socket traffic by default. Its actual WPF
synthetic control mode is the built Desktop application's `--verify-tcp-ui`; it
has 86 assertions. These are historical accepted counts, not new execution here.
The console `--tcp-loopback` option performs real local socket traffic and requires
its own authorized fixture plan; do not run it or any target test during this freeze.

Do not repeat old host/USB/OS matrices for documentation-only edits. A future
clean build must record exact source overlay, selected SDK, lock hashes, commands,
stdout/stderr, exit codes and outcome, separately from runtime acceptance.

## Reproduce a package without replacing evidence

For the preserved TCP profile, `Publish.ps1` checks publish directory, archive and
checksum-sidecar collisions before SDK invocation. It publishes self-contained
Windows x64, stages support files/notices into output, creates a flat ZIP and
lowercase SHA-256 sidecar. It no longer stages vendor notices into tracked source.
Public-main scripts predate those fixes; inspect the selected profile first.

Use one fresh matching name for publish and verification:

```powershell
.\scripts\Publish.ps1 -OutputName win-x64-foundation-recovery-UNIQUE
# Launch/extraction step requires a separate resource-slot grant:
.\scripts\Verify-Package.ps1 -OutputName win-x64-foundation-recovery-UNIQUE
```

The preview verifier also supports distinct `-ValidationName`, an explicit new
absolute `-ExecutionDirectory` and an explicit new `-StorageDirectory` containing
a `.artifacts` directory segment. Those are preview-overlay parameters, not a
promise about old main. It checks ZIP hash, extracts to a new destination and runs
four package gates (model/layout, storage, render, real-window startup), with
60-second process/output deadlines and partial failure receipts. The fifth
TCP UI mode was invoked separately in the retained qualification; the script does
not claim to run it automatically.

Default deeply nested storage failed with SQLite error 14. A deliberately short
synthetic `.artifacts` root passed. This is a diagnostic limitation, not acceptance
of every path length or a silent-relocation policy. Preserve failure receipts and
do not mask the defect by presenting only a short-path pass.

### Exact accepted package integrity

The TCP ZIP is 66,743,668 bytes with 439 entries and SHA-256:

```text
66bb169b749de0ee3f70339a0798c7edea4f49c2d9035b86a01c06d7ceea6c13
```

Verify both the archive digest and per-entry/file manifest before claiming exact
retrieval. Check flat layout, application EXE/DLL/deps/runtimeconfig, Core and
Infrastructure, native SQLite, both runtime packs, licenses and documents.
Reject unexpected source caches, sessions or credentials. November 1 checks
matched all 439 accepted files after execution; one synthetic validation PNG
was an expected extra generated file. ZIP entry count is not an acceptance score.

The artifact embeds documentation as it existed on October 29. New foundation
documentation cannot be inserted into it without changing the accepted ZIP.
Retain that archive unchanged; a new build must have a new identity. Deterministic
compiler settings or selected identical assemblies do not prove byte-identical ZIPs.

## Preserve and restore safely

The November 2 ordinary-source checkpoint preserved three full admitted source
profiles (main, TCP overlay, packaging overlay), raw common Git, all-refs bundle,
historical patches/reviews, selected November 1 evidence and exact accepted ZIP.
The private capture plan and manifest map every payload path and SHA-256. It
verified 612 files / 148,273,227 bytes, current-source equality, membership, copied
Git integrity and bundle verification on a different physical internal disk.
It does not establish offsite, offline or laptop-loss protection.

Exclusions: credentials/DPAPI and private guest baseline, live/failed VHDX and VM
state, historical external Temp working bytes, regenerated caches and expanded
runtime fixtures, and unrelated projects. Earlier checkpoints remain retained for
their admitted history. The private guest custody gap is explicit, not solved by
copying encrypted credentials into ordinary backup storage.

When a separately authorized isolated recovery slot is available:

1. Obtain the private checkpoint and its capture plan from the product/storage
   owner. Verify the sealed manifest, every admitted file's size/hash and the
   expected source/overlay/package identities. Refuse missing or extra payload.
2. Choose a new C: task root and empty output/cache/data directories. Keep the
   original checkpoint, canonical source and accepted ZIP unchanged.
3. Retrieve public base history remotely, or clone the declared all-refs bundle
   for an explicitly offline drill. Check out the selected pinned base. Restore
   only manifest-admitted `source/tcp-worktree/` working files over this disposable
   pinned base when reconstructing TCP; verify all 35 code/script/project/lock
   fingerprints in the public lineage record. Do not call the result remote-only.
4. Do not activate copied linked-worktree `.git` pointers: they retain absolute
   original identities. A restored copy must use its own Git store. Do not alter
   the original store or copy pointers as an expedient repair.
5. Retrieve the exact ZIP independently from `accepted-package/`, verify digest,
   size and entry manifest without launching it, and retain the retrieval receipt.
6. If a build is authorized, use only declared SDK/locks/dependency inputs and
   new output names. Compare whole-package and member hashes, reporting differences
   rather than assuming deterministic output. Do not replace the exact binary.
7. Runtime/data restoration is a separate gate using synthetic configuration and
   sessions. Metadata JSON is not a `.plasession` backup. Preserve SQLite sidecars
   when applicable and use a supported consistent snapshot, not an unchecked live
   file copy. No production/customer data or existing VM access is needed.

### Five independent recovery claims

| Claim | Evidence at this foundation checkpoint | Status/limit |
|---|---|---|
| Source and history retrievable | Fresh public HTTPS clone of `3c8367f`; separate manifest-verified TCP source/Git checkpoint | Public base demonstrated; overlay local backup only, not public remote recovery |
| Exact accepted binary retrievable | ZIP copied to new checkpoint and SHA-256 matched | Local independent-disk retrieval verified; no remote artifact store or release |
| Source rebuild succeeds | November 2 locked publish from the sealed 67-file TCP snapshot with fresh package caches | Qualified result on the recorded host with declared installed SDK and packs. Clean-machine and hermetic toolchain recovery remain open. |
| Rebuild byte equality/explained differences | 431 of 439 ZIP member contents match the accepted preview. Eight differ. | The archive differs. Full binary-difference causality and IL equivalence remain open. |
| Runtime/data restoration demonstrated | Historical synthetic SQLite/configuration, WPF controls and loopback evidence | No new independent restore; no physical Ethernet or full target-OS acceptance |

Independent review `pla-cold-reconstruction-001` returned a qualified pass for the
isolated source publish and package comparison. Its receipt SHA-256 is
`a4b323ee8c1ed910224210ccefd685731927e9b789e27fbb5d57fc1005397770`.
The runtime evidence from this reconstruction consists of packaged metadata.

## Document ownership map

No existing documents are moved or deleted by this candidate.

| Reading need | Current route | Retained history/limits |
|---|---|---|
| Scope/status/rules | [CONTRIBUTING](../CONTRIBUTING.md), Project #2 | Phase 1 acceptance and progress remain dated evidence, not new authority |
| Architecture/measurement | [ARCHITECTURE](ARCHITECTURE.md), [Structurizr source](architecture/workspace.dsl) | `ARCHITECTURE_PROPOSAL.md` and `PROTOCOL_CAPABILITIES.md` predate/describe later planning; source-profile labels resolve apparent contradictions |
| Build/test/package/recovery | This guide, owning scripts, lockfiles, lineage JSON | `PHASE1_WINDOWS_HOST_CHECKS.md` and `PHASE1_SERVER_TEST_PLAN.md` retain original procedures/results and lifecycle limits |
| Acceptance | `PHASE1_ACCEPTANCE.md`, [November 1 TCP result](validation/TCP_PREVIEW_SERVER2019_20261101.json) | Original Server/USB/failure artifacts retained privately; no full acceptance or percentage inferred |
| Rationale/risks/dependencies | `DECISION_LOG.md`, `DEPENDENCIES.md`, architecture sections 9–11 | `ROADMAP.md`, `RISK_REGISTER.md`, Phase 0/capture research remain historical or intended scope |

Common handbook presentation must select exact reviewed files and hashes from
this repository/candidate; it must not recursively import `.artifacts`, private
records or whole checkouts. Keep long historical records reachable without
dominating default search. Product source owns facts; the aggregate owns no
competing architecture or live task status.

## C4 generation and review

`docs/architecture/workspace.dsl` is the authoritative context/container model.
The common foundation route pins official Structurizr `2026.09.19` WAR SHA-256
`ac1a2806e246364b88261a542a36ca7f3006ef48e5de68830afa8672f58690f1`.
Use the shared `validate_model.py` with that WAR and a verified Java 21+ runtime:

The helper and adjacent lock are a declared external documentation dependency:
[Engineering-Recovery `handbook/validate_model.py` at `c168889`](https://github.com/Limited-Underground/Engineering-Recovery/blob/c1688896d27f6d12eb24971faad50b22c68182f6/handbook/validate_model.py)
and [its Structurizr lock](https://github.com/Limited-Underground/Engineering-Recovery/blob/c1688896d27f6d12eb24971faad50b22c68182f6/handbook/structurizr.lock.json).
That existing repository is private; retrieve it only with approved organization
access. The named helper snapshot is preserved in the common tooling candidate;
remote retrieval of that particular commit is not yet verified. Obtain an approved
copy from the common tooling owner or use the standalone official commands below.
It is not bundled in this public product clone. The common handbook owner
retains command logs and `receipt.json` in a fresh model-validation output directory
and returns the receipt plus generated `json/workspace.json` to the product owner.
The product lineage/status records link the actual result; no receipt means pending.
The November 2 common-owner receipt passed all three official commands below with
the author model unchanged. [Lineage](validation/FOUNDATION_LINEAGE_20261102.json)
records the hashes and scope; [workspace.json](architecture/workspace.json) is the
exact generated JSON. This does not demonstrate a product build or runtime restore.

```text
python validate_model.py --java JAVA_EXECUTABLE --jar PINNED_STRUCTURIZR_WAR --model docs/architecture/workspace.dsl --out NEW_VALIDATION_DIRECTORY
```

It preserves a model copy, validates with the official parser, exports JSON and a
local/static site, hashes outputs and verifies the author's input is unchanged.
The shared Studio JBR is Java 25.0.4, not an exact Java 21 installation. Do not
install another toolchain or alter global PATH/JAVA_HOME to duplicate this route.
Only report parser/export PASS when the receipt exists; syntax success does not
prove architecture accuracy. Use ISO/IEC/IEEE 42010:2022 as a review reference,
without a conformance claim. Independent review checks entrypoints, source/profile
fidelity, claims and privacy; authoring alone is not independent acceptance.

Without private helper access, the equivalent
[official Structurizr commands](https://docs.structurizr.com/commands) are available
with the same pinned WAR. Verify its SHA-256 first; use only fresh output folders:

```text
java -Xmx512m -Djava.awt.headless=true -jar PINNED_STRUCTURIZR_WAR validate -workspace docs/architecture/workspace.dsl
java -Xmx512m -Djava.awt.headless=true -jar PINNED_STRUCTURIZR_WAR export -workspace docs/architecture/workspace.dsl -format json -output NEW_JSON_DIRECTORY
java -Xmx512m -Djava.awt.headless=true -jar PINNED_STRUCTURIZR_WAR export -workspace docs/architecture/workspace.dsl -format static -output NEW_STATIC_DIRECTORY
```

Retain tool/runtime/model/output hashes, command logs and exit codes, and compare
the author's input before/after. This documents an alternative procedure, not an
additional execution in this foundation task.
