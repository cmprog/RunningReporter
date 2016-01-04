using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace RunningReporter
{
    public sealed class ActivityRepository
    {
        private readonly List<string> mFilePaths;

        public ActivityRepository(string baseDirectoryPath)
        {
            this.mFilePaths = Directory.GetFiles(baseDirectoryPath, "*.tcx", SearchOption.AllDirectories).ToList();
        }

        public IEnumerable<Activity_t> EnumerateActivities(DateTime startingOnLocal, DateTime endingOnLocal)
        {
            if (endingOnLocal <= startingOnLocal) yield break;

            foreach (var lFilePath in this.mFilePaths)
            {
                var lTrainingCenterDatabase = this.LoadDatabase(lFilePath);
                foreach (var lActivity in lTrainingCenterDatabase.Activities.Activity)
                {
                    var lActivityStartLocal = lActivity.Id.ToLocalTime();
                    if ((startingOnLocal <= lActivityStartLocal) && (lActivityStartLocal < endingOnLocal))
                    {
                        yield return lActivity;
                    }
                }
            }
        }

        private TrainingCenterDatabase_t LoadDatabase(string filePath)
        {
            var lSerializer = new XmlSerializer(typeof(TrainingCenterDatabase_t));
            using (var lFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return (TrainingCenterDatabase_t)lSerializer.Deserialize(lFileStream);
            }
        }
    }
}