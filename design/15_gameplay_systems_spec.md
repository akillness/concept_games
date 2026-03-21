# 15. Gameplay Systems Specification

> 통합 게임플레이 시스템 레퍼런스 — 코어 루프, 시스템 아키텍처, 밸런스 규칙, 구현 매핑을 통합한 단일 참조 문서.

---

## 1. Scene Flow

```
Boot → Hub → Expedition_Runtime → Results → Hub (loop)
```

| Scene | State | C# Entry Point | Key Systems |
|-------|-------|----------------|-------------|
| Boot | `GameFlowState.Boot` | `GameBootstrap.Awake()` | Save init, `SceneFadeController` auto-create |
| Hub | `GameFlowState.Hub` | `HubManager.Start()` | District select, upgrades, restoration visuals, tutorial |
| Expedition_Runtime | `GameFlowState.Expedition` | `ExpeditionDirector.Start()` | Timer, pickups, objectives, district environment, rewards |
| Results | `GameFlowState.Results` | `ResultsManager` + `ResultsHudController` | Star rating, reward display, next action prompt |

### 씬 전환 흐름

- **SceneFadeController** (싱글톤, `DontDestroyOnLoad`): 모든 씬 전환에 페이드 인/아웃 적용 (기본 0.35초)
- **SceneFlowService.LoadWithFade()**: `SceneFadeController.Instance` 존재 시 페이드 경유, 없으면 직접 로드
- **Boot 시작**: `SetOpaque()` → UI 로드 → `FadeIn(0.5f)` → `LoadHub()` (페이드 아웃 0.35초 → Hub 로드 → 페이드 인 0.35초)
- **레이스 컨디션 방지**: `_isTransitioning` 가드 + `LoadSceneAsync` 사용 (동기 로드 금지)

---

## 2. Core Loop (Implementation Mapping)

플레이어의 반복 행동과 이를 구현하는 C# 클래스/메서드 매핑.

| 디자인 단계 | 설명 | 구현 클래스 / 메서드 |
|-------------|------|----------------------|
| 허브에서 목표 확인 | 현재 열린 디스트릭트, 별점, 업그레이드 상태 표시 | `HubManager.Start()`, `HubHudController` |
| 장비/업그레이드 선택 | Harbor Pump, Route Scanner, Pearl Resonator 구매 | `HubManager`, `HubUpgradeDef`, `UpgradeIds` |
| 디스트릭트 진입 | 입장료 지불 후 Expedition_Runtime 씬 로드 | `SceneFlowService.LoadExpedition()`, `SceneFadeController.FadeOut()` |
| 원정 플레이 | 타이머, 픽업 수집, 목표 달성 | `ExpeditionDirector`, `SimplePickup`, `ResourceType` |
| 보상 계산 | 성공/실패 분기, 업그레이드 보너스 | `RewardCalculator.CalculateSuccess()` / `.CalculateFailure()` |
| Results 표시 | 별점, 자원 보상, 다음 행동 안내 | `ResultsManager`, `ResultsHudController`, `StarRatingCalculator` |
| Hub 귀환 | 업그레이드, 허브 복원 상태 반영 | `SceneFlowService.LoadHub()`, `RuntimeArtDirector` |
| 새 지구/기능 해금 | 누적 별점에 따른 디스트릭트 잠금 해제 | `SaveData.starsByDistrict`, `DistrictBalanceDefaults.requiredStars` |

### 세션 구조

| 단계 | 예상 시간 | 핵심 시스템 |
|------|-----------|-------------|
| 허브 준비 | 2~4분 | `HubHudController`, `HubManager` |
| 원정 | 2.5~3.5분 (타이머 기준) | `ExpeditionDirector`, `RunSummary` |
| 귀환 정산 | 2~5분 | `ResultsHudController`, `SaveData` |
| **총 1세션** | **평균 18~25분** | — |

---

## 3. Resource Economy

