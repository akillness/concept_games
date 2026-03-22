# 20. Map Collision & Playable Boundaries Specification

> 플레이어 관점의 맵 너비, 지형 충돌 박스, 캐릭터 물리 파라미터, 피커블 영역을 정의하는 기술 명세.
> 관심사 분리: 이 문서는 레벨 디자인/기획만 다룬다. 밸런스는 design/19, QA는 docs/qa_verification_checklist.md 참조.
> 모든 수치는 소스 코드에서 직접 추출. 참조: `PlayerController.cs`, `SimplePickup.cs`, `ObjectiveBeacon.cs`, `ExpeditionDirector.cs`, `DistrictBalanceDefaults.cs`.

---

## 1. 플레이어 캐릭터 물리 사양

### 1.1 CharacterController 파라미터

소스: `PlayerController.cs` `Start()` (line 34–36) 및 `[SerializeField]` 필드 (line 9–15).

| 파라미터 | 소스 필드 | 값 | 설명 |
|----------|-----------|-----|------|
| height | `_controller.height` | 2.0m | 캡슐 전체 높이 |
| radius | `_controller.radius` | 0.4m | 캡슐 반지름 (유효 폭 = 직경 0.8m) |
| center | `_controller.center` | (0, 1, 0) | 발바닥 기준 Y+1m 오프셋 |
| moveSpeed | `moveSpeed` | 5.0 m/s | 기본 이동 속도 |
| sprintMultiplier | `sprintMultiplier` | 1.6× | 스프린트 배율 → 실제 속도 8.0 m/s |
| acceleration | `acceleration` | 18 m/s² | MoveTowards 가속도 |
| deceleration | `deceleration` | 22 m/s² | MoveTowards 감속도 |
| gravity | `gravity` | -15 m/s² | 수직 중력 가속도 |
| groundedVertical | (코드 로직) | -2.0 m/s | 착지 시 verticalSpeed 리셋값 |
| rotationSharpness | `rotationSharpness` | 12 (Slerp 계수) | 모델 회전 부드러움 (프레임당) |
| animationDampTime | `animationDampTime` | 0.08s | 애니메이션 블렌딩 지수 감쇠 시간 |

### 1.2 이동 범위 계산 (정밀값)

```
0 → 최대속도(5.0m/s) 도달 시간  = 5.0 / 18 = 0.2778s
최대속도(5.0m/s) → 정지 시간   = 5.0 / 22 = 0.2273s
0 → 스프린트(8.0m/s) 도달 시간 = 8.0 / 18 = 0.4444s
스프린트(8.0m/s) → 정지 시간  = 8.0 / 22 = 0.3636s
```

### 1.3 애니메이터 파라미터

소스: `PlayerController.cs` `Update()` (line 89–94). Animator hash는 컴파일 타임 상수.

| 파라미터 해시 | 타입 | 값 범위 | 조건 |
|---------------|------|---------|------|
| `Speed` | float | 0.0 → 0.55 (walk) / 1.0 (sprint) | 지수 감쇠 보간 (dampTime=0.08s) |
| `Sprint` | bool | true/false | isSprinting && speed > 0.1 |
| `Grounded` | bool | true/false | `CharacterController.isGrounded` |
| `VerticalSpeed` | float | -15 ~ 0 | verticalSpeed 직접 전달 |

walk 목표값 0.55, sprint 목표값 1.0은 코드 상수값 (line 89).

---

## 2. 충돌 박스 인벤토리

### 2.1 픽업 오브젝트 (SimplePickup)

소스: `ExpeditionDirector.cs` `EnsureTriggerCollider()` (line 409–426), `SimplePickup.cs` 필드 (line 9–17).

| 항목 | 필드/소스 | 값 |
|------|-----------|-----|
| Collider 타입 | `SphereCollider` (isTrigger=true) | — |
| center | `triggerCollider.center = Vector3.zero` | (0, 0, 0) 로컬 |
| 반지름 — BloomDust / SeedPod | `EnsureTriggerCollider(pickup, 0.9f)` | **0.9m** |
| 반지름 — Scrap | `EnsureTriggerCollider(pickup, 0.7f)` | **0.7m** |
| Bob 진폭 | `bobHeight = 0.3f` | ±0.3m (Y축 sin파) |
| Bob 주기 | `bobSpeed = 2f` → sin(t×2) | π ≈ 3.14초/사이클 |
| 접근 스케일 범위 | `approachRange = 3f` | 3.0m 이내 피드백 |
| 접근 최대 스케일 | `approachScaleMultiplier = 1.3f` | 거리 0m일 때 1.3× |
| 수집 Shrink 시간 | `collectShrinkDuration = 0.15f` | 0 → Scale.zero까지 0.15s |
| 자전 속도 | `pickupRotateSpeed = 90f` (ExpeditionDirector) | 90 deg/s |

