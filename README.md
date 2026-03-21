# Project Moss Harbor

[![Unity](https://img.shields.io/badge/Unity-2022.3.32f1-000000?style=flat-square&logo=unity)](https://unity.com/releases/editor/whats-new/2022.3.32)
[![Status](https://img.shields.io/badge/Status-Prototype-2f7d6b?style=flat-square)](design/00_index.md)
[![Platform](https://img.shields.io/badge/Platform-PC%20%2F%20Steam-4b8bbe?style=flat-square)](design/01_high_concept.md)
[![Last Commit](https://img.shields.io/github/last-commit/akillness/concept_games?style=flat-square)](https://github.com/akillness/concept_games/commits/main)

`Project Moss Harbor`는 오염된 부유 항구를 정화하고 복원하는 탑다운 3D 코지 어드벤처 프로토타입입니다. 짧은 원정에서 자원을 회수하고, 허브로 돌아와 지구를 복원하며, 6개 지구를 순차적으로 되살리는 8시간 분량의 메타 진행을 목표로 설계했습니다.

## Gameplay Preview

![Project Moss Harbor gameplay preview](media/moss-harbor-gameplay.gif)

- MP4 다운로드: [media/moss-harbor-gameplay.mp4](media/moss-harbor-gameplay.mp4)
- 대표 이미지: [media/moss-harbor-gameplay-poster.png](media/moss-harbor-gameplay-poster.png)

## Core Loop

1. 허브에서 목표와 해금 상태를 확인합니다.
2. 원정을 시작해 BloomDust, Scrap, CleanWater, MemoryPearl을 모읍니다.
3. 지구별 목표를 완료하고 Results 화면으로 귀환합니다.
4. 허브 업그레이드와 복원 상태를 갱신하며 다음 지구를 엽니다.

현재 프로토타입은 `Boot -> Hub -> Expedition_Runtime -> Results -> Hub` 흐름을 플레이할 수 있습니다.

## Current Prototype Features

- 지구별 `DistrictContentBundle` 기반 허브/원정/정산 UI
- 원정 보상과 허브 복원 상태가 연결되는 저장 시스템
- 지구 선택, 업그레이드, 스타 해금, 튜토리얼 상태 관리
- ScriptableObject 중심 데이터 구조와 EditMode 테스트
- 구현용 상세 기획 문서 패키지 포함

## Controls

- 이동: `WASD`
- UI 조작: 마우스 클릭
- 허브: `Start Expedition`, 지구 선택, 업그레이드 버튼
- 원정: 픽업 수집 후 `Complete Run` 또는 `Fail Run`

## Open In Unity

1. Unity Hub에서 `concept_game` 폴더를 엽니다.
2. Unity Editor 버전 `2022.3.32f1`을 사용합니다.
3. 시작 씬은 `Assets/Scenes/Boot.unity`입니다.
4. Play를 누르면 허브에서 런타임 UI가 자동 생성됩니다.

## Project Structure

```text
concept_game/
  Assets/
    Scenes/
    Scripts/
    Resources/ScriptableObjects/
    Tests/EditMode/
  Packages/
  ProjectSettings/
design/
  00_index.md
  01_high_concept.md
  ...
media/
  moss-harbor-gameplay.gif
  moss-harbor-gameplay.mp4
  moss-harbor-gameplay-poster.png
```

## Design Docs

- 기획 인덱스: [design/00_index.md](design/00_index.md)
- 하이 콘셉트: [design/01_high_concept.md](design/01_high_concept.md)
- 코어 루프: [design/03_core_loop_and_progression.md](design/03_core_loop_and_progression.md)
- Unity 기술 스펙: [design/09_unity_technical_spec.md](design/09_unity_technical_spec.md)

## Notes

- 루트 `.gitignore`는 Unity `Library`, `Temp`, `Logs`, 생성된 `.csproj`/`.sln` 파일을 제외하도록 구성했습니다.
- 이 저장소는 구현용 프로토타입과 기획 문서를 함께 관리합니다.
