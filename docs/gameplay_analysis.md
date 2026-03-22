# Gameplay Analysis — Player Perspective Technical Report

> 플레이어 관점에서 현재 프로토타입의 플레이 경험을 기술적으로 분석한 문서.
> 코드 분석(`PlayerController`, `ExpeditionDirector`, `DistrictBalanceDefaults`) 기반.

---

## 1. 현재 프로토타입 상태

### 1.1 플레이 가능 루프

```
Boot → Hub → Expedition_Runtime → Results → Hub (반복)
```

완전히 구현된 엔드투엔드 루프. `SceneFlowService` 가 `LoadBoot / LoadHub / LoadExpedition / LoadResults` 전환을 관리하며 `SceneFadeController` 로 페이드 효과를 적용한다.

### 1.2 씬 구성

| 씬 | Build Index | 역할 |
|----|------------|------|
| Boot | 0 | 서비스 초기화, 저장 데이터 로드, 페이드인 |
| Hub | 1 | 지구 선택, 업그레이드 구매, 난이도 설정 |
| Expedition_Runtime | 2 | 픽업 수집, 목표 달성, 타이머 |
| Results | 3 | 별점 평가, 보상 표시, 다음 행동 안내 |
| SampleScene | 4 | 레거시 (사용 안 함) |

---

## 2. 플레이어 이동 시스템 분석

### 2.1 PlayerController 파라미터

| 파라미터 | 값 | 설명 |
|----------|-----|------|
| `moveSpeed` | `5f` | 기본 이동 속도 (m/s) |
| `sprintMultiplier` | `1.6f` | 스프린트 배율 → 최대 8 m/s |
| `rotationSharpness` | `12f` | 회전 보간 속도 (Slerp 인자) |
| `animationDampTime` | `0.08f` | 애니메이션 블렌드 감쇠 시간 |
| `acceleration` | `18f` | 가속률 (m/s²) |
| `deceleration` | `22f` | 감속률 (m/s²) — 가속보다 빠름 |
| `gravity` | `-15f` | 중력 (m/s²) |

### 2.2 CharacterController 설정

| 항목 | 값 |
|------|-----|
| `height` | `2.0f` |
| `radius` | `0.4f` |
| `center` | `(0, 1, 0)` |

### 2.3 입력 및 이동 로직

- 입력: `Input.GetAxisRaw("Horizontal/Vertical")` → WASD / 화살표
- 스프린트: `LeftShift` 또는 `RightShift`
- 속도 보간: `Vector3.MoveTowards(_currentVelocity, targetVelocity, accelRate * dt)`
- 중력: 지면 접지 시 `_verticalSpeed = -2f` 고정, 공중에서 `-15 * dt` 누적
- 회전: 이동 방향으로 `Quaternion.Slerp`, `rotationSharpness = 12f`

### 2.4 애니메이션 파라미터

| 파라미터 | 타입 | 설명 |
|----------|------|------|
| `Speed` | float | 0 (정지) / 0.55 (걷기) / 1.0 (스프린트) |
| `Sprint` | bool | 스프린트 중이고 속도 > 0.1 |
| `Grounded` | bool | CharacterController 접지 여부 |
| `VerticalSpeed` | float | 수직 속도 (낙하 판정) |

애니메이션 속도는 지수 감쇠 보간: `1 - Exp(-dt / dampTime)`

---

## 3. 지구별 플레이 구조

`DistrictBalanceDefaults.cs` 실제 값 기준.

### 3.1 Dock (튜토리얼)

| 항목 | 값 |
|------|-----|
| 잠금 조건 | 0성 (기본 개방) |
| 입장 비용 | BloomDust 5 |
| 타이머 | 210초 |
| 목표 | CollectPickups × 3 |
| BloomDust 픽업 | 3개 × 12 = 36 |
| Scrap 픽업 | 1개 × 5 = 5 |
| 완료 보너스 | BloomDust +25, Scrap +6 |
| 픽업 반경 | 7m |
| 비콘 위치 | `(0, 0.75, 10)` |
| 2성 기준 | 픽업 80% 이상 수집 |
| 3성 기준 | 타이머 65% 이상 잔여 |

### 3.2 Reed Fields

| 항목 | 값 |
|------|-----|
| 잠금 조건 | 1성 |
| 입장 비용 | BloomDust 10 |
| 타이머 | 195초 |
| 목표 | CollectResource SeedPod × 5 |
| 픽업 구성 | BloomDust 2개, Scrap 2개, SeedPod 2개 |
| 완료 보너스 | BloomDust +30, Scrap +8 |
| 픽업 반경 | 9m |

### 3.3 Tidal Vault

| 항목 | 값 |
|------|-----|
| 잠금 조건 | 2성 |
| 입장 비용 | BloomDust 15 |
| 타이머 | 180초 |
| 목표 | CollectResource CleanWater × 3 |
| 픽업 구성 | BloomDust 2개, Scrap 2개, SeedPod 1개 |
| 완료 보너스 | BloomDust +35, Scrap +10 |
| 픽업 반경 | 10m |

