goal: 실제 플레이 telemetry를 route/seedpod 결과와 연결해 expedition 보상값과 압박 밀도를 재조정할 수 있는 데이터 루프를 만든다.

implementation steps:
1. 현재 `RunSummary`, `SeedPodTelemetry`, `Results UI`, expedition route telemetry 구조를 읽고 실제 플레이 비교에 필요한 필드를 확정한다.
2. SeedPod / CleanWater 수치와 route telemetry를 하나의 summary로 묶고, 허브/결과 화면에서 바로 읽을 수 있는 비교 포맷을 만든다.
3. side/elevated route 선택률과 boost pad 사용률을 바탕으로 pickup 보상값 또는 route signal 크기를 재조정한다.
4. 필요하면 원정 레벨의 hazard 밀도 또는 pickup anchor weighting을 소폭 조정해 넓어진 레벨의 압박 약화를 보정한다.
5. EditMode 테스트와 Unity MCP play/stop 검증으로 회귀를 확인한다.
6. `.omu` 및 QA 요약 문서를 최신화하고 커밋/푸시한다.

risks:
- telemetry를 너무 많이 묶으면 Results UI 가독성이 떨어질 수 있다.
- 보상값을 과하게 올리면 elevated/side route가 사실상 정답 루트가 될 수 있다.
- 압박 밀도 보정이 과하면 objective-ready grace의 안정성이 다시 무너질 수 있다.

completion criteria:
- route telemetry와 SeedPod/CleanWater 정보가 같은 결과 요약에서 읽힌다.
- side/elevated route 보상 또는 압박 밀도 조정이 코드에 반영된다.
- EditMode 테스트와 Unity MCP play/stop 검증에서 통과하고 console error 0을 유지한다.
- 관련 `.omu` / `docs/QA` / `docs/summary` 문서가 최신 상태로 동기화된다.
