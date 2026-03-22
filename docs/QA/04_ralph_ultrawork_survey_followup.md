# Ralph + Ultrawork Follow-up (Survey Comparison)

Date: 2026-03-22

## Scope
- 기준 문서:
  - `design/19_balance_survey.md`
  - `docs/QA/03_survey_discussion_actions.md`
- 실행 목표:
  - A1/A2/A3 구현 반영
  - UltraQA 게이트(테스트 + 직접 플레이 + 콘솔 에러 점검)

## Implementation Summary
- A1: SeedPod telemetry 확장
  - `RunSummary`에 `seedPodDelta`, `bioPressUseCount`, `bioPressCleanWaterConverted` 추가
  - Bio Press 실행 시 `SaveService.RecordSeedPodTelemetry(...)`로 요약값 즉시 누적
  - 비율 후보 `6:2`, `5:2`, `6:3` 코드 프로파일 제공
- A2: Boundary profile 외부화
  - `DistrictDef.BoundaryRecoveryProfile` 추가
  - `PlayerController`가 Hub/Expedition의 runtime district를 참조해 경계값 적용
- A3: UV import guardrail 추가
  - `UvImportGuardrail` + Editor 메뉴 추가
  - 검사: `Mesh.isReadable`, UV 채널 존재/길이

## Verification Evidence
- EditMode tests: **45/45 passed**
  - job id: `6084f8a468f141e898d4095bcb4cb754`
- 직접 플레이 검증:
  - Hub play -> stop, Expedition play -> stop 수행
  - 콘솔 에러 로그 조회 결과: **0건**
- UV guardrail 실행:
  - 메뉴: `Tools > Moss Harbor > Validation > Audit Player UV Guardrail`
  - 초기 결과: `UV Guardrail: FAIL | ... critical=2`
  - 조치: `Leopard.fbx` Read/Write Enabled 적용
  - 재실행 결과: `UV Guardrail: OK | ... critical=0, warnings=0`

## Survey Comparison (Before vs After)
1. SeedPod sink (design/19 P1-A)
- Before: sink 도입만 완료, 실측 데이터 없음
- After: run summary telemetry 도입으로 ratio 실험 기반 확보
- 남은 갭: 15-run 자동 수집/비교 리포트 미구현

2. Boundary recovery tuning (docs/QA A2)
- Before: PlayerController 내부 하드코딩 파라미터
- After: district 데이터 기반 프로필 외부화 완료
- 남은 갭: district별 실측 파라미터 최적화 미완료

3. UV regression prevention (docs/QA A3)
- Before: 런타임 fallback 중심
- After: 에디터 단계 guardrail 도입 + 읽기 불가 메시 수정까지 완료
- 남은 갭: 플레이어 외 핵심 프리팹으로 guardrail 적용 범위 확장 필요

## Proposed Next Improvements
1. SeedPod ratio 자동 평가 루프
- 6:2, 5:2, 6:3 각각 15-run 자동 시뮬레이션
- 출력: 평균 재고, 95p 재고, 50+ 폭주 발생률

2. Boundary profile per-district tuning pass
- QA 시나리오 8을 district 전수 반복
- 산출물: district별 center/extent/floor/safe-position 표준값

3. UV guardrail CI gate
- EditMode 스모크에서 guardrail 실행 + critical>0 시 실패 처리
- 릴리즈 직전 수동 체크리스트 의존도를 줄이고 회귀 방지 강화
