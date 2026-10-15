# Capture-engine assessment

No live capture, driver operation or packet-backend qualification has been performed. Phase 1 exposes unavailable states only.

| Approach | Capability basis | Privilege/install | Main qualifications |
|---|---|---|---|
| ETW | Native real-time/event-file consumption | Provider-specific privilege; no third-party driver distribution | Provider/schema must actually supply required bytes; loss/timestamp domains need validation |
| pktmon | In-box filtering, capture, counters and conversion | Privileged capture; OS build feature differences | CLI live display is not proof of a stable binary streaming API; default snap length can truncate payload |
| Existing Npcap | Established packet capture API | Existing authorized driver; admin-only installation settings may apply | Exact OS/driver/API rights, timestamp behavior, drops and overhead need validation |
| Temporary Npcap | Same packet API after install | Driver installation, elevation, possible restart/disruption | Rights, ownership-safe cleanup, crash recovery and residual state unresolved |
| Offline PCAP/PCAPNG | Supplied-file observations | Independent parser can avoid live driver | Separate approval, parser license/format coverage, hostile-file limits and timestamp metadata required |

Recommend native-first architecture, proving a suitable ETW packet provider/live path and evaluating pktmon for bounded diagnostics. Exact backend selection remains Needs Testing, particularly on Server 2019. Optional use of an existing appropriately licensed Npcap installation is the fallback proposal. Never alter or uninstall another application's driver/capture session.

Npcap is proprietary/source-available; open-source Analyzer licensing does not grant redistribution or automated-install rights. Temporary installation is deferred until rights and lifecycle are resolved. Any future workflow needs installation ownership receipts, signed/pinned artifacts, interruption recovery, explicit approval, cleanup verification, and honest residual/reboot reporting.

Record capture point, interface, timestamp domain, original/captured length, truncation, capture-buffer drops, event loss and parser/queue loss separately. Keep original ETL when conversion loses diagnostics. Tool presence is not a supported-capability result.

Sources used in Phase 0: [ETW](https://learn.microsoft.com/en-us/windows/win32/etw/about-event-tracing), [pktmon overview](https://learn.microsoft.com/en-us/windows-server/networking/technologies/pktmon/pktmon), [pktmon start/snap length](https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/pktmon-start), [PCAPNG conversion](https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/pktmon-etl2pcap), [Npcap license](https://github.com/nmap/npcap/blob/master/LICENSE), [Npcap OEM redistribution](https://npcap.com/oem/redist), [Npcap users guide](https://npcap.com/guide/npcap-users-guide.html).
