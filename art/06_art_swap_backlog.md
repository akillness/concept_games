# Art Swap Backlog

**Method**: BMAD-GDS lightweight production backlog  
**Date**: 2026-03-21  
**Status**: Ready

## Objective

`Project Moss Harbor`의 GUI, 맵, 캐릭터 교체 작업을 실제 실행 가능한 단위로 분해한다. 현재 보유 자산과 신규 확보가 필요한 자산을 분리해서 후속 작업의 혼선을 줄인다.

## Ready Now

### Story A1. GUI Pack Integration

- Status: Runtime hookup completed, pending Unity visual tune

- Why: 현재 UI 리소스는 프로젝트 구조에 편입됐지만 실제 HUD 토큰과 버튼 스타일 통일이 필요하다.
- What:
  - `Assets/Art/UI/Casual UI Pack` 기준으로 버튼, 패널, 배경, 진행바 후보 정리
  - 허브 HUD와 결과 화면에 적용할 스타일 토큰 정의
  - TMP 폰트/컬러/스프라이트 매핑 문서화
- Acceptance:
  - 허브 HUD용 UI 토큰 표가 문서에 정리되어 있다
  - 결과 화면용 스타일 후보가 분리되어 있다

### Story A2. Environment Pack Integration

- Status: Runtime hookup completed, pending Unity placement tune

- Why: `Slavic World Free`는 실제 허브/원정 맵용 3D 환경 자산으로 활용 가능하다.
- What:
  - 허브용 구조물, 식생, 길, 장식 후보 분류
  - 원정 지구용 오염 전/후 머티리얼 스왑 전략 정의
  - 공용 환경 라이브러리 기준 정리
- Acceptance:
  - 허브/원정 공용 소품 목록이 정리되어 있다
  - 오염 상태 표현 규칙이 문서에 있다

### Story A3. Unity Verification Pass

- Why: 자산 이동 후 실제 씬/프리팹 참조 무결성 확인이 필요하다.
- What:
  - `Boot`, `Hub`, `Expedition_Runtime`, `Results` 씬 점검
  - Missing Sprite, Missing Prefab, Missing Material 확인
  - `unity-mcp` 기반 점검 실행
- Acceptance:
  - 각 씬의 에러 여부가 기록되어 있다
  - 깨진 참조가 있으면 파일 경로와 대상이 기록되어 있다

## Character Update Status

### Story B1. Final Character Replacement

- Status: Runtime hookup completed, pending Unity scene usage verification

- Why: `Cute Birds`를 대체하고 3D 캐릭터 팩을 활성화해야 했다.
- What:
  - `StylizedCharacterPack`를 `Assets/Art/Characters`로 편입
  - `Cute Birds`를 프로젝트 밖으로 격리
  - `StylizedCharacterPack/Demo`를 프로젝트 밖으로 격리
- Acceptance:
  - 3D 캐릭터 자산이 `Assets/Art/Characters/StylizedCharacterPack` 아래 편입되어 있다
  - 선택 캐릭터 프리팹이 `Assets/Art/Resources/Art/Characters/player_avatar.prefab`로 승격되어 있다
  - `PlayerController`가 런타임에서 위 캐릭터 비주얼을 부착한다
  - `Cute Birds`가 활성 프로젝트 트리에서 제거되어 있다
  - 남은 검증은 Unity scene/prefab 사용 확인뿐이다

## Recommended Order

1. A1 GUI Pack Integration
2. A2 Environment Pack Integration
3. A3 Unity Verification Pass
4. B1 Unity scene usage verification

## Fabric Commands

```bash
cat art/06_art_swap_backlog.md | ./scripts/run-fabric-pattern.sh create_summary
```

```bash
cat art/01_asset_replacement_spec.md art/06_art_swap_backlog.md | ./scripts/run-fabric-pattern.sh moss_harbor_art_ops
```
