using System;
using System.Collections.Generic;

namespace RunningReporter
{
    public sealed class HtmlActivitySummaryColumn
    {
        private readonly string mHeaderText;
        private readonly Func<ActivitySummary, string> mValueSelector;
        private readonly Func<IEnumerable<ActivitySummary>, bool> mIsVisibleSelector;

        public HtmlActivitySummaryColumn(string headerText, Func<ActivitySummary, string> valueSelector)
            : this(headerText, valueSelector, x => true)
        {
            
        }

        public HtmlActivitySummaryColumn(string headerText,
            Func<ActivitySummary, string> valueSelector,
            Func<IEnumerable<ActivitySummary>, bool> isVisibleSelector)
        {
            this.mHeaderText = headerText;
            this.mValueSelector = valueSelector;
            this.mIsVisibleSelector = isVisibleSelector;
        }
        
        public string HeaderText { get { return this.mHeaderText; } }

        public string GetValue(ActivitySummary summary)
        {
            return this.mValueSelector(summary);
        }

        public bool IsVisible(IEnumerable<ActivitySummary> summaries)
        {
            return this.mIsVisibleSelector(summaries);
        }
    }
}