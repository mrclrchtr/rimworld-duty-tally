# Duty Tally Mod - AI Development Guide (Concise)

## 1. Overview

*   **Purpose:** Adds a sortable "Workload" column to RimWorld's "Work" tab, showing the count of assigned work types per pawn.
*   **Core Feature:** Dynamically adds a `PawnColumnDef` to `PawnTableDefOf.Work`.

## 2. Key Components

*   **`DutyTallyInitializer` (`Source/DutyTally.cs`):**
    *   Uses `[StaticConstructorOnStartup]` and `LongEventHandler.QueueLongEvent` to call `AddJobCountColumn`.
    *   `AddJobCountColumn`: Creates and adds the `PawnColumnDef` for the workload column to `PawnTableDefOf.Work.columns`. Checks for existing column to prevent duplicates.
    *   Initializes Harmony (`mrclrchtr.DutyTally`).
*   **`PawnColumnWorkerWorkload` (`Source/DutyTally.cs`):**
    *   Implements the column logic (`PawnColumnWorker`).
    *   `DoCell()`: Renders the workload count.
    *   `Compare()`: Sorts pawns by workload.
    *   `GetAssignedJobsForPawn()`: Calculates the count, considering the `IgnoreInvisibleWorkTypes` setting. Iterates `DefDatabase<WorkTypeDef>.AllDefs`.
*   **`DutyTallyMod` (`Source/DutyTallyMod.cs`):**
    *   Extends `Mod`. Handles the settings UI.
    *   `DoSettingsWindowContents()`: Displays the checkbox for `IgnoreInvisibleWorkTypes`.
*   **`DutyTallySettings` (`Source/DutyTallySettings.cs`):**
    *   Extends `ModSettings`.
    *   `IgnoreInvisibleWorkTypes` (bool): Setting to exclude work types with `visible == false`. Default: `true`.
    *   `ExposeData()`: Saves/loads the setting.

## 3. Technical Details

*   **Dependencies:** Harmony (required, listed in `About/About.xml`).
*   **Initialization:** Via `StaticConstructorOnStartup` and `LongEventHandler`, not explicit Harmony patches for column addition.
*   **Localization:** Standard keyed translations (`Languages/*/Keyed/DutyTally.xml`). Keys: `DutyTally_Workload`, `DutyTally_WorkloadTip`, `DutyTally_IgnoreInvisibleWorkTypes`, `DutyTally_IgnoreInvisibleWorkTypesTip`. Accessed via `.Translate()`.
*   **Project Structure:** Standard RimWorld mod layout (`About/`, `Assemblies/`, `Languages/`, `Source/`). Uses `.csproj` for build configuration.
