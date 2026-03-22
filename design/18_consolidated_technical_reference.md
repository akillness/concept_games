# 18. Consolidated Technical Reference

> 기획 문서(04, 05, 06, 09)와 구현 코드를 통합한 기술 레퍼런스.
> 각 시스템의 기획 의도, 구현 상태, C# 클래스 매핑을 단일 문서로 제공.

---

## 문서 통합 매핑

| 원본 문서 | 핵심 내용 | 이 문서의 섹션 |
|-----------|-----------|---------------|
| 04_system_spec | 10개 시스템 정의 | 섹션 1–10 |
| 05_content_plan | 지구/허브/콘텐츠 구조 | 섹션 11 |
| 06_resources_economy_bm | 자원 경제 수치 | 섹션 12 |
| 09_unity_technical_spec | Unity 프로젝트 구조 | 섹션 13 |
| (구현 분석) | 실제 C# 코드 현황 | 섹션 14 |

---

## 1. 플레이어 이동/행동 시스템

**기획 (04)**:
- WASD / 좌스틱 이동, 탑다운 시점
- 주 행동: 진공 정화기 (Clean)
- 보조 행동: 물살 분무기 또는 장착 모듈 (Spray)
- 궁극 행동: 환경 과부하 정화 (Overdrive)
- 상호작용: 오브젝트 활성화 / 수거 / 대화 (Interact)
- 상태: Idle, Move, Clean, Spray, Interact, Hurt, Overdrive
- 피격 시 넉백 약함, 제어 상실 0.25초 이하

**구현 상태**: 부분 구현
| 기능 | 상태 | C# 클래스 / 비고 |
|------|------|-----------|
| WASD 이동 | 구현 | `PlayerController` (CharacterController 기반) |
| 스프린트 | 구현 | `PlayerController.sprintMultiplier = 1.6f` |
| 가속/감속 | 구현 | `PlayerController.acceleration / deceleration` |
| 중력 | 구현 | `PlayerController.gravity = -15f` |
| 플레이어 비주얼 | 구현 | `RuntimeArtDirector.AttachPlayerVisual()` — prefab `CharacterPlayerAvatar` |
| 정화기 (주 행동) | 미구현 | — |
| 분무기/모듈 (보조 행동) | 미구현 | — |
| 궁극 행동 (Overdrive) | 미구현 | — |
| 피격/넉백 | 미구현 | — |
| 상태 머신 (Idle/Move/…) | 미구현 | — |

**참고**: `ToolDef` ScriptableObject는 정의되어 있으며 (`ContentPaths.DefaultTool = "ScriptableObjects/Tool_Vacuum"`), `HubManager.RuntimeTool`이 로드하지만 실제 정화 로직은 아직 없음.

---

## 2. 정화 시스템

**기획 (04)**:
- 오염 단위 2계층: `Corruption Tile` (바닥, 정화 진행도), `Corruption Node` (핵심, 주변 타일 재오염)
- 타일: 정화 게이지 100%→회복 상태 전환
- 노드: HP 0→비활성화, 살아 있으면 주변 타일 재오염 확률
- 타일 정화 속도 = 기본 정화력 × 툴 배율 × 희귀 버프
- 노드 재오염 주기 = 기본 6초 − 지구 난이도 보정

**구현 상태**: 미구현
| 기능 | 상태 | 비고 |
|------|------|------|
| CorruptionGrid | 미구현 | 09 스펙에 서비스 이름 명시됨 |
| CorruptionNodeController | 미구현 | 09 스펙에 서비스 이름 명시됨 |
| 타일 정화 게이지 | 미구현 | — |
| 노드 HP / 비활성화 | 미구현 | — |
| 재오염 타이머 | 미구현 | — |

**미구현 원인**: 현재 MVP는 "픽업 수집 → 비콘 활성화" 루프로 핵심 루프를 검증하는 Vertical Slice 단계. 정화/오염 시스템은 Production Alpha 이후 추가 예정.

---

## 3. 오염 확산 시스템

**기획 (04)**:
- 무작정 정화 대신 우선순위 판단을 유도하는 게임플레이 압박 장치
- 특정 노드만 확산 (모든 타일이 확산하는 것이 아님)
- 한 섹터 활성 노드 최대 3개
- 재오염은 방금 닦은 곳을 즉시 뒤엎지 않도록 지연 적용

**구현 상태**: 미구현
| 기능 | 상태 |
|------|------|
| 노드 기반 확산 | 미구현 |
| 섹터당 활성 노드 상한 (3개) | 미구현 |
| 재오염 지연 로직 | 미구현 |
| ThreatDirector | 미구현 (09 스펙에 명시) |

---

## 4. 자원 회수 시스템

