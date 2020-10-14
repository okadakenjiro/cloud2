using System;

namespace KintaiSystem.DB.Data
{
    public class OffRequestsData
    {
        public int PrimaryId { get; set; }

        public string UserId { get; set; }

        public string RequestTypeId { get; set; }

        public DateTime ScheduledDate { get; set; }

        public string Reason { get; set; }

        public string ApprovalId { get; set; }

        public int ApprovalFlg { get; set; }

        public DateTime ApprovalDate { get; set; }
    }
}
