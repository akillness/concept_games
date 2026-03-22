# 21. Character & Art Asset Specification

> 캐릭터 텍스처, 머티리얼, UV 레이아웃 사양을 정의하는 아트 기술 명세.
> RuntimeArtDirector.cs 기반 현재 구현 상태와 향후 에셋 교체 계획 포함.

---

## 1. 현재 아트 구현 상태

### RuntimeArtDirector 역할

`MossHarbor.Art.RuntimeArtDirector` (정적 클래스)가 런타임에 모든 비주얼을 생성하고 구성한다.

| 메서드 | 역할 |
|--------|------|
| `AttachPlayerVisual(Transform)` | 플레이어 비주얼 prefab 로드 및 부착 (`Art/Characters/player_avatar`) |
| `DecorateHub(Transform)` | 허브 환경 소품 배치 및 조명 설정 |
| `DecorateHub(Transform, SaveService)` | 위 + 구역 복원 비주얼 추가 |
| `DecorateExpedition(Transform, DistrictDef)` | 원정 환경 소품 배치 및 지구별 분위기 적용 |
| `CreatePickupVisual(Transform, string, Vector3, ResourceType, Color)` | 픽업 오브젝트 비주얼 생성 |
| `CreateObjectiveBeaconVisual(Transform, Vector3)` | 목표 비콘 비주얼 생성 |

### 현재 비주얼 방식

- prefab 기반: `Resources.Load<GameObject>()` 로 `Art/` 경로에서 로드
- 머티리얼: `NormalizeRendererMaterials()` 가 모든 렌더러를 Built-in Standard 셰이더로 정규화
- 텍스처 탐색 순서: `_MainTex` → `_BaseMap` → `_Texture_Albedo` → `_Albedo_Texture` → `_Outside_Texture`
- 색상 탐색 순서: `_Color` → `_BaseColor` → `_Color_Inside` → `_EmissionColor` 등
- 조명: `Exponential` 또는 `ExponentialSquared` 안개 + Directional Light 재구성

### ArtResourcePaths 경로 목록

| 상수 | Resources 경로 |
|------|---------------|
| `CharacterPlayerAvatar` | `Art/Characters/player_avatar` |
| `EnvironmentHubIsland` | `Art/Environment/hub_island` |
| `EnvironmentPier` | `Art/Environment/pier` |
| `EnvironmentMossPatch` | `Art/Environment/moss_patch` |
| `EnvironmentMudPatch` | `Art/Environment/mud_patch` |
| `EnvironmentRoadCobble` | `Art/Environment/road_cobble` |
| `EnvironmentRoadWood` | `Art/Environment/road_wood` |
| `EnvironmentRockCluster` | `Art/Environment/rock_cluster` |
| `EnvironmentVillagePlatform` | `Art/Environment/village_platform` |
| `PropBloomPickup` | `Art/Props/pickup_bloom` |
| `PropScrapPickup` | `Art/Props/pickup_scrap` |
| `PropObjectiveBeacon` | `Art/Props/objective_beacon` |
| `UiPrimaryButton` | `Art/UI/button_primary` |
| `UiPanelRound` | `Art/UI/panel_round` |

---

## 2. 캐릭터 텍스처 사양 (향후)

### 2.1 플레이어 캐릭터

`PlayerController` 기준 CharacterController 높이 2.0f, 반경 0.4f, 중심 Y=1.0f. 비주얼은 localPosition `(0, -0.5, 0)`, localScale `1.05`.

| 항목 | 사양 |
|------|------|
| 모델 폴리곤 | ~3,000 tris (모바일 호환) |
| 텍스처 해상도 | 512×512 (Diffuse) |
| UV 레이아웃 | Single UV set, atlas packed |
| 셰이더 | Built-in Standard (현재 `NormalizeRendererMaterials` 강제 변환) |
| 애니메이션 파라미터 | `Speed` (float), `Sprint` (bool), `Grounded` (bool), `VerticalSpeed` (float) |
| Animator 설정 | `applyRootMotion = false`, `cullingMode = AlwaysAnimate` |
| 색상 팔레트 | 지구별 테마와 대비되는 밝은 톤 |

### 2.2 텍스처 아틀라스 레이아웃 (권장)

```
+--------+--------+
|  Body  |  Head  |
| 256x256| 256x256|
+--------+--------+
|  Arms  |  Legs  |
| 256x256| 256x256|
+--------+--------+
Total: 512×512 single atlas
```

### 2.3 머티리얼 슬롯

| 슬롯 | 용도 | 파라미터 |
|------|------|----------|
| mat_player_body | 캐릭터 몸체 | `_Glossiness = 0.16`, `_Metallic = 0.02` |
| mat_player_glow | 효과 오버레이 | `_Glossiness = 0.45`, `_Metallic = 0.08`, `_EMISSION` 활성 |

