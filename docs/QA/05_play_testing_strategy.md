# Play Testing Strategy (Expedition Traversal + Boundary + Camera)

## Goal
- Expedition 레벨에서 `기믹을 밟고 올라가는 플레이`, `외곽 낙하 제한`, `카메라 연출 안정성`을 플레이 관점으로 검증한다.
- 단순 코드 성공이 아니라 실제 조작, 경로 선택, 충돌 체감, 시야 안정성을 통과 기준으로 삼는다.

## Test Pyramid For This Feature

### Unit Tests: 70%
- 대상:
  - 레벨 빌더가 생성하는 경사, 플랫폼, 레일, 차단 구조의 좌표/크기 규칙
  - 외곽 접근 제한 규칙
  - 카메라 cue 트리거 조건
  - impulse / recovery 보정 규칙
- 목적:
  - 빠르게 구조 회귀를 잡는다.
- 실행:
  - EditMode, 매 수정 직후

### Integration Tests: 20%
- 대상:
  - Expedition scene runtime generation
  - PlayerController + CharacterController + gimmick collider 상호작용
  - ObjectiveBeacon / pickup / camera director 연결
- 목적:
  - 단일 클래스는 통과했지만 실제 조합에서 깨지는 경우를 잡는다.
- 실행:
  - Unity MCP play mode smoke + object existence + console verification

### E2E / Play Tests: 10%
- 대상:
  - 실제 플레이 시나리오
  - route 선택, gimmick 승차, 외곽 추락 시도, beacon 접근, cue 체감
- 목적:
  - 사람이 느끼는 제어감과 공정성을 확인한다.
- 실행:
  - Unity MCP + 수동 플레이 기준의 체크리스트형 검증

## Coverage Goals
- Unit:
  - 레벨 구조 규칙 테스트 추가
  - 충돌 표면/trigger 분리 테스트 추가
- Integration:
  - Expedition runtime object verification 100%
  - console error / exception 0
- E2E:
  - 아래 P0 시나리오 전부 통과

## Core Acceptance Criteria

### 1. Gimmick Stepping
- BoostPad는 `밟을 수 있는 표면`과 `발동 trigger`가 분리되어야 한다.
- 플레이어가 pad 위에 정지 가능해야 한다.
- pad 발동 시 상향/전진 이동이 되더라도 platform clipping이 없어야 한다.
- ramp -> deck -> bridge 이동 중 CharacterController가 튕기거나 미끄러지지 않아야 한다.

### 2. Boundary Restriction
- 메인 ground 외곽은 임의 추락보다 `제한된 경로`와 `의도된 위험 구간` 중심으로 구성되어야 한다.
- 외곽 접근은 rail, blocker, choke path로 유도되어야 한다.
- 의도하지 않은 추락 루트는 0 또는 매우 낮아야 한다.
- 추락이 가능한 구간은 위험-보상 또는 명확한 경로 의미가 있어야 한다.

### 3. Camera Stability
- pickup cue, objective-ready cue, beacon-activate cue 중 어느 것도 이동 제어를 심하게 방해하지 않아야 한다.
- gimmick 승차 중 카메라가 지면/레일/기둥에 과하게 가려지면 실패다.
- camera cue 종료 후 follow 상태로 자연스럽게 복귀해야 한다.

## Play Test Matrix

| Layer | What to verify | Pass condition | Tool |
|------|----------------|----------------|------|
| Unit | 레벨 생성 규칙 | 구조 좌표/크기 예상값 일치 | EditMode |
| Unit | pad/rail 분리 | 표면 collider와 trigger가 동시에 존재 | EditMode |
| Integration | runtime layout 생성 | 필수 오브젝트 모두 존재 | Unity MCP |
| Integration | console health | error / exception 0 | Unity MCP |
| Integration | camera binding | `ExpeditionCameraDirector` 바인딩 | Unity MCP |
| E2E | gimmick 승차 | pad 위 정지 -> 발동 -> landing 성공 | 직접 플레이 |
| E2E | 상승 루트 | main -> ramp -> deck -> beacon bridge 완주 | 직접 플레이 |
| E2E | 외곽 제한 | 임의 외곽 탈출 시도가 rail/blocker로 제한 | 직접 플레이 |
| E2E | 위험 구간 | 허용된 위험 경로만 추락/복귀 가능 | 직접 플레이 |
| E2E | camera cue 체감 | 연출이 보상은 주되 통제감은 유지 | 직접 플레이 |

