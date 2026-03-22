# QA Verification Checklist

> 프로토타입 기능 검증을 위한 QA 체크리스트.
> 관심사 분리: 밸런스는 design/19, 레벨디자인은 design/20 참조.
> 실제 구현 코드(SceneFlowService, PlayerController, ExpeditionDirector, DistrictBalanceDefaults) 기반.

---

## 1. 씬 전환 검증

- [ ] Boot 시작 시 불투명 → 페이드인 (`SceneFadeController` 초기화)
- [ ] Boot → Hub 전환 시 페이드 아웃/인 동작
- [ ] Hub → Expedition 전환 시 페이드 아웃/인 동작
- [ ] Expedition → Results 전환 시 페이드 아웃/인 동작
- [ ] Results → Hub 전환 시 페이드 아웃/인 동작
- [ ] 전환 중 중복 입력 차단 (`_isTransitioning` 가드)
- [ ] 전환 중 버튼 연타 시 씬이 중복 로드되지 않음

---

## 2. 허브 기능 검증

### 2.1 지구 선택

- [ ] 이전 지구 버튼 → 지구 인덱스 감소 및 UI 갱신
- [ ] 다음 지구 버튼 → 지구 인덱스 증가 및 UI 갱신
- [ ] 잠긴 지구에서 Start Expedition 버튼 비활성화
- [ ] 잠긴 지구에서 필요 별점 안내 표시
- [ ] BloomDust 부족 시 Start Expedition 버튼 비활성화

### 2.2 입장 비용 검증 (DistrictBalanceDefaults 기준)

| 지구 | 비용 | Normal 기준 |
|------|------|------------|
| Dock | BloomDust 5 | Easy: 3~4, Hard: 6~7 |
| Reed Fields | BloomDust 10 | — |
| Tidal Vault | BloomDust 15 | — |
| Glass Narrows | BloomDust 20 | — |
| Sunken Arcade | BloomDust 25 | — |
| Lighthouse Crown | BloomDust 30 | — |

- [ ] 각 지구 입장 시 BloomDust가 정확히 차감됨
- [ ] 난이도 Easy 시 비용 0.7× 적용 (Dock: ~3)
- [ ] 난이도 Hard 시 비용 1.3× 적용 (Dock: ~6)

### 2.3 난이도 토글

- [ ] Easy → Normal → Hard → Easy 순환 동작
- [ ] 난이도 변경 시 UI 텍스트 갱신
- [ ] 난이도 설정 저장/복원 (앱 재시작 후 유지)

### 2.4 업그레이드 검증

- [ ] Harbor Pump 구매: Scrap 15 소비, 버튼 `[Installed]` 전환
- [ ] Route Scanner 구매: BloomDust 60 소비, 버튼 `[Installed]` 전환
- [ ] Pearl Resonator 구매: CleanWater 20 소비, 버튼 `[Installed]` 전환
- [ ] 자원 부족 시 업그레이드 버튼 비활성화
- [ ] 이미 설치된 업그레이드 버튼 재클릭 불가
- [ ] 업그레이드 상태 저장/복원 (앱 재시작 후 유지)

### 2.5 튜토리얼

- [ ] 단계 1: Start Expedition 강조 표시 (`StartExpedition`)
- [ ] 단계 2: Results 화면 안내 (`ReviewResults`)
- [ ] 단계 3: 업그레이드 설치 강조 (`InstallUpgrade`)
- [ ] 단계 4: 완료 표시 (`Complete`)
- [ ] 튜토리얼 완료 후 강조 표시 사라짐
- [ ] 튜토리얼 상태 저장/복원

---

## 3. 원정 기능 검증

### 3.1 타이머

