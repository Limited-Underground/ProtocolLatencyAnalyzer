# Dependencies

The owner selected Apache-2.0 for the application. This is distinct from third-party terms. No capture, charting, web UI, MVVM toolkit, or test framework package is used in Phase 1.

The restored application lockfiles resolve exactly six SQLite-related package IDs:

| Package | Version | Declared terms |
|---|---|---|
| Microsoft.Data.Sqlite | 10.0.13 | MIT |
| Microsoft.Data.Sqlite.Core | 10.0.13 | MIT |
| SQLitePCLRaw.bundle_e_sqlite3 | 2.1.13 | Apache-2.0 |
| SQLitePCLRaw.core | 2.1.13 | Apache-2.0 |
| SQLitePCLRaw.provider.e_sqlite3 | 2.1.13 | Apache-2.0 |
| SQLitePCLRaw.lib.e_sqlite3 | 2.1.13 | Apache-2.0 package; native SQLite public-domain dedication |

Versions and integrity hashes are recorded in each `packages.lock.json`. Package nuspec license expressions were inspected in the isolated restore cache. Microsoft.Data.Sqlite 10.0.13 and the Windows runtime 10.0.13 metadata were verified against the official NuGet registration endpoint (published October 13, 2026). The project SDK is 10.0.303; the self-contained runtime is pinned separately to a serviced patch.

Build/restore cache: `.artifacts/packages`; CLI home: `.artifacts/dotnet-home`. NuGet.config clears inherited package sources and uses the official nuget.org source. No machine-wide package source or dependency installation was changed.

Full license texts and runtime third-party notices are distributed in `docs/licenses`, with an inventory in `THIRD-PARTY-NOTICES.txt`. A package audit and a host test do not establish release security, portability on every OS, or future servicing. New dependencies require review of actual pinned artifacts and transitives before adoption.

References: [SQLite provider native bundles](https://learn.microsoft.com/en-us/dotnet/standard/data/sqlite/custom-versions), [SQLite dedication](https://www.sqlite.org/copyright.html), [SQLitePCLRaw license](https://github.com/ericsink/SQLitePCL.raw/blob/master/LICENSE.TXT), [Microsoft provider source license](https://github.com/dotnet/efcore/blob/main/LICENSE.txt).