## Priority Scenarios

### P0-1. BoostPad Surface Validation
- Given:
  - 플레이어가 WestBoostPad 또는 EastBoostPad에 접근한다.
- When:
  - 천천히 걸어서 pad 위에 올라선 뒤 정지한다.
  - 다시 전진 입력으로 pad를 밟아 발동시킨다.
- Then:
  - pad 위 정지가 가능하다.
  - trigger 발동 시 상승/전진 impulse가 적용된다.
  - 발동 직후 지면 관통, 떨림, 즉시 추락이 없다.

### P0-2. Ramp To Deck Traversal
- Given:
  - 플레이어가 main ground 시작점에 있다.
- When:
  - west/east ramp를 타고 elevated deck으로 오른다.
  - beacon bridge까지 이어서 이동한다.
- Then:
  - 경사 진입/상단 접합부에서 막힘이 없다.
  - 불필요한 점프 요구가 없다.
  - camera가 경사면에서 과도하게 흔들리지 않는다.

### P0-3. Outer Boundary Restriction
- Given:
  - 플레이어가 side lane, elevated deck, beacon platform 가장자리로 이동한다.
- When:
  - 외곽으로 이탈하려고 시도한다.
- Then:
  - rail / blocker / choke path가 의도치 않은 추락을 제한한다.
  - 추락 가능 구간은 명시된 위험 경로에서만 발생한다.
  - 제한 구조 때문에 메인 동선이 답답해지지 않는다.

### P0-4. Camera Cue On Traversal
- Given:
  - pickup 직후, objective ready 직후, beacon activate 직전/직후 상황.
- When:
  - 플레이어가 gimmick 승차 또는 경사 이동 중 cue를 본다.
- Then:
  - cue가 너무 길지 않다.
  - 플레이어 위치 상실이 없다.
  - 종료 후 follow view가 즉시 회복된다.

### P1-1. Hazard Pressure After Widening
- Given:
  - 넓어진 ground에서 CentralSweeper, BridgeSweeper가 활성화되어 있다.
- When:
  - 플레이어가 안전 루트와 위험 루트를 각각 사용한다.
- Then:
  - 넓은 공간 때문에 압박이 완전히 사라지지 않는다.
  - 위험 루트는 시간 또는 위치 이점이 있다.

### P1-2. Recovery Fairness
- Given:
  - 의도된 위험 경로 또는 낙하 가능한 구간.
- When:
  - 플레이어가 낙하 후 boundary recovery를 겪는다.
- Then:
  - recovery 지점이 공정하다.
  - 연속 recovery loop가 없다.
  - 카메라/애니메이션이 비정상 상태에 빠지지 않는다.

## Execution Plan

### Phase A. Fast Feedback
1. EditMode tests 추가
2. 구조/충돌 규칙 회귀 확인
3. compile + diagnostics clean 확인

### Phase B. Runtime Smoke
1. Expedition scene load
2. 필수 object 존재 확인
3. play/stop 반복
4. console error / exception 확인

### Phase C. Directed Play
1. BoostPad 승차 루프
2. ramp -> deck -> bridge 완주
3. 외곽 이탈 시도
4. pickup / objective-ready / beacon cue 체감 확인
5. screenshot / gif 증적 수집

### Phase D. Balance Follow-up
1. route 선택률 기록
2. 추락 시도 횟수 / 실제 추락 횟수 기록
3. gimmick 사용률 기록
4. camera cue 방해 여부 기록

## Pass / Fail Rules
- Pass:
  - EditMode green
  - console error / exception 0
  - gimmick 승차 가능
  - 의도치 않은 외곽 추락 루트 제거
  - 카메라 cue가 traversal을 방해하지 않음
- Fail:
  - pad가 여전히 trigger-only여서 밟을 수 없음
  - ramp/deck 접합부에서 stuck 발생
  - 임의 외곽 추락 경로 다수 존재
  - cue 중 시야 상실 또는 조작 상실 체감이 큼

## Notes
- 이 문서는 구현 전 테스트 기준선이다.
- 실제 구현이 끝나면 `docs/qa_verification_checklist.md`에 체크 항목을 동기화하고, `docs/QA/01_ultraqa_execution_report.md`에 검증 증적을 추가한다.
