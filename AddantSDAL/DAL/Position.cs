//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AddantSDAL.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Position
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Position()
        {
            this.PositionDetails = new HashSet<PositionDetail>();
            this.Candidates = new HashSet<Candidate>();
        }
    
        public int IdPosition { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Name { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string JobId { get; set; }
        public Nullable<int> Status { get; set; }
        public string Experience { get; set; }
        public Nullable<System.DateTime> ClosingDate { get; set; }
        public string Location { get; set; }
        public string ReportsTo { get; set; }
        public string AboutCompany { get; set; }
        public string JobOverview { get; set; }
        public string KeyResponsibility { get; set; }
        public string Qualification { get; set; }
        public Nullable<bool> Deleted { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PositionDetail> PositionDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Candidate> Candidates { get; set; }
    }
}
