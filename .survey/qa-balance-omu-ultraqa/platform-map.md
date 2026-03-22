# Platform Map: QA-Balance Orchestration

## Settings
| Concern | Claude | Codex | Gemini | Common Layer |
|---------|--------|-------|--------|--------------|
| Worker model | team launch args | `OMX_TEAM_WORKER_LAUNCH_ARGS` | workflow model flags | `settings.model` |
| Reasoning effort | model profile | `model_reasoning_effort` | model config | `settings.reasoning` |
| Verification target | browser/unity | unity MCP + tests | browser/unity | `settings.verify_target` |
| Telemetry sink | external docs/manual dashboard | state files + docs | workflow logs | `settings.telemetry_sink` |

## Rules
| Concern | Claude / OMC | Codex / OMX | Gemini / OHMG | Common Layer |
|---------|---------------|-------------|---------------|--------------|
| PLAN gate required | yes | yes | yes | `rules.plan_required` |
| completion gate | task terminal only | pending/in_progress=0 before shutdown | same | `rules.shutdown_guard` |
| QA cycle max | manual policy | UltraQA max 5 | workflow policy | `rules.qa_cycle_limit` |
| source-backed survey | optional | required in this repo process | optional | `rules.survey_with_links` |

## Hooks
| Lifecycle | Claude | Codex | Gemini | Common Layer |
|-----------|--------|-------|--------|--------------|
| pre-plan | prompt hooks | state bootstrap | workflow init | `hooks.pre_plan` |
| post-team | mailbox nudge | leader mailbox + status | worker inbox | `hooks.post_execute` |
| post-verify | cleanup hook | worktree cleanup | workspace cleanup | `hooks.post_verify` |
| post-survey | manual summary paste | `.survey/*` 문서 갱신 + QA 문서 연결 | workflow docs update | `hooks.post_survey` |

## Platform Gaps
- 비대화형 PLAN gate에서 승인 전달 방식이 플랫폼별로 다름
- 팀 워커 준비 타임아웃/lock contention 대응 로직이 공통 추상화로 묶이지 않음
- Unity 실행 컨트롤(MCP) 타임아웃/재시도 처리 규칙이 플랫폼별 표준으로 통일되지 않음

## Source Links
- UGS Analytics SQL Data Explorer: https://docs.unity.com/ugs/en-us/manual/analytics/manual/sql-data-explorer-tables
- UGS Remote Config: https://docs.unity.com/ugs/en-us/manual/remote-config/manual/remote-config-files
- UGS Cloud Code: https://docs.unity.com/ugs/en-us/manual/cloud-code/manual
- OpenTelemetry specification overview: https://opentelemetry.io/docs/reference/specification/overview/
