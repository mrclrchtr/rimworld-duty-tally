using UnityEngine;
using Verse;

namespace mrclrchtr.DutyTally.Source
{
    public class DutyTallyMod : Mod
    {
        public static DutyTallySettings Settings;

        public DutyTallyMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<DutyTallySettings>();
        }

        public override string SettingsCategory()
        {
            return "Duty Tally"; // Can be translated later if needed
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("DutyTally_IgnoreInvisibleWorkTypes".Translate(), ref Settings.IgnoreInvisibleWorkTypes, "DutyTally_IgnoreInvisibleWorkTypesTip".Translate());
            listingStandard.End();
            Settings.Write(); // Write settings when the window closes implicitly handles this, but explicit call is fine too.
            base.DoSettingsWindowContents(inRect);
        }
    }
}
