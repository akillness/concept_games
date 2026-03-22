# Task Estimation: Expedition Traversal / Boundary / Camera

## Iteration 1

### Task
- BoostPad를 실제로 밟을 수 있는 표면 + trigger volume 구조로 재설계
- ramp/deck/bridge 접합부 보강
- main ground / side lane / elevated deck 외곽 rail / blocker 추가
- 관련 EditMode 테스트 추가
- Unity MCP smoke verification 수행

### Estimation
- **Story Points**: 13
- **T-Shirt Size**: XL
- **Estimated Time**: 1 week 상당의 상대 난이도

### Breakdown
- BoostPad surface/trigger 분리: 5 SP
- 외곽 rail / blocker / choke path 생성: 5 SP
- 접합부 landing 보강 + 테스트: 3 SP

### Risks
- CharacterController와 impulse가 경사/단차에서 상호작용할 때 예상치 못한 튕김 가능성
- rail 배치가 넓은 공간감을 다시 좁힐 위험

### Acceptance Criteria
- [x] pad 위 정지 가능
- [x] trigger 발동 후 landing 안정
- [x] 임의 외곽 추락 루트 감소
- [x] EditMode green
- [x] runtime object + console smoke 통과

### Result
- 구현:
  - `TraversalBoostPad`를 `solid surface + pulse visual + trigger child` 구조로 재설계
  - `West/EastRamp` base/top landing 추가
  - `MainGround`, `West/EastLane`, `ElevatedDeck`, `BeaconBridge`, `BeaconPlatform` rail 추가
- 검증:
  - EditMode `55/55 passed` (job: `7854bcd892fb4fd8ac463d57854f6dcb`)
  - Runtime objects:
    - `WestBoostPad`, `WestBoostPadTrigger`, `MainGroundWestRail`, `ObjectiveBeacon` 존재 확인
  - Evidence:
    - `concept_game/Assets/Screenshots/expedition_traversal_iteration1_multiview.png`
- 평가:
  - 13SP로 추정한 작업은 실제로도 빌더 구조 변경 + 테스트 보강 + Unity smoke까지 묶인 XL 성격이 맞았다.

## Iteration 2

### Task
- Iteration 1 검증 결과를 반영해 카메라/동선/압박 밀도 미세 조정
- elevated 상태 follow 안정화
- 위험 경로와 제한 경로의 읽힘 보정
- 직접 플레이 기준 최종 검증 및 문서 동기화

### Estimation
- **Story Points**: 8
- **T-Shirt Size**: L
- **Estimated Time**: 2-3일 상당의 상대 난이도

### Breakdown
- 카메라 follow / cue 안정화: 3 SP
- 경로 읽힘 / rail 미세 조정: 3 SP
- 최종 플레이 검증 / 문서 갱신: 2 SP

### Risks
- 1차 수정이 충분하면 2차 작업량이 줄고, 반대로 runtime 문제 발견 시 8SP를 초과할 수 있음

### Acceptance Criteria
- [x] gimmick 승차 중 카메라 안정
- [x] ramp -> deck -> beacon bridge 완주 체감 양호
- [x] 위험 루트와 안전 루트의 구분이 읽힘
- [x] 최종 QA 문서 동기화 완료

### Result
- 구현:
  - elevated 상태에서 `followOffset`에 높이/후방 bias 추가
  - `ObjectiveReady`, `BeaconActivate` cue를 over-shoulder 성격으로 재조정
- 검증:
  - EditMode `55/55 passed` (job: `a2482bb4dc4f407e938699cd668d3c5c`)
  - Runtime objects:
    - `EastBoostPadTrigger`, `BeaconPlatformNorthRail`, `ExpeditionCameraDirector` 존재 확인
  - Console:
    - clean replay 기준 error `0`
  - Evidence:
    - `concept_game/Assets/Screenshots/expedition_traversal_iteration2_sceneview.png`
    - `concept_game/Assets/Screenshots/expedition_traversal_iteration2_westpad_sceneview.png`
- 평가:
  - 8SP 추정 범위 안에서 카메라 보정 + 재검증 + 문서 동기화가 닫혔다.

## Next Estimation Seed

- 다음 권장 스토리:
  - 위험 루트 보상 신호 강화: `5 SP / M`
  - SeedPod / CleanWater 실제 플레이 telemetry 연결: `8 SP / L`
  - 카메라 occlusion / concealment fallback: `5 SP / M`
