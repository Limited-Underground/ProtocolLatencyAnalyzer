workspace "Protocol Latency Analyzer" "Frozen November 2, 2026: published foundation plus preserved unpublished TCP preview; no product or release acceptance." {
    model {
        operator = person "Operator" "Chooses local source and data explicitly; authorizes each target measurement."
        windows = softwareSystem "Windows APIs" "Local Ethernet inventory, filesystem and sockets. A platform dependency, not a new service."
        endpoint = softwareSystem "Authorized TCP endpoint" "Existing listener selected by the operator. Preview-only TCP IPv4; no Analyzer destination agent." {
            tags "TCP Preview"
        }
        analyzer = softwareSystem "Protocol Latency Analyzer" "Limited Underground portable Windows diagnostics. Published foundation at 3c8367f; TCP is a separately preserved uncommitted overlay on 5abeb7f." {
            desktop = container "Windows desktop application" "WPF UI, Core and Infrastructure assemblies share one process. Preview adds explicit Start/Cancel and transient TCP result; source/egress is qualified." ".NET 10 / WPF / win-x64"
            sessions = container "Session files" "Operator-selected schema-v1 .plasession files: metadata and bounded events; no TCP measurement records." "SQLite / Microsoft.Data.Sqlite 10.0.13" {
                tags "Database"
            }
            json = container "Configuration and metadata files" "Explicit JSON configuration save/load and metadata-only export. Neither measurement persistence nor a restorable session backup." "Local JSON files" {
                tags "File Store"
            }
        }
        operator -> analyzer "Uses explicit local controls and file dialogs"
        analyzer -> windows "Uses local inventory, storage and socket APIs"
        analyzer -> endpoint "Preview only: one explicitly requested connect; no payload or DNS" "TCP IPv4"
        operator -> desktop "Selects source, target, timeout, Start/Cancel; views qualified result"
        desktop -> windows "Read-only inventory; preview binds explicit source; no network reconfiguration" ".NET APIs"
        desktop -> endpoint "Unpublished preview only: one connect, then close; no application payload" "TCP IPv4"
        desktop -> sessions "Validates schema/identity; transactional save and read-only open" "Local SQLite"
        desktop -> json "Explicit configuration save/load and session metadata export" "Local filesystem / JSON"
    }
    views {
        systemContext analyzer "SystemContext" "Context: endpoint relationship exists only in the preserved TCP overlay, not published main. No target test or physical-egress claim." {
            include operator
            include windows
            include endpoint
            include analyzer
            autoLayout lr
        }
        container analyzer "Containers" "One desktop runtime with two logical local file stores. Core/Infrastructure are internal assemblies, not independent containers." {
            include operator
            include windows
            include endpoint
            include desktop
            include sessions
            include json
            autoLayout lr
        }
        styles {
            element "Person" {
                shape Person
                background #23405a
                color #ffffff
            }
            element "Software System" {
                background #486579
                color #ffffff
            }
            element "Container" {
                background #416f8a
                color #ffffff
            }
            element "Database" {
                shape Cylinder
            }
            element "File Store" {
                shape Folder
            }
            element "TCP Preview" {
                background #7c5b2e
                color #ffffff
            }
        }
    }
}