### 3.4 Glass Narrows

| 항목 | 값 |
|------|-----|
| 잠금 조건 | 3성 |
| 입장 비용 | BloomDust 20 |
| 타이머 | 180초 |
| 목표 | HoldOut 75초 |
| 픽업 구성 | BloomDust 2개, Scrap 2개, SeedPod 1개 |
| 완료 보너스 | BloomDust +40, Scrap +12 |
| 픽업 반경 | 11m |

### 3.5 Sunken Arcade

| 항목 | 값 |
|------|-----|
| 잠금 조건 | 4성 |
| 입장 비용 | BloomDust 25 |
| 타이머 | 165초 |
| 목표 | CollectPickups × 5 |
| 픽업 구성 | BloomDust 3개, Scrap 3개, SeedPod 1개 |
| 완료 보너스 | BloomDust +50, Scrap +15 |
| 픽업 반경 | 8m |

### 3.6 Lighthouse Crown

| 항목 | 값 |
|------|-----|
| 잠금 조건 | 5성 |
| 입장 비용 | BloomDust 30 |
| 타이머 | 150초 |
| 목표 | HoldOut 90초 |
| 픽업 구성 | BloomDust 3개, Scrap 3개, SeedPod 2개 |
| 완료 보너스 | BloomDust +60, Scrap +20 |
| 픽업 반경 | 13m |

---

## 4. 시스템별 구현 완료율

| 시스템 | 구현율 | 핵심 미구현 항목 |
|--------|--------|----------------|
| 플레이어 이동 (WASD, 스프린트, 중력) | 100% | — |
| 플레이어 비주얼 (prefab 부착, 애니메이션) | 100% | — |
| 자원 수집 (픽업 트리거, bob/proximity) | 100% | — |
| 원정 목표 (CollectPickups/Resource/HoldOut) | 100% | — |
| 타이머 및 난이도 배율 | 100% | — |
| 별점 평가 및 보상 계산 | 100% | — |
| 저장/로드 (자원, 업그레이드, 별점) | 100% | — |
| 허브 업그레이드 (3종 구매) | 100% | — |
| 씬 전환 + 페이드 | 100% | — |
| 허브 시각적 복원 | 50% | 구역별 복원 조건부 랜턴/소품만 구현, 복원 연출 미구현 |
| NPC/퀘스트 | 20% | 대화, 스케줄, 퀘스트 진행 없음 |
| 정화 시스템 | 0% | Corruption Tile/Node, 정화 게이지 없음 |
| 오염 확산 | 0% | ThreatDirector, 재오염 타이머 없음 |
| 맵 경계 | 0% | 무한 평면 |
| 사운드 | 0% | 수집, 비콘, UI, 배경음 없음 |
| 정화기/보조행동/Overdrive | 0% | PlayerController에 미구현 |

---

## 5. 플레이어 경험 분석

### 5.1 첫 플레이 경험 (Dock, Normal)

1. Hub 진입 → 자원 0, 튜토리얼 1단계 활성화
2. BloomDust 5 소비하여 Expedition 시작 (Debug Grant 또는 튜토리얼 지원)
3. Expedition_Runtime: 210초 타이머, BloomDust/Scrap 픽업 4개가 7m 반경 원형 배치
4. 픽업 3개 수집 → ObjectiveBeacon 활성화 → 비콘 진입 → 성공
5. Results: 1~3성, BloomDust/Scrap 보상 표시
6. Hub 복귀 → 자원 증가, Harbor Pump 등 업그레이드 가능

예상 소요 시간: 첫 플레이 ~3분 (Dock Normal)

### 5.2 강점

- 코어 루프가 명확하고 단일 플레이가 3분 내외
- 6개 지구 × 3 난이도 = 18가지 조합으로 재도전 동기 제공
- 지구별 색상(districtColor) + 안개(fogColor) 차이로 시각적 구분 명확
- 저장 시스템이 완전하여 진행 상태 보존
- 난이도 3단계 × 별점 기준 차등으로 숙련도 표현

### 5.3 약점

- 정화/오염 시스템 미구현 → 탐색과 수집만 존재, 위협 없음
- 맵 경계 없음 → 픽업 방향을 잃으면 무한 평면 이탈 가능
- SeedPod 소비처 없음 → 수집하나 활용처 부재
- 사운드 없음 → 수집/비콘 활성화/UI 피드백 전무
- 비주얼이 prefab 기반이나 Primitive 형태 가능성 → 몰입감 제한

---

## 6. 권장 다음 스프린트

우선순위 순:

1. 맵 경계 벽 추가 (무한 평면 이탈 방지)
2. 기본 사운드 이펙트 (수집음, 비콘 활성화음, UI 클릭음)
3. 정화 시스템 프로토타입 (Corruption Tile 2개, CorruptionGrid 서비스)
4. 허브 시각적 복원 연출 (구역 복원 시 파티클/색상 전환)
5. SeedPod 소비처 정의 (업그레이드 또는 허브 장식)
