namespace KintaiSystem.DB.Data
{
    public class AttendanceRecordsData
    {
        public int PrimaryId { get; set; }

        public string UserId { get; set; }

        public string WorkDate { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public bool StartFlg { get; set; }

        public bool EndFlg { get; set; }
    }
}
