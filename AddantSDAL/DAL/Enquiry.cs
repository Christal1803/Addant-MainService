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
    
    public partial class Enquiry
    {
        public int IdEnquiry { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<bool> Deleted { get; set; }
    
        public virtual EnquiryCategory EnquiryCategory { get; set; }
    }
}
