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

## Verification Notes
- EditMode 재검증 79개 통과
- PlayMode 재검증 잡 정상 통과, 콘솔은 정보성 로그만 확인
- sweep hazard launch 완화와 obstacle impulse replace 동작 회귀 테스트 추가
