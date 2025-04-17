# Duty Tally Mod - AI Development Guide (Concise)

## 1. Overview

*   **Purpose:** Adds a sortable "Workload" column to RimWorld's "Work" tab, showing the count of assigned work types per pawn.
*   **Core Features:** Dynamically adds a `PawnColumnDef` to `PawnTableDefOf.Work`. Provides options to calculate workload as a simple count or a weighted sum based on priority.

## 2. Key Components

*   **`DutyTallyInitializer` (`Source/DutyTally.cs`):**
    *   Uses `[StaticConstructorOnStartup]` and `LongEventHandler.QueueLongEvent` to call `AddJobCountColumn`.
    *   `AddJobCountColumn`: Creates and adds the `PawnColumnDef` for the workload column to `PawnTableDefOf.Work.columns`. Checks for existing column to prevent duplicates.
    *   Initializes Harmony (`mrclrchtr.DutyTally`).
*   **`PawnColumnWorkerWorkload` (`Source/DutyTally.cs`):**
    *   Implements the column logic (`PawnColumnWorker`).
    *   `DoCell()`: Renders the workload count/score.
    *   `Compare()`: Sorts pawns by workload score.
    *   `CalculateWorkloadScore()`: Calculates the workload. If `UseWeightedPriorities` is true, calculates a weighted sum using the formula `Max(1, MaxPriorityForWeighting - priority)`. The default `MaxPriorityForWeighting` of 4 results in weights P1=3, P2=2, P3=1, P4=1. Otherwise, calculates a simple count of assigned tasks. Considers the `IgnoreInvisibleWorkTypes` setting. Iterates `DefDatabase<WorkTypeDef>.AllDefs`.
*   **`DutyTallyMod` (`Source/DutyTallyMod.cs`):**
    *   Extends `Mod`. Handles the settings UI.
    *   `DoSettingsWindowContents()`: Displays checkboxes for `IgnoreInvisibleWorkTypes` and `UseWeightedPriorities`.
    *   Also displays a numeric input field for `MaxPriorityForWeighting`.
*   **`DutyTallySettings` (`Source/DutyTallySettings.cs`):**
    *   Extends `ModSettings`.
    *   `IgnoreInvisibleWorkTypes` (bool): Setting to exclude work types with `visible == false`. Default: `true`.
    *   `UseWeightedPriorities` (bool): Setting to switch between simple count and weighted priority scoring. Default: `true`.
    *   `MaxPriorityForWeighting` (int): Setting to determine the maximum priority to apply weighting to. Default: `4`.
    *   `ExposeData()`: Saves/loads the settings.

## 3. Technical Details

*   **Dependencies:** Harmony (required, listed in `About/About.xml`).
*   **Initialization:** Via `StaticConstructorOnStartup` and `LongEventHandler`, not explicit Harmony patches for column addition.
*   **Localization:** Standard keyed translations (`Languages/*/Keyed/DutyTally.xml`). Keys: `DutyTally_Workload`, `DutyTally_WorkloadTip`, `DutyTally_IgnoreInvisibleWorkTypes`, `DutyTally_IgnoreInvisibleWorkTypesTip`. Accessed via `.Translate()`.
*   **Localization Keys:** `DutyTally_Workload`, `DutyTally_WorkloadTip`, `DutyTally_IgnoreInvisibleWorkTypes`, `DutyTally_IgnoreInvisibleWorkTypesTip`, `DutyTally_UseWeightedPriorities`, `DutyTally_UseWeightedPrioritiesTip`, `DutyTally_MaxPriorityForWeighting`, `DutyTally_MaxPriorityForWeightingTip`. Accessed via `.Translate()`.
*   **Project Structure:** Standard RimWorld mod layout (`About/`, `Assemblies/`, `Languages/`, `Source/`). Uses `.csproj` for build configuration.

## 4. Documentation Maintenance

All changes to the mod’s functionality — whether new features, bug fixes, or refactoring — must be reflected in this document immediately. Update this file alongside your code changes so it always represents the mod’s current state. Use commit messages and, if needed, an internal changelog section to track these updates.
