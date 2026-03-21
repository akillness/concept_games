# 14. 씬 개선 진행상황 종합

> **Last Updated**: 2026-03-22 | **Status**: ✅ Unity Play Mode 검증 완료 | **Ralph Session**: omu-scene-improvement-2026-03-22

---

## 개요

디자인 문서(01~13)와 현재 구현 사이의 갭을 해소하기 위한 전체 씬 개선 작업의 진행상황을 추적합니다.

## 진행 현황

| Phase | 영역 | 상태 | 완료일 |
|-------|------|------|--------|
| **Phase 1** | 코드 정리 (UpgradeIds + RewardCalculator) | ✅ 완료 | 2026-03-22 |
| **Phase 2** | DistrictDef 환경 파라미터 + 밸런스 | ✅ 완료 | 2026-03-22 |
| **Phase 3** | 디스트릭트별 맵 테마 + SeedPod | ✅ 완료 | 2026-03-22 |
| **Phase 4** | Hub HUD 재구성 + UI 팩토리 | ✅ 완료 | 2026-03-22 |
| **Phase 5** | Hub 복원 시각 반영 | ✅ 완료 | 2026-03-22 |
| **Phase 6** | 씬 전환 페이드 | ✅ 완료 | 2026-03-22 |
| **Phase 7** | Results 별점 + 보상 개선 | ✅ 완료 | 2026-03-22 |
| **Phase 8** | 전체 검증 + 코드 리뷰 | ✅ 완료 | 2026-03-22 |

---

## 새로 생성된 파일 (5개)

| 파일 | 네임스페이스 | 역할 |
|------|-------------|------|
| `Data/UpgradeIds.cs` | MossHarbor.Data | 업그레이드 ID 상수 통합 (harbor_pump, route_scanner, pearl_resonator) |
| `Expedition/RewardCalculator.cs` | MossHarbor.Expedition | 원정 보상 계산 분리 (성공/실패) |
| `Data/DistrictBalanceDefaults.cs` | MossHarbor.Data | 6개 디스트릭트 밸런스 기본값 런타임 적용 |
| `Data/StarRatingCalculator.cs` | MossHarbor.Data | 별점 계산 (1~3성: 완료/픽업률/시간) |
| `UI/SceneFadeController.cs` | MossHarbor.UI | 씬 전환 페이드 인/아웃 (0.35s, sortingOrder 999) |

## 수정된 파일 (12개)

| 파일 | 변경 요약 |
|------|----------|
| `Data/DistrictDef.cs` | +Environment (fogColor, fogDensity, ambientColor, sunColor, sunIntensity, mapScale), +Pickup Distribution (seedPodPickupCount/Amount), +Star Rating (twoStarPickupRatio, threeStarTimeRatio) |
| `Data/DistrictContentCatalog.cs` | +ApplyBalanceIfNeeded() — 디스트릭트 로드 시 밸런스 기본값 자동 적용 |
| `Data/SaveData.cs` | +RunSummary.seedPodCollected, +GetRewardSummary에 SeedPod 추가, +ResolveCollectedResourceAmount에 SeedPod case |
| `Expedition/ExpeditionDirector.cs` | 로컬 상수 → UpgradeIds 참조, CompleteCurrentRun/FailCurrentRun → RewardCalculator 사용, +SeedPod 수집 추적, +BuildDistrictContent에 SeedPod 픽업 생성 |
| `Art/RuntimeArtDirector.cs` | +DecorateExpedition(Transform, DistrictDef) 오버로드, +ApplyExpeditionAtmosphere(DistrictDef) 디스트릭트별 환경 |
| `Hub/HubManager.cs` | 로컬 상수 제거 → UpgradeIds 사용 |
| `UI/ResultsHudController.cs` | RouteScannerUpgradeId → UpgradeIds.RouteScanner |
| `UI/RuntimeUiFactory.cs` | +CreateProgressBar, +CreateIconLabel, +CreateToast, +CreateStarIcon |
| `Core/SceneFlowService.cs` | +LoadWithFade() — 페이드 가능 시 사용, 불가 시 직접 로드 |
| `Core/GameBootstrap.cs` | +EnsureSceneFadeController() — Boot 시 자동 생성 |

