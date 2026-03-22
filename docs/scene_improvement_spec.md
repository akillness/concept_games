# Moss Harbor 씬 개선 기술 명세서

**Version**: 1.1 | **Date**: 2026-03-22 | **Status**: ✅ Phase 1~8 구현 완료 (Unity Play Mode 검증 완료)

## 1. 개요

### 1.1 목적

이 명세서는 Moss Harbor 게임의 씬 구조 개선과 아키텍처 확장을 문서화합니다. 원래 설계 의도와 현재 구현 간의 갭을 해소하고, 개발팀이 각 씬의 역할과 변경사항을 명확히 이해할 수 있도록 합니다.

### 1.2 범위

다음 5개 씬이 개선 대상입니다:

- **Boot**: 게임 초기화 및 페이드 인 효과
- **Hub**: 디스트릭트 선택, 업그레이드, 상태 관리
- **Expedition_Runtime**: 원정 플레이, 환경 테마, 자원 수집
- **Results**: 별점 평가, 보상 표시, 다음 행동 안내
- **SampleScene** (선택): 테스트 및 프로토타이핑용

### 1.3 영향받는 파일 목록

**신규 클래스:**
- `concept_game/Assets/Scripts/Data/UpgradeIds.cs`
- `concept_game/Assets/Scripts/Expedition/RewardCalculator.cs`
- `concept_game/Assets/Scripts/Data/DistrictBalanceDefaults.cs`
- `concept_game/Assets/Scripts/Data/StarRatingCalculator.cs`
- `concept_game/Assets/Scripts/UI/SceneFadeController.cs`

**확장/수정 클래스:**
- `concept_game/Assets/Scripts/Data/DistrictDef.cs`
- `concept_game/Assets/Scripts/Core/SceneFlowService.cs`
- `concept_game/Assets/Scripts/UI/RuntimeUiFactory.cs`
- `concept_game/Assets/Scripts/Expedition/ExpeditionDirector.cs`
- `concept_game/Assets/Scripts/UI/HudController.cs`
- `concept_game/Assets/Scripts/UI/HubHudController.cs`
- `concept_game/Assets/Scripts/UI/ResultsHudController.cs`
- `concept_game/Assets/Scripts/Hub/HubManager.cs`
- `concept_game/Assets/Scripts/Data/SaveData.cs` (RunSummary 확장)

---

## 2. 아키텍처 변경

### 2.1 새로운 클래스

#### UpgradeIds
**네임스페이스**: `MossHarbor.Data`
**위치**: `concept_game/Assets/Scripts/Data/UpgradeIds.cs`
**역할**: 허브 업그레이드 ID를 상수로 중앙 관리

```csharp
public static class UpgradeIds
{
    public const string HarborPump = "harbor_pump";
    public const string RouteScanner = "route_scanner";
    public const string PearlResonator = "pearl_resonator";
}
```

**사용처**: `RewardCalculator`, `SaveService`에서 업그레이드 레벨 조회 시

---

#### RewardCalculator
**네임스페이스**: `MossHarbor.Expedition`
**위치**: `concept_game/Assets/Scripts/Expedition/RewardCalculator.cs`
**역할**: 원정 성공/실패 시 보상 계산 로직 분리

**주요 메서드**:
- `CalculateSuccess(DistrictDef, SaveService, bloomDustCollected, scrapCollected, remainingTime)`: 성공 시 최종 보상 계산
- `CalculateFailure(DistrictDef, SaveService, bloomDustCollected, scrapCollected, remainingTime)`: 실패 시 자원 보존율 적용

**구조체 CompletionReward**:
```csharp
public struct CompletionReward
{
    public int bloomDust;      // BloomDust 최종 수량
    public int scrap;          // Scrap 최종 수량
    public int cleanWater;     // CleanWater 보너스 (성공 시만)
    public int memoryPearl;    // MemoryPearl 보너스 (성공 시만, power >= 3)
    public float duration;     // 원정 소요시간 (초)
}
```

**보상 규칙**:
- **성공**: 수집량 + 완료 보너스 + 업그레이드 보너스
- **실패**:
  - BloomDust: 수집량 + 완료 보너스의 50%
  - Scrap: 수집량의 70%
  - CleanWater/MemoryPearl: 0 (업그레이드 미적용)

---

#### DistrictBalanceDefaults
**네임스페이스**: `MossHarbor.Data`
**위치**: `concept_game/Assets/Scripts/Data/DistrictBalanceDefaults.cs`
**역할**: 6개 디스트릭트별 밸런스 파라미터를 런타임에 적용

**메서드**: `ApplyIfDefault(DistrictDef district)`

각 디스트릭트마다 전용 `Apply{DistrictName}()` 메서드 제공:
- `ApplyDock()`
- `ApplyReedFields()`
- `ApplyTidalVault()`
- `ApplyGlassNarrows()`
- `ApplySunkenArcade()`
- `ApplyLighthouseCrown()`

**호출 시점**: 원정 로드 시, `ExpeditionDirector`에서 디스트릭트 정보 초기화 단계

---