**접근 스케일 공식** (`SimplePickup.cs` line 65–67):
```
proximity = Clamp01(1 - distance / 3.0)
scaleFactor = Lerp(1.0, 1.3, proximity)
transform.localScale = baseScale * scaleFactor
```
즉, distance=3m → scaleFactor=1.0, distance=0m → scaleFactor=1.3.

### 2.2 목표 비콘 (ObjectiveBeacon)

소스: `ObjectiveBeacon.cs` `EnsureTriggerCollider()` (line 70–83) 및 필드 (line 8–13).
`ExpeditionDirector.cs` `CreateObjectiveBeacon()` (line 396).

| 항목 | 필드/소스 | 값 |
|------|-----------|-----|
| Collider 타입 | `SphereCollider` (isTrigger=true) | — |
| center | `triggerCollider.center = Vector3.zero` | (0, 0, 0) 로컬 |
| 반지름 | `triggerCollider.radius = 1.2f` (ObjectiveBeacon 및 EnsureTriggerCollider 양쪽) | **1.2m** |
| 기본 스케일 (fallback primitive) | `beacon.transform.localScale = (1.5, 1.5, 1.5)` | 1.5 uniform |
| pulseMin / pulseMax | `pulseMin=1f`, `pulseMax=1.15f` | 계수 범위 1.0~1.15 |
| 실제 월드 스케일 (펄스 범위) | baseScale × pulse | **1.5 ~ 1.725** |
| 펄스 공식 | `Lerp(1, 1.15, (sin(t×2π)+1)×0.5)` | 주기 1초 |
| readyRotateSpeed | `readyRotateSpeed = 30f` | 30 deg/s (Ready 상태) |
| lockedColor | R=0.5, G=0.2, B=0.2 (기본; SetTheme()으로 덮어씀) | — |
| readyColor | R=0.2, G=0.9, B=0.6 (기본; SetTheme()으로 덮어씀) | — |

**기존 콜라이더 비활성화 로직**: `EnsureTriggerCollider()` 호출 시 `GetComponentsInChildren<Collider>` 전체를 먼저 disabled하고 루트의 SphereCollider만 활성화. primitive 기반 비콘의 기본 Collider는 이 방식으로 비활성화됨.

### 2.3 플레이어 충돌 볼륨 요약

| 항목 | 값 |
|------|----|
| 타입 | CharacterController (내부 Capsule) |
| 유효 폭 (직경) | 0.8m (radius × 2) |
| 유효 높이 | 2.0m |
| 발 접지 반경 | 0.4m (캡슐 반지름) |
| 캡슐 하단 Y | 0.0m (바닥) — center.y(1.0) - height/2(1.0) = 0 |
| 캡슐 상단 Y | 2.0m — center.y(1.0) + height/2(1.0) = 2.0 |

---

## 3. 픽업 배치 공식 (코드 정밀 재현)

소스: `ExpeditionDirector.cs` `ResolvePickupPosition()` (line 354–359).

```csharp
// 코드 원문
var radius = districtDefinition.pickupSpawnRadius;
var angle = totalPickups <= 1 ? 0f : (Mathf.PI * 2f * index) / totalPickups;
return new Vector3(Mathf.Cos(angle) * radius, 0.75f, Mathf.Sin(angle) * radius);
```

- `total = bloomPickupCount + scrapPickupCount + seedPodPickupCount`
- 인덱스 순서: BloomDust(0..bloom-1) → Scrap(bloom..bloom+scrap-1) → SeedPod(나머지)
- Y 고정값: **0.75m** (픽업 스폰 Y 좌표)
- 비콘 Y 고정값: **0.75m** (`beaconPosition` 정의값)
- 플레이어 스폰: 원점 (0, 0, 0) 추정 (스폰 오브젝트 씬 배치값)

**인접 픽업 최소 이격 거리 (chord length)**:
```
chord = 2 × radius × sin(π / total)
```

---

## 4. 지구별 플레이 가능 영역 (정밀 계산값)

### 4.1 픽업 좌표 전체 목록