| Resource | 한국어 명칭 | Source | Sink | C# Type |
|----------|-------------|--------|------|---------|
| BloomDust | 기본 통화 | 픽업 수집, 완료 보너스, Route Scanner 보너스 | 디스트릭트 입장료 | `ResourceType.BloomDust` |
| Scrap | 구조물 복원 재료 | 픽업 수집, 완료 보너스 | Harbor Pump 업그레이드 관련 복원 | `ResourceType.Scrap` |
| SeedPod | 정원/식생 복원 재료 | 픽업 수집 (디스트릭트별) | 향후 사용 예정 | `ResourceType.SeedPod` |
| CleanWater | 장비 충전 재료 | Harbor Pump 업그레이드 완료 보너스 | Pearl Resonator 관련 | `ResourceType.CleanWater` |
| MemoryPearl | 희귀 메타 재료 | Pearl Resonator (D3+ 디스트릭트) | 향후 사용 예정 | `ResourceType.MemoryPearl` |

### 실패 시 자원 보존율

| 자원 | 보존 규칙 | 구현 |
|------|-----------|------|
| BloomDust | 수집량 + `completionBonus × 50%` | `RewardCalculator.CalculateFailure()` |
| Scrap | 수집량 × 70% | `RewardCalculator.CalculateFailure()` |
| CleanWater | 0 (업그레이드 보너스 미적용) | `RewardCalculator.CalculateFailure()` |
| MemoryPearl | 0 (업그레이드 보너스 미적용) | `RewardCalculator.CalculateFailure()` |
| SeedPod | 수집량 × 70% | `ExpeditionDirector.FailCurrentRun()` |

---

## 4. Difficulty System

난이도는 디스트릭트별 기본값 위에 오프셋을 적용하는 방식으로 설계되었다. 현재 구현은 Normal 기준값으로 고정되어 있으며, Easy/Hard 오프셋은 향후 설정 메뉴 추가 시 적용 예정이다.

| Parameter | Easy | Normal (현재 구현) | Hard |
|-----------|------|-------------------|------|
| Timer Multiplier | 1.3× | 1.0× | 0.8× |
| Pickup Target Offset | -1 | 0 | +1 |
| Entry Cost Multiplier | 0.7× | 1.0× | 1.3× |
| 2-Star Pickup Ratio | -0.10 | 0 | +0.05 |
| 3-Star Time Ratio | +0.10 | 0 | -0.05 |
| Fail Resource Retention | 85% | 70% | 50% |

### 난이도 축 (디자인 원칙)

| 축 | 쉬움 상태 | 어려움 상태 |
|----|-----------|-------------|
| 오염 밀도 | 노드 2개 | 노드 3개 + 재오염 증가 |
| 지형 장애 | 거의 없음 | 이동 우회 필요 |
| 환경 위험 | 약함 | 동시 처리 필요 |
| 요구 자원 | 단일 자원 중심 | 자원 조합 요구 |

**밸런스 목표**: "편안하지만 무의미하지는 않다" — 액션 숙련보다 루트 판단과 도구 선택이 중요하다.

---

## 5. District Progression & Balance

`DistrictBalanceDefaults.ApplyIfDefault()` 가 런타임에 적용하는 기본값 기준.

| District | Required Stars | Entry Cost (BloomDust) | Timer | Objective Type | Bloom Pickups | Scrap Pickups | SeedPod Pickups | Completion Bonus (Bloom/Scrap) | 2-Star Ratio | 3-Star Time Ratio |
|----------|---------------|------------------------|-------|----------------|---------------|---------------|-----------------|-------------------------------|-------------|-------------------|
| **dock** | 0 | 5 | 210s | CollectPickups(3) | 3×12 | 1×5 | — | 25 / 6 | 0.80 | 0.65 |
| **reed_fields** | 1 | 10 | 195s | CollectResource(SeedPod, 5) | 2×10 | 2×5 | 2×3 | 30 / 8 | 0.75 | 0.60 |
| **tidal_vault** | 2 | 15 | 180s | CollectResource(CleanWater, 3) | 2×10 | 2×6 | 1×2 | 35 / 10 | 0.75 | 0.55 |
| **glass_narrows** | 3 | 20 | 180s | HoldOut(75s) | 2×12 | 2×7 | 1×3 | 40 / 12 | 0.70 | 0.50 |
| **sunken_arcade** | 4 | 25 | 165s | CollectPickups(5) | 3×14 | 3×8 | 1×3 | 50 / 15 | 0.70 | 0.50 |
| **lighthouse_crown** | 5 | 30 | 150s | HoldOut(90s) | 3×15 | 3×10 | 2×4 | 60 / 20 | 0.65 | 0.45 |