**기획 (04)**:
- 5종 자원: Bloom Dust, Scrap, Seed Pod, Clean Water, Memory Pearl
- 타일 정화 시 Bloom Dust 확률 드랍
- 노드 제거 시 Scrap, Seed Pod, Clean Water 드랍
- 숨겨진 오브젝트에서 Memory Pearl 또는 장식 설계도

**구현 상태**: 구현 (픽업 수집 방식으로 단순화)
| 기능 | C# 클래스 |
|------|-----------|
| 픽업 수집 (트리거) | `SimplePickup.OnTriggerEnter()` |
| 픽업 구성 | `SimplePickup.Configure(ResourceType, amount, rotateSpeed)` |
| 시각 피드백 (bob, proximity scale, shrink) | `SimplePickup` 내부 애니메이션 |
| 픽업 비주얼 팩토리 | `RuntimeArtDirector.CreatePickupVisual()` — BloomDust: `PropBloomPickup`, Scrap: `PropScrapPickup` |
| 런타임 자원 추적 | `ExpeditionDirector.Collect(ResourceType, amount)` |
| 보상 계산 (성공) | `RewardCalculator.CalculateSuccess()` |
| 보상 계산 (실패) | `RewardCalculator.CalculateFailure()` |
| 자원 영속 저장 | `SaveService.AddResource()` / `GetResource()` |

**픽업 생성 규칙** (`ExpeditionDirector.BuildDistrictContent()`):
- BloomDust: 큐브 형태, `PrimitiveType.Cube`
- Scrap: 구체, `PrimitiveType.Sphere`
- SeedPod: 캡슐, `PrimitiveType.Capsule`
- 위치: 픽업 수만큼 원 분할 배치 (`pickupSpawnRadius` 기준)

**실패 시 자원 보존율** (`DifficultyConfig.FailResourceRetention()`):
- Bloom Dust: 수집분 전체 + 완수 보너스의 50%
- Scrap: 수집분의 70%
- SeedPod: 난이도별 보존율 적용

---

## 5. 원정 목표 시스템

**기획 (04)**:
- 각 섹터: 메인 목표 타입, 목표 수량, 보조 목표, 시간 압박 여부, 환경 위험
- 메인 목표 완료 시 탈출 게이트 활성화

**구현 상태**: 구현
| 기능 | C# 클래스 |
|------|-----------|
| 목표 유형 | `ExpeditionObjectiveType` enum: `CollectPickups`, `CollectResource`, `HoldOut` |
| 목표 추적 | `ObjectiveService` (생성자: `DistrictDef, holdSecondsOverride`) |
| 지구별 목표 지정 | `DistrictDef.objectiveType`, `.objectiveResourceType`, `.objectiveTargetAmount`, `.objectiveHoldSeconds` |
| 수집 등록 | `ObjectiveService.RegisterCollection(ResourceType, amount)` |
| HoldOut 타이머 | `ObjectiveService.Tick(deltaTime)`, `ElapsedSeconds`, `TargetHoldSeconds` |
| 진행률 텍스트 | `ObjectiveService.GetProgressText()` |
| 목표 달성 판정 | `ObjectiveService.IsComplete` → `ExpeditionDirector.ObjectiveReady` |
| 비콘 비주얼 | `RuntimeArtDirector.CreateObjectiveBeaconVisual()` — prefab `PropObjectiveBeacon` |
| 비콘 활성화 트리거 | `ObjectiveBeacon` (pulse animation when ready) |
| 비콘 테마 색상 | `ObjectiveBeacon.SetTheme(Color)` — `DistrictThemeColor` 적용 |
| 탈출 → 결과 씬 | `ExpeditionDirector.ActivateObjectiveBeacon()` → `SceneFlowService.LoadResults()` |
| 타이머 오버런 | `ExpeditionDirector.Update()`: `_remainingTime <= 0` → `FailCurrentRun()` 또는 `CompleteCurrentRun()` |
| ReturnGateController | 미구현 (09 스펙에 명시) |

**목표 유형별 지구 배치** (현재 구현 기준):
| 지구 | 목표 유형 | 목표 |
|------|-----------|------|
| dock | CollectPickups | 3개 픽업 |
| reed_fields | CollectResource | SeedPod 5개 |
| tidal_vault | CollectResource | CleanWater 3개 |
| glass_narrows | HoldOut | 60초 버티기 |
| sunken_arcade | CollectPickups | 5개 픽업 |
| lighthouse_crown | HoldOut | 75초 버티기 |

---

## 6. 허브 복원 시스템

**기획 (04)**:
- 허브 구역 5개: Dock, Market, Greenhouse, Housing, Vista Garden
- 복원 단계: Locked → Cleared → Functional → Upgraded → Flourishing
- 단계 상승 시: NPC 입주, 기능 해금, 장식 슬롯 증가, 일일 주문 수 증가

