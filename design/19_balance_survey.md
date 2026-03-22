# 19. Game Balance Survey & Tuning Recommendations

> BMAD-GDS 밸런스 분석 — 현재 구현 상태 기반 경제 흐름, 난이도 곡선, 진행 속도 평가 및 튜닝 권고.
> 관심사 분리: 이 문서는 밸런스(수치/경제)만 다룬다. 기획은 design/20, QA는 docs/qa_verification_checklist.md 참조.
> 모든 수치는 `DistrictBalanceDefaults.cs` 및 `RewardCalculator.cs` 직접 계산값 기준.

---

## 1. 현재 밸런스 스냅샷

### 1.1 자원 경제 흐름 분석

#### BloomDust 경제 (완전 수집 기준)

| 지구 | 입장 비용 | 픽업 BD | 완료 보너스 | 총 획득 BD | 순이익 BD | BD/초 (총) | 다음 지구까지 런 수 |
|------|-----------|---------|------------|-----------|-----------|-----------|-------------------|
| Dock | ~~5~~ **8** | 3×12=36 | +25 | 61 | **+53** | 0.29 | 1 런 |
| Reed Fields | 10 | 2×10=20 | +30 | 50 | **+40** | 0.26 | 1 런 |
| Tidal Vault | 15 | 2×10=20 | +35 | 55 | **+40** | 0.31 | 1 런 |
| Glass Narrows | ~~20~~ **22** | 2×12=24 | +40 | 64 | **+42** | 0.36 | 1 런 |
| Sunken Arcade | ~~25~~ **28** | 3×14=42 | +50 | 92 | **+64** | 0.56 | 1 런 |
| Lighthouse Crown | ~~30~~ **35** | 3×15=45 | +60 | 105 | **+70** | 0.70 | — |

> 계산식: 순이익 = (bloomPickupCount × bloomPickupAmount + completionBonusBloomDust) - expeditionEntryCost
> BD/초 = 총 획득 BD ÷ runTimerSeconds

**핵심 관측**: 모든 지구에서 단 1런 만에 다음 지구 진입 비용을 충당할 수 있다. BD 병목이 사실상 존재하지 않으며, 플레이어가 BD 부족으로 진행이 막히는 상황은 발생하기 어렵다.

#### Scrap 경제 (완전 수집 기준)

| 지구 | 픽업 Scrap | 완료 보너스 | 총 획득 Scrap | 누적 (5런 기준) |
|------|-----------|------------|--------------|----------------|
| Dock | 1×5=5 | +6 | 11 | 55 |
| Reed Fields | 2×5=10 | +8 | 18 | 90 |
| Tidal Vault | 2×6=12 | +10 | 22 | 110 |
| Glass Narrows | 2×7=14 | +12 | 26 | 130 |
| Sunken Arcade | 3×8=24 | +15 | 39 | 195 |
| Lighthouse Crown | 3×10=30 | +20 | 50 | 250 |

> Harbor Pump 비용: Scrap 15개 → Dock 기준 2런이면 달성 가능 (11+11=22 → 2런 후 여유)

#### SeedPod 경제

| 지구 | 픽업 SeedPod | 총 획득 | 소비처 |
|------|-------------|--------|--------|
| Reed Fields | 2×3=6 | 6 | 없음 (설계 미완) |
| Tidal Vault | 1×2=2 | 2 | 없음 |
| Glass Narrows | 1×3=3 | 3 | 없음 |
| Sunken Arcade | 1×3=3 | 3 | 없음 |
| Lighthouse Crown | 2×4=8 | 8 | 없음 |

> **P1 이슈**: SeedPod는 설계 문서(design/06)에서 "식생 복원, 생물 유인 장식" 소비처가 명시되어 있으나, 현재 구현에서 실제 소비처가 없어 자원이 누적만 된다. 자원 순환이 단절된 상태.

### 1.2 난이도 곡선 분석

