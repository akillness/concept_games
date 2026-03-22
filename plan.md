# Goal
- Expedition의 2층 진입 기믹과 위험/보상 루트를 단순 primitive 중심이 아니라 실제 환경 어셋을 활용한 자연스러운 배치로 재구성한다.
- 상층/우회 루트 보상 신호를 강화하고, 기존 collision 기반 2층 진입이 유지되는지 검증한다.
- 검증 통과 후 문서 동기화와 git push까지 완료한다.

# Implementation Steps
1. 현재 사용 가능한 환경/프롭 어셋을 조사하고, BoostPad/LiftPath/RouteSignal에 대응할 수 있는 어셋 후보를 선정한다.
2. Runtime 레벨 빌더와 아트 런타임 배치를 수정해 primitive-only 기믹을 어셋 기반 장식/지형으로 감싼다.
3. 상층/외곽 lane pickup 보상 신호와 시각 마커를 함께 정리한다.
4. EditMode/PlayMode/Unity MCP runtime 검증을 수행한다.
5. QA/summary/.omu 문서를 최신화하고 커밋/푸시한다.

# Risks
- 런타임에서 사용 가능한 어셋이 프리팹이 아니라 mesh/resource 조합일 수 있어 배치 코드가 예상보다 복잡할 수 있다.
- 어셋 장식이 collider를 방해하면 기존 2층 진입 검증이 다시 깨질 수 있다.
- 장식 오브젝트가 많아지면 시야/가독성이 떨어질 수 있다.

# Completion Criteria
- 2층 진입 collision 루프가 유지된다.
- 기믹과 위험/보상 루트가 환경 어셋 기반으로 더 자연스럽게 보인다.
- 테스트와 Unity runtime 검증이 통과한다.
- 문서와 상태 파일이 최신화되고 git push가 완료된다.
