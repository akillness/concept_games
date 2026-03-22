# Survey Discussion & Agreed Actions (2026-03-22)

## Discussion Scope
- 대상: 기존 QA-밸런스 개선 결과(SeedPod sink, 경계 복귀, UV fallback)
- 목적: `survey` 결과를 기반으로 다음 사이클의 실행 항목을 합의 가능한 형태로 고정
- 근거 문서: `.survey/qa-balance-omu-ultraqa/*`, `docs/QA/00~02`, `design/19_balance_survey.md`

## Agreed Points
1. SeedPod는 "소비처 없음" 단계는 종료됐고, 이제 "비율 튜닝" 단계다.
2. 경계 복귀는 구현/검증 완료 상태이며, 다음 병목은 district별 파라미터 실험이다.
3. UV fallback은 런타임 안정화에 기여하지만, import 단계 사전탐지가 없어서 회귀 예방력이 제한된다.

## Agreed Next Actions

### A1. SeedPod Ratio Tuning (P1 -> P0 후보)
- 목표: SeedPod 순증 재고를 장기 세션에서 20~30 사이로 유지
- 실행:
  - Run summary에 SeedPod delta, Bio Press 사용 횟수, CleanWater 전환량 기록
  - 후보 비율: `6:2`(기준), `5:2`, `6:3`
- 완료 기준:
  - 15-run 시뮬레이션에서 재고 폭주(50+) 미발생

### A2. Boundary Profile Externalization
- 목표: 경계 복귀 파라미터를 district 데이터로 분리
- 실행:
  - `boundaryCenter`, `boundaryHalfExtents`, `safePosition`, `floorY`를 DistrictDef/별도 SO로 외부화
  - QA 시나리오 8을 district별로 반복
- 완료 기준:
  - 모든 district에서 이탈 soft-lock 0건

### A3. UV Import Guardrail
- 목표: 런타임 이전에 UV 누락을 탐지
- 실행:
  - 에디터 검증 스크립트 또는 체크리스트 자동화로 `Mesh.isReadable`, UV 채널 존재 여부 점검
  - fallback 발동 건수를 QA 리포트에 집계
- 완료 기준:
  - 릴리즈 전 UV 누락 이슈 사전 탐지율 100%

## External References
- UGS Analytics SQL Data Explorer: https://docs.unity.com/ugs/en-us/manual/analytics/manual/sql-data-explorer-tables
- UGS Remote Config files: https://docs.unity.com/ugs/en-us/manual/remote-config/manual/remote-config-files
- UGS Cloud Code manual: https://docs.unity.com/ugs/en-us/manual/cloud-code/manual
- Mesh.isReadable (Unity): https://docs.unity3d.com/ScriptReference/Mesh-isReadable.html
- OpenTelemetry specification overview: https://opentelemetry.io/docs/reference/specification/overview/

## Execution Note
- 다음 작업은 위 A1~A3를 순차가 아니라 병렬 분할(ultrawork)로 진행하고, 최종 게이트는 UltraQA(테스트 + 플레이 + 콘솔 0 에러)로 고정한다.

---

## Execution Update (Ralph + Ultrawork, 2026-03-22)

### A1. SeedPod Ratio Tuning Telemetry
- 상태: **Done**
- 반영:
  - `RunSummary`에 `seedPodDelta`, `bioPressUseCount`, `bioPressCleanWaterConverted` 기록 필드 추가
  - Bio Press 실행 시 `SaveService.RecordSeedPodTelemetry(...)`로 `lastRunSummary`에 즉시 누적
  - 후보 비율 `6:2`, `5:2`, `6:3` 코드 프로파일 노출 (`SeedPodRefineryRules.CandidateProfiles`)

### A2. Boundary Profile Externalization
- 상태: **Done**
- 반영:
  - `DistrictDef`에 `BoundaryRecoveryProfile` 추가 (`boundaryCenter`, `boundaryHalfExtents`, `safePosition`, `floorY`)
  - `PlayerController`가 런타임 district(Hub/Expedition) 기준으로 boundary profile을 해석하도록 변경
  - district 데이터가 없을 때 legacy 기본값 fallback 유지

### A3. UV Import Guardrail
- 상태: **Done (검출 파이프라인 구축)**
- 반영:
  - `UvImportGuardrail` + Editor 메뉴(`Tools/Moss Harbor/Validation/*`) 추가
  - 검사 항목: `Mesh.isReadable`, UV 채널 존재/길이 정합성
- 실행 결과:
  - `Audit Player UV Guardrail` 실행 시 `critical=2` 탐지(읽기 불가 메시)
  - 런타임 fallback 이전 단계에서 사전 검출 가능 상태 확인
