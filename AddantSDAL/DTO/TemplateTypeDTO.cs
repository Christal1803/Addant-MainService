using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DTO
{
    public class TemplateTypeDTO
    {
        public int IdTemplateType { get; set; }
        public string Name { get; set; }
        public  PageDTO pageDTO { get; set; }
        public bool? IsDeleted { get; set; }
        public string Description { get; set; }
        public int IdPage { get; set; }
    }
    public class PageDTO
    {
        public int IdPage { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public bool? Deleted { get; set; }

    }
}