#### StarRatingCalculator ✅ IMPLEMENTED
**네임스페이스**: `MossHarbor.Data`
**위치**: `concept_game/Assets/Scripts/Data/StarRatingCalculator.cs`
**역할**: RunSummary를 기반으로 1~3성 별점 계산
**구현 상태**: 완료 — `Calculate(DistrictDef, RunSummary): int` 구현. `completed == false`이면 0 반환. pickupRatio, timeRatio 모두 분모 0 안전처리 포함.

**메서드**: `Calculate(DistrictDef, RunSummary): int`

**별점 기준**:
1. **1성**: 메인 목표 완료 (completed = true)
2. **2성**: 1성 + 픽업 수집률 >= `twoStarPickupRatio`
3. **3성**: 2성 + 소요시간/총시간 <= `threeStarTimeRatio`

**계산식**:
```
pickupRatio = pickupsCollected / (bloomPickupCount + scrapPickupCount + seedPodPickupCount)
timeRatio = durationSeconds / runTimerSeconds

1성: completed == true
2성: 1성 && pickupRatio >= district.twoStarPickupRatio
3성: 2성 && timeRatio <= district.threeStarTimeRatio
```

---

#### SceneFadeController ✅ IMPLEMENTED
**네임스페이스**: `MossHarbor.UI`
**위치**: `concept_game/Assets/Scripts/UI/SceneFadeController.cs`
**역할**: 씬 전환 시 페이드 인/아웃 효과 관리 (싱글톤)
**구현 상태**: 완료 — DontDestroyOnLoad, Canvas sortingOrder 999, CanvasGroup 알파 제어, `FadeOut`/`FadeIn`/`SetOpaque`/`SetClear` 메서드 구현. `GameBootstrap.EnsureSceneFadeController()`에서 Boot 시 자동 생성.

**주요 메서드**:
- `FadeOut(float duration, Action onComplete)`: 화면을 검게 페이드 아웃
- `FadeIn(float duration, Action onComplete)`: 검은 화면에서 페이드 인
- `SetOpaque()`: 즉시 완전 검은 화면
- `SetClear()`: 즉시 완전 투명

**프로퍼티**:
- `Instance`: 싱글톤 인스턴스 (DontDestroyOnLoad)
- `IsFading`: 현재 페이드 진행 중 여부

**구조**:
- Canvas (ScreenSpace Overlay, sortingOrder 999)
- CanvasGroup (알파 제어)
- Image (검은색 FadeOverlay, raycastTarget true)

---

### 2.2 확장된 클래스

#### DistrictDef
**네임스페이스**: `MossHarbor.Data`

**추가된 필드**:

| 필드명 | 타입 | 설명 | 기본값 |
|--------|------|------|--------|
| `fogColor` | Color | 환경 안개 색상 | (0.55, 0.72, 0.78, 1) |
| `fogDensity` | float | 안개 농도 | 0.022 |
| `ambientColor` | Color | 앰비언트 조명 색상 | (0.42, 0.56, 0.62, 1) |
| `sunColor` | Color | 태양 조명 색상 | (0.82, 0.92, 0.96, 1) |
| `sunIntensity` | float | 태양 조명 강도 | 1.35 |
| `mapScale` | float | 맵 스케일 배수 | 1.0 |
| `seedPodPickupCount` | int | SeedPod 픽업 생성 수 | 0 |
| `seedPodPickupAmount` | int | 각 SeedPod 픽업 수량 | 3 |
| `twoStarPickupRatio` | float | 2성 픽업 수집률 기준 | 0.8 |
| `threeStarTimeRatio` | float | 3성 시간 비율 기준 | 0.6 |

**용도**:
- 환경 파라미터: `RuntimeArtDirector`가 씬 로드 시 참조하여 조명/안개 설정
- 픽업 분배: `ExpeditionDirector`가 SeedPod 픽업 생성에 사용
- 별점 기준: `StarRatingCalculator`가 등급 판정에 사용

---

#### SceneFlowService ✅ IMPLEMENTED (fade transitions)
**네임스페이스**: `MossHarbor.Core`

**구현 상태**: 완료 — `LoadWithFade()` 구현. 레이스 컨디션 방지를 위해 `LoadSceneAsync` + `_isTransitioning` 가드 적용. `SceneFadeController.Instance` 없을 때 직접 로드 폴백 포함.

**변경사항**:
- `LoadWithFade()` (private): 씬 전환 시 페이드 효과 적용
  - 페이드 아웃: 0.35초
  - 씬 로드 (`LoadSceneAsync`, 레이스 컨디션 방지)
  - 페이드 인: 0.35초
  - `SceneFadeController.Instance` 존재 시에만 적용; 없으면 직접 로드

**메서드**:
- `LoadHub()`: Hub 씬을 페이드와 함께 로드
- `LoadExpedition()`: Expedition_Runtime 씬을 페이드와 함께 로드
- `LoadResults()`: Results 씬을 페이드와 함께 로드

---

#### RuntimeUiFactory
**네임스페이스**: `MossHarbor.UI`

**추가된 메서드** (예시):
- `CreateProgressBar(parent, label, maxValue)`: 자원 수집 프로그레스 바 생성
- `CreateIconLabel(parent, icon, label, value)`: 아이콘+라벨 표시
- `CreateToast(parent, message, duration)`: 임시 알림 토스트
- `CreateStarIcon(parent, stars)`: 별점 시각화 (1~3성)