#### 타이머 압박도 (초/픽업, 낮을수록 빡빡)

| 지구 | runTimerSeconds | 총 픽업 수 | 초/픽업 | 압박 수준 |
|------|----------------|-----------|---------|---------|
| Dock | 210초 | 4개 | **52.5초** | 매우 낮음 |
| Reed Fields | 195초 | 6개 | **32.5초** | 낮음 |
| Tidal Vault | 180초 | 5개 | **36.0초** | 낮음 |
| Glass Narrows | 180초 | 5개 | **36.0초** | 중간 (HoldOut 75초 포함) |
| Sunken Arcade | 165초 | 7개 | **23.6초** | 높음 |
| Lighthouse Crown | 150초 | 8개 | **18.75초** | 최고 (HoldOut 90초 포함) |

> Dock → Reed Fields 전환 시 초/픽업이 52.5 → 32.5로 38% 급감 (가장 큰 단일 점프).
> Tidal Vault → Glass Narrows는 수치 동일(36초)이나 목표 유형이 CollectResource → HoldOut으로 전환되어 실질 난이도가 크게 상승.

#### 픽업 밀도 (픽업 수 ÷ pickupSpawnRadius, 높을수록 조밀)

| 지구 | 픽업 수 | pickupSpawnRadius | 밀도 (픽업/m) | 판단 요구도 |
|------|--------|------------------|--------------|-----------|
| Dock | 4 | 7m | 0.57 | 낮음 |
| Reed Fields | 6 | 9m | 0.67 | 중간 |
| Tidal Vault | 5 | 10m | 0.50 | 중간 |
| Glass Narrows | 5 | 11m | 0.45 | 중간 (장거리 이동 압박) |
| Sunken Arcade | 7 | 8m | **0.88** | 높음 (가장 조밀) |
| Lighthouse Crown | 8 | 13m | 0.62 | 높음 (방사형 복합) |

#### 별점 난이도 지수 (twoStarPickupRatio × threeStarTimeRatio)

| 지구 | 2성 픽업 비율 | 3성 시간 비율 | 난이도 지수 | 3성 컷 (초) | 실제 부담 |
|------|-------------|-------------|-----------|-----------|---------|
| Dock | 0.80 | 0.65 | **0.520** | 210×0.65=**137초 이내** | 낮음 |
| Reed Fields | 0.75 | 0.60 | **0.450** | 195×0.60=**117초 이내** | 낮음 |
| Tidal Vault | 0.75 | 0.55 | **0.413** | 180×0.55=**99초 이내** | 중간 |
| Glass Narrows | 0.70 | 0.50 | **0.350** | 180×0.50=**90초 이내** | 높음 (HoldOut 75초 내 포함) |
| Sunken Arcade | 0.70 | 0.50 | **0.350** | 165×0.50=**82.5초 이내** | 높음 |
| Lighthouse Crown | 0.65 | ~~0.45~~ **0.65** | **0.423** | 150×0.65=**97.5초 이내** | 달성 가능 (HoldOut 90초 < 97.5초) ✓ |

> **설계 모순**: Lighthouse Crown의 3성 컷 67.5초가 HoldOut 요구 시간 90초보다 짧다. 즉 HoldOut 목표를 완수하는 것만으로도 3성 달성이 불가능하다. 의도된 설계(수집 포기 후 즉시 HoldOut 진입)라면 design/17에 명시된 대로이나, 일반 플레이어에게는 3성 달성 경로가 불투명하게 느껴질 수 있다.

### 1.3 업그레이드 경제 분석

| 업그레이드 | 비용 | Dock 기준 달성 런 수 | 효과 요약 |
|-----------|------|-------------------|---------|
| Harbor Pump | Scrap 15 | **2런** (11+11=22) | CleanWater +3/완료, RewardCalculator에서 cleanWaterBonus 활성화 |
| Route Scanner | BD 60 | **2런** (56+56=112, 60 달성) | 타이머 +timerBonusSeconds, BloomDust ×(bloomMultiplier-1) 추가 |
| Pearl Resonator | CleanWater 20 | Harbor Pump 선행 필수 | recommendedPower≥3 지구 완료 시 MemoryPearl +memoryPearlBonus |

