# Art, QA, And Next Steps

## Current QA Gate

- EditMode 테스트: `66/66 passed`
- 직접 플레이:
  - Hub: `LastRunSummary`, `HubResources` 존재 확인
  - Results: `ResultsBody`, `ResultsNextAction` 존재 확인
  - Expedition: `WestBoostPadDeckDecor`, `ObjectiveBeacon` 존재 확인
  - Expedition: `ExpeditionCameraDirector` component 존재 확인
- 콘솔:
  - Hub / Results / Expedition play/stop 재검증 기준 error `0`
- UV guardrail:
  - `critical=0`
  - `warnings=0`
- 자산 기믹 패스:
  - boost pad / lift path visible layer를 환경 프리팹으로 교체
  - elevated / side lane pickup에 route signal + reward weighting 적용
- telemetry / camera pass:
  - `RunSummary`에 route/boost/objective-ready telemetry 추가
  - camera occlusion fallback 및 objective-ready hazard grace 적용
- operations summary / route retune pass:
  - `RunSummary.GetOperationsSummary()`로 SeedPod/CleanWater 결과와 traversal telemetry를 하나로 통합
  - Hub/Results UI에서 `Rewards`와 `Operations`를 분리 표기
  - side/elevated route의 signal scale과 pickup amount를 상향 조정
- district trend pass:
  - `SaveData.runHistory`에 최근 run snapshot 최대 12개 저장
  - Results UI에 district별 평균 `SeedPod Delta / Bio Press Water / Boost Uses / Objective Ready` 비교 요약 추가

## Art And Asset State

- 플레이어 비주얼:
  - `Art/Characters/player_avatar` 로드 검증됨
  - `Leopard.fbx` readable 상태 정리 완료
- 비콘 비주얼:
  - `Art/Props/objective_beacon` 로드 검증됨
- Expedition 환경 기믹:
  - `road_wood`, `village_platform`, `rock_cluster`, `mud_patch`, `moss_patch`를 runtime decor로 사용
  - collision 지형과 visible decor를 분리해 traversal 신뢰성과 외형 자연스러움을 같이 유지
- 남은 과제:
  - 플레이어 외 핵심 프리팹/메시로 UV guardrail 적용 범위 확대
  - district별 핵심 환경 프리팹 로드 체크 확대

## Agreed Next Cycle

### 1. SeedPod Ratio Automation
- 목표:
  - `6:2 / 5:2 / 6:3` 비교
  - 15-run 기준 재고 20~30 밴드 유지
- 구현:
  - `SeedPodRefineryExperiment` 하네스 추가 완료
  - 후보별 자동 비교 리포트 생성 가능 상태
- 검증:
  - EditMode `50/50` 통과
  - 다음 단계는 실제 플레이 telemetry를 같은 리포트 포맷으로 수집하는 것

### 2. Boundary Tuning Pass
- 목표:
  - district별 soft-lock 0
- 구현:
  - `boundaryCenter`, `boundaryHalfExtents`, `safePosition`, `floorY` 실측값 조정
- 검증:
  - Unity MCP로 district별 진입
  - 경계 이탈 재현 및 복귀 확인

### 2.5. Expedition Layout Polish
- 목표:
  - 넓어진 공간 위에 실제 플레이 압축도를 더 높인다
- 구현:
  - `TraversalBoostPad`를 `solid surface + trigger child`로 분리해 승차 가능 상태로 정리
  - `BoostPadLiftPath`와 `ExitLanding`을 추가해 2층 진입을 collision 기반 상승 루프로 보강
  - `PlayerController.OnControllerColliderHit`로 gimmick collision activation 연결
  - `West/EastRamp` landing, `MainGround/ElevatedDeck/BeaconPlatform` rail 추가로 임의 낙하 루트 제한
  - 상층 진입 시 follow bias, objective/beacon over-shoulder cue 보강
  - boost path / landing / flank에 환경 프리팹 decor를 얹어 asset-based gimmick으로 전환
