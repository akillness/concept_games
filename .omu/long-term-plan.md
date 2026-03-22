# Moss Harbor Long-Term Plan

## Vision
- Moss Harbor를 평면 수집형 프로토타입에서 지형 활용, 공간 해석, 동적 방해요소, 카메라 연출이 결합된 코지 액션 어드벤처로 확장한다.
- 플레이어가 지형과 기믹을 읽고 활용해야 하는 레벨디자인을 구축한다.
- QA/summary/docs가 항상 현재 구현과 같은 상태를 유지하도록 OMU 문서 흐름을 기준선으로 삼는다.

## Pillars
- 공간성: 좁은 평면 대신 넓은 루트, 우회, 상승 동선, 고저차를 제공한다.
- 역동성: 단순 수집이 아니라 타이밍, 회피, 위치 선정이 필요한 방해요소를 추가한다.
- 해답 연출: 목표를 해결했을 때 카메라가 플레이어의 이해를 보상하는 짧은 연출을 제공한다.
- 검증 가능성: 새 기믹은 Unity MCP + EditMode/PlayMode 검증으로 반복 확인 가능해야 한다.

## Current Baseline
- 플레이 루프: Boot -> Hub -> Expedition_Runtime -> Results -> Hub
- 최근 QA 게이트: EditMode 53/53, Expedition runtime layout/object/camera director 확인
- 최근 시스템: SeedPod telemetry + ratio experiment harness, boundary externalization, UV guardrail
- 최근 레벨 업그레이드: 넓은 ground, side lane, elevated deck, beacon platform, boost/sweep gimmick

## Long-Term Tracks
- 레벨디자인 확장: 넓은 지형, 루프형 동선, 상승 경로, 지구별 기믹
- 카메라 언어: 상황별 follow, reveal, shoulder/hero shot, beacon reveal
- 게임성 강화: 목적성 있는 장애물, 위험/보상 루트, objective-ready 이후 공간 재해석
- 전수 QA: 체크리스트 기반 플레이 시나리오 자동화 확대
