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
            var jobCount = GetAssignedJobsForPawn(pawn);
            Text.Anchor = TextAnchor.MiddleCenter;
            try
            {
                Widgets.Label(rect, jobCount.ToString());
            }
            finally
            {
                Text.Anchor = TextAnchor.UpperLeft;
            }
        }

        public override int Compare(Pawn a, Pawn b)
        {
            var aJobs = GetAssignedJobsForPawn(a);
            var bJobs = GetAssignedJobsForPawn(b);
            return aJobs.CompareTo(bJobs);
        }

        public override int GetOptimalWidth(PawnTable table)
        {
            return 70;
        }

        private static int GetAssignedJobsForPawn(Pawn pawn)
        {
            if (!(pawn?.workSettings?.EverWork ?? false))
            {
                return 0;
            }

            IEnumerable<WorkTypeDef> workTypesToConsider = AllWorkTypes;

            // Apply setting filter
            if (DutyTallyMod.Settings.IgnoreInvisibleWorkTypes)
            {
                workTypesToConsider = workTypesToConsider.Where(wt => wt.visible);
            }

            return workTypesToConsider.Count(wt => pawn.workSettings.GetPriority(wt) > 0);
        }
    }

    [StaticConstructorOnStartup]
    public static class DutyTallyInitializer
    {
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
            const string workloadDefName = "DutyTally_Workload";

            try
            {
                PawnTableDef workTableDef = PawnTableDefOf.Work;
                if (workTableDef == null)
                {
                    Log.Error("[DutyTally] Work table def not found");
                    return;
                }

                // Check for existing column to handle hot-reloads
                var existingColumn = workTableDef.columns.FirstOrDefault(c => c.defName == workloadDefName);
                if (existingColumn != null)
                {
                    Log.Warning("[DutyTally] Workload column already exists at position " +
                                workTableDef.columns.IndexOf(existingColumn) + " - skipping");
                    return;
                }

                var jobCountColumnDef = new PawnColumnDef
                {
                    defName = workloadDefName,
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
