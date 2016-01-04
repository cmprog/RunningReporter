using System.Windows.Input;
using GalaSoft.MvvmLight;

namespace RunningReporter
{
    public sealed class CommandViewModel : ViewModelBase
    {
        private string mDisplayText;
        private readonly ICommand mCommand;

        public CommandViewModel(string displayText, ICommand command)
        {
            this.mDisplayText = displayText;
            this.mCommand = command;
        }

        public string DisplayText
        {
            get { return this.mDisplayText; }
            set { this.Set(() => this.DisplayText, ref this.mDisplayText, value); }
        }

        public ICommand Command
        {
            get { return this.mCommand; }
        }
    }
}