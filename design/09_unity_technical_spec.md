# 09. Unity Technical Spec

## 프로젝트 기준

- Unity: 2022.3.32f1
- Render Pipeline: Built-in
- UI: uGUI + TextMeshPro
- 현재 패키지 상태: 기본 패키지 + `com.coplaydev.unity-mcp`

## 씬 구성

| Scene | 역할 |
|-------|------|
| Boot | 서비스 초기화, 저장 로드 |
| MainMenu | 시작 메뉴 |
| Hub | 허브 플레이 |
| Expedition_Runtime | 원정 플레이 공통 런타임 |
| Results | 원정 정산 |

## 폴더 구조 제안

```text
Assets/
  Scripts/
    Core/
    Gameplay/
    Hub/
    Expedition/
    UI/
    Data/
  ScriptableObjects/
  Prefabs/
    Player/
    Enemies/
    Interactables/
    UI/
  Art/
  Audio/
  Scenes/
```

## 주요 서비스

### Core

- `GameBootstrap`
- `GameStateService`
- `SaveService`
- `SceneFlowService`
- `AudioService`

### Hub

- `HubManager`
- `HubConstructionService`
- `NpcScheduleService`
- `DecorationPlacementService`
- `QuestBoardService`

### Expedition

- `ExpeditionDirector`
- `ObjectiveService`
- `CorruptionGrid`
- `CorruptionNodeController`
- `ResourceDropService`
- `ThreatDirector`
- `ReturnGateController`

### Player

- `PlayerController`
- `ToolController`
- `PlayerStats`
- `ModuleLoadout`

## ScriptableObject 데이터 정의

필수 데이터 에셋:

- `DistrictDef`
- `SectorDef`
- `ObjectiveDef`
- `ToolDef`
- `UpgradeDef`
- `ResourceDef`
- `NpcDef`
- `QuestDef`
- `DecorationDef`
- `AudioCueDef`

## 저장 데이터 스키마

```json
{
  "profileId": "slot-0",
  "unlockedDistricts": ["tutorial", "dock"],
  "districtStars": {
    "dock_sector_01": 2
  },
  "resources": {
    "bloomDust": 120,
    "scrap": 12,
    "seedPod": 8,
    "cleanWater": 3,
    "memoryPearl": 1
  },
  "hubState": {
    "dock": 2,
    "market": 1
  },
  "toolLevels": {
    "vacuum": 2,
    "sprayer": 1
  },
  "placedDecorations": []
}
```

## 런타임 상태 흐름

```text
Boot
  -> MainMenu
  -> Hub
  -> Loadout Popup
  -> Expedition_Runtime
  -> Results
  -> Hub
```

## 구현 우선순위

### Vertical Slice

1. 플레이어 이동
2. 정화 타일
3. 오염 노드
4. 자원 드랍
5. 허브 복원 1개 구역
6. 원정 -> 귀환 -> 복원 루프

### Production Alpha

1. 2개 지구
2. 주민 퀘스트
3. 업그레이드
4. 장식 슬롯 배치

### Beta

1. 6개 지구 완료
2. 엔딩
3. 별점/리플레이
4. 폴리시

## 기술적 비목표

- 멀티플레이
- 완전 자유 배치 건축
- 네트워크 저장
- 실시간 대형 AI 군집
- 복잡한 인벤토리 그리드