**구현 상태**: 부분 구현 (이진 복원 상태로 단순화)
| 기능 | C# 클래스 |
|------|-----------|
| 지구 선택 (이전/다음) | `HubManager.SelectNextDistrict()` / `SelectPreviousDistrict()` |
| 복원 상태 저장 | `SaveData.hubZoneRestorationStates` (`SerializableDictionary<string, bool>`) |
| 복원 상태 조회 | `SaveService.IsHubZoneRestored(zoneId)` |
| 복원 상태 설정 | `SaveService.SetHubZoneRestored(zoneId)` |
| 원정 완료 시 자동 복원 | `HubManager.ProcessCompletedRun()` — `lastRunSummary.completed` 시 해당 zone 복원 |
| 시각적 복원 | `RuntimeArtDirector.DecorateHub(parent, saveService)` → `AddRestorationVisuals()` |
| 지구 해금 | `DistrictDef.requiredStars` vs `SaveService.GetTotalDistrictStars()` |
| HubZoneDef | ScriptableObject, `zoneId`, `displayName`, `defaultRestored` 필드 |

**복원 시각 매핑** (`RuntimeArtDirector.AddRestorationVisuals()`):
| zoneId | 시각 효과 |
|--------|-----------|
| `dock` | 청록 포인트 라이트 (range 7) |
| `reed_fields` | 이끼 패치 + 녹색 라이트 |
| `tidal_vault` | 소형 플랫폼 + 파란 라이트 |
| `glass_narrows` | 크리스탈 라이트 (range 8) |
| `sunken_arcade` | 코블 패스 + 앰버 라이트 |
| `lighthouse_crown` | 대형 비콘 라이트 (range 12) |

**미구현 항목**:
- 복원 단계 5단계 (현재 이진 bool만 존재)
- NPC 입주 / 스케줄 연동
- 장식 슬롯 증가
- `HubConstructionService`

---

## 7. 업그레이드 시스템

**기획 (04)**:
- 트리: Tool Power, Capacity, Cooldown, Movement, Rare Drop Chance, Hub Utility
- 정화 효율 위주, 한 번에 3단계 이상 급격히 강해지지 않도록 구간 제한
- 각 지구 종료 시 새 업그레이드 브랜치 1개 해금

**구현 상태**: 구현 (3종 Hub Utility 업그레이드)
| 업그레이드 | ID 상수 | C# 설치 메서드 | 비용 | 효과 |
|-----------|---------|---------------|------|------|
| Harbor Pump | `UpgradeIds.HarborPump = "harbor_pump"` | `HubManager.TryInstallHarborPump()` | Scrap 15 | 진입 비용 감소 + CleanWater 보너스 |
| Route Scanner | `UpgradeIds.RouteScanner = "route_scanner"` | `HubManager.TryInstallRouteScanner()` | BloomDust 60 | 타이머 보너스초 + BloomDust 수집 배율 |
| Pearl Resonator | `UpgradeIds.PearlResonator = "pearl_resonator"` | `HubManager.TryInstallPearlResonator()` | CleanWater 20 | 고난도 지구(recommendedPower >= 3) 완수 시 MemoryPearl +1 |

**업그레이드 데이터 구조** (`HubUpgradeDef` ScriptableObject):
- `costType` (ResourceType), `costAmount` (int)
- `entryCostReduction` (int) — Harbor Pump 전용
- `cleanWaterBonus` (int) — Harbor Pump 전용
- `timerBonusSeconds` (float) — Route Scanner 전용
- `bloomMultiplier` (float) — Route Scanner 전용 (x1.2 등)
- `memoryPearlBonus` (int) — Pearl Resonator 전용

**업그레이드 레벨 저장**: `SaveData.hubUpgradeLevels` (`SerializableDictionary<string, int>`)
현재 구현은 최대 1레벨. `SaveService.SetHubUpgradeLevel(id, level)`은 더 높은 값만 저장함.

**미구현 항목**:
- Tool Power / Capacity / Cooldown / Movement / Rare Drop Chance 트리
- 다단계 레벨 업그레이드
- 지구 종료 시 새 브랜치 해금 로직

---

## 8. 주민/퀘스트 시스템

**기획 (04)**:
- 주민 5명: 선장, 정원사, 기계공, 상인, 조명 장인
- 퀘스트 종류: 자원 납품, 지구 복원, 생물 구조, 장식 배치, 고유 이벤트

