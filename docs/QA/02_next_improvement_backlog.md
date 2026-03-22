# Next Improvement Backlog (Post-UltraQA)

## Priority 1
- SeedPod sink 튜닝/확장
  - 현 상태: Bio Press(6 SeedPod -> Water +2) 1차 반영 완료
  - 후속 A: district별 획득량 대비 최적 변환비율(telemetry 기반) 보정
  - 후속 B: 허브 복원/버프 모듈 소비처 추가

- 맵 경계 회수 파라미터 외부화
  - 현 상태: safe-zone 텔레포트 + cooldown 1차 반영 완료
  - 후속 A: district ScriptableObject로 boundary center/extent/safe position 분리
  - 후속 B: soft boundary 경고 UI + hard recovery 혼합 정책

## Priority 2
- 별점 달성률 텔레메트리 수집
  - district별 1/2/3성 분포
  - 난이도별 이탈률

- 실패 보존율 UX 피드백 보강
  - 실패 결과 화면에서 보존률 계산 근거 표시

## Priority 3
- UV/재질 진단 자동화
  - 플레이어 프리팹 렌더링 시 material texture null 자동 로그
  - 스모크 테스트에서 UV fallback 발동 여부 수집
