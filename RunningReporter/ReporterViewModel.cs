using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Timer = System.Timers.Timer;

namespace RunningReporter
{
    internal sealed class ReporterViewModel : ViewModelBase
    {
        #region Instance Fields --------------------------------------------------------

        private string mBaseDirectoryPath;
        private DateTime mStartDateInclusive;
        private DateTime mEndDateInclusive;

        private string mRenderedHtml;
        private string mRenderedBody;

        private readonly CommandViewModel mPreviousWeekCommand;
        private readonly CommandViewModel mNextWeekCommand;

        private readonly CommandViewModel mClipboardCommand;

        private readonly Timer mRefreshTimer;

        private int mRefreshState;

        #endregion

        #region Constructors -----------------------------------------------------------

        public ReporterViewModel()
        {
            this.mRefreshTimer = new Timer(System.Windows.Forms.SystemInformation.DoubleClickTime * 2);
            this.mRefreshTimer.Elapsed += this.RefreshTimer_Elapsed;
            this.mRefreshTimer.AutoReset = false;

            this.mPreviousWeekCommand = new CommandViewModel("Previous Week", new RelayCommand(() => this.OffsetWeek(-1)));
            this.mNextWeekCommand = new CommandViewModel("Next Week", new RelayCommand(() => this.OffsetWeek(1)));
            this.mClipboardCommand = new CommandViewModel("Copy to Clipboard", new RelayCommand(this.ClipboardImplementation));

            var lDateTimeTodayLocal = DateTime.Today;
            var lWeekStartLocal = lDateTimeTodayLocal.AddDays(-((int) lDateTimeTodayLocal.DayOfWeek));
            this.StartDateInclusive = lWeekStartLocal;
            this.EndDateInclusive = lWeekStartLocal.AddDays(6);

            Application.Current.Exit += CurrentApplication_Exit;

            if (this.IsInDesignMode)
            {
                this.BaseDirectoryPath = "Hello, world!";
            }
            else
            {
                this.LoadBaseDirectoryPath();
            }
        }

        private void CurrentApplication_Exit(object sender, ExitEventArgs e)
        {
            this.mRefreshTimer.Stop();
            this.mRefreshTimer.Dispose();
        }

        #endregion

        #region Instance Properties ----------------------------------------------------

        public override void Cleanup()
        {
            base.Cleanup();

            this.mRefreshTimer.Stop();
            this.mRefreshTimer.Dispose();
        }

        public string BaseDirectoryPath
        {
            get { return mBaseDirectoryPath; }
            set
            {
                if (this.Set(() => this.BaseDirectoryPath, ref this.mBaseDirectoryPath, value))
                {
                    this.RestartTimer();
                }
            }
        }

        public DateTime StartDateInclusive
        {
            get { return mStartDateInclusive; }
            set
            {
                if (this.Set(() => this.StartDateInclusive, ref this.mStartDateInclusive, value))
                {
                    this.RestartTimer();
                }
            }
        }

        public DateTime EndDateInclusive
        {
            get { return mEndDateInclusive; }
            set
            {
                if (this.Set(() => this.EndDateInclusive, ref this.mEndDateInclusive, value))
                {
                    this.RestartTimer();
                }
            }
        }

        public string RenderedHtml
        {
            get { return mRenderedHtml; }
            private set { this.Set(() => this.RenderedHtml, ref this.mRenderedHtml, value); }
        }

        public CommandViewModel PreviousWeekCommand
        {
            get { return this.mPreviousWeekCommand; }
        }

        public CommandViewModel NextWeekCommand
        {
            get { return this.mNextWeekCommand; }
        }

        public CommandViewModel ClipboardCommand
        {
            get { return this.mClipboardCommand; }
        }

        #endregion

        #region Instance Methods -------------------------------------------------------

        private void ClipboardImplementation()
        {
            Clipboard.SetText(this.mRenderedBody, TextDataFormat.Text);
        }

        private void OffsetWeek(int weekCount)
        {
            this.StartDateInclusive = this.StartDateInclusive.AddDays(weekCount*7);
            this.EndDateInclusive = this.EndDateInclusive.AddDays(weekCount*7);
        }

        private void LoadBaseDirectoryPath()
        {
            var lLastUsedDirectoryPath = Properties.Settings.Default.LastUsedBaseDirectoryPath;
            if (!string.IsNullOrWhiteSpace(lLastUsedDirectoryPath) && Directory.Exists(lLastUsedDirectoryPath))
            {
                this.BaseDirectoryPath = lLastUsedDirectoryPath;
                return;
            }

            var lAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var lDatabasePath = Path.Combine(lAppDataPath, "Dropbox\\host.db");

            if (!File.Exists(lDatabasePath)) return;

            var lLines = File.ReadAllLines(lDatabasePath);
            var lFolderPathBase64 = Convert.FromBase64String(lLines[1]);

            var lDropboxDirectoryPath = Encoding.UTF8.GetString(lFolderPathBase64);

            var lReferenceFilePath = Directory
                .EnumerateFiles(lDropboxDirectoryPath, "*.tcx", SearchOption.AllDirectories)
                .FirstOrDefault();

            this.BaseDirectoryPath = (lReferenceFilePath == null)
                ? lDropboxDirectoryPath : Path.GetDirectoryName(lReferenceFilePath);
        }

        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var lCachedState = Interlocked.Increment(ref this.mRefreshState);
            
            var lBaseDirectoryPath = this.BaseDirectoryPath;
            var lStartDateInclusive = this.StartDateInclusive;
            var lEndDateExclusive = this.EndDateInclusive.AddDays(1);

            if (!Directory.Exists(lBaseDirectoryPath)) return;

            Properties.Settings.Default.LastUsedBaseDirectoryPath = lBaseDirectoryPath;
            Properties.Settings.Default.Save();

            this.RenderedHtml = string.Format("<b>{0}</b>", "Calculating Results...");

            var lRepository = new ActivityRepository(lBaseDirectoryPath);
            var lWeeklyActivities = lRepository.EnumerateActivities(lStartDateInclusive, lEndDateExclusive);
            var lActivitySummaries = lWeeklyActivities.Select(x => new ActivitySummary(x)).ToList();

            var lHtmlActivitySummary = new HtmlActivitySummary(lActivitySummaries.AsReadOnly());

            if (lCachedState == this.mRefreshState)
            {
                this.RenderedHtml = lHtmlActivitySummary.ToHtmlString();
                this.mRenderedBody = lHtmlActivitySummary.ToHtmlBodyString();
            }
        }

        private void RestartTimer()
        {
            this.mRefreshTimer.Stop();
            this.mRefreshTimer.Start();
        }

        #endregion
    }
}