using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RunningReporter
{
    public sealed class HtmlActivitySummary
    {
        private readonly ReadOnlyCollection<ActivitySummary> mSummaries;

        private bool mIsLoaded;
        private string mBodyHtml;
        private string mFullHtml;

        private readonly List<HtmlActivitySummaryColumn> mAvailableColumns = new List<HtmlActivitySummaryColumn> {
            new HtmlActivitySummaryColumn("Day", x => x.StartedOnLocal.DayOfWeek.ToString()),
            new HtmlActivitySummaryColumn("Distance", x => x.TotalDistanceMiles.ToString("0.00")),
            new HtmlActivitySummaryColumn("Avg Pace", x => x.AverageTimePerMile.ToString("m\\:ss")),
            new HtmlActivitySummaryColumn("Avg HR", x => x.AverageHearRateBpm.GetValueOrDefault(0.0).ToString("0.0"),
                x => x.Any(z => z.AverageHearRateBpm.HasValue)),
        };

        private readonly List<HtmlActivitySummaryColumn> mApplicableColumns;

        public HtmlActivitySummary(ReadOnlyCollection<ActivitySummary> summaries)
        {
            this.mSummaries = summaries;
            this.mApplicableColumns = this.mAvailableColumns.Where(x => x.IsVisible(this.mSummaries)).ToList();
        }

        private void LoadValues()
        {
            var lRowStringBuilder = new StringBuilder();

            foreach (var lColumn in this.mApplicableColumns)
            {
                var lHeaderBuilder = new TagBuilder("th");
                lHeaderBuilder.InnerHtml = lColumn.HeaderText;
                lRowStringBuilder.Append(lHeaderBuilder);
            }

            var lRowBuilder = new TagBuilder("tr");
            lRowBuilder.InnerHtml = lRowStringBuilder.ToString();
            lRowStringBuilder.Clear();

            var lTableStringBuilder = new StringBuilder();
            lTableStringBuilder.Append(lRowBuilder);

            foreach (var lSummary in this.mSummaries.OrderBy(x => x.StartedOnLocal))
            {
                lRowStringBuilder.Clear();

                foreach (var lColumn in this.mApplicableColumns)
                {
                    var lDataBuilder = new TagBuilder("td");
                    lDataBuilder.InnerHtml = lColumn.GetValue(lSummary);
                    lRowStringBuilder.Append(lDataBuilder);
                }

                lRowBuilder.InnerHtml = lRowStringBuilder.ToString();
                lRowStringBuilder.Clear();

                lTableStringBuilder.Append(lRowBuilder);
            }

            var lTableBuilder = new TagBuilder("table");
            lTableBuilder.InnerHtml = lTableStringBuilder.ToString();

            var lBodyBuilder = new TagBuilder("body");
            lBodyBuilder.InnerHtml = lTableBuilder.ToString();

            const string lcCssPath = @"https://s0.wp.com/wp-content/themes/pub/twentyten/style.css";
            var lLinkBuilder = new TagBuilder("link");
            lLinkBuilder.Attributes.Add("rel", "stylesheet");
            lLinkBuilder.Attributes.Add("type", "text/css");
            lLinkBuilder.Attributes.Add("media", "all");
            lLinkBuilder.Attributes.Add("href", lcCssPath);

            var lHeadBuilder = new TagBuilder("head");
            lHeadBuilder.InnerHtml = lLinkBuilder.ToString();

            var lHtmlBuilder = new TagBuilder("html");
            lHtmlBuilder.InnerHtml = string.Concat(lHeadBuilder, lBodyBuilder);

            this.mFullHtml = lHtmlBuilder.ToString();
            this.mBodyHtml = lTableBuilder.ToString();

            this.mIsLoaded = true;
        }

        private void EnsureValuesAreLoaded()
        {
            if (this.mIsLoaded) return;
            this.LoadValues();
        }

        public string ToHtmlString()
        {
            this.EnsureValuesAreLoaded();
            return this.mFullHtml;
        }

        public string ToHtmlBodyString()
        {
            this.EnsureValuesAreLoaded();
            return this.mBodyHtml;
        }
    }
}