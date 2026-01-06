namespace Api.Application.DTOs
{
    public class EnquiryResponseDto
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string MobileNumber { get; set; }

        public string Product { get; set; }

        public string PrimaryDiscussion { get; set; }

        public bool? status { get; set; }

        public DateOnly? FollowupDate { get; set; }

        public string? FeedBack { get; set; }

        public string? EnquiryTakenBy { get; set; }
        //public string EnquiryTakenByName { get; set; }
    }
}
