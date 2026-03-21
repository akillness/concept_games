# 08. Art and Asset Pipeline

## 아트 방향

- 시점: 탑다운 3D
- 톤: 따뜻하고 차분한 채도의 코지 판타지
- 렌더링: 현재 프로젝트 기준 Built-in Render Pipeline 유지
- 제작 원칙: 저폴리/스타일라이즈드 에셋 조합 + 최소한의 커스텀 머티리얼

## 비주얼 키워드

- moss
- harbor
- fog
- restored light
- floating garden
- soft lantern
- reclaimed nature

## 화면 읽기 원칙

- 정화 가능한 오염은 어두운 채도 + 유기적인 질감
- 복원 가능한 구조물은 실루엣이 깨진 상태
- 상호작용 오브젝트는 밝은 림 또는 아이콘 표시
- 희귀 보상은 청록/금색 계열

## 리소스 범위

### 필수 세트

- 지형 타일 3종
- 식생 15종
- 구조물 20종
- 장식 30종
- NPC 5종
- 생물 8종
- 오염 오브젝트 10종
- UI 아이콘 40종 내외

### 재사용 전략

- 지구마다 신규 모델을 대량 추가하지 않는다.
- 공통 구조물을 팔레트, 데칼, 소품으로 변형한다.
- 오염 단계는 머티리얼/파티클로 표현한다.

## 에셋 후보 및 주의사항

### 환경

- Stylized Nature Pack  
  https://assetstore.unity.com/packages/3d/environments/stylized-nature-pack-37457
- Low Poly Trees - Pack  
  https://assetstore.unity.com/packages/3d/vegetation/trees/low-poly-trees-pack-73954
- Top Down - Fantasy Village DEMO  
  https://assetstore.unity.com/packages/3d/environments/fantasy/top-down-fantasy-village-demo-230547

### UI

- Casual UI Kit - Mobile  
  https://assetstore.unity.com/packages/2d/gui/casual-ui-kit-mobile-167111
- 2D Casual Game UI - Mobile GUI Kit  
  https://assetstore.unity.com/packages/2d/gui/2d-casual-game-ui-mobile-gui-kit-212605

### 오디오

- FREE Casual Game SFX Pack  
  https://assetstore.unity.com/packages/audio/sound-fx/free-casual-game-sfx-pack-54116/reviews
- Casual & Mobile Music and Sounds Pack  
  https://assetstore.unity.com/packages/audio/music/casual-mobile-music-and-sounds-pack-292853

## 파이프라인 주의사항

- 일부 환경 팩은 URP/HDRP 호환 이슈가 있으므로 현재 프로젝트처럼 Built-in 유지가 안정적이다.
- 자연물 팩은 퍼포먼스가 무거울 수 있으므로 LOD/컬링/간소화 전처리가 필요하다.
- UI 팩은 바로 쓰기보다 컬러 토큰과 아이콘 세트를 통일해야 한다.

## 제작 가이드

1. 허브와 원정 지구에 공통 소품 라이브러리를 쓴다.
2. 오염 전/후 상태는 별도 모델보다 머티리얼 스왑과 파티클로 해결한다.
3. 주민/NPC는 저폴리 실루엣과 컬러 포인트로 구분한다.
4. 카메라가 멀기 때문에 텍스처 디테일보다는 실루엣과 컬러 대비가 중요하다.
