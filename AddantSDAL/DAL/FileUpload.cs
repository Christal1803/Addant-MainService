using AddantService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace AddantSDAL.DAL
{
    public class FileUpload
    {
       
        public static string UploadImage(Byte[] img, string folderName = "")
        {
            string url = string.Empty;
            try
            {
                string folderPath = HttpContext.Current.Server.MapPath(Path.Combine("~/Uploads/Image",folderName));
                string ImgName = "Img" + Guid.NewGuid() + ".jpg";
                string imagePath = folderPath+ "/" + ImgName;

                if (Directory.Exists(folderPath))
                {
                    File.WriteAllBytes(imagePath, img);
                    return ImgName;
                }
                else
                {
                    Directory.CreateDirectory(folderPath);
                    File.WriteAllBytes(imagePath, img);
                    //extract url and return
                    return ImgName;
                }
                return url;
            }
            catch (Exception ex) { return string.Empty; }
        }
        public static string UploadResume(Byte[] content, string format)
        {
            string url = string.Empty;
            try
            {
                string folderPath = HttpContext.Current.Server.MapPath("~/Uploads/Resume");
                string resName = "Res" + Guid.NewGuid() +"."+ format;
                string resPath = folderPath + "/" + resName;
                if (Directory.Exists(folderPath))
                {
                    File.WriteAllBytes(resPath, content);
                    return resName;
                }
                else
                {
                    Directory.CreateDirectory(folderPath);
                    File.WriteAllBytes(resPath, content);
                    return resName;
                }
                return url;
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
        public static string UploadText(Byte[] content)
        {
            string url = string.Empty;
            try
            {
                string folderPath = HttpContext.Current.Server.MapPath("~/Uploads/Text");
                string txtFileName = "Txt" + Guid.NewGuid() + ".txt";
                string txtFilePath = folderPath + "/" + txtFileName;
                if (Directory.Exists(folderPath))
                {
                    File.WriteAllBytes(txtFilePath,content);
                    return txtFileName;
                }
                else {
                    Directory.CreateDirectory(folderPath);
                    File.WriteAllBytes(txtFilePath, content);
                    return txtFileName;
                }
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
        public static Byte[] GetImage(string imgPath, string folderName="")
        {
            string folderPath = HttpContext.Current.Server.MapPath(Path.Combine("~/Uploads/Image",folderName));
            try
            {
                if (File.Exists(Path.Combine(folderPath, imgPath)))
                {
                    return File.ReadAllBytes(Path.Combine(folderPath, imgPath));  
                }
                else
                    return null;
            }
            catch (Exception ex) { return null; }
        }
        public static string GetImageUrl(string imgPath, string folderName = "")
        {
            string folderPath = HttpContext.Current.Server.MapPath(Path.Combine("~/Uploads/Image", folderName));
            try
            {
                if (File.Exists(Path.Combine(folderPath, imgPath)))
                {
                    return (Path.Combine(folderPath, imgPath));  
                }
                else
                    return null;
            }
            catch (Exception ex) { return null; }
        }
        public static Byte[] GetResume(string resPath)
        {
            string folderPath = HttpContext.Current.Server.MapPath("~/Uploads/Resume");
            try
            {
                if (File.Exists(Path.Combine(folderPath, resPath)))
                    return File.ReadAllBytes(resPath);
                else
                     return null;
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
        public static Byte[] GetText(string txtFilePath)
        {
            string folderPath = HttpContext.Current.Server.MapPath("~/Uploads/Text");
            try
            {
                if (File.Exists(Path.Combine(folderPath, txtFilePath)))
                    return File.ReadAllBytes(txtFilePath);
                else
                    return null;
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
    }
}