| 지구 | Normal 타이머 | Easy (×1.3) | Hard (×0.8) |
|------|-------------|-------------|-------------|
| Dock | 210초 | 273초 | 168초 |
| Reed Fields | 195초 | 253초 | 156초 |
| Tidal Vault | 180초 | 234초 | 144초 |
| Glass Narrows | 180초 | 234초 | 144초 |
| Sunken Arcade | 165초 | 214초 | 132초 |
| Lighthouse Crown | 150초 | 195초 | 120초 |

- [ ] 타이머 카운트다운 표시 및 동작
- [ ] 난이도별 타이머 배율 적용
- [ ] 타이머 만료 시 `FailRun` 처리

### 3.2 픽업 수집

- [ ] 픽업 bob 애니메이션 동작 (상하 부유)
- [ ] 픽업 proximity scale 동작 (3m 이내 접근 시 크기 증가)
- [ ] 픽업 수집 shrink 애니메이션 → Destroy
- [ ] BloomDust 수집 시 자원 증가
- [ ] Scrap 수집 시 자원 증가
- [ ] SeedPod 수집 시 자원 증가
- [ ] `OnTriggerEnter` 픽업 트리거 감지 정상 동작

### 3.3 목표 유형별 검증

- [ ] `CollectPickups`: 지정 수량 수집 시 ObjectiveReady = true
- [ ] `CollectResource`: 지정 자원 지정 수량 수집 시 ObjectiveReady = true
- [ ] `HoldOut`: 타이머 경과로 목표 달성 (Glass Narrows 75초, Lighthouse Crown 90초)

### 3.4 ObjectiveBeacon

- [ ] 비콘 잠금 상태: 빨간계열 색상 표시
- [ ] 비콘 해제 상태: 녹색계열 색상 + pulse 애니메이션
- [ ] 비콘 진입 시 CompleteRun 처리
- [ ] 비콘 트리거 반경: 1.2f (SphereCollider)
- [ ] `SetTheme(districtColor)` 로 지구 테마색 적용됨

### 3.5 환경

- [ ] Dock 환경: Cyan `(0.40, 0.95, 0.85)` 분위기 적용
- [ ] Reed Fields 환경: Swamp Green `(0.45, 0.78, 0.42)` 분위기 적용
- [ ] Tidal Vault 환경: Deep Blue `(0.28, 0.58, 0.82)` 분위기 적용
- [ ] Glass Narrows 환경: Crystal Ice `(0.72, 0.85, 0.92)` 분위기 적용
- [ ] Sunken Arcade 환경: Neon Amber `(0.92, 0.62, 0.28)` 분위기 적용
- [ ] Lighthouse Crown 환경: Night Indigo `(0.18, 0.22, 0.42)` 분위기 적용
- [ ] 지면 색상 및 스케일 `(1.22, 1, 1.22)` 적용

---

## 4. 결과 화면 검증

### 4.1 별점 표시

| 조건 | 별점 |
|------|------|
| 목표 미달성 또는 시간 초과 | 1성 |
| 목표 달성 + 픽업 기준 미달 | 1성 |
| 목표 달성 + 픽업 기준 충족 | 2성 |
| 목표 달성 + 픽업 + 시간 기준 충족 | 3성 |

- [ ] Dock Normal: 2성 기준 픽업 80% 이상, 3성 기준 타이머 65% 잔여
- [ ] 난이도별 별점 기준 차등 적용

| 항목 | Easy | Normal | Hard |
|------|------|--------|------|
| 2성 픽업 기준 | twoStarPickupRatio -0.10 | 기준값 | +0.05 |
| 3성 시간 기준 | threeStarTimeRatio +0.10 | 기준값 | -0.05 |

- [ ] 1성 표시 (목표 달성 실패 또는 기준 미달)
- [ ] 2성 표시 (픽업 기준 충족)
- [ ] 3성 표시 (시간 기준 추가 충족)

### 4.2 보상 및 정보

- [ ] 획득 BloomDust 총합 표시 (수집 + 완료 보너스)
- [ ] 획득 Scrap 총합 표시 (수집 + 완료 보너스)
- [ ] 목표 진행 상황 요약 표시
- [ ] 다음 행동 안내 텍스트 표시
- [ ] Return To Hub 버튼 → Hub 씬 전환

