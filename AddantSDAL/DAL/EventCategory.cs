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
    
    public partial class EventCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EventCategory()
        {
            this.AddantLives = new HashSet<AddantLife>();
        }
    
        public int IdEventCategory { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string BannerImgUrl { get; set; }
        public Nullable<int> IdMainCategory { get; set; }
        public Nullable<bool> Deleted { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AddantLife> AddantLives { get; set; }
    }
}
