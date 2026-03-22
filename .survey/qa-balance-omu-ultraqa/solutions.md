# Solution Landscape: QA-Balance Orchestration

## Solution List
| Name | Approach | Strengths | Weaknesses | Notes |
|------|----------|-----------|------------|-------|
| UGS Analytics + SQL Data Explorer | 이벤트 수집 + SQL 질의 기반 밸런스 분석 | Unity 프로젝트와 결합이 자연스럽고 테이블 기준 분석 가능 | 이벤트 설계가 선행되지 않으면 분석 가치 낮음 | https://docs.unity.com/ugs/en-us/manual/analytics/manual/sql-data-explorer-tables |
| UGS Remote Config + Game Overrides | 원격 파라미터/조건 분기로 밸런스 실험 | 빌드 재배포 없이 파라미터 조정 가능 | 운영 규칙이 문서화되지 않으면 실험 드리프트 발생 | https://docs.unity.com/ugs/en-us/manual/remote-config/manual/remote-config-files |
| UGS Cloud Code | 밸런스 계산/검증 로직을 서버 쪽으로 이동 | 클라이언트 로직 분산 완화, 정책 일관성 강화 | 서버 함수와 클라 상태의 버전 정합성 관리 필요 | https://docs.unity.com/ugs/en-us/manual/cloud-code/manual |
| PlayFab Telemetry/Insights | 이벤트 수집 및 라이브옵스 분석 스택 | LiveOps 중심 운영 사례가 많고 서비스 기능이 폭넓음 | Unity 기본 스택과 별도 운영 체계가 필요 | https://learn.microsoft.com/en-us/gaming/playfab/ |
| GameAnalytics SDK | 게임 특화 이벤트 대시보드 중심 분석 | 게임 KPI 템플릿 접근이 빠름 | 커스텀 룰/실험은 별도 운영 정책 필요 | https://docs.gameanalytics.com/event-tracking-and-integrations/sdks-and-collection-api/open-source-sdks/cpp/event-tracking |
| OpenTelemetry + Collector | 벤더 중립 관측 표준으로 이벤트/로그/메트릭 수집 | 도구 교체 비용을 줄이고 표준화 가능 | 초기 스키마 설계 및 파이프라인 구축 비용이 큼 | https://opentelemetry.io/docs/reference/specification/overview/ |

## Categories
- Engine-native stack: UGS Analytics, Remote Config, Cloud Code
- Game LiveOps stack: PlayFab, GameAnalytics
- Vendor-neutral observability: OpenTelemetry

## What People Actually Use
- 초기 단계 팀은 Unity 내장/UGS 기능으로 빠르게 시작하고, 운영 단계에서 SQL/대시보드 파이프라인을 증설하는 패턴이 많다.
- 실무에서는 "완전 자동 밸런싱"보다 "원격 파라미터 조정 + QA 회귀 체크리스트"의 혼합 운영이 일반적이다.
- UV/경계/충돌 같은 런타임 품질 이슈는 분석 이벤트보다 늦게 포착되기 쉬워, QA 시나리오에 별도 항목으로 유지하는 사례가 많다.

## Frequency Ranking
(공식 문서/실무 언급량 기반 정성 순위)
1. Engine-native stack (UGS Analytics + Remote Config + Cloud Code)
2. Game LiveOps stack (PlayFab / GameAnalytics)
3. Vendor-neutral observability (OpenTelemetry)

## Curated Sources
- UGS Analytics SQL Data Explorer: https://docs.unity.com/ugs/en-us/manual/analytics/manual/sql-data-explorer-tables
- UGS Remote Config files: https://docs.unity.com/ugs/en-us/manual/remote-config/manual/remote-config-files
- UGS Cloud Code manual: https://docs.unity.com/ugs/en-us/manual/cloud-code/manual
- PlayFab docs portal: https://learn.microsoft.com/en-us/gaming/playfab/
- GameAnalytics Event Tracking: https://docs.gameanalytics.com/event-tracking-and-integrations/sdks-and-collection-api/open-source-sdks/cpp/event-tracking
- OpenTelemetry specification overview: https://opentelemetry.io/docs/reference/specification/overview/
- Mesh read/write runtime 제약: https://docs.unity3d.com/ScriptReference/Mesh-isReadable.html

## Key Gaps
- SeedPod sink(6:2)의 적정 비율을 판단할 실측 이벤트 스키마가 아직 없다.
- 경계 복귀 파라미터가 코드 내부값이라 district별 튜닝 실험이 어렵다.
- UV fallback은 반영됐지만, import 단계 자동 점검(사전 차단) 체계가 없다.

## Contradictions
- "테스트 통과 = 품질 확보"라고 보지만, 실제로는 플레이 체감/운영 데이터가 없으면 밸런스 튜닝 반복 비용이 크게 남는다.
- "원격 파라미터면 빠른 운영 가능"이라고 보지만, 파라미터 변경 이력/검증 규칙이 없으면 회귀 속도도 같이 빨라진다.

## Key Insight
이 프로젝트의 다음 개선 축은 새 기능 추가보다 "운영 가능한 관측 구조"를 먼저 고정하는 것이다. 즉, SeedPod/경계/UV를 각각 독립 이슈로 다루기보다, 이벤트 스키마(무엇을 측정) + 원격 파라미터(무엇을 조정) + QA 게이트(무엇을 통과)로 통합하면 다음 밸런스 패치의 실패 비용을 크게 줄일 수 있다.

## Applied vs Next Improvement Comparison

| Area | Applied in this cycle | Next candidate | Expected effect |
|---|---|---|---|
| Entry economy | Dock/Glass/Sunken/Lighthouse entry cost up | Dynamic entry scaling by fail streak | 초중반 반복 시 비용 체감 완화 + 후반 긴장 유지 |
| HoldOut pressure | Glass 60s / Lighthouse 75s | HoldOut success bonus by remaining time tier | 숙련자 보상 강화, 단순 시간 버티기 단조로움 완화 |
| Failure economy | BD failure formula tightened (collection-only retention) | Resource-specific retention curve (per district/difficulty) | 난이도별 실패 페널티 정합성 향상 |
| Upgrade pacing | Pump/Scanner/Resonator costs retuned + SeedPod sink(Bio Press) 연결 | SeedPod sink ratio tuning (6:2 -> telemetry-based) | dead stock 해소 유지 + 장기 경제 밸런스 안정화 |
| Boundary safety | Player boundary recovery (safe-zone teleport + cooldown) | District-specific boundary profiles (data-driven) | 이탈 soft-lock 제거 + 지구별 체감 이동 자유도 제어 |
| Character UV robustness | Runtime UV mesh fallback (`LeopardMesh_RuntimeUv`) + non-readable guard | Import pipeline auto-check + UV anomaly telemetry | UV 누락으로 인한 시각 회귀 조기 차단 |
| QA execution | UltraQA + Play capture + docs sync | Telemetry-backed balancing dashboard | 체감 기반 튜닝을 수치 기반으로 전환 |

## Next Recommendation
1. SeedPod 변환(6:2)을 고정값으로 두지 말고, district별 획득량 대비 목표 재고 상한(예: 20~30) 기준으로 조정한다.
2. 경계 복귀 파라미터를 district ScriptableObject로 분리해 지구별 이동 자유도와 실패 위험을 독립적으로 튜닝한다.
3. UV fallback 발동/비발동을 이벤트로 남겨 아트 임포트 파이프라인 문제를 QA 이전에 탐지한다.
