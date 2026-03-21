# Fabric Command Playbook

이 문서는 전체 아트 교체 작업을 Fabric 패턴 스타일로 재실행하거나 검토할 때 쓰는 명령 모음이다.

현재 환경에서는 Homebrew 설치 바이너리명이 `fabric-ai`다. 이 문서의 명령은 프로젝트 래퍼 `scripts/run-fabric-pattern.sh`를 기준으로 작성한다.

## Install

```bash
brew install fabric-ai
/opt/homebrew/opt/fabric-ai/bin/fabric-ai --setup
```

## Audit Design Docs

```bash
cat design/08_art_asset_pipeline.md design/09_unity_technical_spec.md | ./scripts/run-fabric-pattern.sh create_summary
```

```bash
cat art/01_asset_replacement_spec.md | ./scripts/run-fabric-pattern.sh summarize
```

## Review Asset Decisions

```bash
cat art/02_asset_inventory_and_decisions.md | ./scripts/run-fabric-pattern.sh extract_wisdom
```

```bash
find concept_game/Assets/Art -maxdepth 3 -type d | ./scripts/run-fabric-pattern.sh summarize
```

## Summarize Current Diff

```bash
git diff --stat | ./scripts/run-fabric-pattern.sh create_summary
```

```bash
git diff -- concept_game/Assets art .omc/state | ./scripts/run-fabric-pattern.sh explain_code
```

## Improve Documentation

```bash
cat art/01_asset_replacement_spec.md | ./scripts/run-fabric-pattern.sh improve_writing
```

```bash
cat art/03_progress_log.md | ./scripts/run-fabric-pattern.sh create_summary
```

## Unity Follow-up

```bash
cat art/05_unity_mcp_and_agent_setup.md | ./scripts/run-fabric-pattern.sh summarize
```

```bash
cat concept_game/Packages/manifest.json | ./scripts/run-fabric-pattern.sh summarize
```