### 환경 테마

| District | 컨셉 | Fog Color | Fog Density | Sun Intensity |
|----------|------|-----------|-------------|---------------|
| dock | 청록 안개 부두 | (0.55, 0.72, 0.78) | 0.022 | 1.35 |
| reed_fields | 초록 습지 | (0.38, 0.52, 0.36) | 0.026 | 1.25 |
| tidal_vault | 깊은 파란 금속 | (0.22, 0.36, 0.52) | 0.018 | 1.40 |
| glass_narrows | 수정 얼음빛 | (0.52, 0.62, 0.70) | 0.015 | 1.60 |
| sunken_arcade | 따뜻한 호박색 | (0.32, 0.24, 0.18) | 0.024 | 1.30 |
| lighthouse_crown | 밤 인디고 | (0.14, 0.16, 0.28) | 0.028 | 1.15 |

환경 파라미터는 `RuntimeArtDirector.ApplyExpeditionAtmosphere(DistrictDef)` 가 씬 로드 시 `RenderSettings` 및 Directional Light에 적용한다.

---

## 6. Star Rating System

`StarRatingCalculator.Calculate(DistrictDef, RunSummary): int` 가 1~3의 정수를 반환한다.

### 계산 공식

```
totalPickups   = bloomPickupCount + scrapPickupCount + seedPodPickupCount
pickupRatio    = pickupsCollected / totalPickups   (totalPickups == 0이면 1.0)
timeRatio      = durationSeconds / runTimerSeconds  (runTimerSeconds == 0이면 1.0)

1성: completed == true
2성: 1성 AND pickupRatio >= district.twoStarPickupRatio
3성: 2성 AND timeRatio  <= district.threeStarTimeRatio
```

### 등급 기준 요약

| 등급 | 조건 | 보상 |
|------|------|------|
| ★☆☆ (1성) | 메인 목표 완료 | 기본 완료 보너스 |
| ★★☆ (2성) | 1성 + 픽업 수집률 ≥ `twoStarPickupRatio` (65~80%) | 추가 BloomDust |
| ★★★ (3성) | 2성 + 소요시간/총시간 ≤ `threeStarTimeRatio` (45~65%) | 장식 설계도, 희귀 씨앗, 추가 대사 이벤트 |

### 구현 참조

- `StarRatingCalculator.cs` (`MossHarbor.Data`)
- `ResultsHudController` 에서 `Calculate()` 결과를 별 아이콘으로 시각화
- `ResultsManager` 에서 `SaveData.starsByDistrict[districtId]` 갱신 후 허브 잠금 해제 판정

---

## 7. User Story Scenarios (Difficulty Test Matrix)

> 6개 지구 × 3 난이도 = 18개 시나리오. 각 시나리오는 예상 플레이 경험을 구체화한 유저 스토리 형태.

### 7.1 Dock (안개 부두)

**Easy**
> "초보 플레이어로서, Dock에 4 BloomDust를 내고 진입하면, 273초 동안 2개 픽업을 수집하고 비콘을 활성화한다. 실패해도 자원의 85%를 보존한다."
- 타이머: 210 × 1.3 = 273초
- 픽업 목표: 3 - 1 = 2개
- 입장 비용: round(5 × 0.7) = 4 BloomDust
- 2성 기준: pickupRatio ≥ 0.70 (기본 0.80 - 0.10)
- 3성 기준: timeRatio ≤ 0.75 (기본 0.65 + 0.10)