**구현 상태**: 부분 구현 (퀘스트 데이터 및 클레임 저장만 존재)
| 기능 | C# 클래스 |
|------|-----------|
| 퀘스트 데이터 | `QuestDef` ScriptableObject (`questId`, `displayName`, `objectiveText`, `rewardType`, `rewardAmount`, `districtId`) |
| 퀘스트 경로 | `ContentPaths.*Quest` — 지구당 1개 퀘스트 파일 |
| 퀘스트 보상 지급 | `HubManager.ProcessCompletedRun()` → `SaveService.AddResource(quest.rewardType, quest.rewardAmount)` |
| 퀘스트 클레임 저장 | `SaveData.claimedQuests` (`SerializableDictionary<string, bool>`) |
| 퀘스트 클레임 조회 | `SaveService.IsQuestClaimed(questId)` |
| 현재 퀘스트 표시 | `HubHudController` 번들 요약 패널에 quest 이름 및 상태 표시 |

**미구현 항목**:
- NPC 스케줄 서비스 (`NpcScheduleService` — 09 스펙 명시)
- NPC 대화 시스템
- 주민 입주 / 상태 관리
- 서브플롯, 관계 시스템 (05: 스코프 초과 시 우선 제거 대상)

---

## 9. 장식/배치 시스템

**기획 (04)**:
- 완전 자유 배치가 아닌 소형/중형/특수 슬롯 기반 제한 배치
- 이유: 구현 비용 절감, 카메라 가독성 유지, 충돌/내비 문제 감소

**구현 상태**: 미구현
| 기능 | 상태 |
|------|------|
| DecorationDef ScriptableObject | 미구현 |
| DecorationPlacementService | 미구현 (09 스펙에 명시) |
| 슬롯 시스템 (소형/중형/특수) | 미구현 |
| 장식 세트 6종 | 미구현 |
| SaveData.placedDecorations | 미구현 (09 스펙 JSON 스키마에 `[]` 있으나 현재 `SaveData`에 미포함) |

**참고**: `RuntimeArtDirector`가 허브와 원정 씬에 환경 장식 오브젝트를 런타임으로 배치하지만, 이는 플레이어가 조작하는 장식 시스템이 아닌 아트 디렉션 전용 코드임.

---

## 10. 저장/진행 시스템

**기획 (04)**:
- 저장 항목: 해금 지구, 지구별 별점/클리어 상태, 허브 복원 단계, 업그레이드 레벨, 주민 상태, 장식 배치 상태, 인벤토리 자원, 옵션 설정

**구현 상태**: 구현 (핵심 항목 전부 구현)
| 기능 | C# 클래스 |
|------|-----------|
| 세이브 데이터 구조 | `SaveData` (JSON serializable, `[Serializable]`) |
| 저장 서비스 | `SaveService` (파일: `moss_harbor_save.json`, `Application.persistentDataPath`) |
| 자원 인벤토리 | `SaveData.resources` (`SerializableDictionary<ResourceType, int>`) |
| 지구별 별점 | `SaveData.districtStars` (`SerializableDictionary<string, int>`) |
| 허브 업그레이드 레벨 | `SaveData.hubUpgradeLevels` (`SerializableDictionary<string, int>`) |
| 허브 구역 복원 상태 | `SaveData.hubZoneRestorationStates` (`SerializableDictionary<string, bool>`) |
| 퀘스트 클레임 상태 | `SaveData.claimedQuests` (`SerializableDictionary<string, bool>`) |
| 마지막 원정 요약 | `SaveData.lastRunSummary` (`RunSummary`) |
| 선택 지구 인덱스 | `SaveData.selectedDistrictIndex` |
| 현재 씬 | `SaveData.currentScene` |
| 난이도 | `SaveData.selectedDifficulty` (`DifficultyLevel` enum: Easy, Normal, Hard) |
| 튜토리얼 단계 | `SaveData.tutorialStage` (`TutorialStage` enum) |
| 직렬화 방식 | `JsonUtility.ToJson/FromJson` + `SerializableDictionary<TKey, TValue>` (keys/values 병렬 리스트) |
| 저장 시점 | 매 `SaveService.Save()` 호출 (자원 변경, 별점 변경 등 즉시 저장) |
| 초기화 | `SaveService.Initialize()` → `LoadOrCreate()` → `EnsureDataDefaults()` |

**튜토리얼 단계** (`TutorialStage` enum):
| 값 | 설명 |
|----|------|
| `StartFirstExpedition = 0` | 첫 원정 출발 유도 |
| `ReviewFirstResults = 1` | 결과 화면 확인 유도 |
| `InstallFirstUpgrade = 2` | Harbor Pump 설치 유도 |
| `Completed = 3` | 튜토리얼 완료 |

**튜토리얼 규칙** (`TutorialStateRules`):
- 초기 단계 자동 결정: `TutorialStateRules.DetermineInitialStage(SaveData)`
- 의미있는 진행 판단: `TutorialStateRules.HasMeaningfulProgress(SaveData)`
- 단계는 역행 불가 (더 높은 값만 저장)