모든 좌표는 `position(i) = (cos(2π×i/total)×radius, 0.75, sin(2π×i/total)×radius)` 적용값.

#### Dock (dock) — radius=7m, total=4, 각도 간격 90°

| 인덱스 | 타입 | X | Z | 트리거 반지름 |
|--------|------|------|------|------------|
| 0 | BloomDust | +7.000 | 0.000 | 0.9m |
| 1 | BloomDust | 0.000 | +7.000 | 0.9m |
| 2 | BloomDust | -7.000 | 0.000 | 0.9m |
| 3 | Scrap | 0.000 | -7.000 | 0.7m |
| — | ObjectiveBeacon | 0.000 | +10.000 | 1.2m |

#### Reed Fields (reed_fields) — radius=9m, total=6, 각도 간격 60°

| 인덱스 | 타입 | X | Z | 트리거 반지름 |
|--------|------|------|------|------------|
| 0 | BloomDust | +9.000 | 0.000 | 0.9m |
| 1 | BloomDust | +4.500 | +7.794 | 0.9m |
| 2 | Scrap | -4.500 | +7.794 | 0.7m |
| 3 | Scrap | -9.000 | 0.000 | 0.7m |
| 4 | SeedPod | -4.500 | -7.794 | 0.9m |
| 5 | SeedPod | +4.500 | -7.794 | 0.9m |
| — | ObjectiveBeacon | +3.000 | +12.000 | 1.2m |

#### Tidal Vault (tidal_vault) — radius=10m, total=5, 각도 간격 72°

| 인덱스 | 타입 | X | Z | 트리거 반지름 |
|--------|------|------|------|------------|
| 0 | BloomDust | +10.000 | 0.000 | 0.9m |
| 1 | BloomDust | +3.090 | +9.511 | 0.9m |
| 2 | Scrap | -8.090 | +5.878 | 0.7m |
| 3 | Scrap | -8.090 | -5.878 | 0.7m |
| 4 | SeedPod | +3.090 | -9.511 | 0.9m |
| — | ObjectiveBeacon | -2.000 | +14.000 | 1.2m |

#### Glass Narrows (glass_narrows) — radius=11m, total=5, 각도 간격 72°

| 인덱스 | 타입 | X | Z | 트리거 반지름 |
|--------|------|------|------|------------|
| 0 | BloomDust | +11.000 | 0.000 | 0.9m |
| 1 | BloomDust | +3.399 | +10.462 | 0.9m |
| 2 | Scrap | -8.899 | +6.466 | 0.7m |
| 3 | Scrap | -8.899 | -6.466 | 0.7m |
| 4 | SeedPod | +3.399 | -10.462 | 0.9m |
| — | ObjectiveBeacon | +4.000 | +15.000 | 1.2m |

#### Sunken Arcade (sunken_arcade) — radius=8m, total=7, 각도 간격 ≈51.43°

| 인덱스 | 타입 | X | Z | 트리거 반지름 |
|--------|------|------|------|------------|
| 0 | BloomDust | +8.000 | 0.000 | 0.9m |
| 1 | BloomDust | +4.988 | +6.255 | 0.9m |
| 2 | BloomDust | -1.780 | +7.799 | 0.9m |
| 3 | Scrap | -7.208 | +3.471 | 0.7m |
| 4 | Scrap | -7.208 | -3.471 | 0.7m |
| 5 | Scrap | -1.780 | -7.799 | 0.7m |
| 6 | SeedPod | +4.988 | -6.255 | 0.9m |
| — | ObjectiveBeacon | -3.000 | +12.000 | 1.2m |

#### Lighthouse Crown (lighthouse_crown) — radius=13m, total=8, 각도 간격 45°

| 인덱스 | 타입 | X | Z | 트리거 반지름 |
|--------|------|------|------|------------|
| 0 | BloomDust | +13.000 | 0.000 | 0.9m |
| 1 | BloomDust | +9.192 | +9.192 | 0.9m |
| 2 | BloomDust | 0.000 | +13.000 | 0.9m |
| 3 | Scrap | -9.192 | +9.192 | 0.7m |
| 4 | Scrap | -13.000 | 0.000 | 0.7m |
| 5 | Scrap | -9.192 | -9.192 | 0.7m |
| 6 | SeedPod | 0.000 | -13.000 | 0.9m |
| 7 | SeedPod | +9.192 | -9.192 | 0.9m |
| — | ObjectiveBeacon | 0.000 | +18.000 | 1.2m |

---

