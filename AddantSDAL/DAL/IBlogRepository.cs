using AddantSDAL.DTO;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
    public interface IBlogRepository
    {
        DALResult<int> ComposeBlog(BlogDTO blogDTO);
        DALResult<List<BlogDTO>> GetAllBlog(bool isAdminCall = false, DateTime? startDate = null, DateTime? endDate = null, string MainHeader = "", string CreatedBy = "");
        DALResult<BlogDTO> GetBlogById(int blogId);
        DALResult<bool> Delete(int blogId, bool isDelete);

        DALResult<bool> Delete(int blogId);
        DALResult<bool> UpdateDeletedStatus(int Blogid, bool isDeleted);

    }
}