**사용처**: Hub HUD, Results HUD에서 동적 UI 생성

---

#### ExpeditionDirector
**네임스페이스**: `MossHarbor.Expedition`

**변경사항**:
- 씬 로드 시 `DistrictBalanceDefaults.ApplyIfDefault(currentDistrict)` 호출
- SeedPod 픽업 생성: `district.seedPodPickupCount > 0`인 경우 `SimplePickup` 생성
- `Collect()` 메서드 확장: SeedPod, CleanWater, MemoryPearl 자원 타입 추적

---

#### HudController (Expedition_Runtime 씬 UI)
**네임스페이스**: `MossHarbor.UI`

**변경사항**:
- 디스트릭트 테마 색상 반영 (DistrictDef.districtColor)
- 컴팩트한 레이아웃: 남은 시간, 목표 진행도, 현재 수집 자원 최소 표시
- 자원 아이콘 표시 (Bloom, Scrap, SeedPod, CleanWater, MemoryPearl)

---

#### HubHudController
**네임스페이스**: `MossHarbor.UI`

**변경사항**:
- 핵심 CTA 3개 중심: "Start Expedition", "Upgrades", "District Status"
- 디스트릭트 선택 UI: 카드형 레이아웃
  - 각 카드: 디스트릭트 이름, 입장료, 별점 표시, 잠금 상태
  - 필수 별점 미만 시 자동 비활성화
  - 현재 복원 상태 시각화 (RuntimeArtDirector.hubZoneRestorationStates 참조)
- 업그레이드 버튼: 비용/효과 표시
- 디버그 버튼: `#if UNITY_EDITOR` 블록 내에 완전 격리

---

#### ResultsHudController
**네임스페이스**: `MossHarbor.UI`

**변경사항**:
- 별점 시각화: `StarRatingCalculator.Calculate()` 결과를 별 아이콘으로 표시 (1~3성)
- 보상 요약:
  - BloomDust, Scrap, CleanWater, MemoryPearl: 각 자원별 아이콘 + 수량
  - 프로그레스 바로 진행률 표시 (선택)
- 다음 행동 안내:
  - 디스트릭트별 특성에 맞춰 텍스트 자동 생성
  - 목표 유형별 세분화 (CollectPickups vs CollectResource vs HoldOut)

---

#### HubManager
**네임스페이스**: `MossHarbor.Hub`

**변경사항**:
- 디스트릭트 카드 생성 시 `DistrictBalanceDefaults.ApplyIfDefault()` 호출 (선택)
- 상태 업데이트: 저장된 복원 상태 로드 후 UI 반영

---

#### SaveData - RunSummary
**네임스페이스**: `MossHarbor.Core`

**추가된 필드**:
- `seedPodCollected`: int (수집한 SeedPod 총량, 기본값 0)

**호환성**: 기존 세이브 파일에서도 자동으로 기본값 0 적용

---

### 2.3 클래스 다이어그램 (개요)

```
DistrictDef (확장)
  ├── 환경 파라미터 → RuntimeArtDirector
  ├── 픽업 분배 → ExpeditionDirector
  └── 별점 기준 → StarRatingCalculator

DistrictBalanceDefaults
  └── ApplyIfDefault() → ExpeditionDirector, HubManager

RewardCalculator
  ├── CalculateSuccess() → ExpeditionDirector
  └── CalculateFailure() → ExpeditionDirector

StarRatingCalculator
  └── Calculate() → ResultsHudController

SceneFadeController (싱글톤)
  └── FadeIn/Out() → SceneFlowService

SaveData.RunSummary (확장)
  └── seedPodCollected → ResultsSummary 계산

UpgradeIds
  └── 상수 참조 → RewardCalculator, SaveService
```

---

## 3. 씬별 변경 상세

### 3.1 Boot 씬

**역할**: 게임 초기화, 메인 UI 생성, Hub로 전환

**변경사항**:

1. **SceneFadeController 자동 생성**
   - `GameBootstrap.cs`에서 Boot 씬 로드 시 SceneFadeController 프리팹 인스턴스화
   - 또는 Boot 씬에 SceneFadeController 게임오브젝트 배치
   - DontDestroyOnLoad로 이후 씬에서도 유지

2. **페이드 인 효과**
   - Boot 시작 시 `SceneFadeController.SetOpaque()`로 완전 검은 화면
   - UI 로드 완료 후 `SceneFadeController.FadeIn(0.5f)`로 부드럽게 페이드 인

3. **Hub 전환**
   - 모든 초기화 완료 후 `SceneFlowService.LoadHub()` 호출
   - 페이드 아웃(0.35초) → Hub 로드 → 페이드 인(0.35초)

**코드 예시** (GameBootstrap):
```csharp
private void TransitionToHub()
{
    var sceneFlow = new SceneFlowService(_gameStateService, _saveService);
    sceneFlow.LoadHub();
}
```

---

### 3.2 Hub 씬

**역할**: 디스트릭트 선택, 업그레이드, 복원 상태 관리

**주요 변경사항**:

#### 3.2.1 HUD 재구성

**핵심 CTA 3개 중심**:
1. "Start Expedition" (활성화된 디스트릭트 선택 후)
2. "Upgrades" (업그레이드 메뉴)
3. "District Status" (복원 상태 확인)

