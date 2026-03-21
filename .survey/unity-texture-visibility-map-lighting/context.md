# Context: unity-texture-visibility-map-lighting

## Workflow Context

현재 프로젝트는 Unity 2022.3 Built-in Render Pipeline 기반이며, 최근 `Assets/Art/Resources` 경로를 통해 실제 캐릭터/환경/GUI 자산을 런타임에 붙였다. 하지만 로컬 조사 결과, 문제는 단순 광량 부족 하나가 아니다. `Hub`와 `Expedition_Runtime` 모두 기본 RenderSettings와 단일 Directional Light 위주로 유지되고 있고, 반사/프로브/베이크 컨텍스트가 부족하다. 여기에 일부 핵심 환경/캐릭터 머티리얼이 Built-in 프로젝트 안에서 URP 계열 shader 자산 흔적을 유지하고 있다. 이 조합에서는 텍스처가 존재해도 화면에서는 `flat`, `grey`, `too dark`처럼 보이기 쉽다.

Unity 공식 문서도 Built-in에서 씬의 전체 밝기와 질감 읽힘은 개별 텍스처보다 `Lighting Settings`, `ambient source`, `environment reflections`, `Light Probes`, `Reflection Probes` 조합으로 결정된다고 설명한다.

출처:
- https://docs.unity3d.com/es/2019.4/Manual/lighting-window.html
- https://docs.unity3d.com/ja/6000.0/Manual/lighting-ambient-light.html
- https://docs.unity3d.com/cn/2023.2/Manual/UsingReflectionProbes.html
- https://docs.unity3d.com/cn/2018.4/Manual/LightProbes-MovingObjects.html

## Affected Users

| Role | Responsibility | Skill Level |
|------|----------------|-------------|
| 게임플레이 개발자 | 런타임에서 프리팹/씬을 조립하고 화면이 읽히는 상태를 유지 | 중급 |
| 테크니컬 아티스트 | 재질, 라이트, 프로브, fog, 베이크 파이프라인 정합성 관리 | 중급-고급 |
| 아트 디렉터 | 코지 판타지/항구/복원된 빛 톤을 유지하면서 플레이 가독성 확보 | 중급 |
| Unity 초중급 사용자 | 샘플 씬 없이 새 씬을 구성하고 왜 어두운지 추적 | 초급-중급 |

## Current Workarounds

1. Lighting Settings에서 ambient source와 intensity를 올린다.
한계: 반사와 동적 오브젝트 조명은 여전히 부족할 수 있다.

2. Directional Light intensity만 높인다.
한계: 화면 전체가 하얗게 뜨거나 실루엣만 살고 재질 정보는 여전히 죽는다.

3. Fog를 넣어 분위기를 맞춘다.
한계: 광원/반사가 없으면 텍스처 읽힘 문제를 해결하지 못하고 오히려 더 뿌옇게 만들 수 있다.

4. 텍스처 import 설정만 손본다.
한계: 재질이 받을 광원과 반사가 없으면 import 최적화만으로는 해결이 안 된다.

5. 런타임 스크립트로 fog/ambient만 덮어쓴다.
한계: Reflection Probe, Light Probe, bake 데이터가 없으면 플레이 전/후 일관성이 무너진다.

## Adjacent Problems

- 새 씬에서 기본 Skybox/ambient가 샘플 씬보다 훨씬 차갑고 평평하게 느껴진다.
- 움직이는 캐릭터는 lightmap bounced light를 받지 못하므로 더 쉽게 회색으로 보인다.
- Reflection Probe가 없으면 metallic/smoothness 기반 재질이 거의 정보를 잃는다.
- Built-in에서는 post-processing이 기본 제공이 아니기 때문에 색 대비/분위기 조정이 조명 단계에 과도하게 몰린다.
- 런타임에서 RenderSettings를 바꾸면 에디터 정적 씬 상태와 플레이 상태가 달라 디버깅이 더 어려워진다.

## User Voices

> 새 Scene가 샘플보다 지나치게 어둡고, light/probe/generate lighting 점검이 먼저 필요하다는 보고
출처: https://stackoverflow.com/questions/60216572/unity-fresh-scene-is-too-dark

> 재질을 붙였는데 보이지 않거나 invisible처럼 느껴질 때 shader/lighting context를 먼저 확인해야 한다는 사례
출처: https://stackoverflow.com/questions/57278157/adding-material-to-gameobject-in-unity-is-making-it-invisible

> 스타일라이즈드 환경은 강한 대비보다 색조 통일, 길/지면과 오브젝트 분리, fog와 차가운 휴식톤이 읽힘에 중요하다는 피드백
출처: https://johanhandin.com/stylized-environment

## Current Project Findings

- `Hub.unity`와 `Expedition_Runtime.unity`의 RenderSettings는 fog off, 기본 skybox, 낮은 ambient 계열에서 시작한다.
- 두 씬의 Directional Light는 강도 1, 그림자 off, 회전 0/0/0 상태다.
- `RuntimeArtDirector`는 플레이 시 fog/ambient를 덮어쓰지만 Reflection Probe, Light Probe, bake 컨텍스트는 제공하지 않는다.
- `player_avatar.prefab`은 Leopard 모델과 `MediumController`를 연결하고 있고, Leopard 재질은 albedo/normal/metallic/AO 텍스처를 올바르게 참조한다.
- `Leopard_Material.mat`과 `EA03_LPNP-Slavika_Colorsheet_v1.mat`는 같은 shader guid를 공유하고 있고, 프로젝트는 Built-in RP인데 `com.unity.render-pipelines.universal` 패키지는 없다.
- 런타임 환경 프리팹 상당수가 동일한 공통 환경 머티리얼 하나를 공유하고 있으며, 이 머티리얼은 environment/specular reflection이 꺼져 있다.
- 따라서 현재 문제는 텍스처 누락보다 `Built-in/URP 머티리얼 불일치 + scene lighting context 부족`의 결합으로 보는 것이 타당하다.

로컬 근거 파일:
- `concept_game/Assets/Scenes/Hub.unity`
- `concept_game/Assets/Scenes/Expedition_Runtime.unity`
- `concept_game/Assets/Scripts/Art/RuntimeArtDirector.cs`
- `concept_game/Assets/Art/Resources/Art/Characters/player_avatar.prefab`
- `concept_game/Assets/Art/Characters/StylizedCharacterPack/Materials/Characters/Leopard_Material.mat`
- `concept_game/Assets/Art/Environment/Slavic World Free/Material/EA03_LPNP-Slavika_Colorsheet_v1.mat`
- `concept_game/ProjectSettings/GraphicsSettings.asset`
