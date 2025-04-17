using Verse;

namespace mrclrchtr.DutyTally.Source
{
    public class DutyTallySettings : ModSettings
    {
        public bool IgnoreInvisibleWorkTypes;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref IgnoreInvisibleWorkTypes, "ignoreInvisibleWorkTypes", true);
            base.ExposeData();
        }
    }
}
