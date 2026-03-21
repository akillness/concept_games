# Asset Inventory and Decisions

**Date**: 2026-03-21
**Status**: Working

## Current Imported Asset Roots

| Current Path | Category | Decision | Reason |
|---|---|---|---|
| `concept_game/Assets/Assets` | UI pack | Moved to `Assets/Art/UI/Casual UI Pack` | GUI 교체 후보. 프로젝트 구조에 맞게 편입 |
| `concept_game/Assets/StylizedCharacterPack` | Character pack | Moved to `Assets/Art/Characters/StylizedCharacterPack` | 3D 캐릭터 프리팹/모델 포함. 현재 활성 캐릭터 리소스 |
| `concept_game/Assets/EmaceArt/Slavic World Free` | Environment pack | Moved to `Assets/Art/Environment/Slavic World Free` | 맵 교체 후보. Built-in 기준 환경 자산으로 적합 |
| `concept_game/Assets/Stylized3DMonster` | Monster demo pack | Quarantined | 현재 게임 톤과 불일치, 사용 흔적 없음, free trial/demo 성격 |

## Safe Removal Targets

### UI Pack

- `concept_game/Assets/Art/UI/Casual UI Pack/Scenes`

### Character Packs

- `art/removed_reference_assets/characters/Cute Birds (replaced by StylizedCharacterPack)`
- `art/removed_reference_assets/characters/StylizedCharacterPack Demo`

### Slavic World Free

- `concept_game/Assets/Art/Environment/Slavic World Free/Thanks and checkout this`
- `concept_game/Assets/Art/Environment/Slavic World Free/SourceFiles`
- `concept_game/Assets/Art/Environment/Slavic World Free/URP_Support`
- `concept_game/Assets/Art/Environment/Slavic World Free/Scene`

### Monster Pack

- `art/removed_reference_assets/root/Stylized3DMonster`로 이동

## Notes

- 현재 imported art pack들은 모두 `git status` 기준 untracked 상태다.
- 코드와 ScriptableObject에서 특정 팩 이름을 직접 참조하는 흔적은 발견되지 않았다.
- 실제 씬/프리팹 참조 검증은 Unity Editor 리임포트 후 한 번 더 필요하다.
- 직접 삭제가 차단되어 제외 리소스는 `art/removed_reference_assets/`로 이동했다.
- `Cute Birds`는 교체 완료 후 격리했다.
- `StylizedCharacterPack`는 3D 캐릭터 자산으로 적합하며 현재 활성 캐릭터 팩이다.
- `StylizedCharacterPack` 내부의 `Demo` 콘텐츠는 활성 프로젝트 트리 밖으로 격리했다.
- 실제 런타임에 사용할 선택 자산은 `concept_game/Assets/Art/Resources/Art` 아래로 승격했다.
- 현재 승격 자산:
  - UI: `button_primary.png`, `panel_round.png`
  - Character: `player_avatar.prefab`
  - Props: `pickup_bloom.prefab`, `pickup_scrap.prefab`, `objective_beacon.prefab`
  - Environment: `hub_island.prefab`, `pier.prefab`, `moss_patch.prefab`, `rock_cluster.prefab`
- 런타임 연결 대상 코드:
  - `RuntimeUiFactory` -> UI 스프라이트
  - `PlayerController` -> 캐릭터 비주얼 프리팹
  - `HubManager` / `ExpeditionDirector` -> 환경 장식
  - `ExpeditionDirector` -> 픽업/비콘 프리팹