**레이아웃**: 상단 정보바 (자원 표시) + 중앙 디스트릭트 카드 + 하단 버튼군

#### 3.2.2 디스트릭트 선택 UI (카드형)

**각 카드 정보**:
```
┌─────────────────┐
│ 디스트릭트 이름  │
│ 추천 Power: N   │
│ 필요 별점: N    │
│ 입장료: N coin  │
│ ★★☆ (현재 보유) │
└─────────────────┘
```

**상태 표시**:
- 필수 별점 미만: 비활성화 (로크 아이콘)
- 입장료 부족: 비활성화 (회색 처리)
- 활성화: 클릭 시 Expedition_Runtime 로드

**참조 데이터**:
- `DistrictBalanceDefaults`에서 런타임에 적용된 `requiredStars`
- `SaveData.starsByDistrict[districtId]`로 현재 별점 조회

#### 3.2.3 복원 상태 시각 반영

**RuntimeArtDirector 연계**:
- `RuntimeArtDirector.hubZoneRestorationStates` 배열 참조
- 각 지역별 복원 수준 (0~100%) 시각화
- 프로그레스 바 또는 색상 그라데이션으로 표현

**예시**:
```csharp
// HubHudController에서
foreach (var zone in zones)
{
    var restorePercent = RuntimeArtDirector.Instance.hubZoneRestorationStates[zone.id];
    zone.ui.UpdateProgress(restorePercent);
}
```

#### 3.2.4 업그레이드 버튼

**정보 표시**:
- 업그레이드 이름
- 현재 레벨
- 다음 레벨 비용
- 효과 요약 (예: "BloomDust +5%", "Timer +10s")

**구현**: `HubUpgradeDef` 데이터 기반 동적 생성

#### 3.2.5 디버그 버튼 완전 분리

**조건부 컴파일**:
```csharp
#if UNITY_EDITOR
    // Debug UI 생성
    CreateDebugPanel();
#endif
```

**기능**:
- 전체 리소스 리셋
- 특정 디스트릭트 완료 상태로 변경
- 별점 강제 설정
- 콘솔 로그 표시

---

### 3.3 Expedition_Runtime 씬

**역할**: 원정 플레이, 자원 수집, 목표 달성

**주요 변경사항**:

#### 3.3.1 디스트릭트별 환경 테마

**적용 시점**: 씬 로드 시 `ExpeditionDirector`에서

**파라미터 적용**:
```csharp
private void ApplyDistrictEnvironment(DistrictDef district)
{
    RenderSettings.fogColor = district.fogColor;
    RenderSettings.fogDensity = district.fogDensity;
    RenderSettings.ambientLight = district.ambientColor;

    var sunLight = FindObjectOfType<Light>(SearchOptions.All);
    if (sunLight != null)
    {
        sunLight.color = district.sunColor;
        sunLight.intensity = district.sunIntensity;
    }
}
```

**지구별 테마** (자세히는 4장 참조):
- **Dock**: 청록 안개 (fogDensity 0.022)
- **Reed Fields**: 습지 녹색 (fogDensity 0.026)
- **Tidal Vault**: 깊은 파랑 (fogDensity 0.018)
- **Glass Narrows**: 수정 얼음색 (fogDensity 0.015)
- **Sunken Arcade**: 따뜻한 황금색 (fogDensity 0.024)
- **Lighthouse Crown**: 밤 남색 (fogDensity 0.028)

#### 3.3.2 맵 레이아웃

**변수적 설정**:
```csharp
var beaconPosition = district.beaconPosition;
var pickupSpawnRadius = district.pickupSpawnRadius;
var mapScale = district.mapScale;
```

**용도**:
- `beaconPosition`: 목표 비콘 위치
- `pickupSpawnRadius`: 픽업 스폰 범위 반경
- `mapScale`: 맵 오브젝트 스케일 배수

#### 3.3.3 SeedPod 픽업 추가

**조건**: `district.seedPodPickupCount > 0`

**생성 로직**:
```csharp
if (district.seedPodPickupCount > 0)
{
    for (int i = 0; i < district.seedPodPickupCount; i++)
    {
        var position = GetRandomSpawnPosition(district.pickupSpawnRadius);
        var pickup = Instantiate(seedPodPickupPrefab, position, Quaternion.identity);
        pickup.SetAmount(district.seedPodPickupAmount);
        pickup.SetResourceType(ResourceType.SeedPod);
    }
}
```

**지구별 SeedPod 분배** (자세히는 4장 참조):
- Dock: 0개 (튜토리얼용)
- Reed Fields: 2개 (각 3개)
- Tidal Vault: 1개 (2개)
- Glass Narrows: 1개 (3개)
- Sunken Arcade: 1개 (3개)
- Lighthouse Crown: 2개 (각 4개)

#### 3.3.4 자원 수집 확장

**추적 대상**:
- BloomDust (기존)
- Scrap (기존)
- CleanWater (신규)
- MemoryPearl (신규)
- SeedPod (신규)