- 검증:
  - EditMode `59/59` 통과
  - Unity MCP play/stop 재검증 기준 console error `0`
  - 현재 PlayMode 회귀는 `TraversalBoostPadPlayModeTests.WestBoostPad_AppliesLaunchImpulseTowardUpperLanding` 기준으로 유지
  - scene/multiview capture: `expedition_traversal_iteration1_multiview.png`, `expedition_traversal_iteration2_sceneview.png`, `expedition_traversal_iteration2_westpad_sceneview.png`, `expedition_boostlift_sceneview.png`, `expedition_asset_gimmick_sceneview.png`

### 2.6. Survey-Driven Balance Pass
- 목표:
  - 경제/공간/난이도/카메라를 하나의 밸런스 루프로 다시 묶는다
- 구현:
  - `.survey/balance-evolution-expedition-redesign/` 결과를 기준으로 위험 루트 보상 신호 강화
  - `ExpeditionPickupRouteRules`로 elevated / side lane pickup 보상 가중치와 route signal 분기 추가
  - `RunSummary`에 route/boost/objective-ready telemetry 기록 추가
  - objective-ready 직후 hazard grace window 추가
  - 카메라 occlusion / concealment fallback 적용
- 검증:
  - EditMode `63/63` 통과
  - Unity MCP 기준 `ObjectiveBeacon`, `WestBoostPadDeckDecor`, `ExpeditionCameraDirector` 확인
  - play/stop 재검증 기준 console error `0`

### 2.7. Telemetry Follow-up
- 목표:
  - route telemetry를 실제 플레이 의사결정에 연결한다
- 구현:
  - `RunSummary.GetOperationsSummary()`로 SeedPod/CleanWater 결과와 route telemetry를 같은 summary에 노출
  - Hub/Results UI에서 Rewards / Operations 구조로 읽을 수 있게 유지
  - side/elevated route signal scale과 pickup reward를 상향 조정해 route 유도력을 보강
  - `SaveData.RecordRunSummary()`와 `GetDistrictOperationsComparisonSummary()`로 district trend 비교 기반을 추가
- 검증:
  - EditMode `66/66` 통과
  - Hub / Results / Expedition play/stop 기준 console error `0`
  - Results scene에서 `ResultsBody`, `ResultsNextAction` 존재 확인
  - route 선택률, 기믹 접촉률, beacon 도달 시간, Bio Press 사용률을 다음 실제 플레이 로그에서 다시 측정

### 3. UV Guardrail Expansion
- 목표:
  - 플레이어 외 핵심 프리팹까지 회귀 예방
- 구현:
  - 검사 대상 에셋 목록 확장
  - 필요 시 README/QA 캡처 증적 추가
- 검증:
  - guardrail 재실행
  - critical 발생 시 릴리즈 게이트 실패로 처리

### 4. Checklist Closure Pass
- 목표:
  - 현재 운영 게이트에서 전수 QA 단계로 이동
- 구현:
  - 씬 전환, 저장/복원, 결과 화면, 장기 세션 항목 순차 닫기
- 검증:
  - `docs/qa_verification_checklist.md` 체크 상태 갱신

## BMAD-GDS Execution Shape

- Ralph:
  - 구현이 아니라 검증 통과까지 계속 밀어붙이는 기준 역할
- BMAD-GDS:
  - design -> architecture -> production -> QA 순환
- ultrateam:
  - SeedPod / Boundary / UV 3레인 병렬 처리
- Unity MCP:
  - 테스트, 오브젝트 확인, 콘솔 점검, 스크린샷 증적 수집 자동화 축

## Update Rule

- 새 구현이 끝나면 먼저 이 문서의 `Agreed Next Cycle`와 `Current QA Gate`를 갱신한다.
- 이어서 상세 근거를 `docs/QA/`와 관련 design/art 문서에 동기화한다.