> Pearl Resonator는 Harbor Pump 설치 후 CleanWater를 20개 모아야 하는데, Harbor Pump가 1회 완료당 CleanWater 3개를 제공하므로 최소 7회 완료가 필요하다. 또한 Pearl Resonator 효과 발동 조건이 recommendedPower≥3 (Glass Narrows 이상)이므로, 실제 MemoryPearl을 획득하려면 게임 후반 진입이 전제된다. 대부분의 플레이어가 이 자원을 경험하지 못할 가능성이 높다.

#### 실패 시 자원 보존 (RewardCalculator.CalculateFailure 기준)

| 자원 | 보존율 | 계산식 |
|------|--------|--------|
| BloomDust | 수집분 100% + 완료보너스 50% | `bloomDustCollected + RoundToInt(completionBonusBloomDust * 0.5)` |
| Scrap | 수집분 70% | `RoundToInt(scrapCollected * 0.7)` |
| CleanWater | **0%** | 미지급 |
| MemoryPearl | **0%** | 미지급 |

> 설계 문서 design/07에 "획득 자원의 70%를 보존"으로 명시되어 있으나 실제 코드는 자원마다 다르다: BD는 픽업분 전액 + 보너스 절반, Scrap은 70%로 일치. 실질 보존율은 설계 의도보다 후하다.

---

## 2. 장르 벤치마크 비교

| 타이틀 | 세션 시간 | 자원 종류 | 진행 구조 | 실패 페널티 | 반복 플레이 유인 |
|--------|---------|----------|---------|-----------|--------------|
| Spiritfarer | ~20분 | 3-4종 | 선형 (NPC 중심) | 없음 (사망 없음) | 감정적 결말 |
| A Short Hike | ~5분/세션 | 1종 (골든 페더) | 비선형 자유 탐색 | 없음 | 발견의 즐거움 |
| Unpacking | ~15분 | 0종 (퍼즐) | 선형 에피소드 | 없음 | 내러티브 |
| Celeste | ~3-8분/챕터 | 0종 (수집품) | 선형 + 선택적 B-Side | 즉사 재시작 | 숙련 도전 |
| **Moss Harbor** | **~3분** | **5종** | **반선형 (스타 잠금)** | **자원 일부 손실** | **스타 달성 + 업그레이드** |

**장르 내 포지션 분석:**

- **세션 시간**: Moss Harbor의 ~3분 세션은 장르 평균(10-15분)보다 현저히 짧다. 이는 반복 플레이를 전제한 설계이나, 짧은 세션이 "허브 복귀 → 업그레이드" 루프와 맞물려야 만족감이 유지된다. 현재 업그레이드 3종 뿐이라 루프 소진이 빠를 수 있다.
- **자원 종류**: 5종은 장르 내 최다 수준이다. Spiritfarer의 다양한 재료도 맥락(NPC 요리/공예)이 명확해 인지 부하가 낮지만, Moss Harbor의 5종은 소비처가 불균등(SeedPod 미소비)해 인지 부하가 체감 난이도를 올릴 수 있다.
- **실패 페널티**: Celeste의 즉사 재시작과 Spiritfarer의 무페널티 중간에 위치한다. 자원 일부 손실은 적절한 중간 지점이나, BD 보존이 후해 페널티 체감이 약하다.
- **스타 시스템**: Celeste의 B-Side와 유사하게 "이미 클리어한 구간을 더 높은 조건으로 재도전"하는 구조다. 재도전 동기 설계는 유효하나, 스타 요구량(0-1-2-3-4-5)이 선형 누적이라 후반 진입 벽이 낮다.

---

## 3. 식별된 밸런스 이슈

