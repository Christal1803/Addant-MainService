using AddantSDAL.DTO;
using AddantService;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
    public class BlogRepository: IBlogRepository
    {
        public DALResult<List<BlogDTO>> GetAllBlog(bool isAdminCall = false, DateTime? startDate = null, DateTime? endDate = null, string MainHeader = "", string CreatedBy = "")
        {
            try
            {
                List<BlogDTO> res = new List<BlogDTO>();
                using (var gt = new AddantEntities1())
                {
                    if (startDate != null && endDate != null)
                     res = gt.Blogs.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.CreatedOn) >= System.Data.Entity.DbFunctions.TruncateTime(startDate) && System.Data.Entity.DbFunctions.TruncateTime(x.CreatedOn) <= System.Data.Entity.DbFunctions.TruncateTime(endDate)
                 //    && x.Deleted == false
                     )
                            .Select(x=> new BlogDTO {
                        BannerImgUrl = x.BannerImgUrl,
                        ContentUrl = x.ContentUrl,
                        ConverImgUrl = x.ConverImgUrl,
                        CreatedOn = x.CreatedOn,
                        IdBlog = x.IdBlog,
                        IsDeleted = x.IsDeleted,
                        MainHeader = x.MainHeader,
                        CreatedBy = x.CreatedBy,
                        ProfilePicUrl = x.ProfilePicUrl,
                        MinReadTime = x.MinReadTime,
                        BlogContent = x.BlogContent,
                        Deleted=x.Deleted
                    }).ToList();
                    else
                        res = gt.Blogs //.Where(// && x.Deleted == false)
                            .Select(x => new BlogDTO
                            {
                                BannerImgUrl = x.BannerImgUrl,
                                ContentUrl = x.ContentUrl,
                                ConverImgUrl = x.ConverImgUrl,
                                CreatedOn = x.CreatedOn,
                                IdBlog = x.IdBlog,
                                IsDeleted = x.IsDeleted,
                                MainHeader = x.MainHeader,
                                CreatedBy = x.CreatedBy,
                                ProfilePicUrl = x.ProfilePicUrl,
                                MinReadTime = x.MinReadTime,
                                BlogContent = x.BlogContent,
                                Deleted = x.Deleted

                            }).ToList();
                    res = res.Where(a => (a.MainHeader !=null? (a.MainHeader.ToUpper().Contains(MainHeader != null ? MainHeader.ToUpper() : "") || string.IsNullOrWhiteSpace(MainHeader)) : string.IsNullOrWhiteSpace(MainHeader))
                    || (a.CreatedBy != null ?(a.CreatedBy.ToUpper().Contains(MainHeader != null ? MainHeader.ToUpper() : "") || string.IsNullOrWhiteSpace(MainHeader)): string.IsNullOrWhiteSpace(MainHeader))
                    )
                        //.Where(a => a.CreatedBy.ToUpper().Contains(CreatedBy != null ? CreatedBy?.ToUpper() : "") || string.IsNullOrWhiteSpace(CreatedBy))
                        .OrderByDescending(c => c.IdBlog).OrderByDescending(c => c.IsDeleted).ToList();
                    res = res.Where(x => x.Deleted != true).ToList();


                    if (!isAdminCall)
                        res = res.Where(x=>x.IsDeleted == false&& x.Deleted != true).ToList();

                    return new DALResult<List<BlogDTO>>(Status.Found, res ,null ,null);
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return new DALResult<List<BlogDTO>>(Status.Exception, null, null ,null); }
        }
        public DALResult<BlogDTO> GetBlogById(int blogId) 
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    BlogDTO res = gt.Blogs.Where(x=> x.IdBlog == blogId
                    // && x.Deleted == false
                    ).Select( x => new BlogDTO
                    {
                        BannerImgUrl = x.BannerImgUrl,
                        IdBlog = x.IdBlog,
                        IsDeleted = x.IsDeleted,
                        ContentUrl = x.ContentUrl,
                        ConverImgUrl = x.ConverImgUrl,
                        CreatedOn = x.CreatedOn,
                        MainHeader = x.MainHeader,
                        CreatedBy = x.CreatedBy,
                        ProfilePicUrl = x.ProfilePicUrl,
                        MinReadTime = x.MinReadTime,
                        BlogContent = x.BlogContent,
                        Deleted=x.Deleted
                    }).OrderByDescending(c=>c.CreatedOn).FirstOrDefault();
                    return new DALResult<BlogDTO>(Status.Found , res , null,null);
                }
            }
            catch (Exception ex) { return new DALResult<BlogDTO>(Status.Exception, null, null,null); }
        }
        public DALResult<int> ComposeBlog(BlogDTO blogDTO)
        {
            int _blogId = 0;
            try
            {
                var _data = new Blog();
                using (var gt = new AddantEntities1())
                {
                    Blog existingBlog = gt.Blogs.Where(x => x.IdBlog == blogDTO.IdBlog
                    // && x.Deleted == false 
                    ).FirstOrDefault();
                    if (blogDTO != null)
                    {
                        _data = new Blog
                        {
                            BannerImgUrl = blogDTO.BannerImgUrl,
                            IdBlog = blogDTO.IdBlog,
                            ContentUrl = blogDTO.ContentUrl,
                            ConverImgUrl = blogDTO.ConverImgUrl,
                            CreatedOn = blogDTO.CreatedOn != null ? blogDTO.CreatedOn : DateTime.Now,
                            IsDeleted = blogDTO.IsDeleted,
                            MainHeader = blogDTO.MainHeader,
                            CreatedBy = blogDTO.CreatedBy,
                            ProfilePicUrl = blogDTO.ProfilePicUrl,
                            MinReadTime = blogDTO.MinReadTime,
                            BlogContent = blogDTO.BlogContent,
                            Deleted=false
                        }; 
                    }
                    if (existingBlog != null)
                    {
                        _data.ConverImgUrl = string.IsNullOrEmpty(blogDTO.ConverImgUrl) ? existingBlog.ConverImgUrl : blogDTO.ConverImgUrl;
                        _data.BannerImgUrl = string.IsNullOrEmpty(blogDTO.BannerImgUrl) ? existingBlog.BannerImgUrl : blogDTO.BannerImgUrl;
                        _data.ProfilePicUrl = string.IsNullOrEmpty(blogDTO.ProfilePicUrl) ? existingBlog.ProfilePicUrl : blogDTO.ProfilePicUrl;
                        gt.Entry(existingBlog).CurrentValues.SetValues(_data);
                        _blogId = _data.IdBlog;
                    }
                    else {
                        gt.Blogs.Add(_data);
                    }
                    gt.SaveChanges();
                    _blogId = _data.IdBlog;
                    return new DALResult<int>(Status.Created , _blogId, null,null);
                }
            }
            catch (Exception ex) { return new DALResult<int>(Status.Exception, _blogId, null,null); }
        }

        public DALResult<bool> Delete(int blogId, bool isDelete)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    Blog res = gt.Blogs.Where(x=> x.IdBlog == blogId).FirstOrDefault();
                    res.IsDeleted = isDelete;
                    gt.Entry(res).CurrentValues.SetValues(res);
                    gt.SaveChanges();

                    return new DALResult<bool>(Status.Deleted , true,null ,null);
                }
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null,null); }
        }

		public DALResult<bool> Delete(int blogId)
		{
            try
            {
                using (var gt = new AddantEntities1())
                {
                    Blog res = gt.Blogs.Where(x => x.IdBlog == blogId).FirstOrDefault();
                  //  res.Deleted = true;
                    gt.Entry(res).CurrentValues.SetValues(res);
                    gt.SaveChanges();

                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }
        }


        public DALResult<bool> UpdateDeletedStatus(int Blogid, bool isDeleted)
        {
            try
            {
                using (var gt = new AddantEntities1())
                {
                    Blog res = gt.Blogs.Where(x => x.IdBlog == Blogid).FirstOrDefault();
                    res.Deleted = true;
                    gt.Entry(res).CurrentValues.SetValues(res);
                    gt.SaveChanges();

                    return new DALResult<bool>(Status.Deleted, true, null, null);
                }
            }
            catch (Exception ex) { return new DALResult<bool>(Status.Exception, false, null, null); }
        }
    }
}
