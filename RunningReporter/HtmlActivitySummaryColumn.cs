using System;
using System.Collections.Generic;
using System.Linq;

namespace RunningReporter
{
    public abstract class HtmlActivitySummaryColumn
    {
        private readonly string mHeaderText;

        protected HtmlActivitySummaryColumn(string headerText)
        {
            this.mHeaderText = headerText;
        }
        
        public string HeaderText { get { return this.mHeaderText; } }

        public abstract bool IsNumeric { get; }

        public abstract bool IsVisible(IEnumerable<ActivitySummary> summaries);
        public abstract string GetTextValue(ActivitySummary summary);
        public abstract string GetTotalValue(IEnumerable<ActivitySummary> summaries);
    }

    public sealed class HtmlActivityDistanceColumn : HtmlActivitySummaryColumn
    {
        public HtmlActivityDistanceColumn() : base("Distance")
        {
        }

        public override bool IsNumeric
        {
            get { return true; }
        }

        public override bool IsVisible(IEnumerable<ActivitySummary> summaries)
        {
            return true;
        }

        public override string GetTextValue(ActivitySummary summary)
        {
            return summary.TotalDistanceMiles.ToString("0.00");
        }

        public override string GetTotalValue(IEnumerable<ActivitySummary> summaries)
        {
            return summaries.Sum(x => x.TotalDistanceMiles).ToString("0.00");
        }
    }

    public sealed class HtmlActivityAvgHeartRateColumn : HtmlActivitySummaryColumn
    {
        public HtmlActivityAvgHeartRateColumn() : base("Avg HR")
        {
        }

        public override bool IsNumeric
        {
            get { return true; }
        }

        public override bool IsVisible(IEnumerable<ActivitySummary> summaries)
        {
            return summaries.Any(x => x.AverageHearRateBpm.HasValue);
        }

        public override string GetTextValue(ActivitySummary summary)
        {
            if (summary.AverageHearRateBpm.HasValue)
            {
                return summary.AverageHearRateBpm.Value.ToString("0.0");
            }

            return string.Empty;
        }

        public override string GetTotalValue(IEnumerable<ActivitySummary> summaries)
        {
            if (!summaries.Any(x => x.AverageHearRateBpm.HasValue)) return "---";

            var lTotalWeightedDistance = summaries
                .Where(x => x.AverageHearRateBpm.HasValue)
                .Sum(x => x.TotalDistanceMiles);

            if (lTotalWeightedDistance <= 0.01) return "---";

            var lTotalWeightedHeartRate = summaries.Where(x => x.AverageHearRateBpm.HasValue)
                .Sum(x => x.AverageHearRateBpm.Value*x.TotalDistanceMiles);
            return (lTotalWeightedHeartRate/lTotalWeightedDistance).ToString("0.0");
        }
    }

    public sealed class HtmlActivityDayColumn : HtmlActivitySummaryColumn
    {
        public HtmlActivityDayColumn() : base("Day")
        {
        }

        public override bool IsNumeric
        {
            get { return false; }
        }

        public override bool IsVisible(IEnumerable<ActivitySummary> summaries)
        {
            return true;
        }

        public override string GetTextValue(ActivitySummary summary)
        {
            return summary.StartedOnLocal.DayOfWeek.ToString();
        }

        public override string GetTotalValue(IEnumerable<ActivitySummary> summaries)
        {
            return "Totals";
        }
    }

    public sealed class HtmlActivityAvgPaceColumn : HtmlActivitySummaryColumn
    {
        public HtmlActivityAvgPaceColumn() : base("Avg Pace")
        {
        }

        public override bool IsNumeric
        {
            get { return true; }
        }

        public override bool IsVisible(IEnumerable<ActivitySummary> summaries)
        {
            return true;
        }

        public override string GetTextValue(ActivitySummary summary)
        {
            return summary.AverageTimePerMile.ToString("m\\:ss");
        }

        public override string GetTotalValue(IEnumerable<ActivitySummary> summaries)
        {
            var lTotalWeightedDistance = summaries.Sum(x => x.TotalDistanceMiles);

            if (lTotalWeightedDistance <= 0.01) return string.Empty;

            var lTotalWeightedPaceSeconds = summaries
                .Sum(x => x.AverageTimePerMile.TotalSeconds * x.TotalDistanceMiles);

            var lWeightedPace = TimeSpan.FromSeconds(lTotalWeightedPaceSeconds/lTotalWeightedDistance);
            return lWeightedPace.ToString("m\\:ss");
        }
    }
}