**난이도 설정** (`DifficultyConfig`):
- `TimerMultiplier(difficulty)`: 타이머 배율
- `EntryCostMultiplier(difficulty)`: 진입 비용 배율
- `FailResourceRetention(difficulty)`: 실패 시 SeedPod 보존율
- `TwoStarPickupRatioOffset(difficulty)`: 2성 픽업 비율 보정
- `ThreeStarTimeRatioOffset(difficulty)`: 3성 시간 비율 보정
- `DisplayName(difficulty)`: UI 표시명

**미구현 항목**:
- 옵션 설정 (`SaveData`에 필드 없음)
- 장식 배치 상태 (09 스펙 JSON의 `placedDecorations`)
- 주민 상태

---

## 11. 콘텐츠 구조 (from 05)

### 지구 → DistrictDef 매핑

| 기획명 | districtId | C# 상수 | ScriptableObject 경로 |
|--------|-----------|---------|----------------------|
| 안개 부두 | `dock` | `ContentPaths.DefaultDistrict` | `ScriptableObjects/District_Dock` |
| 갈대 습지 | `reed_fields` | `ContentPaths.ReedDistrict` | `ScriptableObjects/District_ReedFields` |
| 조수 금고 | `tidal_vault` | `ContentPaths.VaultDistrict` | `ScriptableObjects/District_TidalVault` |
| 유리 해협 | `glass_narrows` | `ContentPaths.NarrowsDistrict` | `ScriptableObjects/District_GlassNarrows` |
| 침수 아케이드 | `sunken_arcade` | `ContentPaths.ArcadeDistrict` | `ScriptableObjects/District_SunkenArcade` |
| 등대 왕관 | `lighthouse_crown` | `ContentPaths.CrownDistrict` | `ScriptableObjects/District_LighthouseCrown` |

지구 로드: `DistrictContentCatalog.LoadByIndex(0~5)` 또는 `LoadByDistrictId(districtId)`.

`DistrictContentBundle`은 `DistrictDef` + `HubZoneDef` + `QuestDef` 세 자산을 묶은 값 타입.
ScriptableObject에 기본값(entryCost=10, timer=180)이 그대로인 경우 `DistrictContentCatalog.ApplyBalanceIfNeeded()`가 클론을 생성하고 `DistrictBalanceDefaults.ApplyIfDefault()`를 적용함. 공유 에셋 변경 방지를 위한 `Object.Instantiate` 클론 패턴.

### 허브 콘텐츠 단계 (05 → 구현 매핑)

| 05 단계 | 허브 구역 | 구현 상태 (`SaveData.hubZoneRestorationStates`) |
|---------|---------|------------------------------------------------|
| 초반 | Dock, Market | `dock` bool, Market은 `SaveData`에 미정의 |
| 중반 | Greenhouse, Housing 추가 | 미구현 구역 |
| 후반 | Vista Garden 추가, 야간 조명 | 미구현 구역 |

현재 구현은 지구 이름(`dock`, `reed_fields` 등)이 곧 허브 구역 ID이므로 05의 Dock/Market/Greenhouse/Housing/Vista Garden 5구역 구조와 1:1 매핑이 아님. 추후 `HubZoneDef`의 `zoneId` 체계를 허브 구역과 지구로 분리 필요.

### 원정당 콘텐츠 구성 (05 기획 vs 현재 구현)

| 요소 | 기획 | 구현 |
|------|------|------|
| 메인 목표 | 1개 (필수) | 구현 (`ObjectiveService`) |
| 서브 목표 | 1~2개 | 미구현 |
| 오염 노드 | 2~3개 | 미구현 |
| 자원 상자 | 4~7개 | 지구별 픽업 개수 (bloom+scrap+seedpod 합계) |
| 희귀 오브젝트 | 0~1개 | 미구현 |
| 구조 생물 | 0~2개 | 미구현 |

---

## 12. 자원 경제 (from 06)

### 자원 → ResourceType 매핑

| 기획 자원명 | C# Enum 값 | 정수 값 | 획득 경로 | 소비처 |
|------------|-----------|--------|-----------|--------|
| Bloom Dust | `ResourceType.BloomDust` | 0 | 픽업 수집, 원정 완수 보너스 | 지구 진입 비용, Route Scanner 비용 |
| Scrap | `ResourceType.Scrap` | 1 | 픽업 수집, 원정 완수 보너스 | Harbor Pump 설치 비용 |
| Seed Pod | `ResourceType.SeedPod` | 2 | 픽업 수집 (reed_fields 이후) | 미래 사용 (06: 식생 복원) |
| Clean Water | `ResourceType.CleanWater` | 3 | Harbor Pump 설치 보너스, tidal_vault 완수 | Pearl Resonator 설치 비용 |
| Memory Pearl | `ResourceType.MemoryPearl` | 4 | Pearl Resonator 활성 시 D3+ 완수 | 미래 사용 (06: 지구 해금, 고급 모듈) |