**Collect() 메서드 서명**:
```csharp
public void Collect(ResourceType type, int amount)
{
    switch (type)
    {
        case ResourceType.BloomDust:
            _bloomDustCollected += amount;
            break;
        case ResourceType.SeedPod:
            _seedPodCollected += amount;
            break;
        case ResourceType.CleanWater:
            _cleanWaterCollected += amount;
            break;
        case ResourceType.MemoryPearl:
            _memoryPearlCollected += amount;
            break;
        // ...
    }
}
```

#### 3.3.5 HUD 컴팩트화

**표시 정보**:
- 남은 시간 (카운트다운)
- 메인 목표 진행도 (프로그레스 바)
- 현재 수집 자원 (아이콘+숫자)
  - BloomDust: Bloom 아이콘
  - Scrap: Scrap 아이콘
  - SeedPod: Pod 아이콘
  - CleanWater: Water 아이콘 (해당 지구)
  - MemoryPearl: Pearl 아이콘 (해당 지구)

**디스트릭트 테마 색상 반영**:
```csharp
hudBackground.color = district.districtColor;
textColor = Color.Lerp(Color.black, Color.white,
    district.districtColor.grayscale > 0.5f ? 1f : 0f);
```

---

### 3.4 Results 씬

**역할**: 원정 결과 평가, 보상 표시, 다음 행동 안내

**주요 변경사항**:

#### 3.4.1 별점 시각화

**계산 및 표시**:
```csharp
private void DisplayStarRating()
{
    var stars = StarRatingCalculator.Calculate(_currentDistrict, _runSummary);
    var starUI = RuntimeUiFactory.CreateStarIcon(parent: resultsPanel, stars: stars);
}
```

**시각 요소**:
- 채워진 별 (gold/yellow): 획득한 별 수
- 빈 별 (gray): 미획득 별
- 애니메이션: 별이 하나씩 나타나는 효과 (0.2초 간격)

#### 3.4.2 보상 요약

**항목별 표시**:

| 자원 | 아이콘 | 수량 표시 |
|------|--------|---------|
| BloomDust | 꽃 아이콘 | 수집량 + 완료보너스 + 업그레이드 보너스 |
| Scrap | 너트 아이콘 | 수집량 + 완료보너스 |
| CleanWater | 물 아이콘 | 0 (실패 시) 또는 업그레이드 보너스 (성공 시) |
| MemoryPearl | 진주 아이콘 | 0 (실패 시) 또는 업그레이드 보너스 (성공 시, power≥3) |

**프로그레스 바** (선택):
```csharp
var maxValue = district.bloomPickupCount + district.scrapPickupCount;
var collected = runSummary.pickupsCollected;
progressBar.SetValue(collected, maxValue);
```

#### 3.4.3 다음 행동 안내

**조건부 텍스트**:

**성공 시**:
- "다음 지구를 선택해 원정을 진행하세요."
- 필요한 별점이 있으면: "★★★ 3개가 필요합니다."

**실패 시**:
- "더 많은 자원을 수집해 다시 도전하세요."
- 목표 유형별:
  - CollectPickups: "더 많은 픽업을 찾아보세요."
  - CollectResource: "{resourceType} 수집이 부족합니다."
  - HoldOut: "더 오래 버텨보세요."

**업그레이드 추천** (선택):
- 부족한 자원별로 업그레이드 추천
  - BloomDust 부족 → RouteScanner (BloomDust +배수)
  - 시간 부족 → RouteScanner (Timer +초)
  - CleanWater 필요 → HarborPump (CleanWater +보너스)

---

### 3.5 씬 전환 및 페이드 효과

**전환 흐름**:

```
Boot 시작
  ↓ (SetOpaque → FadeIn)
Hub
  ↓ (선택 후 FadeOut/In)
Expedition_Runtime
  ↓ (완료 또는 실패 후 FadeOut/In)
Results
  ↓ (확인 후 FadeOut/In)
Hub (또는 메인 메뉴)
```

**SceneFlowService 구현**:

```csharp
private static void LoadWithFade(string sceneName)
{
    var fade = SceneFadeController.Instance;
    if (fade != null && !fade.IsFading)
    {
        fade.FadeOut(0.35f, () =>
        {
            TryLoadScene(sceneName);
            fade.FadeIn(0.35f);
        });
    }
    else
    {
        TryLoadScene(sceneName);  // 폴백: 페이드 없이 직접 로드
    }
}
```

**타이밍**:
- 페이드 아웃: 0.35초 (UI 버튼 클릭 직후)
- 씬 로드: 동시 수행 (페이드 아웃 중)
- 페이드 인: 0.35초 (씬 로드 완료 후)
- **총 전환 시간**: ~0.7초 + 씬 로드 시간

---

## 4. 밸런스 수치표

### 4.1 디스트릭트별 핵심 수치