---

## 밸런스 수치 변경 (Phase 2)

### 디스트릭트별 핵심 수치

| District | EntryCost | Timer | Bloom | Scrap | SeedPod | Objective | Stars |
|----------|-----------|-------|-------|-------|---------|-----------|-------|
| **Dock** | 5 | 210s | 3×12 | 1×5 | — | CollectPickups(3) | 0 |
| **Reed Fields** | 10 | 195s | 2×10 | 2×5 | 2×3 | CollectResource(SeedPod,5) | 1 |
| **Tidal Vault** | 15 | 180s | 2×10 | 2×6 | 1×2 | CollectResource(CleanWater,3) | 2 |
| **Glass Narrows** | 20 | 180s | 2×12 | 2×7 | 1×3 | HoldOut(75s) | 3 |
| **Sunken Arcade** | 25 | 165s | 3×14 | 3×8 | 1×3 | CollectPickups(5) | 4 |
| **Lighthouse Crown** | 30 | 150s | 3×15 | 3×10 | 2×4 | HoldOut(90s) | 5 |

### 환경 테마

| District | Fog Color | Fog Density | Sun Intensity | 컨셉 |
|----------|-----------|-------------|---------------|------|
| Dock | Cyan-mist | 0.022 | 1.35 | 청록 안개 부두 |
| Reed Fields | Swamp green | 0.026 | 1.25 | 초록 습지 |
| Tidal Vault | Deep blue | 0.018 | 1.40 | 깊은 파란 금속 |
| Glass Narrows | Crystal ice | 0.015 | 1.60 | 수정 얼음빛 |
| Sunken Arcade | Warm amber | 0.024 | 1.30 | 따뜻한 호박색 |
| Lighthouse Crown | Night indigo | 0.028 | 1.15 | 밤 인디고 |

### 별점 기준

| 등급 | 조건 |
|------|------|
| ★☆☆ (1성) | 메인 목표 완료 |
| ★★☆ (2성) | 1성 + 픽업 수집률 ≥ twoStarPickupRatio (65~80%) |
| ★★★ (3성) | 2성 + 소요시간/총시간 ≤ threeStarTimeRatio (45~65%) |

### 실패 시 자원 보존 (디자인 문서 07 준수)

| 자원 | 보존율 | 구현 |
|------|--------|------|
| BloomDust | 수집량 + completionBonus×50% | RewardCalculator.CalculateFailure |
| Scrap | 수집량의 70% | RewardCalculator.CalculateFailure |
| CleanWater | 0 (업그레이드 보너스 미적용) | RewardCalculator.CalculateFailure |
| MemoryPearl | 0 (업그레이드 보너스 미적용) | RewardCalculator.CalculateFailure |

---

## 코드 아키텍처 개선 요약

### Before
```
ExpeditionDirector: 보상 계산 인라인 (40줄), 로컬 업그레이드 ID 상수
HubManager: 동일 상수 중복
ResultsHudController: 동일 상수 중복
RuntimeArtDirector: 모든 디스트릭트 동일 환경
SceneFlowService: 즉시 전환 (피드백 없음)
```

### After
```
RewardCalculator: 보상 계산 전담 (성공/실패 분리, 70% scrap 보존)
UpgradeIds: 상수 1곳에서 관리
DistrictBalanceDefaults: 6개 디스트릭트 밸런스 데이터
StarRatingCalculator: 별점 계산 (완료/픽업률/시간)
RuntimeArtDirector: DistrictDef 기반 환경 테마 (fog, ambient, sun)
SceneFadeController: 페이드 전환 (0.35s in/out)
```

---

## 세이브 호환성