### 기획(06) vs 구현 수치 비교

| 지구 | 기획 진입 비용 | **구현 진입 비용** | 기획 목표 픽업 | **구현 목표** | 기획 시간 | **구현 시간** | 기획 완수 BloomDust | **구현 완수 BloomDust** | 기획 완수 Scrap | **구현 완수 Scrap** |
|------|------------|----------------|-------------|------------|---------|-------------|------------------|---------------------|---------------|-------------------|
| Sunken Dock | 10 | **8** | 3 | **3** | 180초 | **210초** | 30 + 수집 | **20 + 수집** | 8 + 수집 | **6 + 수집** |
| Reed Fields | 18 | **10** | 5 | **SeedPod 5개** | 220초 | **185초** | 45 + 수집 | **30 + 수집** | 16 + 수집 | **8 + 수집** |
| Tidal Vault | 28 | **15** | 7 | **CleanWater 3개** | 260초 | **180초** | 70 + 수집 | **35 + 수집** | 24 + 수집 | **10 + 수집** |
| Glass Narrows | 36 | **22** | 8 | **HoldOut 60초** | 300초 | **180초** | 90 + 수집 | **40 + 수집** | 30 + 수집 | **12 + 수집** |
| Sunken Arcade | 44 | **28** | 9 | **5개 픽업** | 320초 | **165초** | 110 + 수집 | **50 + 수집** | 36 + 수집 | **15 + 수집** |
| Lighthouse Crown | 54 | **35** | 10 | **HoldOut 75초** | 360초 | **150초** | 140 + 수집 | **60 + 수집** | 44 + 수집 | **20 + 수집** |

> 구현 수치는 `DistrictBalanceDefaults.cs`에 하드코딩된 값 기준. 기획 대비 진입 비용과 완수 보너스가 낮고, 타이머도 전반적으로 짧음 — 정화/오염 시스템이 없는 현재 Vertical Slice에 맞게 조정된 값.

### 허브 업그레이드 수치 (06 기획 vs 구현)

| 업그레이드 | 기획 비용 | 구현 비용 (`HubUpgradeDef.costAmount`) | 기획 효과 | 구현 효과 |
|-----------|---------|--------------------------------------|---------|---------|
| Harbor Pump | Scrap 15 | Scrap 15 (`HarborPumpUpgrade.costAmount`) | 진입 비용 -4, CleanWater +3 | `entryCostReduction`, `cleanWaterBonus` 필드로 ScriptableObject에서 설정 |
| Route Scanner | BloomDust 60 | BloomDust 60 (`RouteScannerUpgrade.costAmount`) | 원정 시간 +25초, BloomDust x1.2 | `timerBonusSeconds`, `bloomMultiplier` 필드 |
| Pearl Resonator | CleanWater 20 | CleanWater 20 (`PearlResonatorUpgrade.costAmount`) | 고난도 완수 시 MemoryPearl +1 | `memoryPearlBonus` 필드, `recommendedPower >= 3` 조건 |

### 경제 곡선 (06 의도 vs 현재 구현 상태)

| 단계 | 기획 의도 | 구현 상태 |
|------|---------|---------|
| 초반 | Bloom Dust 중심, Scrap 첫 병목 | dock: BloomDust 5 진입, Scrap 6 보너스 → Harbor Pump(Scrap 15) 자연 병목 형성 |
| 중반 | SeedPod + CleanWater 균형, 허브 vs 업그레이드 선택 | reed_fields/tidal_vault에서 SeedPod/CleanWater 등장. Pearl Resonator는 CleanWater 20 요구 |
| 후반 | MemoryPearl + 특수 조건 병목 | glass_narrows/lighthouse_crown(recommendedPower 3~4)에서 Pearl Resonator 활성 시 MemoryPearl 획득 가능 |

**BM 정책 (06 원문 유지)**:
- Steam 권장가: USD 12.99 (8시간 완결형)
- 하드 커런시, 스태미나, 로그인 보상, 강제 광고 없음
- 사후 확장: OST, Harbor Decor Pack (코스메틱 DLC), 무료 기념 맵 1개

---

## 13. Unity 프로젝트 구조 (from 09, updated)

### 프로젝트 설정

| 항목 | 값 |
|------|-----|
| Unity 버전 | 2022.3.32f1 |
| Render Pipeline | Built-in (Standard) |
| UI 시스템 | uGUI + TextMeshPro |
| 네임스페이스 루트 | `MossHarbor` |
| 추가 패키지 | `com.coplaydev.unity-mcp` |

