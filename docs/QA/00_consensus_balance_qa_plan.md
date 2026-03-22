# QA-Centric Balance Consensus (OMU + Team + Ralph)

## Overview
- 목적: 밸런스 수치 조정과 QA 검증 루프를 분리 운영하면서, 릴리즈 전 실패 가능 항목(P0/P1)을 줄인다.
- 합의 기반: `omx team` 분석 결과 + UltraQA 실검증 + Unity Play Mode 캡처.
- 범위: 경제 밸런스, 난이도 곡선, 실패 보존율, 캐릭터 UV fallback, QA 체크리스트 정합성.

## Agreed Changes

| Area | Decision | Status |
|---|---|---|
| Entry economy | Dock/Glass/Sunken/Lighthouse 진입 비용 상향 유지 | Done |
| Early reward inflation | Dock 완료 BD 보너스 25 -> 20 | Done |
| Difficulty spike | Reed Fields 타이머 195 -> 185 | Done |
| HoldOut pressure | Glass 75 -> 60, Lighthouse 90 -> 75 | Done |
| Upgrade pacing | Harbor Pump 20 / Route Scanner 80 / Pearl Resonator 12 | Done |
| SeedPod sink | Harbor Pump 연동 `Bio Press` 변환 추가 (6 SeedPod -> Water +2) | Done |
| Failure retention | 실패 시 BD = 수집분 x 난이도 보존율(보너스 가산 제거) | Done |
| UV safety | 캐릭터 알베도 누락 시 런타임 UV checker 텍스처 자동 생성/적용 | Done |
| UV mesh safety | 캐릭터 메시 UV 누락 시 런타임 복제 메시(`*_RuntimeUv`) 생성 | Done |
| Map boundary recovery | 플레이어 경계 이탈/낙하 시 safe position 복귀 + cooldown | Done |
| QA checklist sync | 비용/타이머/HoldOut/업그레이드 수치 최신화 | Done |

## Verification Contract
1. EditMode tests must pass.
2. Unity console must have no Error/Exception after verification loop.
3. Play mode checks for Hub + Expedition runtime object creation and screenshot evidence.
4. QA docs must be updated in `docs/QA` and checklist aligned.

## Evidence
- Team run: `design-docs-qa-bmad-gds-uv` (task-1/2 completed, shutdown completed).
- Test job (fail): `60ea6101ddd7466bbb8a14fde2073163`
- Test job (fail, stale assembly): `8463615e3a494049b23483d945187966`
- Test job (pass): `9a7099a0852e47bdb21bb68097a5ccb7` (22/22 passed)
- Test job (re-verify pass): `0335ba20178640a1bdb8dc589d1bd6ef` (22/22 passed)
- Test job (deletion-state re-verify pass): `bfd9f97338d145ebb3e1665ca4e7c6fd` (22/22 passed)
- Test job (SeedPod sink pass): `ef64c40537954a219ab36e6ea473f01c` (26/26 passed)
- Test job (boundary + UV pass): `bf8703e1376e4fa5b8bfdba0a32e4e6a` (34/34 passed)
- Play captures:
  - `concept_game/Assets/Screenshots/qa_hub_playmode.png`
  - `concept_game/Assets/Screenshots/qa_expedition_playmode.png`
  - `concept_game/Assets/Screenshots/qa_hub_playmode_r2.png`
  - `concept_game/Assets/Screenshots/qa_expedition_playmode_r2.png`
  - `concept_game/Assets/Screenshots/qa_hub_playmode_r3.png`
  - `concept_game/Assets/Screenshots/qa_expedition_playmode_r3.png`
  - `concept_game/Assets/Screenshots/qa_hub_seedpod_sink_r2.png`
  - `concept_game/Assets/Screenshots/qa_hub_playmode_r4.png`
  - `concept_game/Assets/Screenshots/qa_expedition_playmode_r4.png`
  - `concept_game/Assets/Screenshots/qa_expedition_boundary_recovery_r4.png`
- Change control note:
  - 사용자 승인으로 기존 삭제 상태를 유지한 채 검증 수행 (`유지한채진행해 내가지웟어`).
  - 콘솔은 clear 후 Hub/Expedition 재플레이 기준 Error 0으로 확인.

## Residual Risks
- SeedPod sink는 구현됐지만, 경제 튜닝(소비량/획득량 기준)의 실측 데이터가 아직 부족함.
- 맵 경계 복귀는 구현됐지만, 지구별/난이도별 경계 반경 최적값 실측이 부족함.
- 테스트는 EditMode 기준이며 실제 런타임 장기 세션(10~20분) 데이터는 추가 수집 필요.

## Next Gate
- 다음 스프린트 시작 전 아래 2개를 P1에서 P0로 승격 검토:
  - SeedPod sink 밸런스 튜닝(6:2 비율 검증)
  - 경계 회수 파라미터를 district 데이터로 외부화(지구별 safe-zone 튜닝)
