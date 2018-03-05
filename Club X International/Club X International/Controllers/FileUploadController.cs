using Club_X_International.DataConnect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Club_X_International.Controllers
{
    public class FileUploadController : Controller
    {
        private Repository _repo = new Repository();
        // GET: FileUpload
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UploadFile(string id)
        {
            var blog = _repo.FindBlogByID(int.Parse(id));
            if (Request.Files.Count > 0)
            {
                try
                {
                    //Get all files from Request object
                    var files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        if (file.ContentLength > (4 * 1024 * 1024))
                        {
                            ModelState.AddModelError("CustomErrors", "The picture must not be greater than 4MB");
                            return Json("The picture must not be greater than 4MB");
                        }
                        if (!(file.ContentType == "imge/jpeg" || file.ContentType == "imge/png"))
                        {
                            ModelState.AddModelError("CustomErrors", "This image format is not supported use either JPEG or PNG");
                            return Json("This image format is not supported use either JPEG or PNG");
                        }
                        if (blog.BlogPicture != null)
                        {
                            var Dir = Server.MapPath("~/BlogImages");
                            var fileToDel = Path.Combine(Dir, blog.BlogPicture);
                            if (System.IO.File.Exists(fileToDel))
                            {
                                try
                                {
                                    System.IO.File.Delete(fileToDel);
                                }
                                catch (System.IO.IOException e)
                                {
                                    ModelState.AddModelError("CustomErrors", "Picture can't be deleted. pls check with your system adminstrator ");
                                    Console.WriteLine(e.Message);
                                    return Json("Picture can't be deleted. pls check with your system adminstrator");
                                }
                            }
                        }
                        var FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var folderToSaveFile = Server.MapPath("~/BlogImages");
                        var pathToSaveFile = Path.Combine(folderToSaveFile, FileName);
                        file.SaveAs(pathToSaveFile);
                        blog.BlogPicture = FileName;
                        _repo.EditBlog(blog, Session["UserId"].ToString());
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No file selected");
            }
        }
    }
}