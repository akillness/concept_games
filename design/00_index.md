# Project Moss Harbor - Design Index

## 문서 목적

이 폴더는 `Project Moss Harbor`의 구현용 기획 패키지다. 문서 목표는 "개발자가 별도 인터뷰 없이 시스템을 구현할 수 있을 정도의 결정 상태"를 만드는 것이다.

## 게임 한 줄 요약

`Project Moss Harbor`는 오염된 부유 정원 항구를 복원하는 탑다운 3D 캐주얼 어드벤처다. 플레이어는 12~18분 길이의 원정에서 슬러지를 정화하고 자원을 회수하며, 허브로 돌아와 시장/정원/거주 구역을 복구한다. 총 6개 지구, 약 8시간 분량의 본편, 이후 자유 복원 모드를 제공한다.

## 문서 순서

1. [01_high_concept.md](/Users/jangyoung/.superset/projects/concept_games/design/01_high_concept.md)
2. [02_market_and_benchmarks.md](/Users/jangyoung/.superset/projects/concept_games/design/02_market_and_benchmarks.md)
3. [03_core_loop_and_progression.md](/Users/jangyoung/.superset/projects/concept_games/design/03_core_loop_and_progression.md)
4. [04_system_spec.md](/Users/jangyoung/.superset/projects/concept_games/design/04_system_spec.md)
5. [05_content_plan.md](/Users/jangyoung/.superset/projects/concept_games/design/05_content_plan.md)
6. [06_resources_economy_bm.md](/Users/jangyoung/.superset/projects/concept_games/design/06_resources_economy_bm.md)
7. [07_rules_balance_failstates.md](/Users/jangyoung/.superset/projects/concept_games/design/07_rules_balance_failstates.md)
8. [08_art_asset_pipeline.md](/Users/jangyoung/.superset/projects/concept_games/design/08_art_asset_pipeline.md)
9. [09_unity_technical_spec.md](/Users/jangyoung/.superset/projects/concept_games/design/09_unity_technical_spec.md)
10. [10_production_plan.md](/Users/jangyoung/.superset/projects/concept_games/design/10_production_plan.md)
11. [11_task_estimation.md](/Users/jangyoung/.superset/projects/concept_games/design/11_task_estimation.md)
12. [12_sprint_alpha_1_backlog.md](/Users/jangyoung/.superset/projects/concept_games/design/12_sprint_alpha_1_backlog.md)
13. [13_wrapper_followup_plan.md](/Users/jangyoung/.superset/projects/concept_games/design/13_wrapper_followup_plan.md)
- [14. Scene Improvement Progress](14_scene_improvement_progress.md)
- [15. Gameplay Systems Specification](15_gameplay_systems_spec.md)
- [16. Gameplay Enhancement Changelog](16_gameplay_enhancement_changelog.md)
- [17. District Map Layouts](17_district_map_layouts.md)
- [18. Consolidated Technical Reference](18_consolidated_technical_reference.md)

## 제품 원칙

- 세션 길이: 10~20분
- 본편 길이: 7.5~9시간
- 조작 복잡도: 이동 + 주 행동 + 보조 행동 + 궁극기 + 상호작용
- 아트 스코프: 저채도/저폴리 기반의 톱다운 3D
- BM: 완결형 프리미엄 게임, 이후 소규모 코스메틱 DLC 가능
- 구현 기준: Unity 2022.3.32f1, Built-in Render Pipeline, uGUI/TMP 중심
