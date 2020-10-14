using System;

namespace KintaiSystem.DB.Data
{
    public class ProjectUptimesData
    {
        public int PrimaryId { get; set; }

        public DateTime WorkDate { get; set; }

        public string ProjectId { get; set; }

        public string UserId { get; set; }

        public int WorkMinute { get; set; }

        public string Remarks { get; set; }
    }
}