emissive 머티리얼의 경우 `_EmissionColor = tintColor * 0.45f` 로 설정됨 (`NormalizeRendererMaterials` 내부 로직).

---

## 3. 픽업 에셋 사양

`CreatePickupVisual()` 호출 시 `resourceType`에 따라 경로와 스케일이 다르게 적용된다.

| 픽업 | prefab 경로 | localScale | 트리거 반경 | emissive |
|------|-------------|-----------|------------|---------|
| BloomDust | `Art/Props/pickup_bloom` | `1.9` | `0.9f` | true |
| Scrap | `Art/Props/pickup_scrap` | `1.4` | `0.7f` | false |

### 3.1 BloomDust Pickup
- 색상: 지구 `districtColor` tint 적용 + emissive
- 향후 형태: 크리스탈 형태 권장
- 텍스처: 64×64 glow gradient

### 3.2 Scrap Pickup
- 색상: 지구 `districtColor` tint 적용
- 향후 형태: 기어/너트 형태 권장
- 텍스처: 64×64 metal roughness

### 3.3 SeedPod Pickup
- 현재 Primitive Capsule 방식 (별도 prefab 미지정)
- 향후 형태: 씨앗 포드 형태 권장
- 텍스처: 64×64 organic pattern

### 3.4 ObjectiveBeacon
- prefab 경로: `Art/Props/objective_beacon`
- localScale: `2.8`
- 트리거 반경: `1.2f`
- 기본 색상: `(0.35, 0.85, 0.74)` emissive

---

## 4. 환경 에셋 사양

### 4.1 허브 환경

허브 분위기 (`ApplyHubAtmosphere`):

| 항목 | 값 |
|------|-----|
| 안개 모드 | `ExponentialSquared` |
| 안개 밀도 | `0.012f` |
| 안개 색상 | `(0.76, 0.90, 0.84)` |
| 주변광 색상 | `(0.62, 0.74, 0.66)` |
| 태양광 색상 | `(1.0, 0.93, 0.80)` |
| 태양광 강도 | `1.45f` |
| 태양 각도 | `(36, 134, 0)` |
| 지면 색상 | `(0.34, 0.47, 0.41)` |
| 지면 스케일 | `(1.7, 1, 1.7)` |

허브 소품 배치:

| 오브젝트 | 경로 | 위치 | 스케일 |
|----------|------|------|--------|
| HubPlatform | `village_platform` | `(0, -0.04, -2)` | `1.15` |
| HubRoadCenter | `road_cobble` | `(0, -0.01, 10)` | `1.15` |
| HubDockPlankA | `road_wood` | `(8, -0.02, 2)` | `1.1` |
| HubIsland | `hub_island` | `(-14, -0.05, 6)` | `0.95` |
| HubPier | `pier` | `(12, -0.02, -4)` | `1.0` |
| HubMossPatchNorth | `moss_patch` | `(10, 0.02, 10)` | `1.25` |
| HubRockCluster | `rock_cluster` | `(-12, 0, -10)` | `0.9` |

랜턴 (PointLight + Cylinder Post + Sphere Orb):

| 랜턴 | 위치 | 색상 | 범위 | 강도 |
|------|------|------|------|------|
| HubLanternA | `(-4, 0, 8)` | `(1.0, 0.84, 0.56)` | 6.0 | 1.7 |
| HubLanternB | `(5, 0, 12)` | `(1.0, 0.78, 0.48)` | 6.5 | 1.7 |
| HubLanternC | `(9, 0, -1)` | `(1.0, 0.80, 0.54)` | 5.8 | 1.6 |

### 4.2 지구별 환경 머티리얼

`DistrictBalanceDefaults.cs` 실제 값 기준:

