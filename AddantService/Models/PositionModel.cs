using AddantSDAL.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AddantService.Models
{
    public class PositionModel
    {
        public int IdPosition { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public string JobId { get; set; }
        public JobStatusEnum JobStatus { get; set; }
        //public enum Status { get; set; }
        public string Experience { get; set; }
        public string Location { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string ReportsTo { get; set; }
        public string AboutCompany { get; set; }
        public string JobOverview { get; set; }
        public string KeyResponsibility { get; set; }
        public string Qualification { get; set; }
        public List<PositionDetail> positionDetails { get; set; }
        public bool Deleted { get; set; }



    }

    public class PositionDetail
    {

        public int IdPositionDetail { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public int? IdPosition { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string SubHeader { get; set; }
        public string SubHeaderContent { get; set; }
    }
    public class StatusModel
    {
        public int idPosition { get; set; }
        public int Status { get; set; }

    }
}