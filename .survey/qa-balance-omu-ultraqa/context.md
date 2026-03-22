# Context: QA-Balance Orchestration

## Workflow Context
현 상태는 밸런스 수치 변경 -> 문서 업데이트 지연 -> 테스트 실패 재발 패턴이 반복된다. `team` 분석으로 문서화는 빨라졌지만, 실제 Unity 검증과 연결되지 않으면 회귀가 남는다.

공식 플랫폼 기준으로도 "분석 이벤트 수집(Analytics) - 규칙 분기(Remote Config/Game Overrides) - 서버 로직(Cloud Code)"이 분리되어 있어 운영 절차를 묶지 않으면 누락이 발생하기 쉽다.
- UGS Analytics SQL Data Explorer: https://docs.unity.com/ugs/en-us/manual/analytics/manual/sql-data-explorer-tables
- UGS Remote Config files: https://docs.unity.com/ugs/en-us/manual/remote-config/manual/remote-config-files
- UGS Cloud Code manual: https://docs.unity.com/ugs/en-us/manual/cloud-code/manual

## Affected Users
| Role | Responsibility | Skill Level |
|------|----------------|-------------|
| Game Designer | 수치 의도 정의 | High |
| Game QA | 플레이/회귀 검증 | High |
| Unity Developer | 코드/데이터 반영 | Mid-High |

## Current Workarounds
1. 문서 수동 갱신 후 QA가 별도 체크리스트를 다시 작성
2. 테스트 실패 시 ad-hoc 문자열/수치 핫픽스
3. 플레이 검증을 스크린샷으로만 보관하고 정량 지표 누락
4. UV/메시 이슈는 런타임에서 발견 후 사후 대응 (임포트 파이프라인 사전 탐지 부족)

## Adjacent Problems
- SeedPod 경제 루프 단절 (1차 sink 반영 완료, 비율 튜닝 미완료)
- 경계 이탈 복귀는 반영됐지만 district별 파라미터 튜닝 미완료
- 아트/UV 이슈가 gameplay QA에 늦게 반영
- Mesh read/write 설정에 따라 런타임 UV 처리 전략이 달라져야 함 (`Mesh.isReadable`)
  - https://docs.unity3d.com/ScriptReference/Mesh-isReadable.html

## User Voices
- "문서는 업데이트됐는데 실제 플레이 값이 다르다"
- "테스트는 통과했는데 플레이 체감 난이도는 여전히 튄다"
- "라이브 운영 중에는 수치 롤백보다 안전한 점진 실험이 필요하다"
