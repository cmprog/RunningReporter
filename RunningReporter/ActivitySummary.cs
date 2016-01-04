using System;
using System.Linq;

namespace RunningReporter
{
    public sealed class ActivitySummary
    {
        private readonly DateTime mStartedOnLocal;
        private readonly double mTotalDistanceMeters;
        private readonly double mTotalTimeSeconds;
        private readonly double? mAverageHeartRateBpm;

        public ActivitySummary(Activity_t activity)
        {
            this.mStartedOnLocal = activity.Id.ToLocalTime();
            this.mTotalDistanceMeters = activity.Lap.Sum(x => x.DistanceMeters);
            this.mTotalTimeSeconds = activity.Lap.Sum(x => x.TotalTimeSeconds);
            this.mAverageHeartRateBpm = activity.Lap
                .Where(x => x.AverageHeartRateBpm != null)
                .Select(x => new double?(x.AverageHeartRateBpm.Value))
                .Average();
        }

        public DateTime StartedOnLocal
        {
            get { return this.mStartedOnLocal; }
        }

        public double TotalDistanceMeters { get { return this.mTotalDistanceMeters; } }
        public double TotalTimeSeconds { get { return this.mTotalTimeSeconds; } }
        public double? AverageHearRateBpm { get { return this.mAverageHeartRateBpm; } }

        public double TotalDistanceMiles
        {
            get { return 0.000621371192 * this.TotalDistanceMeters; }
        }

        public TimeSpan TotalTime
        {
            get { return TimeSpan.FromSeconds(this.TotalTimeSeconds); }
        }

        public TimeSpan AverageTimePerMile
        {
            get { return TimeSpan.FromTicks((long)(this.TotalTime.Ticks / this.TotalDistanceMiles)); }
        }
    }
}