### 3.1 P0 (Critical — 즉시 수정 필요)

**[RESOLVED] P0-A: 전 지구 순이익 양수 — 경제 병목 부재**

모든 지구에서 단 1런으로 다음 지구 진입 비용을 회수한다. 최소 비용/수익 비율이 Dock 기준 1120% (비용 5, 수익 61)로, 경제적 긴장감이 없다. 플레이어가 "무엇이 부족한지"를 느낄 기회가 없고 자원 관리의 전략적 의미가 희석된다.

설계 문서(design/06) 원안의 "신규 기능 해금은 2~3회 원정 내 달성"은 현재 수치로는 모두 1~2런으로 달성되어 원안보다 훨씬 쉽다. 특히 설계 원안(design/06)과 현 구현 수치가 크게 다르다: 원안 Dock 입장 비용 10BD / 완료 30BD vs. 현재 5BD / 25BD.

> **수정 (2026-03-22)**: 입장 비용 전반 인상 — Dock 5→**8**, Glass Narrows 20→**22**, Sunken Arcade 25→**28**, Lighthouse Crown 30→**35**. `DistrictBalanceDefaults.cs` 반영 완료.
> 신규 비용 기준 Dock 순이익: 61 - 8 = **+53 BD** (1320% → 663%). 경제 긴장감 일부 회복.

**[RESOLVED] P0-B: Lighthouse Crown 3성 수학적 불가 구조**

3성 달성 조건(67.5초 이내 완료)이 HoldOut 목표 시간(90초)보다 짧다. HoldOut이 필수 목표이므로 3성은 달성 불가능하다. 의도된 설계라면 UI/문서에서 명시적으로 안내해야 하며, 의도되지 않았다면 `threeStarTimeRatio`를 0.45 이상으로 조정해야 한다.

> **수정 (2026-03-22)**: `threeStarTimeRatio` 0.45 → **0.65** 로 상향. 새 3성 컷 = 150 × 0.65 = **97.5초**, HoldOut 90초를 초과하므로 수학적 달성 가능 구조 복원. `DistrictBalanceDefaults.cs` 반영 완료.

### 3.2 P1 (Important — 다음 마일스톤 전 수정)

**[RESOLVED — 부분] P1-B 관련: 스타 요구량 비선형 강화**

> **수정 (2026-03-22)**: Glass Narrows 3→**4**, Sunken Arcade 4→**6**, Lighthouse Crown 5→**8**. `DistrictBalanceDefaults.cs` 반영 완료.
> 전체 요구량 배열: [0, 1, 2, **4**, **6**, **8**] — 후반 진입 허들 강화. (P1-B 권고값 [0,1,3,5,8,12] 대비 보수적 조정)

**P1-A: SeedPod 소비처 전무**

SeedPod는 Reed Fields 이상 모든 지구에서 누적되지만 소비처가 구현되어 있지 않다. 설계 문서(design/06)의 "식생 복원, 생물 유인 장식" 소비처가 구현 미완 상태다. 이는 자원 순환 단절로 이어지며, 후반부 SeedPod 과잉 축적이 경제 설계의 신뢰도를 낮춘다.

**P1-B: 스타 요구량 선형 구조로 인한 진행 벽 부재**

현재 스타 요구량: [0, 1, 2, 3, 4, 5] — 등차수열. 각 지구를 1회 클리어(1성)하면 다음 지구에 즉시 진입할 수 있어 반복 플레이 동기가 낮다. 1성 클리어만으로 게임 전체를 선형 통과할 수 있는 구조다.

**P1-C: MemoryPearl 접근성 과도하게 낮음**

Pearl Resonator 업그레이드 조건(Harbor Pump 선행 + CleanWater 20개 + recommendedPower≥3 지구) 때문에 대부분의 플레이어가 MemoryPearl을 한 번도 경험하지 못할 가능성이 높다. 설계 문서(design/06)에서 MemoryPearl이 "지구 해금 핵심 조건 + 고급 모듈 + 엔딩 직전 최종 복원"으로 중요 자원임에도 획득 경로가 지나치게 좁다.