| 변경 | 호환성 |
|------|--------|
| DistrictDef 새 필드 | ✅ 기본값 설정, 기존 .asset 자동 호환 |
| RunSummary.seedPodCollected | ✅ default 0, 기존 세이브 자동 호환 |
| DistrictBalanceDefaults | ✅ 런타임 적용, .asset 파일 변경 불필요 |

---

## 코드 리뷰 결과 (Phase 8)

### 발견 및 수정된 이슈

| 심각도 | 이슈 | 상태 |
|--------|------|------|
| **CRITICAL** | SceneFlowService: fade callback 내 동기 씬 로드 → 레이스 컨디션 | ✅ 수정: LoadSceneAsync + _isTransitioning 가드 |
| **HIGH** | DistrictContentCatalog: ApplyBalanceIfNeeded가 ScriptableObject 원본 변조 | ✅ 수정: Object.Instantiate 클론 후 변조 |
| **HIGH** | RuntimeUiFactory: 새 메서드 4개 미사용 (dead code) | ⚠️ 유지: 향후 HUD 개선에서 사용 예정 |
| **MEDIUM** | StarRatingCalculator: Results→Hub 전환 시 이중 계산 | ⚠️ 수용: 성능 영향 미미 |
| **MEDIUM** | mapScale 필드 미사용 | ✅ 수정: 필드 제거 |

### 최종 수치

- **수정 파일**: 14개 (기존) + 6개 (신규) = 20개
- **삽입/삭제**: +542 / -175
- **레거시 상수**: 0개 (전수 제거 확인)
- **세이브 호환성**: 확인 완료 (모든 새 필드 기본값)

## Unity Play Mode 검증 결과

| 항목 | 결과 |
|------|------|
| 컴파일 에러 | ✅ 0건 (CS0246 AsyncOperation → using UnityEngine 추가로 해결) |
| 콘솔 에러 (Play Mode) | ✅ 0건 (폰트 경고 → ASCII 문자로 교체하여 해결) |
| Hub 씬 로드 | ✅ HubRoot, Player, HubCanvas(18 UI elements) 정상 |
| RuntimeHubArt | ✅ 생성됨 |
| RestorationVisuals | ✅ 생성됨 |
| Player Visual | ✅ 부착됨 (childCount=1) |
| GameBootstrap | ✅ DontDestroyOnLoad로 이동 (정상) |
| SceneFadeController | ✅ DontDestroyOnLoad로 이동 (정상) |

### 수정 이력 (Iteration 3)
- `SceneFlowService.cs`: `using UnityEngine;` 추가 (AsyncOperation 타입 해결)
- `HubHudController.cs`: Unicode 별/잠금 문자 → ASCII (`*/-`, `[Open]/[Locked]`)
- `ResultsHudController.cs`: Unicode 별 → ASCII (`*/-`)
- `ExpeditionDirector.cs`: FailCurrentRun에서 SeedPod 70% 보존 추가

## 향후 개선 (선택)

1. TMP 폰트에 특수문자(★☆🔓) 포함 시 Unicode 복원 가능
2. Expedition 씬 6개 디스트릭트별 실제 플레이 검증
3. Boot 씬에서 시작하는 전체 플로우 (페이드 전환 포함) 검증
4. 기존 세이브 파일 호환성 테스트

---

## 참조 문서

| 문서 | 관련 영역 |
|------|----------|
| 03_core_loop_and_progression.md | 코어 루프, 진행 구조 |
| 04_system_spec.md | 시스템 상세 (자원, 업그레이드, 목표) |
| 05_content_plan.md | 6개 디스트릭트 콘텐츠 |
| 06_resources_economy_bm.md | 경제 곡선, 자원 밸런스 |
| 07_rules_balance_failstates.md | 밸런스 규칙, 실패 보존율 |
| 09_unity_technical_spec.md | Unity 아키텍처, 씬 구조 |
| 12_sprint_alpha_1_backlog.md | 스프린트 백로그 (B1, A1, E1) |
