using Verse;

namespace mrclrchtr.DutyTally.Source
{
    public class DutyTallySettings : ModSettings
    {
        public bool IgnoreInvisibleWorkTypes = true;
        public bool UseWeightedPriorities = true;
        public int MaxPriorityForWeighting = 4;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref IgnoreInvisibleWorkTypes, "ignoreInvisibleWorkTypes", true);
            Scribe_Values.Look(ref UseWeightedPriorities, "useWeightedPriorities", true);
            Scribe_Values.Look(ref MaxPriorityForWeighting, "maxPriorityForWeighting", 4);
            base.ExposeData();
        }
    }
}
