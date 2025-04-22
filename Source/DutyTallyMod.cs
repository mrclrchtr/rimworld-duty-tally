using UnityEngine;
using Verse;

namespace mrclrchtr.DutyTally.Source
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DutyTallyMod : Mod
    {
        private static DutyTallySettings? _settings;

        public static DutyTallySettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    throw new InvalidOperationException("Settings accessed before initialization.");
                }

                return _settings;
            }
        }


        public DutyTallyMod(ModContentPack content) : base(content)
        {
            _settings = GetSettings<DutyTallySettings>();
        }

        public override string SettingsCategory()
        {
            return "DutyTally";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            if (_settings == null)
            {
                Log.Error("DutyTally settings are null in DoSettingsWindowContents. This should not happen.");
                return; // Don't try to draw the UI if settings are missing
            }

            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("DutyTally_IgnoreInvisibleWorkTypes".Translate(),
                ref _settings.IgnoreInvisibleWorkTypes, "DutyTally_IgnoreInvisibleWorkTypesTip".Translate());
            listingStandard.Gap();
            listingStandard.CheckboxLabeled("DutyTally_UseWeightedPriorities".Translate(),
                ref _settings.UseWeightedPriorities, "DutyTally_UseWeightedPrioritiesTip".Translate());
            listingStandard.Gap();

            listingStandard.Gap();
            string maxPriorityBuffer = _settings.MaxPriorityForWeighting.ToString();
            Rect numericFieldRect = listingStandard.GetRect(Text.LineHeight);
            Rect labelRect = numericFieldRect.LeftPart(0.8f).Rounded();
            Rect fieldRect = numericFieldRect.RightPart(0.2f).Rounded();
            Widgets.Label(labelRect, "DutyTally_MaxPriorityForWeighting".Translate());
            TooltipHandler.TipRegion(labelRect, "DutyTally_MaxPriorityForWeightingTip".Translate());
            Widgets.TextFieldNumeric(fieldRect, ref _settings.MaxPriorityForWeighting, ref maxPriorityBuffer, 1, 99);

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}