### 4.2 플레이 가능 영역 요약표

유효 범위 계산 방법:
- X 범위: `max(pickupSpawnRadius, |beaconX|)`
- Z 최대: `max(pickupSpawnRadius, beaconZ + 1.2)` (비콘 트리거 끝단)
- Z 최소: `-pickupSpawnRadius` (음수 방향 픽업 끝단)
- 권장 경계 박스: 유효 범위 + 여유 2m (벽 두께 + 플레이어 반지름 포함)

| 지구 | 픽업 반경 | 비콘 위치 | X 유효 범위 | Z 유효 범위 | 인접 픽업 최소 이격 |
|------|-----------|-----------|------------|------------|-------------------|
| Dock | 7m | (0, 0.75, 10) | ±7.0m (14m 폭) | -7.0 ~ +11.2m (18.2m) | 9.899m |
| Reed Fields | 9m | (3, 0.75, 12) | ±9.0m (18m 폭) | -9.0 ~ +13.2m (22.2m) | 9.000m |
| Tidal Vault | 10m | (-2, 0.75, 14) | ±10.0m (20m 폭) | -10.0 ~ +15.2m (25.2m) | 11.756m |
| Glass Narrows | 11m | (4, 0.75, 15) | ±11.0m (22m 폭) | -11.0 ~ +16.2m (27.2m) | 12.931m |
| Sunken Arcade | 8m | (-3, 0.75, 12) | ±8.0m (16m 폭) | -8.0 ~ +13.2m (21.2m) | 6.942m |
| Lighthouse Crown | 13m | (0, 0.75, 18) | ±13.0m (26m 폭) | -13.0 ~ +19.2m (32.2m) | 9.950m |

**Z 방향 횡단 시간** (최대 Z 범위 기준):

| 지구 | Z 범위 | 기본 속도 5.0m/s | 스프린트 8.0m/s |
|------|--------|-----------------|----------------|
| Dock | 18.2m | 3.64초 | 2.28초 |
| Reed Fields | 22.2m | 4.44초 | 2.78초 |
| Tidal Vault | 25.2m | 5.04초 | 3.15초 |
| Glass Narrows | 27.2m | 5.44초 | 3.40초 |
| Sunken Arcade | 21.2m | 4.24초 | 2.65초 |
| Lighthouse Crown | 32.2m | 6.44초 | 4.03초 |

---

## 5. 충돌 감지 흐름도

```
Player (CharacterController)
  │
  └─ OnTriggerEnter(Collider other)
        │
        ├─ SimplePickup.OnTriggerEnter()
        │    ├─ TryGetComponent<PlayerController>() → 실패 시 무시
        │    ├─ _isCollecting == true → 무시 (중복 수집 방지)
        │    ├─ _director.Collect(resourceType, amount)
        │    │    ├─ _bloomDustCollected / _scrapCollected / _seedPodCollected 증가
        │    │    ├─ _pickupsCollected++
        │    │    └─ _objectiveService.RegisterCollection()
        │    ├─ _isCollecting = true
        │    └─ Update() → shrink(0.15s) → Destroy(gameObject)
        │
        └─ ObjectiveBeacon.OnTriggerEnter()
             ├─ TryGetComponent<PlayerController>() → 실패 시 무시
             ├─ _director.ObjectiveReady == false → 무시
             └─ _director.ActivateObjectiveBeacon()
                  └─ CompleteCurrentRun()
                       ├─ RewardCalculator.CalculateSuccess()
                       ├─ SaveService.AddResource() (각 자원)
                       ├─ SaveService.SetLastRunSummary()
                       └─ SceneFlowService.LoadResults()
```

**Guard 조건 요약**:
- `SimplePickup`: `_isCollecting` 플래그로 중복 수집 차단 (트리거가 여러 번 발화해도 1회만 처리)
- `ObjectiveBeacon`: `_director.ObjectiveReady` 체크. 조건 미충족 시 진입해도 아무 반응 없음
- `ExpeditionDirector.ActivateObjectiveBeacon()`: `!_runActive || !ObjectiveReady` 이중 가드

---

## 6. 픽업-비콘 오버랩 분석

**오버랩 판정 기준**: 픽업 중심과 비콘 중심 사이 거리 < 픽업 반지름 + 비콘 반지름

