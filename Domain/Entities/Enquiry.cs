using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Domain.Entities
{
    public class Enquiry
    {
        public int? Id { get; set; }
        public string? Name { get; set; }

        public string? MobileNumber { get; set; }

        public string? Product { get; set; }

        public string? PrimaryDiscussion { get; set; }

        public bool? status { get; set; }

        public DateOnly? FollowupDate { get; set; }

        public string? FeedBack { get; set; }

        [NotMapped]
        public string? EnquiryTakenBy { get; set; }

    }
}
