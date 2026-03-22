# Summary Index

이 폴더는 다음 개선 사이클을 위한 최신 요약 기준선이다. 상세 기획/이력은 `design/`, QA 증적은 `docs/QA/`, 아트 작업 이력은 `art/`에 남기고, 여기에는 현재 구현값과 다음 실행 계획만 압축한다.

## Current Snapshot

- 빌드 기준: Unity `2022.3.32f1`
- 현재 플레이 루프: `Boot -> Hub -> Expedition_Runtime -> Results -> Hub`
- 현재 운영 게이트:
  - EditMode `45/45` 통과
  - Hub/Expedition 직접 플레이 재검증 완료
  - 콘솔 error `0`
  - UV guardrail `critical=0`, `warnings=0`
- 최근 완료:
  - SeedPod telemetry 확장
  - district별 `BoundaryRecoveryProfile` 외부화
  - UV import guardrail 도입 및 플레이어 메시 readable 정리

## Read Order

1. [01_gameplay_and_systems.md](/Users/jangyoung/.superset/projects/concept_games/docs/summary/01_gameplay_and_systems.md)
2. [02_runtime_and_technical_baseline.md](/Users/jangyoung/.superset/projects/concept_games/docs/summary/02_runtime_and_technical_baseline.md)
3. [03_art_qa_and_next_steps.md](/Users/jangyoung/.superset/projects/concept_games/docs/summary/03_art_qa_and_next_steps.md)

## Source Of Truth Map

- 제품/플레이 진입: [README.md](/Users/jangyoung/.superset/projects/concept_games/README.md)
- 상세 설계 인덱스: [design/00_index.md](/Users/jangyoung/.superset/projects/concept_games/design/00_index.md)
- QA 운영 게이트: [qa_verification_checklist.md](/Users/jangyoung/.superset/projects/concept_games/docs/qa_verification_checklist.md)
- QA 실행 로그: [01_ultraqa_execution_report.md](/Users/jangyoung/.superset/projects/concept_games/docs/QA/01_ultraqa_execution_report.md)
- 현재 백로그: [02_next_improvement_backlog.md](/Users/jangyoung/.superset/projects/concept_games/docs/QA/02_next_improvement_backlog.md)

## Open Risks

- SeedPod는 sink가 생겼지만 비율 자동 평가 루프가 아직 없다.
- 경계 복귀는 외부화 완료 상태지만 district별 실측 튜닝은 남아 있다.
- UV guardrail은 플레이어 핵심 에셋만 닫혔고, 환경/프리팹 범위 확장은 다음 사이클 과제다.
- 전체 체크리스트 전수 완료는 아직 아니고 현재는 운영 게이트만 통과했다.

## Working Rule

- 다음 BMAD-GDS 작업은 이 폴더를 먼저 읽고, 세부 근거가 필요할 때만 원문 문서를 참조한다.
- 새 구현이나 검증이 끝나면 먼저 이 폴더를 갱신한 뒤 원문 문서를 동기화한다.
