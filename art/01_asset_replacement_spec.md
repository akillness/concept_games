# Asset Replacement Specification

**Author**: Codex
**Date**: 2026-03-21
**Status**: In Progress

## Overview

이 문서는 `Project Moss Harbor`의 GUI, 맵, 캐릭터 리소스를 현재 프로젝트 구조에 맞게 재편입하고 교체하기 위한 실행 사양이다. 기준은 `design/08_art_asset_pipeline.md`와 `design/09_unity_technical_spec.md`이며, Unity 프로젝트는 `2022.3.32f1`, Built-in Render Pipeline, `uGUI + TMP`를 유지한다.

## Background

현재 `concept_game/Assets`에는 환경 팩, UI 팩, 캐릭터 팩, 데모 씬, 원본 PSD, 문서 파일이 혼재되어 있다. 이 상태는 다음 문제를 만든다.

- 실제 사용 리소스와 참조용 샘플 리소스가 구분되지 않는다.
- Unity 프로젝트 제안 구조인 `Assets/Art`와 맞지 않는다.
- Built-in 기준 프로젝트에 필요 없는 URP/데모 파일이 남아 있다.
- 이후 GUI, 맵, 캐릭터 교체 시 영향 범위를 추적하기 어렵다.

## Goals

- GUI, 맵, 캐릭터 관련 리소스를 `concept_game/Assets/Art` 아래로 재배치한다.
- 현재 디자인 방향과 맞는 리소스만 유지하고 샘플/참조용 리소스는 제거한다.
- 문서 기반으로 어떤 리소스를 유지/삭제했는지 추적 가능하게 만든다.
- Unity MCP와 Fabric 기반 후속 작업 명령 체계를 문서화한다.

## Non-Goals

- 이번 작업에서 신규 상용/외부 에셋을 다운로드하거나 라이선스를 확정하지 않는다.
- Unity Editor 내부에서 수동 씬 편집으로 레이아웃을 재저작하지 않는다.
- 런타임 비주얼의 세밀한 튜닝과 최종 아트 폴리시는 이번 패스 범위에서 제외한다.

## Replacement Targets

### GUI

- 소스 후보: 기존 `Assets/Assets/UI Elements`, `Assets/Assets/Raw and SpriteSheets`
- 목표 위치: `concept_game/Assets/Art/UI/`
- 적용 원칙:
  - `uGUI + TMP` 유지
  - 화이트/블랙/엑스트라 스프라이트는 공통 UI 부품으로 보관
  - 샘플 씬은 유지하지 않는다

### Map

- 소스 후보: 기존 `Assets/EmaceArt/Slavic World Free`
- 목표 위치: `concept_game/Assets/Art/Environment/Slavic World Free`
- 적용 원칙:
  - Built-in 기준 머티리얼, 텍스처, 프리팹만 유지
  - `URP_Support`, 홍보 문서, 샘플 프리뷰 씬, 원본 소스 파일은 제거 우선
  - 환경 소품은 허브/원정 양쪽에서 공용 라이브러리로 사용할 수 있게 유지

### Character

- 활성 소스: `concept_game/Assets/StylizedCharacterPack`
- 활성 목표 위치: `concept_game/Assets/Art/Characters/StylizedCharacterPack`
- 교체된 이전 후보: `Cute Birds` -> `art/removed_reference_assets/characters/`
- 적용 원칙:
  - 3D 캐릭터 프리팹과 모델을 활성 자산으로 유지
  - `Demo` 씬, 데모 스크립트, 데모 UI는 프로젝트 밖으로 격리
  - 캐릭터 프리팹이 참조하는 모델/머티리얼/텍스처는 같은 팩 내부에 유지
  - 실제 런타임 사용 대상 프리팹은 `concept_game/Assets/Art/Resources/Art/Characters`와 `concept_game/Assets/Art/Resources/Art/Props`로 승격해 `Resources.Load` 경로로 고정한다
  - 환경/프랍 하위 폴더는 현재 팩 참조 안정성을 위해 유지하되, 실제 사용 확정 전까지는 캐릭터 보조 자산으로 본다

## Target Folder Structure

```text
concept_game/Assets/Art/
  Characters/
    StylizedCharacterPack/
  Environment/
    Slavic World Free/
  Resources/
    Art/
      Characters/
      Environment/
      Props/
      UI/
  UI/
    Casual UI Pack/
```

## Acceptance Criteria

- `art/` 문서 폴더가 생성되어 작업 기록이 남아 있다.
- `concept_game/Assets/Art` 아래에 GUI, 맵, 캐릭터 리소스가 분류되어 있다.
- 실제 런타임에서 사용할 선택 자산이 `concept_game/Assets/Art/Resources/Art` 아래로 승격되어 있다.
- 런타임 코드가 GUI, 캐릭터, 맵 장식, 픽업, 비콘에 대해 위 리소스를 우선 사용한다.
- 명백한 데모/참조/문서/원본 소스 리소스가 Unity 프로젝트 밖으로 격리 이동되었다.
- Unity MCP와 Fabric 후속 명령이 문서화되었다.
- 삭제/유지 판단이 문서로 남아 있다.

## Risks

- Unity Editor에서 현재 프로젝트가 이미 열려 있어 최종 플레이 검증과 씬 저장 검증은 열린 에디터 세션에서 수행해야 한다.
- `unity-mcp` 서버는 정상 응답하지만, 현재 세션에서 Unity 에디터가 이미 프로젝트를 열고 있어 배치 검증은 차단된다.
- `fabric-ai`는 설치됐지만 기본 vendor/model 설정 전까지 실제 패턴 호출은 불가하다.
- 현재 세션 정책상 직접 삭제 명령이 차단되어, 미사용 리소스는 `art/removed_reference_assets/`에 보관했다.
- `StylizedCharacterPack`와 `Slavic World Free`의 런타임 연결은 코드에 반영됐지만, prefab scale/orientation과 sprite import 상태는 Unity Editor 플레이 테스트 전까지 확정할 수 없다.