**P1-D: Dock → Reed Fields 난이도 점프**

초/픽업 기준 Dock(52.5초)에서 Reed Fields(32.5초)로 38% 급감하며, 동시에 목표 유형도 CollectPickups(단순) → CollectResource:SeedPod(특정 자원 수집)로 전환된다. 첫 번째 진입 관문에서 가장 큰 단일 난이도 점프가 발생하는 것은 튜토리얼 이후 이탈 위험이 높다.

### 3.3 P2 (Nice to have — 여유 시 개선)

**P2-A: 지구별 기믹 차이가 수치 조정에만 의존**

fogDensity, pickupSpawnRadius 등 환경 수치 변화가 있으나, 각 지구 고유의 기믹(조수문, 포위형 오염군, HoldOut 등)이 난이도 축에 명시적으로 반영되지 않아 수치 곡선과 체감 곡선이 불일치할 수 있다. Glass Narrows의 fogDensity(0.015)가 오히려 전체에서 가장 낮아 시야가 열려있지만 장거리 이동이 긴장감을 대체한다 — 이 설계 의도가 수치만 봐서는 파악되지 않는다.

**P2-B: HoldOut vs CollectPickups 난이도 편차**

같은 지구 내에서 HoldOut(Glass Narrows 75초, Lighthouse Crown 90초)과 CollectPickups(Dock 3개, Sunken Arcade 5개)의 절대 난이도 편차가 크다. HoldOut 지구는 수집 후 거점 방어라는 복합 판단이 요구되어 CollectPickups 대비 체감 난이도가 급상승한다.

**P2-C: 실패 BloomDust 보존 로직이 설계 문서와 불일치**

RewardCalculator.CalculateFailure는 `bloomDustCollected + RoundToInt(completionBonusBloomDust * 0.5)`를 반환한다. 즉 수집분은 100% 보존되고 보너스의 50%가 추가된다. 설계 문서(design/07)의 "획득 자원의 70% 보존"과 다르며, 실질 보존율이 설계 의도보다 높아 실패 페널티가 더욱 약해진다.

---

## 4. 튜닝 권고

### 4.1 경제 조정

| 파라미터 | 현재값 | 권고값 | 이유 |
|----------|--------|--------|------|
| Dock `expeditionEntryCost` | 5 | **8** | 첫 런 순이익 과다 (56BD → 53BD), 자원 관리 감각 초기화 |
| Dock `completionBonusBloomDust` | 25 | **20** | 튜토리얼 지구 과도한 보상, 실질 BD/s 하향 |
| Reed Fields `runTimerSeconds` | 195초 | **185초** | Dock 대비 압박 점프 완화 (52.5 → 30.8초/픽업) |
| SeedPod 소비처 | 없음 | **허브 장식 복원 비용** | 자원 순환 필수, design/06 원안 구현 |
| `CalculateFailure` BloomDust | 수집분 100% + 보너스 50% | **수집분 70% + 보너스 0%** | 설계 문서 70% 보존 원칙 준수, 실패 페널티 정상화 |
| Pearl Resonator `CleanWater` 비용 | 20 | **12** | 접근성 상향, MemoryPearl 경험 플레이어 증가 |

### 4.2 진행 속도 조정

**스타 요구량 재배치 — 비선형 구조로 전환:**

| 지구 | 현재 requiredStars | 권고값 | 적용값 (2026-03-22) | 변화 이유 |
|------|-------------------|--------|---------------------|---------|
| Dock | 0 | **0** | **0** | 유지 (튜토리얼) |
| Reed Fields | 1 | **1** | **1** | 유지 |
| Tidal Vault | 2 | **3** | **2** | 미조정 |
| Glass Narrows | 3 | **5** | **4** ✓ | 중간 허들 강화 (보수적 조정) |
| Sunken Arcade | 4 | **8** | **6** ✓ | 상급 진입 전 숙련 확보 |
| Lighthouse Crown | 5 | **12** | **8** ✓ | 최종 지구 진입 허들 강화 |

