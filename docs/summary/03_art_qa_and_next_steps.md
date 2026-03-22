# Art, QA, And Next Steps

## Current QA Gate

- EditMode 테스트: `53/53 passed`
- 직접 플레이:
  - Expedition runtime layout 확인
  - `ExpeditionCameraDirector` 바인딩 확인
- 콘솔:
  - `The referenced script (Unknown)` 경고 재확인 필요
- UV guardrail:
  - `critical=0`
  - `warnings=0`

## Art And Asset State

- 플레이어 비주얼:
  - `Art/Characters/player_avatar` 로드 검증됨
  - `Leopard.fbx` readable 상태 정리 완료
- 비콘 비주얼:
  - `Art/Props/objective_beacon` 로드 검증됨
- 남은 과제:
  - 플레이어 외 핵심 프리팹/메시로 UV guardrail 적용 범위 확대
  - district별 핵심 환경 프리팹 로드 체크 확대

## Agreed Next Cycle

### 1. SeedPod Ratio Automation
- 목표:
  - `6:2 / 5:2 / 6:3` 비교
  - 15-run 기준 재고 20~30 밴드 유지
- 구현:
  - `SeedPodRefineryExperiment` 하네스 추가 완료
  - 후보별 자동 비교 리포트 생성 가능 상태
- 검증:
  - EditMode `50/50` 통과
  - 다음 단계는 실제 플레이 telemetry를 같은 리포트 포맷으로 수집하는 것

### 2. Boundary Tuning Pass
- 목표:
  - district별 soft-lock 0
- 구현:
  - `boundaryCenter`, `boundaryHalfExtents`, `safePosition`, `floorY` 실측값 조정
- 검증:
  - Unity MCP로 district별 진입
  - 경계 이탈 재현 및 복귀 확인

### 2.5. Expedition Layout Polish
- 목표:
  - 넓어진 공간 위에 실제 플레이 압축도를 더 높인다
- 구현:
  - pickup/hazard/boost 배치 밀도 재조정
  - 상층/하층 왕복 동선에 위험-보상 루트 추가
  - 카메라 cue 지속시간과 위치 튜닝
- 검증:
  - Unity MCP play mode 기준 GIF/README 증적 업데이트
  - objective-ready / beacon-activate 시점 직접 재검증

### 3. UV Guardrail Expansion
- 목표:
  - 플레이어 외 핵심 프리팹까지 회귀 예방
- 구현:
  - 검사 대상 에셋 목록 확장
  - 필요 시 README/QA 캡처 증적 추가
- 검증:
  - guardrail 재실행
  - critical 발생 시 릴리즈 게이트 실패로 처리

### 4. Checklist Closure Pass
- 목표:
  - 현재 운영 게이트에서 전수 QA 단계로 이동
- 구현:
  - 씬 전환, 저장/복원, 결과 화면, 장기 세션 항목 순차 닫기
- 검증:
  - `docs/qa_verification_checklist.md` 체크 상태 갱신

## BMAD-GDS Execution Shape

- Ralph:
  - 구현이 아니라 검증 통과까지 계속 밀어붙이는 기준 역할
- BMAD-GDS:
  - design -> architecture -> production -> QA 순환
- ultrateam:
  - SeedPod / Boundary / UV 3레인 병렬 처리
- Unity MCP:
  - 테스트, 오브젝트 확인, 콘솔 점검, 스크린샷 증적 수집 자동화 축

## Update Rule

- 새 구현이 끝나면 먼저 이 문서의 `Agreed Next Cycle`와 `Current QA Gate`를 갱신한다.
- 이어서 상세 근거를 `docs/QA/`와 관련 design/art 문서에 동기화한다.
