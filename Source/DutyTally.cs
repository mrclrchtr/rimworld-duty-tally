using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace mrclrchtr.DutyTally.Source
{
    public class PawnColumnWorkerWorkload : PawnColumnWorker_Text
    {
        protected override TextAnchor Anchor => TextAnchor.MiddleCenter;

        protected override string GetTextFor(Pawn pawn)
        {
            return CalculateWorkloadScore(pawn);
        }

        private static string CalculateWorkloadScore(Pawn pawn)
        {
            if (!(pawn?.workSettings?.EverWork ?? false))
            {
                return "0";
            }

            IEnumerable<WorkTypeDef> workTypesToConsider = DefDatabase<WorkTypeDef>.AllDefsListForReading;

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
                    .ToString()
                :
                // Original count logic
                workTypesToConsider
                    .Count(wt => pawn.workSettings.GetPriority(wt) > 0)
                    .ToString();
        }
    }
}
