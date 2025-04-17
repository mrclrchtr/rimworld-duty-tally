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
            listingStandard.Gap();

            listingStandard.Gap();
            string maxPriorityBuffer = Settings.MaxPriorityForWeighting.ToString();
            Rect numericFieldRect = listingStandard.GetRect(Text.LineHeight);
            Rect labelRect = numericFieldRect.LeftPart(0.8f).Rounded();
            Rect fieldRect = numericFieldRect.RightPart(0.2f).Rounded();
            Widgets.Label(labelRect, "DutyTally_MaxPriorityForWeighting".Translate());
            TooltipHandler.TipRegion(labelRect, "DutyTally_MaxPriorityForWeightingTip".Translate());
            Widgets.TextFieldNumeric<int>(fieldRect, ref Settings.MaxPriorityForWeighting, ref maxPriorityBuffer, 1, 99);

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}
