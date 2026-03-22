# Next Improvement Backlog (Post-UltraQA)

## Completed In This Cycle (2026-03-22)
- A1 SeedPod telemetry: `RunSummary`에 delta/use/conversion 기록 필드 및 Bio Press 누적 반영 완료
- A2 Boundary externalization: district별 `BoundaryRecoveryProfile` 데이터 경유 복구 로직 반영 완료
- A3 UV guardrail: editor 사전검출 스크립트/메뉴 추가 완료

## Priority 1
- SeedPod ratio 실험 자동화(6:2 / 5:2 / 6:3)
  - 15-run 시뮬레이션 수집 스크립트 추가
  - 목표: SeedPod 재고 20~30 밴드 유지

- UV guardrail 범위 확장
  - 현재 플레이어 핵심 에셋은 `critical=0` 확인
  - 지구별 핵심 프리팹/메시까지 검사 대상을 확장 필요

## Priority 2
- 경계 회수 district 튜닝
  - 현재는 외부화 완료, 값은 기본값 중심
  - district별 center/extent/floor/safe-position 실측 튜닝 필요

- 별점 달성률 텔레메트리 수집
  - district별 1/2/3성 분포
  - 난이도별 이탈률

## Priority 3
- 실패 보존율 UX 피드백 보강
  - 실패 결과 화면에서 보존률 계산 근거 표시
