# Unity MCP and Agent Setup

## Unity MCP

현재 프로젝트 `manifest.json`에는 `com.coplaydev.unity-mcp`가 이미 포함되어 있다. 현재 세션에서 `http://localhost:8080/health` 응답이 확인되어 서버 기동 상태도 확인했다.

또한 배치 모드 검증은 동일 프로젝트를 이미 열고 있는 Unity 인스턴스 때문에 차단되었다. 즉, 현재 확인 가능한 최선의 상태는 "Unity Editor는 열려 있고 MCP 서버도 살아 있음"이다.

현재 세션에서는 Unity MCP 원격 도구 호출이 가능해졌고, `Expedition_Runtime` 씬에 대해 실제 Play Mode 캡처와 콘솔 점검까지 수행했다.

### Required Check

1. Unity Editor에서 `concept_game` 프로젝트를 연다.
2. `Window -> MCP -> Start`를 실행한다.
3. 터미널에서 아래를 확인한다.

```bash
curl http://localhost:8080/health
```

```bash
./scripts/check-unity-project-lock.sh
```

### After Server Start

- 씬별 오브젝트 레이아웃 확인
- `Assets/Art` 이동 후 깨진 참조 확인
- `art/removed_reference_assets/`로 빠진 샘플 리소스가 실제로 불필요한지 최종 확인
- UI Sprite/Prefab 할당 상태 확인
- 필요한 경우 `manage_ui`, `manage_prefabs`, `manage_asset`, `read_console`, `run_tests` 순으로 점검

## Agent Manager

현재 저장소에는 `agents/EMP_0001.md`가 추가되어 최소 에이전트 구성이 준비되어 있다. `agent-manager` 목록 조회에서 `art-director` 에이전트가 인식되는 것까지 확인했다.

### Commands

```bash
python3 /Users/jangyoung/.agents/skills/agent-manager/scripts/main.py list
python3 /Users/jangyoung/.agents/skills/agent-manager/scripts/main.py status art-director
python3 /Users/jangyoung/.agents/skills/agent-manager/scripts/main.py start art-director
```

## Verification Checklist

- Unity Hub에서 `2022.3.32f1`로 프로젝트 오픈
- Console 치명 오류 0개 확인
- `Boot`, `Hub`, `Expedition_Runtime`, `Results` 씬 오픈 확인
- `Assets/Art` 하위 폴더와 리소스 표시 확인
- `Assets/Art/Resources/Art` 하위 선택 자산 표시 확인
- `Assets/Art/Characters/StylizedCharacterPack` 하위 프리팹 표시 확인
- 플레이 시 `RuntimePlayerVisual`, `RuntimeHubArt`, `RuntimeExpeditionArt` 오브젝트 생성 확인
- HUD 버튼/패널이 `Assets/Art/Resources/Art/UI` 스프라이트로 표시되는지 확인
- 이동 입력 시 Leopard 캐릭터가 Idle/Walk 전환, Shift 입력 시 Run 전환하는지 확인
- 허브 씬에서 랜턴 조명, 코블 길, 부두 판자가 보이는지 확인
- 원정 씬에서 진흙 패치, 깨진 길, 더 짙은 fog 톤이 보이는지 확인
- 깨진 Material, Missing Prefab, Missing Sprite 여부 확인
- `Cute Birds` 관련 참조가 남아 있지 않은지 확인

## 2026-03-22 Expedition Runtime Verification

- Unity MCP로 `Expedition_Runtime` 씬 활성 상태 확인
- Play Mode에서 `GeneratedRunContent`, `RuntimePlayerVisual`, `ObjectiveBeacon` 생성 확인
- 플레이 전후 스크린샷:
  - `Assets/Screenshots/expedition_runtime_before_fix.png`
  - `Assets/Screenshots/expedition_runtime_after_fix.png`
  - `Assets/Screenshots/expedition_runtime_final_pass.png`
- 확인된 핵심 원인:
  - 프로젝트는 Built-in RP인데 일부 캐릭터/프롭 머티리얼이 URP 전용 셰이더를 참조
  - 원정 HUD가 상단 시야를 과도하게 점유
  - 원정 맵 오브젝트 배치가 카메라 프레이밍과 맞지 않아 배경이 비어 보임
- 적용한 수정:
  - 런타임 Built-in `Standard` 머티리얼 정규화
  - 원정 맵 오브젝트 재배치와 라이트/안개/배경색 보정
  - 원정 HUD 높이와 불투명도 축소
- 잔여 콘솔 메시지:
  - `Ignoring depth surface load action as it is memoryless`
  - `Ignoring depth surface store action as it is memoryless`

## 2026-03-22 Hub / Results Verification

- `Hub` Play Mode 스크린샷:
  - `Assets/Screenshots/hub_runtime_pass1.png`
  - `Assets/Screenshots/hub_runtime_final2.png`
- `Results` Play Mode 스크린샷:
  - `Assets/Screenshots/results_runtime_pass1.png`
  - `Assets/Screenshots/results_runtime_final2.png`
- 적용한 추가 수정:
  - `RuntimeUiFactory` 버튼 배경을 더 어둡게 조정하고 라벨을 흰색으로 통일
  - `HubHudController` 패널 크기, 버튼 위치, 폰트 크기를 축소해 맵 점유율을 완화
  - `ResultsHudController`의 `Next Action`을 본문 위 별도 영역으로 분리하고 결과 본문 중복 라인을 제거
- 현재 판단:
  - `Hub`, `Expedition_Runtime`, `Results` 모두 플레이 가능한 상태
  - 자주색 머티리얼 깨짐과 치명 콘솔 에러는 재현되지 않음
  - 남은 콘솔 2건은 Unity MCP 캡처 경로에서 반복되는 비차단 메시지로 분류