| 지구 | 가장 가까운 픽업 (타입) | 픽업-비콘 거리 | 오버랩 임계값 | 결과 |
|------|------------------------|----------------|--------------|------|
| Dock | BloomDust[1] (0, +7) | **3.000m** | 2.1m | 이격 OK (여유 0.900m) |
| Reed Fields | BloomDust[1] (+4.5, +7.79) | **4.465m** | 2.1m | 이격 OK (여유 2.365m) |
| Tidal Vault | BloomDust[1] (+3.09, +9.51) | **6.787m** | 2.1m | 이격 OK |
| Glass Narrows | BloomDust[1] (+3.40, +10.46) | **4.578m** | 2.1m | 이격 OK |
| Sunken Arcade | BloomDust[2] (-1.78, +7.80) | **4.374m** | 2.1m | 이격 OK |
| Lighthouse Crown | BloomDust[2] (0, +13) | **5.000m** | 2.1m | 이격 OK |

**결론**: 현재 6개 지구 전체에서 픽업-비콘 트리거 오버랩 없음. 가장 위험한 경우는 Dock BloomDust[1]과 비콘 사이 3.000m (임계 2.1m 대비 0.9m 여유).

**인접 픽업 간 오버랩 분석** (가장 좁은 Sunken Arcade 기준):
- 최소 인접 이격: 6.942m (Sunken Arcade, total=7, radius=8)
- BloomDust+BloomDust 오버랩 임계: 0.9+0.9=1.8m → 여유 5.142m
- 오버랩 없음. 전 지구 안전.

---

## 7. 지형 충돌 요구사항 (현재 미구현)

**현재 상태**: 바닥은 평면 Plane 오브젝트, 벽/장애물/경계 콜라이더 없음.
플레이어가 맵 외부로 이동하는 것을 막는 물리 경계가 없는 상태.

### 7.1 맵 경계 권장 사양 (BoxCollider 벽)

권장 경계 박스 = 유효 범위 + 여유 2m. 벽 높이 3m (플레이어 높이 2m + 1m 여유).

| 지구 | X 범위 | Z 범위 | 벽 높이 | 비고 |
|------|--------|--------|---------|------|
| Dock | -9.0 ~ +9.0 | -9.0 ~ +13.2 | 3m | 비대칭 Z (비콘 방향 북쪽) |
| Reed Fields | -11.0 ~ +11.0 | -11.0 ~ +15.2 | 3m | — |
| Tidal Vault | -12.0 ~ +12.0 | -12.0 ~ +17.2 | 3m | — |
| Glass Narrows | -13.0 ~ +13.0 | -13.0 ~ +18.2 | 3m | — |
| Sunken Arcade | -10.0 ~ +10.0 | -10.0 ~ +15.2 | 3m | — |
| Lighthouse Crown | -15.0 ~ +15.0 | -15.0 ~ +21.2 | 3m | 비콘 Y=18 → 상당히 북쪽 |

경계 박스 구현 방식 (권장):
- 4면 BoxCollider (North/South/East/West) + 하단 바닥 Plane
- BoxCollider는 `isTrigger=false` (물리 차단)
- 각 벽 두께: 1m (플레이어 capsule radius 0.4m보다 충분히 두껍게)

### 7.2 향후 지형 충돌 오브젝트 목록

| 항목 | 콜라이더 타입 | 예상 크기 | 목적 | 우선 지구 |
|------|-------------|-----------|------|---------|
| 수로/물웅덩이 감속 구역 | BoxCollider (trigger) | 폭 2~4m, 깊이 가변 | 이동 속도 저하 트리거 | Dock, Reed Fields |
| 오염 슬라임 영역 | MeshCollider or BoxCollider (trigger) | 불규칙 | 이동 제한/페널티 | Dock |
| 갈대 수로 분기벽 | BoxCollider (solid) | 폭 1m, 높이 1.5m | 경로 분기 강제 | Reed Fields |
| 조수문 게이트 | BoxCollider (solid, animated) | 폭 3m, 높이 2.5m | 통과 시 타이머 페널티 | Tidal Vault |
| 잔해 장애물 | BoxCollider (solid) | 1~2m 큐브 | 우회 유도 | Sunken Arcade |
| 절벽 낙하 방지벽 | BoxCollider (solid) | 폭 가변, 높이 3m | 낙하 방지 | Lighthouse Crown |
| 야간 파도 위험 구역 | BoxCollider (trigger) | 해안선 기반 | 접근 경고/페널티 | Lighthouse Crown |

### 7.3 CharacterController vs. MeshCollider 호환성

