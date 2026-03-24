# Progress

## Current Checklist
- [x] OMU 계획/검증 워크플로우 부트스트랩
- [x] Unity 인스턴스 상태 및 테스트 자산 파악
- [x] 플레이 동작성/기믹 밸런스 병렬 분석
- [x] 수정 적용 및 테스트 보강
- [x] Unity 재검증 및 업데이트 항목 정리

## Discovered Improvements
- [x] bridge sweeper가 전체 elevated route를 덮지 않도록 footprint 축소
- [x] side lane이 실제 보상 루프로 사용되도록 pickup anchor ordering 개선
- [x] 부스트 착지 지점과 objective beacon의 시각적 텔레그래프 강화
- [x] 카메라 occlusion fallback 다중 후보 탐색
- [x] district objective feasibility end-to-end 테스트
- [x] sweep hazard 충돌이 플레이어를 과도하게 띄우지 않도록 impulse 프로파일 재조정
- [x] obstacle impulse가 누적 launch로 번지지 않도록 player external impulse replace 경로 추가
- [x] objective-critical pickup quota를 ground route 우선으로 재배치해 1층 플레이성 복구
- [x] 1층 ground anchor 수를 늘려 collect-pickups 지구의 목표량 자체를 1층에서 처리 가능하게 확장
- [x] 수동 pitch 램프를 landing-to-landing 연결 램프로 교체해 2층에서 1층으로의 복귀 경로 복원
- [x] 층 이동 시 카메라 추적이 크게 벌어지면 follow pose로 즉시 catch-up
- [x] Expedition 배경 리소스를 layout-aware 외곽/후경 배치로 재설계해 중앙 동선/기믹 가림 제거
- [x] boost pad rock decor를 바깥 shoulder로 이동시켜 전경 clutter를 완화
- [ ] beacon ready 상태 체류 완료 판정 보강
- [ ] boost landing distance-aware 검증 추가
- [ ] camera cue와 catch-up 규칙의 runtime self-play 검증

## Verification Notes
- Unity EditMode 79 tests passed
- Unity PlayMode job passed and no error/exception was reported in console
- 새 회귀 조건은 sweep hazard recoverable tuning과 non-stacking obstacle impulse를 고정한다
- 이번 objective-aware pickup 변경은 Unity MCP 서버 health는 유지됐지만 연결된 Unity instance가 사라져 재검증을 완료하지 못했다
- 배경 배치 루프는 Unity session 부재로 editor runtime 확인 대신 `dotnet build`에서 `MossHarbor.Runtime`/`MossHarbor.Tests.EditMode` 경고 0, 오류 0만 확인했다