### 씬 구조 (현재)

| Scene 이름 | 역할 | C# 상수 |
|-----------|------|---------|
| `Boot` | 서비스 초기화, 저장 로드, Hub로 전환 | `SceneFlowService.BootSceneName` |
| `MainMenu` | 시작 메뉴 (현재 플로우에서 생략 가능) | `SceneFlowService.MainMenuSceneName` |
| `Hub` | 허브 플레이, 업그레이드, 지구 선택 | `SceneFlowService.HubSceneName` |
| `Expedition_Runtime` | 원정 플레이 공통 런타임 | `SceneFlowService.ExpeditionSceneName` |
| `Results` | 원정 결과 및 별점 표시 | `SceneFlowService.ResultsSceneName` |

**씬 전환 흐름**:
```
Boot
  -> (자동) Hub
  -> (StartExpedition) Expedition_Runtime
  -> (ActivateBeacon / 타이머 종료) Results
  -> (ReturnToHub) Hub
```

**페이드 애니메이션** (`SceneFlowService`):
- FadeOut: 0.4초 (`DefaultFadeOutDuration`)
- FadeIn: 0.35초 (`DefaultFadeInDuration`)
- `SceneFadeController` (uGUI Image 기반, `SetOpaque()`, `FadeIn()`, `FadeOut()`)

### 구현된 서비스 목록

| 서비스 | 기획(09) | 구현 | 파일 | 비고 |
|--------|----------|------|------|------|
| `GameBootstrap` | 계획 | 구현 | `Core/GameBootstrap.cs` | 싱글턴, DontDestroyOnLoad |
| `GameStateService` | 계획 | 구현 | `Core/GameStateService.cs` | `GameFlowState` enum, 이벤트 |
| `SaveService` | 계획 | 구현 | `Core/SaveService.cs` | JSON 파일 저장 |
| `SceneFlowService` | 계획 | 구현 | `Core/SceneFlowService.cs` | 페이드 전환 |
| `SceneFadeController` | 미계획 | 구현 | `UI/SceneFadeController.cs` | uGUI 페이드 오버레이 |
| `AudioService` | 계획 | 미구현 | — | — |
| `HubManager` | 계획 | 구현 | `Hub/HubManager.cs` | 업그레이드, 지구 선택, 퀘스트 |
| `HubConstructionService` | 계획 | 미구현 | — | — |
| `NpcScheduleService` | 계획 | 미구현 | — | — |
| `DecorationPlacementService` | 계획 | 미구현 | — | — |
| `QuestBoardService` | 계획 | 미구현 | — | — |
| `ExpeditionDirector` | 계획 | 구현 | `Expedition/ExpeditionDirector.cs` | 픽업 생성, 타이머, 보상 |
| `ObjectiveService` | 계획 | 구현 | `Expedition/ObjectiveService.cs` | 목표 추적 (3종 타입) |
| `CorruptionGrid` | 계획 | 미구현 | — | — |
| `CorruptionNodeController` | 계획 | 미구현 | — | — |
| `ResourceDropService` | 계획 | 미구현 | — | 현재는 `ExpeditionDirector` 내부에서 처리 |
| `ThreatDirector` | 계획 | 미구현 | — | — |
| `ReturnGateController` | 계획 | 미구현 | — | — |
| `PlayerController` | 계획 | 구현 | `Gameplay/PlayerController.cs` (추정) | WASD 이동, 스프린트 |
| `ToolController` | 계획 | 미구현 | — | — |
| `PlayerStats` | 계획 | 미구현 | — | — |
| `ModuleLoadout` | 계획 | 미구현 | — | — |
| `RuntimeArtDirector` | 미계획 | 구현 | `Art/RuntimeArtDirector.cs` | 허브/원정 환경 아트, 머티리얼 |
| `RewardCalculator` | 미계획 | 구현 | `Expedition/RewardCalculator.cs` | 성공/실패 보상 계산 |
| `StarRatingCalculator` | 미계획 | 구현 | `Data/StarRatingCalculator.cs` | 별점 계산 (1~3성) |

### 폴더 구조 (현재)

