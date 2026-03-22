# Goal

`Project Moss Harbor`의 밸런스/QA 워크플로를 `$omu` 표준 단계로 재실행해, 팀 분석 결과를 정리하고 `$ultraqa` 기반 실제 검증(테스트+플레이)을 통과시키며, `docs/QA` 문서 패키지와 `survey` 비교안을 작성한다. 캐릭터 UV 정보 누락 시 런타임 대체 UV 텍스처를 생성/적용 가능한 안전장치를 코드에 추가한다.

# Implementation Steps

1. `team` 산출물 검증
   - `design/19_balance_survey.md`, `docs/qa_verification_checklist.md` 변경 품질 점검
   - 중복/오탈자/구버전 수치 정리
2. 밸런스/UV 코드 보강
   - `RuntimeArtDirector`에 UV/알베도 누락 fallback 텍스처 생성 로직 추가
   - 밸런스 문서의 수치와 현재 코드(`DistrictBalanceDefaults`, `RewardCalculator`) 정합성 맞춤
3. `technical-writing` 문서화
   - `docs/QA/`에 QA 운영 문서, UltraQA 로그, 합의된 개선 항목 작성
4. `ultraqa` 사이클 실행
   - Unity 테스트 실행(`run_tests`)
   - 콘솔 에러 점검(`read_console`)
   - 플레이 모드/씬 동작 검증(직접 플레이 루프 + 캡처)
   - 실패 시 수정 후 재검증(최대 5사이클)
5. `survey` 비교안 작성
   - 개선 전/후 QA-밸런스 접근 비교
   - 다음 개선 제안 정리

# Risks

- Unity 에디터 상태(컴파일/리로드 타이밍)로 테스트 결과가 지연될 수 있음
- 팀 자동 병합 커밋에 문서 중복/형식 오염이 포함됐을 수 있음
- UV 이슈는 모델 데이터 자체 문제면 코드 fallback만으로 완전 해결이 어려울 수 있음

# Completion Criteria

- `docs/QA/` 폴더 생성 및 핵심 QA 문서 2개 이상 작성 완료
- UltraQA 검증 결과(테스트/콘솔/플레이)가 문서로 남고 주요 실패 이슈가 0 또는 명시적 잔여 리스크로 정리됨
- `design/19` 및 `docs/qa_verification_checklist`가 최신 밸런스 값과 일치
- UV 누락 대응 로직(생성+적용)이 코드로 반영되고 검증 기록이 남음
- `.survey/...` 비교 문서에 다음 개선 제안이 포함됨
