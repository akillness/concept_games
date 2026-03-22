# Runtime And Technical Baseline

## Scene Flow

- 시작 씬: `Assets/Scenes/Boot.unity`
- 런타임 플로우: `Boot -> Hub -> Expedition_Runtime -> Results -> Hub`
- 전환 제어:
  - `SceneFlowService`
  - `SceneFadeController`

## Key Runtime Systems

| 영역 | 현재 기준 |
|------|-----------|
| Hub | district 선택, 난이도 순환, 업그레이드 구매, Bio Press 사용 |
| Expedition | district data 기반 목표/타이머/픽업/비콘 생성 |
| Results | 별점 계산, 자원 보상, 허브 복귀 |
| Save | 자원, 별점, 지구 선택, 업그레이드, 튜토리얼, 허브 복원 상태 저장 |
| Art | runtime player visual, pickup/beacon visuals, district environment theming |

## Data And Telemetry

- `DistrictDef`
  - entry cost
  - timer
  - objective
  - star ratios
  - `BoundaryRecoveryProfile`
- `SaveData.RunSummary`
  - 수집 자원 요약
  - 목표 요약
  - `seedPodDelta`
  - `bioPressUseCount`
  - `bioPressCleanWaterConverted`

## Verified Runtime Objects

- Hub:
  - `RuntimePlayerVisual`
- Expedition:
  - `RuntimePlayerVisual`
  - `ObjectiveBeacon`

## Editor And Validation Tooling

- UV guardrail menu:
  - `Tools/Moss Harbor/Validation/Audit Player UV Guardrail`
- 검증 항목:
  - `Mesh.isReadable`
  - UV 채널 존재 여부
  - UV 길이 정합성

## Unity MCP Plan Hooks

| 목적 | Unity MCP 액션 |
|------|----------------|
| EditMode 회귀 확인 | `run_tests`, `get_test_job` |
| 씬/런타임 오브젝트 확인 | `manage_scene`, `find_gameobjects` |
| 플레이 전환 및 상태 확인 | `manage_editor`, `read_console` |
| 증적 스크린샷 확보 | `manage_camera.screenshot` |
| 프리팹/에셋 범위 확장 점검 | `manage_asset`, `manage_prefabs` |

## Constraints

- 전체 체크리스트 전수 완료 상태는 아니다.
- 경계 파라미터는 외부화만 끝났고 district별 최적값은 아직 미정이다.
- UV guardrail은 플레이어 핵심 자산 기준으로 닫혔고 확장 커버리지가 다음 단계다.
