# Short-Term Plan

## Sprint Goal
- Expedition 런타임 레벨에서 목표 달성 가능성을 높이고, 이동/방해/연출 기믹의 밸런스를 테스트 기반으로 재조정한다.

## Scope
- 레이아웃, 부스트 패드, 스위프 해저드, 픽업 우선 루트 규칙 분석
- EditMode + PlayMode + Unity 씬 검증 병행
- 발견된 개선점을 업데이트 백로그에 누적

## Backlog
- [x] 목표 달성 코어 루트의 최소 안전 동선 검증
- [x] 부스트 패드 도착 지점과 상승량 재검토
- [x] 방해 기믹과 Objective Ready grace 구간의 체감 난이도 검증
- [x] 시각 연출 오브젝트가 이동 라인 가독성을 방해하는지 점검
- [x] 수정 결과를 반영한 회귀 테스트 추가

## Update Items
- [x] bridge sweeper 영향 범위를 중앙 스로트만 덮도록 축소
- [x] Objective Ready grace window 확장 및 hazard multiplier 완화
- [x] pickup anchor 순서를 route-aware 배치로 재설계해 side lane 사용 보장
- [x] 부스트 착지 마커, 우선 루트 마커, 비콘 ready halo/light 추가
- [x] 카메라 occlusion fallback을 단순 pull-forward에서 shoulder/height 탐색 방식으로 확장
- [x] 실제 district objective 계약과 travel margin을 함께 검증하는 feasibility 테스트 추가
- [x] sweep hazard impulse를 recoverable push/lift/cooldown 값으로 재조정
- [x] obstacle impulse 누적을 차단해 수집 루트 복귀 가능성 회복
- [x] 목표 달성에 필요한 초기 pickup 슬롯을 1층 ground route에 우선 배치해 2층 강제 진입 완화
- [x] collect-pickups 최대 목표량을 1층 ground anchor만으로 충족하도록 레이아웃 확장
- [x] 2층 top landing과 1층 base landing을 직접 잇는 양방향 traversal ramp로 하강 복귀 가능성 복구
- [x] 큰 층 이동 뒤 카메라가 플레이어를 즉시 다시 붙잡도록 catch-up snap 규칙 추가
- [x] Expedition 배경 데코를 layout-aware shoulder/backdrop 배치로 재구성해 기믹/이동선 겹침 제거
- [x] boost pad 주변 rock decor를 외측 shoulder 방향으로 고정해 중앙 차선 시야 방해 제거
- [ ] objective ready 직전부터 비콘 위에 서 있던 플레이어도 즉시 완료 처리되도록 beacon 체류 판정 보강
- [ ] boost pad launch를 target distance 기반으로 검증해 overshoot/undershoot 회귀 방지
- [ ] 카메라 catch-up이 cue 연출과 충돌하지 않는지 Unity runtime self-play로 재검증

## Verification Notes
- EditMode 재검증 79개 통과
- PlayMode 재검증 잡 정상 통과, 콘솔은 정보성 로그만 확인
- sweep hazard launch 완화와 obstacle impulse replace 동작 회귀 테스트 추가
- 이번 루프의 objective-aware pickup quota 수정은 Unity 인스턴스 연결 해제로 editor-side 재검증이 보류됨
- 카메라 catch-up 수정도 현재 Unity instance 미연결로 editor-side verify가 보류됨
- 이번 루프는 Unity 세션이 없어 editor-run test 대신 `dotnet build`로 `MossHarbor.Runtime`/`MossHarbor.Tests.EditMode` 무경고 컴파일만 확인