**Normal**
> "일반 플레이어로서, Dock에 5 BloomDust를 내고 진입하면, 210초 동안 3개 픽업을 수집하고 비콘을 활성화한다."
- 타이머: 210초
- 픽업 목표: 3개
- 입장 비용: 5 BloomDust
- 2성 기준: pickupRatio ≥ 0.80
- 3성 기준: timeRatio ≤ 0.65

**Hard**
> "숙련 플레이어로서, Dock에 7 BloomDust를 내고 진입하면, 168초 동안 4개 픽업을 수집한다. 실패 시 자원의 50%만 보존된다."
- 타이머: 210 × 0.8 = 168초
- 픽업 목표: 3 + 1 = 4개
- 입장 비용: round(5 × 1.3) = 7 BloomDust
- 2성 기준: pickupRatio ≥ 0.85 (기본 0.80 + 0.05)
- 3성 기준: timeRatio ≤ 0.60 (기본 0.65 - 0.05)

### 7.2 Reed Fields (갈대 습지)

**Easy**
> "초보 플레이어로서, Reed Fields에 7 BloomDust를 내고 진입하면, 254초 동안 SeedPod 4개를 수집한다. 실패해도 자원의 85%를 보존한다."
- 타이머: 195 × 1.3 = 253.5 → 254초
- 픽업 목표: 5 - 1 = 4개 (SeedPod 기준)
- 입장 비용: round(10 × 0.7) = 7 BloomDust
- 2성 기준: pickupRatio ≥ 0.65 (기본 0.75 - 0.10)
- 3성 기준: timeRatio ≤ 0.70 (기본 0.60 + 0.10)

**Normal**
> "일반 플레이어로서, Reed Fields에 10 BloomDust를 내고 진입하면, 195초 동안 SeedPod 5개를 수집한다. 총 6개의 픽업(2B+2S+2SP)이 배치되어 있다."
- 타이머: 195초
- 픽업 목표: 5개 (SeedPod)
- 입장 비용: 10 BloomDust
- 2성 기준: pickupRatio ≥ 0.75 (총 픽업 6개 중 4.5개 이상)
- 3성 기준: timeRatio ≤ 0.60 (195s × 0.60 = 117초 이내)

**Hard**
> "숙련 플레이어로서, Reed Fields에 13 BloomDust를 내고 진입하면, 156초 동안 SeedPod 6개를 수집한다. 실패 시 자원의 50%만 보존된다."
- 타이머: 195 × 0.8 = 156초
- 픽업 목표: 5 + 1 = 6개
- 입장 비용: round(10 × 1.3) = 13 BloomDust
- 2성 기준: pickupRatio ≥ 0.80 (기본 0.75 + 0.05)
- 3성 기준: timeRatio ≤ 0.55 (기본 0.60 - 0.05)

### 7.3 Tidal Vault (조수 금고)

**Easy**
> "초보 플레이어로서, Tidal Vault에 11 BloomDust를 내고 진입하면, 234초 동안 CleanWater 2개를 수집한다. 실패해도 자원의 85%를 보존한다."
- 타이머: 180 × 1.3 = 234초
- 픽업 목표: 3 - 1 = 2개 (CleanWater 기준)
- 입장 비용: round(15 × 0.7) = 11 BloomDust
- 2성 기준: pickupRatio ≥ 0.65 (기본 0.75 - 0.10)
- 3성 기준: timeRatio ≤ 0.65 (기본 0.55 + 0.10)

**Normal**
> "일반 플레이어로서, Tidal Vault에 15 BloomDust를 내고 진입하면, 180초 동안 CleanWater 3개를 수집한다. 총 5개의 픽업(2B+2S+1SP)이 배치되어 있다."
- 타이머: 180초
- 픽업 목표: 3개 (CleanWater)
- 입장 비용: 15 BloomDust
- 2성 기준: pickupRatio ≥ 0.75 (총 픽업 5개 중 3.75개 이상)
- 3성 기준: timeRatio ≤ 0.55 (180s × 0.55 = 99초 이내)