> 비선형 요구량 [0, 1, 3, 5, 8, 12]로 변경 시 후반 지구 진입에 재도전이 강제되어 스타 시스템의 존재 이유가 생긴다. Celeste B-Side처럼 "다시 돌아가서 더 잘 클리어"하는 경험이 강화된다.
> **적용된 값 [0, 1, 2, 4, 6, 8]** — 권고값 대비 보수적이지만 방향성 일치. `DistrictBalanceDefaults.cs` 반영 완료.

**업그레이드 비용 조정:**

| 업그레이드 | 현재 비용 | 권고 비용 | 이유 |
|-----------|---------|---------|-----|
| Harbor Pump | Scrap 15 | **Scrap 20** | 2런 달성에서 3런으로 연장, 초반 경제 긴장감 |
| Route Scanner | BD 60 | **BD 80** | 2런 달성에서 2-3런으로 조정, 누적 시간 확보 |
| Pearl Resonator | CleanWater 20 | **CleanWater 12** | 접근성 상향 |

### 4.3 난이도 곡선 완화

**HoldOut 시간 조정:**

| 지구 | 현재 HoldOut | 권고 HoldOut | 이유 |
|------|------------|------------|-----|
| Glass Narrows | 75초 | **60초** | 180초 타이머 내 수집(~30초) + 이동(~15초) + HoldOut(75초) = 120초 여유 60초 → 60초로 하향 시 여유 75초 확보 |
| Lighthouse Crown | 90초 | **75초** | 150초 타이머와의 비율 완화, 3성 컷(67.5초)과의 모순 해소 가능성 |

**3성 시간 비율 조정:**

| 지구 | 현재 threeStarTimeRatio | 권고값 | 이유 |
|------|------------------------|--------|-----|
| Glass Narrows | 0.50 (90초 컷) | **0.45** (81초 컷) | HoldOut 60초 + 수집/이동 21초로 달성 가능한 범위 |
| Lighthouse Crown | 0.45 (67.5초 컷) | **수정 불필요** | HoldOut 75초로 조정 시 구조적 모순 해소 |

> Lighthouse Crown HoldOut을 90→75초로 줄이면, 3성 컷 67.5초 내에 HoldOut(75초)이 여전히 초과된다. 근본 해결책은 threeStarTimeRatio를 0.55 이상으로 상향(82.5초 컷)하거나, "HoldOut 목표는 3성 조건에서 시간 측정에서 제외"하는 규칙을 명시하는 것이다.

---

## 5. 구현 우선순위

| 순위 | 항목 | 코드 위치 | 영향도 | 구현 난이도 | 비고 |
|------|------|---------|--------|-----------|-----|
| 1 | **SeedPod 소비처 추가** | 허브 장식/복원 시스템 | 높음 | 중간 | 자원 순환 복구, P1-A |
| 2 | ~~**스타 요구량 비선형 재배치**~~ **[DONE]** | `DistrictBalanceDefaults.cs` `requiredStars` | 높음 | 낮음 | Glass Narrows 3→4, Sunken Arcade 4→6, Lighthouse Crown 5→8 |
| 3 | ~~**Lighthouse Crown 3성 모순 해소**~~ **[DONE]** | `DistrictBalanceDefaults.cs` `threeStarTimeRatio` | 높음 | 낮음 | threeStarTimeRatio 0.45→0.65 (3성 컷 97.5s > HoldOut 90s) |
| 4 | **실패 BD/Scrap 보존 로직 개선** | `RewardCalculator.CalculateFailure()` | 중간 | 낮음 | **[DONE]** DifficultyLevel 파라미터 추가, FailResourceRetention 적용 |
| 5 | ~~**입장 비용 미세 조정**~~ **[DONE]** | `DistrictBalanceDefaults.cs` `expeditionEntryCost` | 중간 | 낮음 | Dock 5→8, Glass Narrows 20→22, Sunken Arcade 25→28, Lighthouse Crown 30→35 |
| 6 | **완료 보너스 조정** | `DistrictBalanceDefaults.cs` `completionBonusBloomDust` | 중간 | 낮음 | Dock 25→20 |
| 7 | **HoldOut 시간 하향** | `DistrictBalanceDefaults.cs` `objectiveHoldSeconds` | 중간 | 낮음 | Glass Narrows 75→60, Lighthouse 90→75 |
| 8 | **Harbor Pump 비용 상향** | HubUpgradeDef ScriptableObject | 낮음 | 낮음 | Scrap 15→20 |
| 9 | **Pearl Resonator 비용 하향** | HubUpgradeDef ScriptableObject | 낮음 | 낮음 | CleanWater 20→12 |
| 10 | **Reed Fields 타이머 완화** | `DistrictBalanceDefaults.cs` `runTimerSeconds` | 낮음 | 낮음 | 195→185초 |

