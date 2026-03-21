# Solution Landscape: unity-texture-visibility-map-lighting

## Solution List

| Name | Approach | Strengths | Weaknesses | Notes |
|------|----------|-----------|------------|-------|
| Built-in 친화 머티리얼 재매핑 | URP 흔적 머티리얼을 Built-in/Standard 기준 머티리얼로 교체 | 가장 직접적인 호환성 복구 | 머티리얼 참조 범위 파악 필요 | 현재 프로젝트 최우선 |
| Directional key light 재구성 | 태양광 각도/색/강도/그림자 조정 | 가장 즉각적 | 반사/간접광 부족은 못 메움 | 거의 모든 자료에서 1순위 |
| Ambient / Skybox 정리 | Lighting Settings의 ambient source, skybox, intensity 조정 | 전체 톤 정리 | 씬별 bake 상태 영향 큼 | Built-in 핵심 |
| Reflection Probe 추가 | 환경 반사와 metallic 재질 복원 | PBR 텍스처 읽힘 개선 | 씬 배치와 bake 필요 | 캐릭터/금속/젖은 재질에 중요 |
| Light Probe 추가 | 동적 캐릭터/픽업의 bounced light 확보 | 이동체 품질 개선 | 배치 손이 감 | 캐릭터 가독성 핵심 |
| Texture import 점검 | filter, mipmap, aniso, normal 지정 수정 | 바닥/길 텍스처 선명도 개선 | 조명 문제는 별도 | 2차 최적화 성격 |
| Fog / composition 조정 | 거리감, foreground/background 분리 | 탑다운 맵 가독성 개선 | 잘못 쓰면 더 뿌옇다 | 분위기와 읽힘 동시 조절 |
| Bake / Generate Lighting 정리 | 씬 lighting data 생성 | 플레이/에디터 일관성 확보 | 에디터 접근 필요 | Built-in에서는 자주 필요 |

## Categories

### 1. Scene Lighting
- Directional Light 방향/강도/색
- Ambient source / intensity
- Skybox / sun source

### 2. Reflections and Probes
- Reflection Probe
- Light Probe
- 대형 동적 오브젝트는 LPPV 고려

### 3. Material and Texture Pipeline
- Built-in vs URP 머티리얼 정합성
- Standard/PBR 재질 유지 여부
- texture import 설정
- normal / metallic / AO 맵 활성 확인

### 4. Readability and Composition
- fog
- 지면과 상호작용 오브젝트 색 대비
- 따뜻한 허브 vs 차가운 원정 톤 분리

### 5. Debug Workflow
- Lighting window
- Generate Lighting
- Frame Debugger
- Scene View lighting debug

## What People Actually Use

실무에서는 보통 아래 순서로 움직인다.

1. Directional Light와 ambient부터 조정
2. Reflection Probe를 한두 개 넣어 PBR 재질이 살아나는지 확인
3. 캐릭터가 여전히 뜨면 Light Probe 추가
4. 그 다음에 fog와 색 대비를 잡아 맵 읽힘을 조정
5. 마지막에 texture import/filter/aniso를 미세 조정

즉 “텍스처가 안 보인다”를 텍스처 임포트부터 파는 경우보다, 씬 광원 컨텍스트를 먼저 세우는 경우가 훨씬 많다.

## Frequency Ranking

1. Built-in 친화 머티리얼 재매핑
2. Lighting Settings / ambient / skybox 조정
3. Reflection Probe 추가
4. Fog와 거리감 조정
5. Generate Lighting / bake 데이터 재생성
6. Texture import settings 점검
7. Light Probe / LPPV 추가

## Key Gaps

- 현재 프로젝트는 런타임 fog/ambient는 추가했지만 Reflection Probe와 Light Probe 계층이 비어 있다.
- 현재 프로젝트가 Built-in RP인데도 핵심 환경/캐릭터 머티리얼 일부가 URP 계열 shader 자산 흔적을 유지하고 있다.
- 현재 씬 광원은 기본 Directional Light 1개 중심이라 캐릭터/환경 PBR 재질이 충분한 정보를 받지 못한다.
- 현재 `RuntimeArtDirector`는 오브젝트 배치에는 강하지만, 씬 라이팅 인프라까지 소유하지 않는다.
- 현재 Codex 세션에서는 Unity MCP tool 노출이 안 돼서, scene bake/probe 생성은 코드나 YAML로 처리하거나 열린 Unity에서 후속 검증이 필요하다.

## Contradictions

- 시장/튜토리얼에서는 “라이트 강도만 높이면 된다”는 인상이 많지만, 실제로는 반사/간접광/프로브가 없으면 재질 정보가 계속 죽는다.
- fog는 분위기 도구로 소개되지만, 가독성 개선용으로는 조명과 색 대비를 먼저 세워야 효과가 난다.
- 텍스처가 선명하지 않다는 체감이 실제로는 import 문제가 아니라 광원 배치 문제인 경우가 많다.
- 이 프로젝트처럼 파이프라인이 Built-in인데 머티리얼은 URP 흔적을 가지면, 라이트를 아무리 만져도 밑단 호환성 문제가 계속 남는다.

## Key Insight

이 프로젝트의 문제는 텍스처 파일 불량보다 `Built-in/URP 머티리얼 불일치 + 씬 라이팅 인프라 부재`에 가깝다. 따라서 개선 우선순위는 `Built-in 친화 머티리얼 재매핑 -> Directional Light 재설계 -> ambient/skybox 정리 -> Reflection Probe/Light Probe 계층 추가 -> fog와 맵 구성 리밸런스 -> texture import 미세조정` 순서가 가장 타당하다.

## Sources

- Unity Lighting Window: https://docs.unity3d.com/es/2019.4/Manual/lighting-window.html
- Unity ambient light: https://docs.unity3d.com/ja/6000.0/Manual/lighting-ambient-light.html
- Unity Reflection Probes: https://docs.unity3d.com/cn/2023.2/Manual/UsingReflectionProbes.html
- Unity Light Probes for moving objects: https://docs.unity3d.com/cn/2018.4/Manual/LightProbes-MovingObjects.html
- Unity Light Probe troubleshooting: https://docs.unity.cn/Manual/light-probes-troubleshooting.html
- Unity Light Probe Proxy Volume: https://docs.unity3d.com/kr/2023.1/Manual/class-LightProbeProxyVolume.html
- Unity Texture import settings: https://docs.unity3d.com/es/2021.1/Manual/class-TextureImporter.html
- Stack Overflow: scene too dark https://stackoverflow.com/questions/60216572/unity-fresh-scene-is-too-dark
- Stack Overflow: material invisible https://stackoverflow.com/questions/57278157/adding-material-to-gameobject-in-unity-is-making-it-invisible
- Stylized environment readability note: https://johanhandin.com/stylized-environment
- Fog for top-down games: https://kvachev.com/blog/posts/fog-for-topdown-games/
