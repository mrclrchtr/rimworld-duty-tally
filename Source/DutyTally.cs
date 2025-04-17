using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using System.Collections.Generic;

namespace mrclrchtr.DutyTally.Source
{
    public class PawnColumnWorkerWorkload : PawnColumnWorker
    {
        private static readonly WorkTypeDef[] AllWorkTypes = DefDatabase<WorkTypeDef>.AllDefs.ToArray();

        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            var workloadScore = CalculateWorkloadScore(pawn);
            Text.Anchor = TextAnchor.MiddleCenter;
            try
            {
                Widgets.Label(rect, workloadScore.ToString());
            }
            finally
            {
                Text.Anchor = TextAnchor.UpperLeft;
            }
        }

        public override int Compare(Pawn a, Pawn b)
        {
            var aJobs = CalculateWorkloadScore(a);
            var bJobs = CalculateWorkloadScore(b);
            return aJobs.CompareTo(bJobs);
        }

        public override int GetOptimalWidth(PawnTable table)
        {
            return 70;
        }

        private static int CalculateWorkloadScore(Pawn pawn)
        {
            if (!(pawn?.workSettings?.EverWork ?? false))
            {
                return 0;
            }

            IEnumerable<WorkTypeDef> workTypesToConsider = AllWorkTypes;

            if (DutyTallyMod.Settings.IgnoreInvisibleWorkTypes)
            {
                workTypesToConsider = workTypesToConsider.Where(wt => wt.visible);
            }

            return DutyTallyMod.Settings.UseWeightedPriorities
                ?
                // Calculate weighted sum of priorities dynamically
                workTypesToConsider
                    .Select(wt => pawn.workSettings.GetPriority(wt))
                    .Where(priority => priority > 0)
                    .Sum(priority => Math.Max(1, 1 + DutyTallyMod.Settings.MaxPriorityForWeighting - priority))
                :
                // Original count logic
                workTypesToConsider
                    .Count(wt => pawn.workSettings.GetPriority(wt) > 0);
        }
    }

    [StaticConstructorOnStartup]
    // ReSharper disable once UnusedType.Global
    public static class DutyTallyInitializer
    {
        private const string WorkloadColumnDefName = "DutyTally_Workload";

        static DutyTallyInitializer()
        {
            var harmony = new Harmony("mrclrchtr.DutyTally");

            try
            {
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                LongEventHandler.QueueLongEvent(AddJobCountColumn, "Initializing Duty Tally", true, null);
            }
            catch (Exception ex)
            {
                harmony.UnpatchAll("mrclrchtr.DutyTally");
                Log.Error($"[DutyTally] Initialization failed: {ex}");
            }
        }

        private static void AddJobCountColumn()
        {
            try
            {
                var workTableDef = PawnTableDefOf.Work;
                if (workTableDef == null)
                {
                    Log.Error("[DutyTally] Work table def not found");
                    return;
                }

                // Check for existing column to handle hot-reloads
                var existingColumn = workTableDef.columns.FirstOrDefault(c => c.defName == WorkloadColumnDefName);
                if (existingColumn != null)
                {
                    Log.Warning("[DutyTally] Workload column already exists at position " +
                                workTableDef.columns.IndexOf(existingColumn) + " - skipping");
                    return;
                }

                var jobCountColumnDef = new PawnColumnDef
                {
                    defName = WorkloadColumnDefName,
                    workerClass = typeof(PawnColumnWorkerWorkload),
                    sortable = true,
                    label = "DutyTally_Workload".Translate(),
                    headerTip = "DutyTally_WorkloadTip".Translate()
                };
                workTableDef.columns.Add(jobCountColumnDef);
                Log.Message("[DutyTally] Successfully added workload column");
            }
            catch (Exception ex)
            {
                Log.Error($"[DutyTally] Critical error: {ex}");
            }
        }
    }
}
