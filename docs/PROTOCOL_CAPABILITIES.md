# Protocol capability assessment

All protocol functions below are planned assessment scope, not Phase 1 implementation. Initial subset and specialized module order remain unapproved. Source-side use must not require an Analyzer destination agent, although useful responses require appropriate existing services.

| Protocol | Active candidate | Passive candidate / limitations | Cooperation | Proposed phase |
|---|---|---|---|---|
| ICMP | Echo RTT/nonresponse | Matched echo intervals/errors | Echo responder | 2/3 |
| TCP | Socket-connect duration | Observed handshake, reset, retransmission indicators | Listening service | 2/3 |
| UDP | Defined payload/response interval | Datagram metadata; semantic matching | Defined responder for RTT | 4 |
| DNS | Query-to-matched-response, fallback | Matched transactions; TCP framing | Resolver | 2/3 |
| HTTP/HTTPS | Headers/body completion, status; observable stages | Clear HTTP transactions; encrypted transport metadata | HTTP endpoint | 2/3 |
| TLS | Own handshake and certificate result | Available handshake metadata; encryption limits | TLS endpoint | 2, separate module later |
| SIP | OPTIONS/authorized transactions | Transaction/dialog matching, observable SDP | SIP service | 4 |
| RTP/RTCP | Defined stream/receiver reports | Clock-aware jitter, sequence observations, RTCP | Receiver for synthetic tests | 4 |
| HL7 MLLP | Framed message-to-correlated ACK | Reassembled framing and ACK matching | ACK-producing endpoint | 4 |
| HL7 FHIR | HTTP operations and response validation | HTTP visibility limits | Authorized FHIR endpoint | 4 |
| XML/SOAP | Request/response/fault matching | Observable framed envelopes | Compatible service | 4 |
| MQTT | Connection/QoS acknowledgments; defined request/reply | Control packets if visible | Broker; responder for request/reply | 4 |
| WebSocket | Upgrade, matched ping/pong or application reply | Clear frames; wss opacity | Responding endpoint | 4 |
| Custom TCP/UDP | Encoding/framing/payload/matching | Profile-defined semantics | Defined responder for turnaround | 4 |

Never combine unrelated measures into unexplained total latency. Probe nonresponse differs from proven network loss. Socket connect duration differs from captured SYN-to-SYN/ACK. HTTP reused connection stages are not zero-duration new handshakes. UDP send success does not establish round-trip time. HL7 ACK turnaround includes application processing; match MSH-10/MSA-2 within connection/session and distinguish ACK stages. RTP capture sequence gaps are not proven receiver loss. MQTT acknowledgment is not downstream business completion.

TCP needs bounded stream reassembly; packet boundaries are not application boundaries. Fragmentation, gaps, overlaps, truncation, drops and eviction must qualify results. Passive TLS does not reveal arbitrary application payloads; no interception/decryption is planned initially.

References: [TCP](https://www.rfc-editor.org/info/rfc9293/), [RTP/RTCP](https://www.rfc-editor.org/info/rfc3550/), [TLS 1.3](https://www.rfc-editor.org/info/rfc8446/), [SIP](https://www.rfc-editor.org/info/rfc3261/), [MQTT 5](https://docs.oasis-open.org/mqtt/mqtt/v5.0/os/mqtt-v5.0-os.html), [WebSocket](https://www.rfc-editor.org/info/rfc6455/).