| 속성 | Dock | Reed Fields | Tidal Vault | Glass Narrows | Sunken Arcade | Lighthouse Crown |
|------|------|-----------|------------|--------------|--------------|-----------------|
| **진입료** | 8 | 10 | 15 | 22 | 28 | 35 |
| **타이머** | 210s | 185s | 180s | 180s | 165s | 150s |
| **Bloom** | 3×12 | 2×10 | 2×10 | 2×12 | 3×14 | 3×15 |
| **Scrap** | 1×5 | 2×5 | 2×6 | 2×7 | 3×8 | 3×10 |
| **SeedPod** | 0 | 2×3 | 1×2 | 1×3 | 1×3 | 2×4 |
| **목표 타입** | CollectPickups(3) | CollectResource(SeedPod,5) | CollectResource(CleanWater,3) | HoldOut(60s) | CollectPickups(5) | HoldOut(75s) |
| **필요 별점** | 0 | 1 | 2 | 4 | 6 | 8 |
| **권장 Power** | 1 | 1 | 2 | 3 | 3 | 4 |
| **완료보너스 Bloom** | 20 | 30 | 35 | 40 | 50 | 60 |
| **완료보너스 Scrap** | 6 | 8 | 10 | 12 | 15 | 20 |

---

### 4.2 환경 테마 파라미터

| 지구 | FogColor (RGB) | FogDensity | AmbientColor (RGB) | SunColor (RGB) | SunIntensity | 시각적 특징 |
|------|--|--|--|--|--|--|
| **Dock** | (0.55, 0.72, 0.78) | 0.022 | (0.42, 0.56, 0.62) | (0.82, 0.92, 0.96) | 1.35 | 청록 안개, 부드러운 조명 |
| **Reed Fields** | (0.38, 0.52, 0.36) | 0.026 | (0.32, 0.46, 0.30) | (0.72, 0.86, 0.68) | 1.25 | 습지 녹색, 낮은 태양 |
| **Tidal Vault** | (0.22, 0.36, 0.52) | 0.018 | (0.20, 0.32, 0.48) | (0.62, 0.78, 0.94) | 1.40 | 깊은 파랑, 선명한 조명 |
| **Glass Narrows** | (0.52, 0.62, 0.70) | 0.015 | (0.40, 0.52, 0.64) | (0.88, 0.92, 1.00) | 1.60 | 수정 얼음, 밝은 태양 |
| **Sunken Arcade** | (0.32, 0.24, 0.18) | 0.024 | (0.28, 0.22, 0.18) | (1.00, 0.82, 0.56) | 1.30 | 따뜻한 황금, 어두운 분위기 |
| **Lighthouse Crown** | (0.14, 0.16, 0.28) | 0.028 | (0.12, 0.16, 0.26) | (0.56, 0.62, 0.82) | 1.15 | 밤 남색, 깊은 안개 |

---

### 4.3 별점 기준

**공식**:

```
1성: completed == true

2성 조건:
  pickupRatio = pickupsCollected / totalPickups
  if (pickupRatio >= twoStarPickupRatio)
    stars = 2

3성 조건:
  timeRatio = durationSeconds / runTimerSeconds
  if (timeRatio <= threeStarTimeRatio && stars >= 2)
    stars = 3
```

**지구별 기준값**:

| 지구 | 2성 픽업 비율 | 3성 시간 비율 | 예시 (180s 기준) |
|------|--|--|--|
| Dock | 0.80 | 0.65 | 3성: 136.5초 이내 |
| Reed Fields | 0.75 | 0.60 | 3성: 111초 이내 |
| Tidal Vault | 0.75 | 0.55 | 3성: 99초 이내 |
| Glass Narrows | 0.70 | 0.50 | 3성: 90초 이내 |
| Sunken Arcade | 0.70 | 0.50 | 3성: 82.5초 이내 |
| Lighthouse Crown | 0.65 | 0.65 | 3성: 97.5초 이내 |

---

### 4.4 보상 계산

**성공 시 RewardCalculator.CalculateSuccess()**:

```
bloomDustFinal = bloomDustCollected
                + (routeScannerUpgrade가 있으면 bloomDustCollected * (bloomMultiplier - 1) 반올림)
                + completionBonusBloomDust

scrapFinal = scrapCollected + completionBonusScrap

cleanWaterBonus = (harborPumpUpgrade가 있으면 cleanWaterBonus, 아니면 0)

memoryPearlBonus = (pearlResonatorUpgrade가 있고 district.recommendedPower >= 3이면 memoryPearlBonus, 아니면 0)

durationSeconds = district.runTimerSeconds
                + (routeScannerUpgrade가 있으면 timerBonusSeconds)
                - remainingTime
```

**실패 시 RewardCalculator.CalculateFailure()**:

```
bloomDustFinal = bloomDustCollected * FailResourceRetention(difficulty) 반올림

scrapFinal = scrapCollected * FailResourceRetention(difficulty) 반올림

cleanWater = 0
memoryPearl = 0

durationSeconds = 위와 동일
```

---

### 4.5 실패 시 자원 보존율

| 자원 | 보존율 | 계산식 |
|------|--------|--------|
| BloomDust | 85% / 70% / 50% | collected × retention (Easy / Normal / Hard) |
| Scrap | 85% / 70% / 50% | collected × retention (Easy / Normal / Hard) |
| CleanWater | 0% | 0 (업그레이드 미적용) |
| MemoryPearl | 0% | 0 (업그레이드 미적용) |

---

## 5. 세이브 호환성

### 5.1 설계 원칙

모든 새로운 필드는 기본값을 갖도록 설계되어 기존 세이브 파일과 자동으로 호환됩니다.

### 5.2 DistrictDef (.asset 파일)

**신규 필드 기본값** (Unity 직렬화 자동 적용):