### 4.3 실패 시 보존율

| 자원 | Easy | Normal | Hard |
|------|------|--------|------|
| BloomDust 보존 | 85% | 70% | 50% |
| Scrap 보존 | 85% | 70% | 50% |

- [ ] 실패 시 수집 자원 보존율 적용 후 저장
- [ ] 실패 시 완료 보너스 지급 안 됨

---

## 5. 저장 시스템 검증

- [ ] BloomDust, Scrap, SeedPod, CleanWater, MemoryPearl 저장/복원
- [ ] 지구 선택 인덱스 저장/복원
- [ ] 난이도 설정 저장/복원
- [ ] Harbor Pump, Route Scanner, Pearl Resonator 업그레이드 레벨 저장/복원
- [ ] 총 획득 별점(Stars) 저장/복원
- [ ] 지구별 허브 구역 복원 상태 저장/복원 (`IsHubZoneRestored`)
- [ ] 튜토리얼 단계 저장/복원
- [ ] 퀘스트 클리어 상태 저장/복원
- [ ] 앱 강제 종료 후 재시작 시 모든 데이터 보존

---

## 6. 물리/충돌 검증

- [ ] CharacterController 지면 충돌 정상 동작
- [ ] 중력 적용: 공중에서 낙하 후 지면 안착
- [ ] 이동 가속 체감 (`acceleration = 18f`)
- [ ] 이동 감속 체감 (`deceleration = 22f`, 가속보다 빠른 감속)
- [ ] 스프린트 속도 증가 체감 (기본 5 m/s → 8 m/s)
- [ ] CharacterController 높이 2.0, 반경 0.4 기준 지면 관통 없음
- [ ] 픽업 `SphereCollider.isTrigger = true` 정상 감지
- [ ] 비콘 `SphereCollider.isTrigger = true` 정상 감지 (반경 1.2f)

---

## 7. 아트/비주얼 검증

- [ ] 플레이어 비주얼 prefab `Art/Characters/player_avatar` 로드 성공
- [ ] 픽업 비주얼 prefab `Art/Props/pickup_bloom`, `pickup_scrap` 로드 성공
- [ ] 비콘 비주얼 prefab `Art/Props/objective_beacon` 로드 성공
- [ ] 환경 소품 prefab 8종 (`hub_island`, `pier`, `moss_patch`, `mud_patch`, `road_cobble`, `road_wood`, `rock_cluster`, `village_platform`) 로드 성공
- [ ] 허브 랜턴 3개 생성 및 PointLight 동작
- [ ] 원정 신호 랜턴 2개 생성 및 PointLight 동작
- [ ] 안개(Fog) 허브: `ExponentialSquared`, 원정: `Exponential` 적용
- [ ] 지구 구역 복원 시 RestorationVisuals 조건부 생성

---

## 8. 알려진 이슈

- [ ] **SampleScene (Build Index 4)** 사용되지 않음 → Build Settings에서 제거 필요
- [ ] **맵 경계 없음** → 플레이어가 픽업 방향에서 벗어나 무한 평면 이탈 가능
- [ ] **SeedPod 소비처 없음** → Reed Fields에서 수집 가능하나 활용처 미정의
- [ ] **사운드 없음** → 수집, 비콘 활성화, UI 버튼, 배경음 전무
- [ ] **정화/오염 시스템 미구현** → 위협 요소 없이 탐색과 수집만 존재
- [ ] **ToolDef ScriptableObject** (`ScriptableObjects/Tool_Vacuum`) 로드되나 실제 정화 로직 없음
- [ ] **HoldOut 목표 시각 피드백 미확인** → Glass Narrows/Lighthouse Crown에서 HoldOut 진행률 UI 표시 여부 검증 필요