```text
Assets/
  Scripts/
    Art/
      RuntimeArtDirector.cs
    Core/
      GameBootstrap.cs
      GameStateService.cs
      SaveService.cs
      SceneFlowService.cs
    Data/
      ContentPaths.cs
      DistrictBalanceDefaults.cs
      DistrictContentCatalog.cs
      DistrictDef.cs
      ResourceType.cs
      SaveData.cs
      StarRatingCalculator.cs
      UpgradeIds.cs
      (HubUpgradeDef.cs, HubZoneDef.cs, QuestDef.cs, ToolDef.cs — ScriptableObject 정의)
    Expedition/
      ExpeditionDirector.cs
      ObjectiveService.cs
      RewardCalculator.cs
      (SimplePickup.cs, ObjectiveBeacon.cs)
    Hub/
      HubManager.cs
    UI/
      HubHudController.cs
      ResultsHudController.cs
      ResultsManager.cs
      RuntimeUiFactory.cs
      SceneFadeController.cs
  ScriptableObjects/
    District_Dock, District_ReedFields, …
    Zone_Dock, Zone_ReedFields, …
    Quest_RestoreDock, Quest_ClearReedFields, …
    Upgrade_HarborPump, Upgrade_RouteScanner, Upgrade_PearlResonator
    Tool_Vacuum
  Prefabs/
    (CharacterPlayerAvatar, PropBloomPickup, PropScrapPickup, PropObjectiveBeacon)
    (환경 프리팹: ArtResourcePaths 상수 참조)
```

### ScriptableObject 데이터 정의 (현재 구현 기준)

| SO 클래스 | 용도 | 주요 필드 |
|----------|------|----------|
| `DistrictDef` | 지구 정의 | `districtId`, `displayName`, `expeditionEntryCost`, `runTimerSeconds`, `objectiveType`, `completionBonus*`, `pickup*Count/Amount`, `districtColor`, 환경 설정, 별점 비율 |
| `HubZoneDef` | 허브 구역 정의 | `zoneId`, `districtId`, `displayName`, `defaultRestored` |
| `QuestDef` | 퀘스트 정의 | `questId`, `districtId`, `displayName`, `objectiveText`, `rewardType`, `rewardAmount` |
| `HubUpgradeDef` | 허브 업그레이드 정의 | `costType`, `costAmount`, `entryCostReduction`, `cleanWaterBonus`, `timerBonusSeconds`, `bloomMultiplier`, `memoryPearlBonus` |
| `ToolDef` | 도구 정의 | (필드 확인 필요 — `PlayerController` 미구현 상태) |

---

## 14. 구현 완료율 요약

### 시스템별 완료율

| 카테고리 | 구현 항목 | 미구현 항목 | 완료율 |
|----------|----------|-----------|--------|
| 핵심 서비스 (Bootstrap, State, Save, SceneFlow) | 4 | 0 | 100% |
| 플레이어 이동/행동 | 이동, 스프린트, 가속, 중력, 비주얼 (5) | 정화기, 분무기, 궁극기, 피격, 상태머신 (5) | 50% |
| 전투/정화/오염 | 0 | CorruptionGrid, CorruptionNode, 확산, ThreatDirector (4) | 0% |
| 자원 회수 | 픽업 수집, 트리거, 비주얼, 보상 계산 (4) | ResourceDropService 분리, 노드 드랍 (2) | 67% |
| 원정 목표 | ObjectiveService (3종), 비콘, 타이머 (5) | 서브 목표, ReturnGate (2) | 71% |
| 허브 복원 | 이진 복원 상태, 시각 효과, 지구 해금 (3) | 5단계 단계, NPC 연동, 슬롯 (3) | 50% |
| 업그레이드 | HarborPump, RouteScanner, PearlResonator (3) | Tool/Capacity/Movement 트리 (3+) | 50% |
| 주민/퀘스트 | 퀘스트 데이터, 클레임, 보상 지급 (3) | NPC 스케줄, 대화, 입주 (3) | 50% |
| 장식/배치 | 0 | DecorationDef, 슬롯 시스템, 6세트 (3+) | 0% |
| 저장/진행 | SaveData, SaveService, 튜토리얼, 난이도 (5) | 옵션 설정, 장식 저장 (2) | 71% |
| UI/HUD | HubHud, ExpeditionHud (추정), ResultsHud, SceneFade (4) | LoadoutPopup, 미니맵 (2) | 67% |
| 아트/환경 | RuntimeArtDirector (허브+원정 아트, 머티리얼) (1) | 레벨별 고유 아트 에셋 완성도 | — |

### 개발 단계 진행 현황 (09 기준)

| 단계 | 목표 | 완료 여부 |
|------|------|---------|
| **Vertical Slice** | 이동, 정화 타일, 오염 노드, 자원 드랍, 허브 복원 1구역, 원정→귀환→복원 루프 | 부분 완료 (이동 + 자원 드랍 + 루프 구현, 정화/오염 미구현) |
| **Production Alpha** | 2개 지구, 주민 퀘스트, 업그레이드, 장식 슬롯 | 미착수 |
| **Beta** | 6개 지구 완료, 엔딩, 별점/리플레이, 폴리시 | 미착수 |

### 기술적 비목표 (09 원문 유지)

- 멀티플레이
- 완전 자유 배치 건축
- 네트워크 저장
- 실시간 대형 AI 군집
- 복잡한 인벤토리 그리드