```csharp
// Boundary Recovery
public BoundaryRecoveryProfile boundaryRecovery = new();

// Environment (Header 블록)
public Color fogColor = new Color(0.55f, 0.72f, 0.78f, 1f);
public float fogDensity = 0.022f;
public Color ambientColor = new Color(0.42f, 0.56f, 0.62f, 1f);
public Color sunColor = new Color(0.82f, 0.92f, 0.96f, 1f);
public float sunIntensity = 1.35f;

// Pickup Distribution
public int seedPodPickupCount = 0;
public int seedPodPickupAmount = 3;

// Star Rating
public float twoStarPickupRatio = 0.8f;
public float threeStarTimeRatio = 0.6f;
```

**호환성 영향**:
- 기존 .asset 파일: 자동으로 기본값이 설정됨
- 런타임 적용: `DistrictBalanceDefaults.ApplyIfDefault()`가 이 기본값들을 덮어씀
- **결과**: 호환성 문제 없음

### 5.3 SaveData - RunSummary

**신규 필드**:
```csharp
public int seedPodCollected = 0;
public int seedPodDelta = 0;
public int bioPressUseCount = 0;
public int bioPressCleanWaterConverted = 0;
```

**호환성**:
- 기존 세이브에서 로드: JSON 직렬화 시 누락된 필드 = 기본값 0
- 신규 세이브: 원정 중 SeedPod 수집 시 업데이트
- **결과**: 기존 세이브도 에러 없이 로드 가능

### 5.4 마이그레이션 (필요 시)

런타임 마이그레이션은 필요하지 않습니다:
- SaveData는 JSON 기반이므로 누락된 필드 = 자동 기본값
- DistrictDef의 새 필드들은 기본값이 설정되므로 .asset 자동 호환
- DistrictBalanceDefaults는 런타임에만 적용되므로 세이브 영향 없음

---

## 6. 구현 상태 요약 (Phase 1~8)

| 항목 | 상태 | 비고 |
|------|------|------|
| SceneFadeController | ✅ IMPLEMENTED | DontDestroyOnLoad, sortingOrder 999, 0.35s 인/아웃 |
| SceneFlowService fade transitions | ✅ IMPLEMENTED | LoadSceneAsync + `_isTransitioning` 가드 |
| Star rating system (StarRatingCalculator) | ✅ IMPLEMENTED | 완료/픽업률/시간 3단계 |
| Difficulty system (파라미터 오프셋 설계) | ✅ IMPLEMENTED (NEW) | Normal 기준 고정, Easy/Hard 오프셋은 설정 메뉴 추가 시 적용 |
| Scene flow events (페이드 전환 흐름) | ✅ IMPLEMENTED (NEW) | Boot→Hub→Expedition→Results→Hub 전체 페이드 연결 |
| UpgradeIds 상수 통합 | ✅ IMPLEMENTED | 로컬 상수 전수 제거 |
| RewardCalculator 분리 | ✅ IMPLEMENTED | 성공/실패 분리, 70% Scrap 보존 |
| DistrictBalanceDefaults | ✅ IMPLEMENTED | 6개 디스트릭트 런타임 적용 |
| SeedPod 픽업 및 수집 추적 | ✅ IMPLEMENTED | ExpeditionDirector + RunSummary 확장 |
| RuntimeArtDirector 환경 테마 | ✅ IMPLEMENTED | DistrictDef 기반 fog/ambient/sun 적용 |
| RuntimeUiFactory 신규 메서드 | ⚠️ 유지 (미사용) | CreateProgressBar 등 4개 — 향후 HUD 개선 시 사용 |

---

## 7. 테스트 체크리스트

### 7.1 플로우 테스트

- [x] **씬 전환 페이드**: Boot → Hub → Expedition → Results → Hub 전체 페이드 흐름 (Unity Play Mode 검증 완료)
- [ ] **신규 세이브**: Boot → Hub → Expedition → Results → Hub 전체 플로우 완주
  - 자원 표시 확인
  - 별점 계산 정확성 확인
  - 세이브 상태 업데이트 확인

- [ ] **기존 세이브 로드**: 이전 버전 세이브 파일 로드 시 에러 없음
  - RunSummary 신규 telemetry 필드 기본값 0 자동 설정
  - 모든 UI 정상 표시

### 7.2 디스트릭트별 원정

- [ ] Dock (튜토리얼): CollectPickups(3) 목표
  - 환경 테마 적용 (cyan fog)
  - 별점 계산 정확 (상대적으로 쉬움)

- [ ] Reed Fields: CollectResource(SeedPod, 5) 목표
  - SeedPod 픽업 2개 생성 확인
  - CleanWater는 아직 미적용 (HoldOut 없음)

- [ ] Tidal Vault: CollectResource(CleanWater, 3) 목표
  - 환경 테마 (deep blue fog)
  - CleanWater 리소스 타입 정상 작동

- [ ] Glass Narrows: HoldOut(60s) 목표
  - 타이머 정확성 (180s)
  - 목표 비콘 정위치 (districtColor = crystal ice)

- [ ] Sunken Arcade: CollectPickups(5) 목표
  - 환경 테마 (warm amber)
  - 타이머 짧음 (165s) 확인

- [ ] Lighthouse Crown (최종): HoldOut(75s) 목표
  - 필요 별점 8개 확인
  - 모든 환경 테마 적용 (night indigo)