---

## 6. 수치 변경 전후 비교 요약

### BloomDust 경제 (권고 적용 후 추정)

| 지구 | 현재 순이익 | 권고 후 순이익 | 변화 |
|------|-----------|-------------|-----|
| Dock | +56 BD | +45 BD (비용 8, 보너스 20) | -11 BD |
| Reed Fields | +40 BD | +40 BD (변경 없음) | 동일 |
| Tidal Vault | +40 BD | +40 BD | 동일 |
| Glass Narrows | +44 BD | +44 BD | 동일 |
| Sunken Arcade | +67 BD | +67 BD | 동일 |
| Lighthouse Crown | +75 BD | +75 BD | 동일 |

> 초반(Dock) 과잉 수익만 선택적으로 조정. 중후반 경제 흐름은 유지하여 진행에 따른 성장감은 보존.

### 실패 보존율 비교

| 자원 | 현재 (코드 기준) | 권고 후 | 설계 문서 원안 |
|------|--------------|--------|-------------|
| BloomDust | 수집분 100% + 보너스 50% | 수집분 70% | 70% |
| Scrap | 수집분 70% | 수집분 70% | 70% |
| CleanWater | 0% | 0% | 0% |
| MemoryPearl | 0% | 0% | 50% (설계 원안) |

> 설계 문서(design/07)에서 "메모리 자원은 원정 실패 시 절반만 보존"이라고 명시되어 있으나 현재 코드는 MemoryPearl 0% 보존이다. MemoryPearl 희소성을 유지하되 접근성을 높이려면 실패 시 50% 보존을 구현하는 것이 원안에 부합한다.

---

## 7. 감시 지표 (Playtest KPI)

플레이테스트 시 아래 지표를 수집해 밸런스 검증에 활용할 것:

| 지표 | 목표 범위 | 측정 방법 |
|------|---------|---------|
| Dock 첫 클리어율 | 95% 이상 | 세션 로그 |
| Reed Fields 첫 클리어율 | 80% 이상 | 세션 로그 |
| Tidal Vault 도달률 | 60% 이상 | 세션 로그 |
| Glass Narrows 도달률 | 40% 이상 | 세션 로그 |
| 평균 런당 자원 보존율(실패) | 45-60% | RewardCalculator 로그 |
| 스타 3개 달성률 (Dock) | 40% 이상 | 별점 로그 |
| 스타 3개 달성률 (Lighthouse Crown) | 15% 이하 | 별점 로그 |
| SeedPod 최대 누적량 (완주 기준) | 40개 이하 | 자원 로그 |
| Harbor Pump 구매 시점 (런 수) | 3-5런 | 업그레이드 로그 |

---

*최종 수정: 2026-03-22 | 참조: DistrictBalanceDefaults.cs, RewardCalculator.cs, design/06, design/07, design/17*