| 지구 | districtColor | fogColor | fogDensity | ambientColor | sunColor | sunIntensity |
|------|---------------|----------|-----------|--------------|---------|-------------|
| Dock | `(0.40, 0.95, 0.85)` Cyan | `(0.55, 0.72, 0.78)` | 0.022 | `(0.42, 0.56, 0.62)` | `(0.82, 0.92, 0.96)` | 1.35 |
| Reed Fields | `(0.45, 0.78, 0.42)` Swamp Green | `(0.38, 0.52, 0.36)` | 0.026 | `(0.32, 0.46, 0.30)` | `(0.72, 0.86, 0.68)` | 1.25 |
| Tidal Vault | `(0.28, 0.58, 0.82)` Deep Blue | `(0.22, 0.36, 0.52)` | 0.018 | `(0.20, 0.32, 0.48)` | `(0.62, 0.78, 0.94)` | 1.40 |
| Glass Narrows | `(0.72, 0.85, 0.92)` Crystal Ice | `(0.52, 0.62, 0.70)` | 0.015 | `(0.40, 0.52, 0.64)` | `(0.88, 0.92, 1.0)` | 1.60 |
| Sunken Arcade | `(0.92, 0.62, 0.28)` Neon Amber | `(0.32, 0.24, 0.18)` | 0.024 | `(0.28, 0.22, 0.18)` | `(1.0, 0.82, 0.56)` | 1.30 |
| Lighthouse Crown | `(0.18, 0.22, 0.42)` Night Indigo | `(0.14, 0.16, 0.28)` | 0.028 | `(0.12, 0.16, 0.26)` | `(0.56, 0.62, 0.82)` | 1.15 |

원정 분위기 (`ApplyExpeditionAtmosphere`):
- 안개 모드: `Exponential` (허브는 `ExponentialSquared`)
- 지면 색상: `(0.22, 0.33, 0.31)`, 스케일: `(1.22, 1, 1.22)`

### 4.3 구역 복원 비주얼 (RestorationVisuals)

`SaveService.IsHubZoneRestored(zoneId)` 기반으로 조건부 생성:

| 구역 | zoneId | 추가 비주얼 |
|------|--------|------------|
| Dock | `dock` | 랜턴 `(12, 0, 0)` 청록색 `(0.4, 0.95, 0.85)` |
| Reed Fields | `reed_fields` | MossPatch + 랜턴 `(-8, 0, 14)` 녹색 `(0.45, 0.78, 0.42)` |
| Tidal Vault | `tidal_vault` | VillagePlatform + 랜턴 `(6, 0, 16)` 파란색 `(0.28, 0.58, 0.82)` |
| Glass Narrows | `glass_narrows` | 랜턴 `(-10, 0, 20)` 크리스탈 `(0.72, 0.85, 0.92)` |
| Sunken Arcade | `sunken_arcade` | RoadCobble + 랜턴 `(0, 0, 22)` 앰버 `(0.92, 0.62, 0.28)` |
| Lighthouse Crown | `lighthouse_crown` | 비콘 랜턴 `(0, 0, 26)` 흰빛 `(1.0, 0.95, 0.8)` |

---

## 5. 에셋 디렉토리 구조 (향후)

```
concept_game/Assets/Art/
├── Characters/
│   └── player_avatar.prefab              (현재 사용 중)
├── Environment/
│   ├── hub_island.prefab                 (현재 사용 중)
│   ├── pier.prefab                       (현재 사용 중)
│   ├── moss_patch.prefab                 (현재 사용 중)
│   ├── mud_patch.prefab                  (현재 사용 중)
│   ├── road_cobble.prefab                (현재 사용 중)
│   ├── road_wood.prefab                  (현재 사용 중)
│   ├── rock_cluster.prefab               (현재 사용 중)
│   └── village_platform.prefab           (현재 사용 중)
├── Props/
│   ├── pickup_bloom.prefab               (현재 사용 중)
│   ├── pickup_scrap.prefab               (현재 사용 중)
│   └── objective_beacon.prefab           (현재 사용 중)
├── Textures/
│   ├── Characters/
│   │   ├── T_Player_Diffuse.png          (512×512, 향후)
│   │   └── T_Player_Emission.png         (256×256, 향후)
│   ├── Pickups/
│   │   ├── T_BloomDust_Glow.png          (64×64, 향후)
│   │   ├── T_Scrap_Metal.png             (64×64, 향후)
│   │   └── T_SeedPod_Organic.png         (64×64, 향후)
│   └── Environment/
│       └── T_Ground_District.png         (256×256 per district, 향후)
└── UI/
    ├── button_primary.prefab             (현재 사용 중)
    └── panel_round.prefab                (현재 사용 중)
```

---

## 6. 아트 방향 요약 (design/08 기준)

- 시점: 탑다운 3D
- 톤: 따뜻하고 차분한 채도의 코지 판타지
- 렌더링: Built-in Render Pipeline (URP/HDRP 호환 이슈로 유지)
- 제작 원칙: 저폴리/스타일라이즈드, 공통 소품 재활용, 머티리얼 스왑으로 오염 상태 표현
- 비주얼 키워드: moss, harbor, fog, restored light, soft lantern, reclaimed nature
- 화면 읽기: 정화 가능 오염 → 어두운 채도 + 유기적 질감 / 상호작용 → 밝은 림 또는 아이콘
- 카메라가 멀기 때문에 텍스처 디테일보다 실루엣과 컬러 대비가 중요