**Hard**
> "숙련 플레이어로서, Tidal Vault에 20 BloomDust를 내고 진입하면, 144초 동안 CleanWater 4개를 수집한다. 실패 시 자원의 50%만 보존된다."
- 타이머: 180 × 0.8 = 144초
- 픽업 목표: 3 + 1 = 4개
- 입장 비용: round(15 × 1.3) = 20 BloomDust
- 2성 기준: pickupRatio ≥ 0.80 (기본 0.75 + 0.05)
- 3성 기준: timeRatio ≤ 0.50 (기본 0.55 - 0.05)

### 7.4 Glass Narrows (유리 해협)

**Easy**
> "초보 플레이어로서, Glass Narrows에 14 BloomDust를 내고 진입하면, 234초 동안 75초를 버티며 픽업을 수집한다. 실패해도 자원의 85%를 보존한다."
- 타이머: 180 × 1.3 = 234초
- 목표: HoldOut 75초 (난이도 무관 동일)
- 입장 비용: round(20 × 0.7) = 14 BloomDust
- 2성 기준: pickupRatio ≥ 0.60 (기본 0.70 - 0.10)
- 3성 기준: timeRatio ≤ 0.60 (기본 0.50 + 0.10)

**Normal**
> "일반 플레이어로서, Glass Narrows에 20 BloomDust를 내고 진입하면, 180초 동안 75초를 버티며 픽업을 수집한다. 총 5개의 픽업(2B+2S+1SP)이 배치되어 있다."
- 타이머: 180초
- 목표: HoldOut 75초
- 입장 비용: 20 BloomDust
- 2성 기준: pickupRatio ≥ 0.70 (총 픽업 5개 중 3.5개 이상)
- 3성 기준: timeRatio ≤ 0.50 (180s × 0.50 = 90초 이내)

**Hard**
> "숙련 플레이어로서, Glass Narrows에 26 BloomDust를 내고 진입하면, 144초 동안 75초를 버티며 픽업을 수집한다. 실패 시 자원의 50%만 보존된다."
- 타이머: 180 × 0.8 = 144초
- 목표: HoldOut 75초
- 입장 비용: round(20 × 1.3) = 26 BloomDust
- 2성 기준: pickupRatio ≥ 0.75 (기본 0.70 + 0.05)
- 3성 기준: timeRatio ≤ 0.45 (기본 0.50 - 0.05)

### 7.5 Sunken Arcade (침수 아케이드)

**Easy**
> "초보 플레이어로서, Sunken Arcade에 18 BloomDust를 내고 진입하면, 215초 동안 픽업 4개를 수집한다. 실패해도 자원의 85%를 보존한다."
- 타이머: 165 × 1.3 = 214.5 → 215초
- 픽업 목표: 5 - 1 = 4개
- 입장 비용: round(25 × 0.7) = 18 BloomDust
- 2성 기준: pickupRatio ≥ 0.60 (기본 0.70 - 0.10)
- 3성 기준: timeRatio ≤ 0.60 (기본 0.50 + 0.10)

**Normal**
> "일반 플레이어로서, Sunken Arcade에 25 BloomDust를 내고 진입하면, 165초 동안 픽업 5개를 수집한다. 총 7개의 픽업(3B+3S+1SP)이 배치되어 있다."
- 타이머: 165초
- 픽업 목표: 5개
- 입장 비용: 25 BloomDust
- 2성 기준: pickupRatio ≥ 0.70 (총 픽업 7개 중 4.9개 이상)
- 3성 기준: timeRatio ≤ 0.50 (165s × 0.50 = 82.5초 이내)

**Hard**
> "숙련 플레이어로서, Sunken Arcade에 33 BloomDust를 내고 진입하면, 132초 동안 픽업 6개를 수집한다. 실패 시 자원의 50%만 보존된다."
- 타이머: 165 × 0.8 = 132초
- 픽업 목표: 5 + 1 = 6개
- 입장 비용: round(25 × 1.3) = 33 BloomDust
- 2성 기준: pickupRatio ≥ 0.75 (기본 0.70 + 0.05)
- 3성 기준: timeRatio ≤ 0.45 (기본 0.50 - 0.05)