Unity `CharacterController`는 일반 `Collider`를 통과하지 못함. 단, `isTrigger=true`인 콜라이더는 물리 차단 없이 통과하고 `OnTriggerEnter` 이벤트만 발화.

지형 구현 시 주의:
- 경계 벽: `isTrigger=false` → 플레이어 물리 차단
- 감속/페널티 구역: `isTrigger=true` → 통과 가능, 이벤트 처리
- `CharacterController.Move()` 기반이므로 `Rigidbody` 충돌 이벤트(`OnCollisionEnter`) 미발화

---

## 8. 픽업 밀도 분석

`pickupDensity = total / (π × radius²)` 계산값 (반경 내 원형 영역 기준).

| 지구 | 픽업 수 | 반경 | 면적 (π×r²) | 픽업 밀도 |
|------|---------|------|------------|---------|
| Dock | 4 | 7m | 153.94m² | 0.0260 /m² |
| Reed Fields | 6 | 9m | 254.47m² | 0.0236 /m² |
| Tidal Vault | 5 | 10m | 314.16m² | 0.0159 /m² |
| Glass Narrows | 5 | 11m | 380.13m² | 0.0132 /m² |
| Sunken Arcade | 7 | 8m | 201.06m² | **0.0348 /m²** (최고) |
| Lighthouse Crown | 8 | 13m | 530.93m² | 0.0151 /m² |

Sunken Arcade가 픽업 밀도 최고 (0.0348/m²). 7개 픽업이 반경 8m에 압축 배치. Glass Narrows가 최저 (0.0132/m²).

---

## 9. 플레이어 시점 분석

### 9.1 카메라 시야와 맵 범위 비교

현재 카메라 구성 정보는 씬 배치값에 의존하며 코드에 명시되지 않음 (카메라 파라미터는 씬 Inspector에서 설정).

**추정 기준**: 탑다운 또는 쿼터뷰 3D 카메라로 추정 (게임 장르 기반).

| 맵 | X 폭 | Z 깊이 | 스크롤 필요 여부 (추정 시야 20×15m 기준) |
|----|------|--------|---------------------------------------|
| Dock | 14m | 18.2m | Z 방향 소폭 스크롤 가능성 |
| Reed Fields | 18m | 22.2m | X/Z 모두 스크롤 가능성 있음 |
| Tidal Vault | 20m | 25.2m | 스크롤 필요 |
| Glass Narrows | 22m | 27.2m | 스크롤 필요 |
| Sunken Arcade | 16m | 21.2m | Z 방향 스크롤 가능성 |
| Lighthouse Crown | 26m | 32.2m | 양방향 스크롤 필요 |

카메라 파라미터 확정 전까지 이 항목은 추정값. 실제 필드각(FOV), 카메라 높이, 종횡비는 씬 설정 확인 필요.

### 9.2 픽업 Y 좌표와 카메라 시야

- 픽업 스폰 Y: **0.75m** → 지면에서 0.75m 부양 (bob ±0.3m → 실제 Y = 0.45 ~ 1.05m)
- 비콘 Y: **0.75m** → 동일
- 플레이어 캡슐 중심 Y: **1.0m** (center offset)
- 플레이어 눈높이 추정: 1.6 ~ 1.8m (시각적 머리 위치)

픽업이 지면 0.75m에 있어 탑다운 카메라에서 가시성 양호. Bob 효과로 움직임 인지 향상.

---

## 10. 구현 우선순위 권고

현재 미구현 항목 중 게임플레이 직접 영향도 기준:

| 우선순위 | 항목 | 영향 | 관련 지구 |
|----------|------|------|---------|
| 1 | 맵 경계 BoxCollider 벽 | 낙하/이탈 버그 방지 | 전 지구 |
| 2 | 감속 구역 trigger 볼륨 | 지구별 기믹 차별화 | Dock, Reed Fields |
| 3 | 조수문 게이트 (animated) | Tidal Vault 핵심 기믹 | Tidal Vault |
| 4 | 잔해 장애물 BoxCollider | 우회 동선 강제 | Sunken Arcade |
| 5 | 절벽 낙하 방지 BoxCollider | 안전성 | Lighthouse Crown |
| 6 | 오염 슬라임 MeshCollider | 비주얼 피드백 연동 | Dock, Reed Fields |

---

*최종 수정: 2026-03-22 | 참조: PlayerController.cs, SimplePickup.cs, ObjectiveBeacon.cs, ExpeditionDirector.cs, DistrictBalanceDefaults.cs, design/17_district_map_layouts.md*
