using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DTO
{
    [DataContract(Name ="Position")]
    public enum JobStatusEnum { Hidden=1, Active, Closed }

    public class PositionDTO
    {
        [DataMember]
        public JobStatusEnum? JobStatus { get; set; }

        [DataMember]
        public int IdPosition { get; set; }
        [DataMember]
        public DateTime CreatedDate { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool IsDeleted { get; set; }
        [DataMember]
        public string JobId { get; set; }
        //[DataMember]
        //public int? Status { get; set; }
        [DataMember]
        public string Experience { get; set; }
        [DataMember]
        public string Location { get; set; }
        [DataMember]
        public DateTime? ClosingDate { get; set; }
        [DataMember]
        public string ReportsTo { get; set; }
        [DataMember]
        public string AboutCompany { get; set; }
        [DataMember]
        public string JobOverview { get; set; }
        [DataMember]
        public string KeyResponsibility { get; set; }
        [DataMember]
        public string Qualification { get; set; }


        [DataMember]
        public List<PositionDetailDTO> positionDetailDTOs { get; set; }

        [DataMember]
        public bool? Deleted { get; set; }
    }
    public class PositionDetailDTO
    {
        [DataMember]
        public int IdPositionDetail { get; set; }
        [DataMember]
        public int? IdPosition { get; set; }
        [DataMember]
        public DateTime? CreatedDate { get; set; }
        [DataMember]
        public bool IsDeleted { get; set; }
        [DataMember]
        public string SubHeader { get; set; }
        [DataMember]
        public string SubHeaderContent { get; set; }
        [DataMember]
        public bool? Deleted { get; set; }
    }
}