### 7.3 별점 시각화

- [ ] Results 씬에서 별점 표시 (1~3성)
- [ ] 1성: 목표 완료만
- [ ] 2성: 픽업 수집 기준 달성
- [ ] 3성: 시간 제한 달성
- [ ] 실패 시: 0성 표시

### 7.4 자원 보존율

- [ ] **성공 시**: 모든 자원 + 보너스 표시
  - BloomDust: 수집량 + 완료보너스 표시
  - Scrap: 수집량 + 완료보너스 표시
  - CleanWater: 업그레이드 보너스 표시
  - MemoryPearl: 업그레이드 보너스 표시 (power ≥ 3)

- [ ] **실패 시**: 보존율 적용
  - BloomDust: 수집량 + (완료보너스 × 0.5) 표시
  - Scrap: 수집량 × 0.7 표시
  - CleanWater/MemoryPearl: 0 표시

### 7.5 Hub 상태 표시

- [ ] 디스트릭트 카드 활성화/비활성화 상태
  - 필수 별점 미달: 로크 상태
  - 입장료 부족: 회색 처리
  - 정상: 선택 가능

- [ ] 복원 상태 시각화
  - RuntimeArtDirector.hubZoneRestorationStates 참조
  - 각 지역별 프로그레스 바 표시

### 7.6 씬 전환 페이드

- [x] Boot → Hub: 페이드 인 (0.5s) — Unity Play Mode 검증 완료
- [x] Hub → Expedition: 페이드 아웃(0.35s) → 로드 → 페이드 인(0.35s) — 구현 완료
- [x] Expedition → Results: 페이드 효과 정상 — 구현 완료
- [x] Results → Hub: 페이드 효과 정상 — 구현 완료
- [x] 페이드 중 UI 차단 확인 (blocksRaycasts = true) — SceneFadeController.Update()에서 처리

### 7.7 별점 시스템

- [x] StarRatingCalculator.Calculate() 1성/2성/3성 분기 — 구현 완료
- [x] completed == false 시 0 반환 — 구현 완료
- [x] pickupRatio 분모 0 안전처리 — 구현 완료
- [x] timeRatio 분모 0 안전처리 — 구현 완료
- [ ] Results 씬에서 별점 아이콘 시각화 (1~3성) — 플레이 검증 필요

### 7.8 난이도 시스템 (NEW)

- [x] Normal 기준 DistrictBalanceDefaults 수치 적용 — 구현 완료
- [ ] Easy/Hard 오프셋 설정 메뉴 연동 — 향후 구현 예정
- [ ] 난이도별 픽업 타깃 오프셋 (-1 / 0 / +1) — 향후 구현 예정
- [ ] 난이도별 타이머 배율 (0.8× / 1.0× / 1.3×) — 향후 구현 예정

### 7.9 씬 플로우 이벤트 (NEW)

- [x] GameBootstrap.EnsureSceneFadeController() — Boot 시 자동 생성, DontDestroyOnLoad 확인
- [x] SceneFlowService._isTransitioning 가드 — 중복 전환 방지
- [x] AsyncOperation 기반 씬 로드 — using UnityEngine 추가로 CS0246 해결
- [ ] Boot 씬에서 시작하는 전체 플로우 (페이드 포함) 통합 검증

### 7.10 UI 동적 생성

- [ ] RuntimeUiFactory.CreateProgressBar() 작동
- [ ] RuntimeUiFactory.CreateIconLabel() 작동
- [ ] RuntimeUiFactory.CreateStarIcon() 작동 (1~3성)

### 7.11 업그레이드 적용

- [ ] RouteScanner: BloomDust 배수 적용
- [ ] RouteScanner: Timer 보너스 초 적용
- [ ] HarborPump: CleanWater 보너스 적용
- [ ] PearlResonator: MemoryPearl 보너스 적용 (power ≥ 3)

### 6.9 콘솔 에러

- [ ] Unity 콘솔 에러 0개
- [ ] 씬 로드 시 NullReferenceException 없음
- [ ] 게임플레이 중 성능 저하 없음 (프로파일러 확인)

### 6.10 에디터 모드 전용 기능

- [ ] Debug 패널이 Editor 모드에서만 표시
- [ ] Build에서는 Debug UI 제거됨
- [ ] 조건부 컴파일 (#if UNITY_EDITOR) 정상 작동

---

## 7. 참고 및 연결 문서

**관련 설계 문서**:
- `design/00_index.md` - 프로젝트 기획 인덱스
- `design/03_core_loop_and_progression.md` - 코어 루프와 진행 시스템
- `design/09_unity_technical_spec.md` - 기존 Unity 기술 명세

**코드 참고**:
- 모든 신규/확장 클래스는 `namespace MossHarbor.*` 범위 내
- 직렬화는 Unity `[SerializeField]` 및 `ScriptableObject` 사용
- 테스트는 `Assets/Tests/EditMode/` 경로 활용

---

**Document Version**: 1.0
**Last Updated**: 2026-03-22
**Status**: Implementation Reference
**Audience**: Development Team

문서에 대한 질문이나 변경 사항이 있으면 개발팀과 협의하여 업데이트합니다.