### 7.6 Lighthouse Crown (등대 왕관)

**Easy**
> "초보 플레이어로서, Lighthouse Crown에 21 BloomDust를 내고 진입하면, 195초 동안 90초를 버티며 픽업을 수집한다. 실패해도 자원의 85%를 보존한다."
- 타이머: 150 × 1.3 = 195초
- 목표: HoldOut 90초 (난이도 무관 동일)
- 입장 비용: round(30 × 0.7) = 21 BloomDust
- 2성 기준: pickupRatio ≥ 0.55 (기본 0.65 - 0.10)
- 3성 기준: timeRatio ≤ 0.55 (기본 0.45 + 0.10)

**Normal**
> "일반 플레이어로서, Lighthouse Crown에 30 BloomDust를 내고 진입하면, 150초 동안 90초를 버티며 픽업을 수집한다. 총 8개의 픽업(3B+3S+2SP)이 배치되어 있다."
- 타이머: 150초
- 목표: HoldOut 90초
- 입장 비용: 30 BloomDust
- 2성 기준: pickupRatio ≥ 0.65 (총 픽업 8개 중 5.2개 이상)
- 3성 기준: timeRatio ≤ 0.45 (150s × 0.45 = 67.5초 이내)

**Hard**
> "숙련 플레이어로서, Lighthouse Crown에 39 BloomDust를 내고 진입하면, 120초 동안 90초를 버티며 픽업을 수집한다. 실패 시 자원의 50%만 보존된다."
- 타이머: 150 × 0.8 = 120초
- 목표: HoldOut 90초
- 입장 비용: round(30 × 1.3) = 39 BloomDust
- 2성 기준: pickupRatio ≥ 0.70 (기본 0.65 + 0.05)
- 3성 기준: timeRatio ≤ 0.40 (기본 0.45 - 0.05)

### 7.7 세션 시간 예측 매트릭스

| 지구 | Easy | Normal | Hard |
|------|------|--------|------|
| Dock | ~4분 | ~3.5분 | ~2.8분 |
| Reed Fields | ~5분 | ~3.3분 | ~2.6분 |
| Tidal Vault | ~5분 | ~3분 | ~2.4분 |
| Glass Narrows | ~5분 | ~3분 | ~2.4분 |
| Sunken Arcade | ~4.5분 | ~2.8분 | ~2.2분 |
| Lighthouse Crown | ~4분 | ~2.5분 | ~2분 |

### 7.8 난이도 진행 추천

- **첫 플레이**: Easy로 Dock ~ Tidal Vault, Normal로 Glass Narrows 이후
- **2회차**: Normal 전체 → Hard Dock/Reed Fields
- **도전**: Hard 전 지구 3성 클리어

---

## 8. Balance Rules

디자인 문서 `07_rules_balance_failstates.md` 기준.

### 기본 규칙

1. 원정은 메인 목표 완료 후 언제든 탈출 가능
2. 플레이어 HP가 0이 되면 강제 귀환 (실패 처리)
3. 강제 귀환 시 Scrap 수집량 70% 보존, BloomDust는 수집량 + 완료 보너스의 50%
4. CleanWater / MemoryPearl은 원정 실패 시 미적용 (0 반환)
5. 별점은 `시간`, `픽업 수집률` 두 축으로 산정

### 밸런스 철학

- "편안하지만 무의미하지는 않다" — 너무 어렵게 죽이는 대신, 정화 효율이 떨어지게 만든다
- 액션 숙련보다 루트 판단과 도구 선택이 중요하다
- 반복 피로 방지: 지구마다 목표 타입이 다르고, 시각적 테마가 확연히 다르다

### 적/위험 설계 원칙

