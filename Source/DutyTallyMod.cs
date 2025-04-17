using UnityEngine;
using Verse;

namespace mrclrchtr.DutyTally.Source
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DutyTallyMod : Mod
    {
        public static DutyTallySettings Settings;

        public DutyTallyMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<DutyTallySettings>();
        }

        public override string SettingsCategory()
        {
            return "DutyTally";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("DutyTally_IgnoreInvisibleWorkTypes".Translate(),
                ref Settings.IgnoreInvisibleWorkTypes, "DutyTally_IgnoreInvisibleWorkTypesTip".Translate());
            listingStandard.Gap();
            listingStandard.CheckboxLabeled("DutyTally_UseWeightedPriorities".Translate(),
                ref Settings.UseWeightedPriorities, "DutyTally_UseWeightedPrioritiesTip".Translate());
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}
