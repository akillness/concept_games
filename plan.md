goal: Expedition 리디자인의 다음 사이클로 route telemetry, camera occlusion fallback, objective-ready 완충을 추가해 실제 플레이 체감과 밸런스 측정 기반을 강화한다.

implementation steps:
1. Expedition/Results 흐름을 읽고 RunSummary에 route 선택, boost pad 사용, objective-ready 시점을 남길 telemetry 필드를 설계한다.
2. SimplePickup/ExpeditionDirector에 route tier 수집과 boost pad 사용 집계를 연결하고, 결과 UI/문서에서 읽을 수 있게 반영한다.
3. ExpeditionCameraDirector에 geometry occlusion fallback을 추가해 상층 구조물과 램프 구간에서 카메라가 가려질 때 자동으로 앞으로 당겨지도록 만든다.
4. objective-ready 직후의 급격한 압박을 줄이기 위해 짧은 grace window 또는 hazard 완충 로직을 추가한다.
5. EditMode/PlayMode 테스트를 추가하고 Unity MCP로 play/stop, console, runtime object를 검증한다.
6. .omu/docs 상태를 최신화하고 결과를 커밋/푸시한다.

risks:
- 카메라 occlusion 보정이 과하면 연출용 cue 구도가 깨질 수 있다.
- telemetry 필드 추가가 저장/결과 UI 포맷과 어긋날 수 있다.
- objective-ready grace가 과하면 HoldOut 긴장감이 사라질 수 있다.

completion criteria:
- route/boost/objective-ready telemetry가 RunSummary에 기록되고 Results/QA 문서에서 확인 가능하다.
- 상층 구조물 근처에서 follow/cue 카메라가 geometry에 막히면 fallback 위치로 보정된다.
- objective-ready 직후의 급격한 압박이 완충되고 관련 테스트가 추가된다.
- EditMode/PlayMode 검증과 Unity MCP play/stop 재검증에서 통과하고 console error 0을 유지한다.
