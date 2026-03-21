# Goal

`Project Moss Harbor`의 텍스처 가시성 문제를 “텍스처 파일”이 아니라 “씬 라이팅/반사/프로브/맵 가독성” 문제로 재정의하고, 허브와 원정 씬이 스타일라이즈드 자산을 제대로 보여주도록 Built-in 기준의 라이팅 인프라를 추가한다.

# Implementation Steps

1. `survey` 결과와 로컬 진단을 기준으로 원인을 고정한다.
   - 1차 원인: Built-in 프로젝트 안의 URP 계열 머티리얼 흔적
   - 2차 원인: reflection/light probe와 bake 데이터 부재
2. 공통 라이팅 전략을 확정한다.
   - Directional Light 재구성
   - ambient / fog / skybox 기준 정리
   - Reflection Probe / Light Probe 계층 추가
3. 허브 씬과 원정 씬의 콘셉트를 분리해 적용한다.
   - 허브: 따뜻한 복원 조명, 부두/길/랜턴 강조
   - 원정: 차가운 안개, 진흙/오염 패치, 신호광 강조
4. 캐릭터 표시 품질을 보강한다.
   - Leopard Animator는 유지
   - 동적 캐릭터가 probe/ambient를 받을 수 있게 경로 추가
   - Built-in에서 안정적으로 보이는 머티리얼/셰이더 조합으로 정리
5. 텍스처 import/필터링은 2차 조정 대상으로 남기고, 필요한 자산만 선별 조정한다.
6. 문서와 상태 파일을 갱신하고, 열린 Unity에서 후속 검증 체크리스트를 남긴다.

# Risks

- 현재 Codex 세션에서 Unity MCP 도구가 직접 노출되지 않아, Reflection Probe / Light Probe / Scene bake를 전부 자동화하지 못할 수 있다.
- 같은 프로젝트를 여는 Unity 인스턴스가 이미 살아 있어 배치 검증은 계속 막힌다.
- 씬 YAML 직접 편집은 가능하지만, Unity가 다시 저장하면서 구조를 재정렬할 수 있다.
- Probe와 lighting data는 실제 Unity 에디터에서 bake/refresh하지 않으면 플레이 상태가 달라질 수 있다.
- URP 흔적 머티리얼을 잘못 건드리면 현재 보이는 프리팹 전체가 동시에 깨질 수 있다.

# Completion Criteria

- 허브/원정 씬의 Directional Light와 RenderSettings 전략이 명확히 분리된다.
- 캐릭터와 주요 환경 프리팹이 씬 광원 컨텍스트에서 더 이상 “텍스처가 안 보이는” 상태가 아니게 된다.
- Reflection Probe / Light Probe 또는 그에 준하는 보완 계층이 프로젝트에 추가된다.
- `art/`와 `.survey/` 문서에 원인과 해결 근거가 남는다.
- 열린 Unity 에디터에서 확인할 체크리스트가 구체적으로 정리된다.
