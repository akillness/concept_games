# Gameplay And Systems

## Core Loop

1. 허브에서 지구와 난이도를 선택한다.
2. 원정에서 BloomDust, Scrap, SeedPod를 모으고 지구 목표를 달성한다.
3. Results에서 별점과 보상을 확정한다.
4. 허브에서 업그레이드와 지구 복원을 진행한 뒤 다음 지구를 연다.

## Current District Progression

| 지구 | 필요 별점 | 입장 비용 | 타이머 | 목표 | 완료 보너스 |
|------|-----------|-----------|--------|------|-------------|
| Dock | 0 | 8 BD | 210초 | CollectPickups(3) | BD +20 / Scrap +6 |
| Reed Fields | 1 | 10 BD | 185초 | CollectResource(SeedPod, 5) | BD +30 / Scrap +8 |
| Tidal Vault | 2 | 15 BD | 180초 | CollectResource(CleanWater, 3) | BD +35 / Scrap +10 |
| Glass Narrows | 4 | 22 BD | 180초 | HoldOut(60초) | BD +40 / Scrap +12 |
| Sunken Arcade | 6 | 28 BD | 165초 | CollectPickups(5) | BD +50 / Scrap +15 |
| Lighthouse Crown | 8 | 35 BD | 150초 | HoldOut(75초) | BD +60 / Scrap +20 |

## Difficulty Rules

| 난이도 | 타이머 | 입장 비용 | 실패 자원 보존율 |
|--------|--------|-----------|------------------|
| Easy | x1.3 | x0.7 | 85% |
| Normal | x1.0 | x1.0 | 70% |
| Hard | x0.8 | x1.3 | 50% |

## Star Rules

| 지구 | 2성 픽업 비율 | 3성 시간 비율 | 3성 컷 |
|------|---------------|---------------|--------|
| Dock | 0.80 | 0.65 | 136.5초 |
| Reed Fields | 0.75 | 0.60 | 111초 |
| Tidal Vault | 0.75 | 0.55 | 99초 |
| Glass Narrows | 0.70 | 0.50 | 90초 |
| Sunken Arcade | 0.70 | 0.50 | 82.5초 |
| Lighthouse Crown | 0.65 | 0.65 | 97.5초 |

## Resource Loop

- BloomDust: 입장 비용과 허브 업그레이드의 기본 통화
- Scrap: Harbor Pump 설치와 후반 보상 축
- SeedPod: 원정 수집 후 `Bio Press`로 `6 -> CleanWater +2` 변환 가능
- CleanWater: Harbor Pump / Pearl Resonator 쪽 진행에 사용
- MemoryPearl: `recommendedPower >= 3` 지구를 Pearl Resonator 활성 상태로 완수할 때 보너스 지급

## Current System Additions

- SeedPod telemetry:
  - `seedPodDelta`
  - `bioPressUseCount`
  - `bioPressCleanWaterConverted`
- SeedPod ratio automation:
  - `SeedPodRefineryExperiment.DefaultQAFifteenRunPlan`
  - `SeedPodRefineryExperiment.CompareCandidateProfiles(...)`
- Boundary recovery:
  - district별 `BoundaryRecoveryProfile`
  - 경계 이탈/낙하 시 safe position 복귀
- UV safety:
  - 런타임 fallback 유지
  - 에디터 import guardrail 추가

## Next Balance Questions

1. SeedPod `6:2 / 5:2 / 6:3` 중 어떤 비율이 15-run 기준 재고 20~30 밴드를 유지하는가.
2. Glass Narrows, Lighthouse Crown의 HoldOut과 3성 동선이 실제 플레이 로그에서도 기대 수준으로 유지되는가.
3. 후반 지구 별점 게이트 `4/6/8`이 재도전 유인을 만들면서도 과도한 막힘을 만들지 않는가.

## Current Experiment Baseline

- 기본 비교 플랜:
  - `Reed Fields(6) -> Tidal Vault(2) -> Glass Narrows(3)`를 5회 반복한 15-run QA 시나리오
  - 가정: 허브 복귀당 refine 최대 1회
- 현재 비교 결과:
  - `high-yield (6:3)` -> baseline과 같은 재고 곡선, 더 높은 CleanWater 수익
  - `baseline (6:2)` -> 현재 라이브 규칙
  - `fast-sink (5:2)` -> 재고는 더 빨리 소모하지만 기본 QA 플랜 기준 target band 거리 손해가 큼
- 해석:
  - 자동 비교 하네스는 준비됐고, 다음 결정 포인트는 실제 플레이 로그를 같은 포맷으로 붙여서 이 순위가 유지되는지 확인하는 것이다.
