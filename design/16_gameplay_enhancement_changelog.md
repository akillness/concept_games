# 16. Gameplay Enhancement Changelog

> Sprint: 2026-03-22 — Gameplay depth and polish pass

## Changes

### Difficulty System (NEW)
- **DifficultyLevel.cs**: `Easy`, `Normal`, `Hard` enum
- **DifficultyConfig.cs**: Per-difficulty multipliers for timer (1.3x/1.0x/0.8x), pickup targets, entry cost (0.7x/1.0x/1.3x), star thresholds, fail retention (85%/70%/50%)
- **SaveData.cs**: Added `selectedDifficulty` field persisted across sessions
- **ExpeditionDirector.cs**: Timer scaled by difficulty multiplier
- **HubManager.cs**: Entry cost scaled by difficulty; `CycleDifficulty()` method
- **HubHudController.cs**: Difficulty toggle button in hub UI
- **StarRatingCalculator.cs**: Star thresholds offset by difficulty

### Collision & Physics Enhancement
- **PlayerController.cs**: Replaced `transform.position` movement with `CharacterController` for proper collision response and gravity
- **PlayerController.cs**: Added acceleration/deceleration smoothing (18/22 units/s²) for responsive yet weighty movement
- **PlayerController.cs**: Movement speed tuned to 5.0 (was 4.5), sprint multiplier to 1.6 (was 1.45)
- **SimplePickup.cs**: Added vertical bob animation (sine wave, 0.3 height, 2 Hz)
- **SimplePickup.cs**: Added proximity scale feedback (1.3x when player within 3 units)
- **SimplePickup.cs**: Collection shrink animation (0.15s scale-to-zero before destroy)
- **ObjectiveBeacon.cs**: Pulse scale animation when ready (1.0–1.15 breathing)
- **ObjectiveBeacon.cs**: Static when locked, rotation only when ready (30 deg/s)

### Animation Values Tuning
- Pickup rotation speed: 90 → 60 deg/s (calmer ambient feel)
- Beacon rotation speed: 45 → 30 deg/s (majestic)
- Scene fade: asymmetric timing (0.4s out, 0.35s in) for snappier perceived response

### Scene Transition Improvements
- **SceneFlowService.cs**: Added `CurrentSceneName` property, `OnSceneTransitionStarted`/`OnSceneTransitionCompleted` events
- **SceneFlowService.cs**: Configurable fade durations (constants, not hardcoded)
- **GameBootstrap.cs**: Boot scene starts opaque, fades in on first Hub load
- **GameBootstrap.cs**: Improved Boot→Hub transition (handles both "Boot" and "SampleScene")
- **GameStateService.cs**: Added `OnStateChanged` event for UI subscription

### Design Documentation
- **design/15_gameplay_systems_spec.md**: Consolidated spec merging core loop, systems, balance, difficulty, user stories
- **design/16_gameplay_enhancement_changelog.md**: This file — sprint changelog
- **docs/scene_improvement_spec.md**: Updated implementation status

## Files Changed (Code)

| File | Type | Summary |
|------|------|---------|
| `Scripts/Data/DifficultyLevel.cs` | NEW | Difficulty enum |
| `Scripts/Data/DifficultyConfig.cs` | NEW | Difficulty parameter multipliers |
| `Scripts/Data/SaveData.cs` | EDIT | Added selectedDifficulty field |
| `Scripts/Data/StarRatingCalculator.cs` | EDIT | Difficulty-aware star thresholds |
| `Scripts/Core/GameBootstrap.cs` | EDIT | Boot fade-in, improved scene check |
| `Scripts/Core/SceneFlowService.cs` | EDIT | Events, configurable fades |
| `Scripts/Core/GameStateService.cs` | EDIT | OnStateChanged event |
| `Scripts/Gameplay/Player/PlayerController.cs` | EDIT | CharacterController, accel/decel |
| `Scripts/Expedition/SimplePickup.cs` | EDIT | Bob, proximity scale, shrink collect |
| `Scripts/Expedition/ObjectiveBeacon.cs` | EDIT | Pulse, conditional rotation |
| `Scripts/Expedition/ExpeditionDirector.cs` | EDIT | Difficulty timer scaling |
| `Scripts/Hub/HubManager.cs` | EDIT | Difficulty cost, CycleDifficulty |
| `Scripts/UI/HubHudController.cs` | EDIT | Difficulty button |
| `Scripts/UI/ResultsHudController.cs` | EDIT | Difficulty-aware star display |
| `Scripts/UI/ResultsManager.cs` | EDIT | Difficulty-aware star save |

## Files Changed (Docs)

| File | Type |
|------|------|
| `design/15_gameplay_systems_spec.md` | NEW |
| `design/16_gameplay_enhancement_changelog.md` | NEW |
| `docs/scene_improvement_spec.md` | EDIT |
| `README.md` | EDIT |

---

## Sprint 4: Balance P0 Fixes (2026-03-22)

### Critical Fixes
- **Lighthouse Crown 3-star impossibility**: `threeStarTimeRatio` 0.45 → 0.65 (3-star cutoff 97.5s now exceeds 90s HoldOut)
- **Economy tightening**: Entry costs increased — Dock 5→8, Glass Narrows 20→22, Sunken Arcade 25→28, Lighthouse Crown 30→35
- **Star gate steepening**: Required stars — Glass Narrows 3→4, Sunken Arcade 4→6, Lighthouse Crown 5→8
- **Fail retention consistency**: `RewardCalculator.CalculateFailure` now accepts `DifficultyLevel` and applies `DifficultyConfig.FailResourceRetention` to bloomDust and scrap (was hardcoded 0.5/0.7)

### Files Changed
| File | Type | Change |
|------|------|--------|
| `DistrictBalanceDefaults.cs` | EDIT | Entry costs, star requirements, Lighthouse Crown threeStarTimeRatio |
| `RewardCalculator.cs` | EDIT | Added DifficultyLevel parameter to CalculateFailure, uses FailResourceRetention |
| `ExpeditionDirector.cs` | EDIT | Passes selectedDifficulty to CalculateFailure |
