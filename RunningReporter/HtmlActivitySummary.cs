using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace RunningReporter
{
    public sealed class HtmlActivitySummary
    {
        private readonly ReadOnlyCollection<ActivitySummary> mSummaries;

        private bool mIsLoaded;
        private string mBodyHtml;
        private string mFullHtml;

        private readonly List<HtmlActivitySummaryColumn> mAvailableColumns = new List<HtmlActivitySummaryColumn> {
            new HtmlActivityDayColumn(),
            new HtmlActivityDistanceColumn(),
            new HtmlActivityAvgPaceColumn(),
            new HtmlActivityAvgHeartRateColumn(),
        };

        private readonly List<HtmlActivitySummaryColumn> mApplicableColumns;

        public HtmlActivitySummary(ReadOnlyCollection<ActivitySummary> summaries)
        {
            this.mSummaries = summaries;
            this.mApplicableColumns = this.mAvailableColumns.Where(x => x.IsVisible(this.mSummaries)).ToList();
        }

        private void LoadValues()
        {
            const string lcCssPath = @"https://s0.wp.com/wp-content/themes/pub/twentyten/style.css";

            var lStringBuilder = new StringBuilder();

            lStringBuilder.Append("<table>");
            lStringBuilder.Append("<tr>");
            foreach (var lColumn in this.mApplicableColumns)
            {
                lStringBuilder.Append("<th>").Append(lColumn.HeaderText).Append("</th>");
            }
            lStringBuilder.Append("</tr>");


            const string lcNumericFormat = "<td style=\"text-align: right\">{0}</td>";
            const string lcTextFormat = "<td>{0}</td>";

            foreach (var lSummary in this.mSummaries.OrderBy(x => x.StartedOnLocal))
            {
                lStringBuilder.Append("<tr>");
                foreach (var lColumn in this.mApplicableColumns)
                {
                    var lStringFormat = lColumn.IsNumeric ? lcNumericFormat : lcTextFormat;
                    lStringBuilder.AppendFormat(lStringFormat, lColumn.GetTextValue(lSummary));
                }
                lStringBuilder.Append("</tr>");
            }

            lStringBuilder.Append("<tr>");
            foreach (var lColumn in this.mApplicableColumns)
            {
                var lStringFormat = lColumn.IsNumeric ? lcNumericFormat : lcTextFormat;
                lStringBuilder.AppendFormat(lStringFormat, lColumn.GetTotalValue(this.mSummaries));
            }
            lStringBuilder.Append("</tr>");

            lStringBuilder.Append("</table>");

            this.mBodyHtml = lStringBuilder.ToString();

            lStringBuilder.Insert(0, "<div id=\"wrapper\" class=\"hfeed\"><div id=\"main\"><div id=\"container\"><div id=\"content\" role=\"main\"><div class=\"post type-post\"><div class=\"entry-content\">");
            lStringBuilder.Insert(0, "<body class=\"home blog logged-in admin-bar no-customize-support custom-background mp6 customizer-styles-applied highlander-enabled highlander-light infinite-scroll neverending\">");
            lStringBuilder.Insert(0, "</head>");
            lStringBuilder.Insert(0, string.Format("<link rel=\"{0}\" type=\"{1}\" media=\"{2}\" href=\"{3}\" />", "stylesheet", "text/css", "all", lcCssPath));
            lStringBuilder.Insert(0, "<head>");
            lStringBuilder.Insert(0, "<html>");

            lStringBuilder.Append("</div></div></div></div></div></div>");
            lStringBuilder.Append("</body>");
            lStringBuilder.Append("</html>");

            this.mFullHtml = lStringBuilder.ToString();

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