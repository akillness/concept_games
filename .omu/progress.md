# Moss Harbor Progress

## Active Stage
- 개발

## In Progress
- [x] Expedition ground 확장 및 레벨 루프 재설계
- [x] 고저차/램프/상승 경로 추가
- [x] 동적 기믹(타이밍/회피/활용) 추가
- [x] 카메라 이벤트 연출 추가
- [x] BoostPad 승차 가능 표면 + trigger 분리 및 착지면 보강
- [x] 외곽 rail / blocker로 임의 낙하 경로 제한
- [x] 자산 기반 환경 기믹으로 boost path / lift path visible layer 치환
- [x] 위험 루트 pickup 보상 신호 강화
- [x] Unity MCP verification + README 최신 GIF sync

## Done This Sprint
- [x] `ExpeditionLevelLayoutBuilder`로 넓은 ground, side lane, ramp, elevated deck, beacon bridge 구축
- [x] `TraversalBoostPad`, `SweepHazard` 추가
- [x] `ExpeditionCameraDirector` 추가 및 pickup / objective-ready / beacon-activate cue 연결
- [x] `TraversalBoostPad`를 `solid surface + pulse visual + trigger child` 구조로 재설계
- [x] `BoostPadLiftPath` / `ExitLanding` 추가로 2층 진입을 collision 기반으로 보강
- [x] `West/EastRamp` base/top landing, `MainGround/ElevatedDeck/BeaconPlatform` perimeter rail 추가
- [x] `PlayerController` collision-hit와 `TraversalBoostPadTriggerRelay`를 연결해 gimmick activation 신뢰성 보강
- [x] 상층 진입 시 follow bias와 over-shoulder objective/beacon cue 보강
- [x] `RuntimeArtDirector.CreateEnvironmentDecor` 추가로 runtime 환경 프리팹 decor 배치 지원
- [x] `ExpeditionPickupRouteRules` 추가로 elevated / side lane route 보상 가중치와 signal decor 적용
- [x] `Camera.main` 미탐지 fallback과 비콘 완료 지연 처리
- [x] EditMode 59/59 통과
- [x] Expedition play/stop 재검증 기준 console error 0
- [x] 자산 기믹 검증 스크린샷 확보 (`concept_game/Assets/Screenshots/expedition_asset_gimmick_sceneview.png`)
- [x] 최신 Expedition 리디자인 GIF와 poster를 `media/`에 추가하고 `README.md` 갱신
- [x] `.survey/balance-evolution-expedition-redesign/` 패키지 작성
- [x] 플레이 관점 테스트 전략 문서화 (`docs/QA/05_play_testing_strategy.md`)
- [x] 2회 반복 추정/검증 문서화 (`docs/QA/06_expedition_traversal_estimation.md`)

## Next Candidate Tasks
- [ ] SeedPod / CleanWater 실제 플레이 telemetry 수집
- [ ] Collect -> HoldOut 전환 스파이크 완화
- [ ] 카메라 occlusion / concealment 안정화
- [ ] 넓은 레벨에서의 기믹 압박 밀도 재조정

## Done Before This Sprint
- [x] SeedPod telemetry / ratio experiment harness
- [x] boundary recovery externalization
- [x] UV import guardrail + readable mesh fix
- [x] docs summary hub 구축
