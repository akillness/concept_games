---
phase: 3 (Solutioning)
generated: 2026-03-21
tool: Gemini CLI
level: 2
---

# BMAD Phase 3 — Gemini 코드베이스 분석 보고서

## 1. 구현 갭 분석

| 분류 | 설계된 서비스 | 구현 상태 | 비고 |
|------|-------------|----------|------|
| **Core** | GameBootstrap, GameStateService, SaveService, SceneFlowService, AudioService | **구현됨 (4/5)** | AudioService 미확인 |
| **Data** | DistrictDef, ToolDef, QuestDef, HubUpgradeDef, SaveData 등 | **구현됨** | SO 구조 양호 |
| **Hub** | HubManager, HubConstructionService, NpcScheduleService, DecorationPlacementService, QuestBoardService | **부분 (1/5)** | HubManager만 존재 |
| **Expedition** | ExpeditionDirector, ObjectiveService, CorruptionGrid, ResourceDropService, ThreatDirector, ReturnGateController | **부분 (3/6)** | Director, ObjectiveService, SimplePickup 존재 |
| **Player** | PlayerController, ToolController, PlayerStats, ModuleLoadout | **부분 (1/4)** | PlayerController만 존재 |
| **UI** | HudController, HubHudController, ResultsHudController, RuntimeUiFactory, ResultsManager | **구현됨 (5/5)** | |

### 주요 누락 시스템
- **정화 시스템** (CorruptionGrid, CorruptionNodeController) — 핵심 게임플레이
- **위협 시스템** (ThreatDirector) — 원정 긴장감
- **NPC/퀘스트** (NpcScheduleService, QuestBoardService)
- **장식/건축** (DecorationPlacementService, HubConstructionService)
- **도구/모듈** (ToolController, PlayerStats, ModuleLoadout)

## 2. 코드 아키텍처 리뷰

### 강점
- ScriptableObject 기반 데이터 분리 (DistrictDef, ToolDef 등)
- DistrictContentBundle로 지구별 콘텐츠 묶음 관리
- Boot → Hub → Expedition → Results 씬 흐름 확립

### 개선 필요
- 서비스 간 종속성이 싱글톤/정적 접근에 의존 → Service Locator 패턴 권장
- 씬 전환 시 데이터 전달 로직 분산 → 통합 영속성 관리 필요
- 일부 매니저 클래스의 책임 과다 (God Object 경향)

## 3. 라이팅/머티리얼 이슈 진단

### 원인
- 외부 에셋이 URP/Lit 셰이더 사용 → Built-in에서 렌더링 실패
- Reflection Probe / Light Probe 부재
- 라이트맵 데이터 없음 또는 URP 기준 생성

### 수정 우선순위: **최우선 (Hotfix)**
1. URP/Lit → Standard 셰이더 일괄 변환
2. Directional Light 재구성 (허브: 따뜻한 톤 / 원정: 차가운 톤)
3. Ambient / Fog / Skybox 설정
4. Reflection Probe + Light Probe 배치
5. 라이트맵 재베이크

## 4. 차기 스프린트 추천 (Top 5)

1. **[Hotfix] 머티리얼/조명 복구** — 가시성 확보 없이는 레벨 디자인/테스트 불가
2. **[Core] 정화 시스템 프로토타입** — CorruptionGrid + 시각 변화 핵심 루프
3. **[Core] 영속성 관리자 강화** — 씬 전환 + 세이브/로드 안정화
4. **[Loop] 자원 → 허브 보상 루프** — 원정 결과가 허브 복원으로 연결
5. **[UI] 게임 루프 피드백** — 정화율 진행도 + 탐험 결과 화면 연동
