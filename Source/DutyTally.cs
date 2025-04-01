using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace DutyTally
{
    public class PawnColumnWorkerWorkload : PawnColumnWorker
    {
        private static readonly WorkTypeDef[] AllWorkTypes = DefDatabase<WorkTypeDef>.AllDefs.ToArray();

        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            int jobCount = GetAssignedJobsForPawn(pawn);
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect, jobCount.ToString());
            Text.Anchor = TextAnchor.UpperLeft;
        }

        public override int Compare(Pawn a, Pawn b)
        {
            return GetAssignedJobsForPawn(a).CompareTo(GetAssignedJobsForPawn(b));
        }

        public override int GetOptimalWidth(PawnTable table)
        {
            return 70;
        }

        private static int GetAssignedJobsForPawn(Pawn pawn)
        {
            return pawn?.workSettings?.EverWork ?? false
                ? AllWorkTypes.Count(wt => pawn.workSettings.GetPriority(wt) > 0)
                : 0;
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

            var workloadDefName = "DutyTally_Workload";

            try
            {
                PawnTableDef workTableDef = PawnTableDefOf.Work;
                if (workTableDef == null)
                {
                    Log.Error("[DutyTally] Work table def not found");
                    return;
                }

                // Check for existing column
                if (workTableDef.columns.Any(c => c.defName == workloadDefName))
                {
                    Log.Warning("[DutyTally] Column already exists - skipping");
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

                // Insert near "Priority" column
                int researchIndex = workTableDef.columns.FindIndex(c => c.defName == "WorkPriority_Research");
                if (researchIndex == -1)
                {
                    Log.Warning("[DutyTally] WorkPriority_Research column not found - appending to end");
                    researchIndex = workTableDef.columns.Count;
                }

                int insertIndex = researchIndex + 1;
                workTableDef.columns.Insert(insertIndex, jobCountColumnDef);
                Log.Message("[DutyTally] Successfully added workload column");
            }
            catch (Exception ex)
            {
                Log.Error($"[DutyTally] Critical error: {ex}");
            }
        }
    }
}