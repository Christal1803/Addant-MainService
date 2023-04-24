using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DTO
{
	[DataContract(Name = "Candidate")]
	public class CandidateDTO
	{
		[DataMember]
		public int IdCandidate { get; set; }
		[DataMember]
		public int? IdPosition { get; set; }
		[DataMember]
		public Nullable<DateTime> CreatedOn { get; set; }
		[DataMember]
		public string FirstName { get; set; }
		[DataMember]
		public string LastName { get; set; }
		[DataMember]
		public string Email { get; set; }
		[DataMember]
		public string Mobile { get; set; }
		[DataMember]
		public string Description { get; set; }
		[DataMember]
		public string ResumeUrl { get; set; }
		[DataMember]
		public bool? IsDeleted { get; set; }
		[DataMember]
		public string Position { get; set; }
		[DataMember]
		public string Status { get; set; }

        [DataMember]
        public bool? Deleted { get; set; }
    }
	[DataContract(Name = "ApplicationBucket")]
	public class ApplicationBucketDTO
	{
		[DataMember]
		public int? IdPosition { get; set; }
		[DataMember]
		public string PositionName { get; set; }
		[DataMember]
		public int TotalCount { get; set; }
		[DataMember]
		public int UnderReviewCount { get; set; }
		[DataMember]
		public int ShortListedCount { get; set; }
		[DataMember]
		public int NewApplicationCount { get; set; }
	}
}
