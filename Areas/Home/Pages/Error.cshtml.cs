using Microsoft.AspNetCore.Mvc;

namespace KintaiSystem.Areas.Home.Pages
{
    public class ErrorModel
    {
        [ViewData]
        public string Title { get; } = "Error";

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}