# Moss Harbor Short-Term Plan

## Sprint Goal
- Expedition 레벨을 넓히고 고저차/기믹을 추가해 지형 활용 플레이를 만들고, 핵심 순간 카메라 연출을 붙인다.

## Sprint Scope
- 더 넓은 ground / side lanes / elevated routes / ramps 추가
- 동적 방해요소 또는 활용 기믹 추가
- objective 해결/획득 순간 카메라 연출 추가
- Unity MCP 기반 검증과 문서 동기화

## Backlog
- [x] Expedition 레벨 레이아웃 확장 설계 및 적용
- [x] 상승 가능한 경로와 넓은 우회 동선 추가
- [x] 동적 방해요소 또는 활용형 기믹 구현
- [x] pickup/objective/beacon 이벤트 기반 카메라 연출 구현
- [x] BoostPad 승차 가능 표면 + trigger volume 분리
- [x] ramp/deck 접합 landing 및 외곽 rail로 추락 경로 제한
- [x] boost path / landing visible layer를 환경 자산 기반 decor로 치환
- [x] elevated / side lane pickup에 보상 신호 강화 적용
- [x] route / boost / objective-ready telemetry를 Results 기준으로 기록
- [x] Expedition 카메라 occlusion fallback 적용
- [x] objective-ready 직후 hazard grace window 적용
- [x] SeedPod / CleanWater 결과와 route telemetry를 같은 operations summary로 통합
- [x] side/elevated route signal scale 및 pickup amount 재조정
- [x] 플레이 관점 테스트 전략 기준선 문서화 (`docs/QA/05_play_testing_strategy.md`)
- [x] 2회 반복 추정/검증 문서화 (`docs/QA/06_expedition_traversal_estimation.md`)
- [x] Unity MCP 검증 및 README 최신 GIF 동기화
- [ ] 넓어진 공간의 압박 약화 여부를 기믹 밀도와 경로 시간으로 재측정
- [ ] operations summary를 district별 실제 플레이 로그 비교표로 확장
- [ ] route telemetry를 실제 플레이 로그와 연결해 district별 reward/signal 기준선 재조정
