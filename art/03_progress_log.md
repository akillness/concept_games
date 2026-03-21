# Progress Log

## 2026-03-21

### Completed

- `design/00_index.md`, `design/08_art_asset_pipeline.md`, `design/09_unity_technical_spec.md`, `README.md` 기준 확인
- 현재 Unity 프로젝트의 아트 팩 루트 확인
- `unity-mcp`, `fabric`, `agent-manager` 실행 가능 여부 확인
- 아트 교체 사양서와 인벤토리 문서 작성 시작
- `concept_game/Assets/Art/UI/Casual UI Pack`으로 UI 팩 이동
- `concept_game/Assets/Art/Characters/Cute Birds`로 캐릭터 팩 이동
- `concept_game/Assets/Art/Environment/Slavic World Free`로 환경 팩 이동
- `Stylized3DMonster`와 데모/원본/샘플 리소스를 `art/removed_reference_assets/`로 격리 이동
- `fabric-ai` 설치 확인
- Fabric 커스텀 패턴 `moss_harbor_art_ops` 생성
- `scripts/run-fabric-pattern.sh`, `scripts/check-unity-project-lock.sh` 추가
- `agents/EMP_0001.md`로 `agent-manager`용 최소 에이전트 정의 추가
- `agent-manager` 목록 조회로 `art-director` 인식 확인
- Unity 배치 모드 검증 시도
- `Art Swap Backlog` 문서 추가
- `StylizedCharacterPack`를 `Assets/Art/Characters`로 이동
- `Cute Birds`를 교체 완료 상태로 격리
- `StylizedCharacterPack/Demo`를 프로젝트 밖으로 격리
- 선택 자산을 `concept_game/Assets/Art/Resources/Art` 아래로 승격
- `RuntimeUiFactory`가 실제 UI 스프라이트를 로드하도록 갱신
- `PlayerController`가 실제 캐릭터 프리팹 비주얼을 부착하도록 갱신
- `HubManager`, `ExpeditionDirector`가 실제 환경 프리팹을 런타임 장식으로 배치하도록 갱신
- `ExpeditionDirector`가 실제 픽업/비콘 프리팹을 우선 사용하도록 갱신
- `ObjectiveBeacon`를 자식 프리팹 렌더러와 호환되도록 갱신
- `PlayerController`가 `StylizedCharacterPack` Animator 파라미터(`Speed`, `Sprint`, `Grounded`, `VerticalSpeed`)를 실제로 구동하도록 갱신
- Leopard 프리팹의 컨트롤러와 텍스처 참조 연결 상태 확인
- 허브 배경에 플랫폼, 코블 길, 부두 판자, 따뜻한 랜턴 조명을 추가
- 원정 배경에 오염 진흙 패치, 깨진 길, 차가운 신호 조명, 더 짙은 안개 톤을 추가

### Environment Check

- `unity-mcp` health endpoint 정상 응답 확인
- `fabric-ai` 바이너리는 설치됨
- Fabric 기본 vendor/model 미설정
- Unity 프로젝트는 이미 다른 Unity 인스턴스가 열고 있어 배치 검증 차단
- 프로젝트 내부 `agents/` 추가 완료
- Codex MCP 설정에 `unity` 엔트리 추가
- `Assets/Art/Resources` 경로 생성 및 선택 자산 배치 확인
- 셸 기준 Unity 잠금 파일 지속 확인
- 현재 Codex 세션에서는 Unity MCP 서버 health는 확인되지만 MCP tool 목록 자체는 직접 노출되지 않음

### Planned Next

- Unity Editor에서 `Assets/Art` 리임포트 및 `Assets/Art/Resources` 재인덱싱 확인
- `Boot`, `Hub`, `Expedition_Runtime`, `Results` 씬에서 `RuntimePlayerVisual`, `RuntimeHubArt`, `RuntimeExpeditionArt`, 스프라이트 기반 HUD 확인
- 필요 시 `unity-mcp`로 프리팹 스케일/회전/위치 보정
- `/opt/homebrew/opt/fabric-ai/bin/fabric-ai --setup` 실행 후 패턴 실제 호출
- `StylizedCharacterPack` 캐릭터 프리팹과 선택 환경 프리팹이 실제 플레이 흐름에서 정상 표시되는지 최종 확인
- 걷기/대시 입력 시 캐릭터가 Idle/Walk/Run 상태 전환하는지 확인

## 2026-03-22

### Completed

- `survey` 기준으로 텍스처 미표시 문제를 조사하고 `.survey/unity-texture-visibility-map-lighting/` 문서 세트 작성
- `omu` 계획서를 `plan.md`로 작성하고 수동 승인 후 실행 단계로 전환
- Unity MCP로 `Expedition_Runtime` 씬의 실제 플레이 화면과 콘솔을 기준으로 문제를 재현
- 프로젝트가 Built-in Render Pipeline인데 캐릭터/프롭 일부가 URP 전용 셰이더를 참조하던 구조 문제를 확인
- `RuntimeArtDirector`에 런타임 Built-in `Standard` 머티리얼 정규화 로직 추가
- 플레이어, 픽업, 비콘, 환경 프리팹 인스턴스가 플레이 시점에 Built-in 호환 머티리얼로 대체되도록 갱신
- `ObjectiveBeacon`과 픽업 오브젝트가 concave `MeshCollider` trigger 경고 없이 동작하도록 정리
- `Expedition_Runtime` 배경 맵 배치를 중앙 집중형으로 다시 구성하고 지형 색/안개/주광 방향을 보정
- `HudController`를 줄여 원정 HUD가 배경 맵을 과도하게 가리지 않도록 조정
- Unity MCP Play Mode 캡처로 자주색 머티리얼 깨짐이 제거된 것을 확인

### Verification Notes

- `Expedition_Runtime` Play Mode에서 `RuntimePlayerVisual`, `GeneratedRunContent`, `ObjectiveBeacon` 생성 확인
- 원정 씬의 플레이어, 비콘, 픽업이 더 이상 자주색으로 표시되지 않음
- 원정 HUD가 이전 대비 축소되어 배경 가시성이 개선됨
- `Hub` Play Mode에서 `RuntimeHubArt`, `RuntimePlayerVisual`, 축소된 허브 HUD, 공통 버튼 가독성 개선 확인
- `Results` Play Mode에서 텍스트 겹침이 제거되고 `Next Action` / 본문 / 버튼 계층이 분리된 것 확인
- Console 치명 오류는 제거되었고, 현재 잔여 메시지는 `Ignoring depth surface load/store action as it is memoryless` 2건
- 위 depth surface 메시지는 Unity MCP 스크린샷 호출 시점에 반복 재현되며, 현재 플레이 로직 오류나 머티리얼 깨짐과는 직접 연결되지 않는 비차단 항목으로 분류

### Remaining

- 원정 씬은 플레이 가능 상태이나 배경 밀도와 라이팅 품질은 추가 아트 패스 여지 있음
- 허브 배경의 좌하단 대형 프롭과 버튼 배치 관계는 추가 미감 조정 여지 있음
- 잔여 depth surface 메시지가 Unity MCP 캡처 경로 한정인지, 실제 빌드 플레이에서도 발생하는지는 후속 확인 필요