- 적은 다수의 체력 덩어리로 만들지 않는다
- 정화 흐름을 방해하는 유닛 위주로 설계한다
- 플레이어를 즉사시키는 패턴은 넣지 않는다
- 카메라 탑다운 시점에서 읽기 어려운 투사체는 사용하지 않는다
- 넉백은 약하게, 제어 상실 시간은 0.25초 이하

### 실패 상태

| 발생 조건 | 결과 |
|-----------|------|
| HP 0 | 강제 귀환, Scrap 70% / BloomDust 수집+보너스50% 보존 |
| 핵심 오브젝트 파괴 | 강제 귀환, 동일 보존율 |
| 타이머 만료 | 강제 귀환, 동일 보존율 |

### 시스템 실패 금지 항목

- 장시간 파밍 후 완전한 무보상
- 저장되지 않는 허브 변경
- 지구를 다시 처음부터 해야만 하는 구조

### 기본 수치 (Phase 8 검증 완료)

| 항목 | 수치 |
|------|------|
| 플레이어 기본 HP | 100 |
| 기본 이동속도 | 4.5 |
| 기본 정화력 | 10/초 |
| 보조 스킬 쿨다운 | 8초 |
| 궁극기 충전 시간 | 평균 90초 |
| 씬 페이드 시간 | 0.35초 인/아웃 |

---

## 9. Upgrade System

Hub에서 구매 가능한 3개의 업그레이드. ID 상수는 `UpgradeIds.cs` (`MossHarbor.Data`)에서 중앙 관리.

### Harbor Pump (`UpgradeIds.HarborPump = "harbor_pump"`)

- **효과**: 원정 성공 시 CleanWater 보너스 지급
- **보상 연계**: `RewardCalculator.CalculateSuccess()` → `cleanWaterBonus = harborPumpUpgrade.cleanWaterBonus`
- **설계 의도**: CleanWater 경제(Tidal Vault 이후)로의 진입 가속

### Route Scanner (`UpgradeIds.RouteScanner = "route_scanner"`)

- **효과 1**: 원정 타이머 연장 (`routeScannerUpgrade.timerBonusSeconds`)
- **효과 2**: BloomDust 수집량 배율 보너스 (`routeScannerUpgrade.bloomMultiplier`)
- **보상 연계**: `RewardCalculator.CalculateSuccess()` → `routeScannerBloomBonus`, 타이머 보너스는 `duration` 계산에 반영
- **참조 위치**: `HubManager`, `ResultsHudController` (기존 로컬 상수 → `UpgradeIds.RouteScanner` 리팩터링 완료)

### Pearl Resonator (`UpgradeIds.PearlResonator = "pearl_resonator"`)

- **효과**: recommendedPower ≥ 3인 디스트릭트(Glass Narrows 이상) 성공 시 MemoryPearl 보너스 지급
- **보상 연계**: `RewardCalculator.CalculateSuccess()` → `memoryPearlBonus = pearlResonatorUpgrade.memoryPearlBonus`
- **설계 의도**: 후반 빌드 선택의 핵심 재화 공급원

### 업그레이드 공통 규칙

- 전투 계수가 아니라 "정화 효율" 및 "자원 수급" 위주
- 한 번에 3단계 이상 급격히 강해지지 않도록 구간 제한
- 업그레이드 레벨 조회: `SaveService.GetHubUpgradeLevel(upgradeId)`
- 업그레이드 데이터 로드: `Resources.Load<HubUpgradeDef>(ContentPaths.*)` (경로는 `ContentPaths` 상수 클래스 참조)

---

## 참조 문서

| 문서 | 관련 영역 |
|------|----------|
| `03_core_loop_and_progression.md` | 코어 루프, 세션 구조, 진행 축 |
| `04_system_spec.md` | 시스템 목록, 정화/오염/자원/저장 상세 |
| `07_rules_balance_failstates.md` | 밸런스 규칙, 실패 상태, 별점 철학 |
| `14_scene_improvement_progress.md` | Phase별 구현 현황, Unity Play Mode 검증 결과 |
| `docs/scene_improvement_spec.md` | 클래스 수준 기술 명세 (신규 클래스, 확장 클래스, 씬별 상세) |
