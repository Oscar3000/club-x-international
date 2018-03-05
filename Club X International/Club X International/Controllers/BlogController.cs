using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Club_X_International.Models;
using Club_X_International.DataConnect;
using System.IO;
using PagedList;
using Club_X_International.Models.ViewModels;
using System.Data.Entity.Infrastructure;

namespace Club_X_International.Controllers
{
    [Authorize]
    [HandleError(View = "Error")]
    public class BlogController : Controller
    {
        private Repository _repo = new Repository();

        // GET: Blog
        [AllowAnonymous]
        public ActionResult Index(string searchString,string CurrentFilter, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = CurrentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            int PageSize = 6;
            int pageNumber = (page ?? 1);
            ViewData["Blogs"] =  _repo.GetBlogs(searchString).ToPagedList(pageNumber, PageSize);
            return View();
        }

        // GET: Blog/Details/5
        [AllowAnonymous]
        public ActionResult Details(string title,int? id)
        {
            if (title == null || id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = _repo.FindBlogByTitle(title,id);
            ViewData["BlogByWriter"] = _repo.getBlogByWriter(blog.Name,title);
            var viewModels = new ViewModels();
            viewModels.Blog = blog;
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(viewModels);
        }

        // GET: Blog/Create
        public ActionResult Create()
        {
            //var blog = new Blog();
            //blog.BlogID = Guid.NewGuid().ToString();
            return View();
        }

        // POST: Blog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BlogID,Title,BlogContent")] Blog blog, HttpPostedFileBase PostedPicture)
        {
            
            if (ModelState.IsValid)
            {
                var user = _repo.FindUserByID(Session["UserId"].ToString());
                if (user.IsWriter == true)
                {
                    if (PostedPicture != null)
                    {
                        if (PostedPicture.ContentLength > (4 * 1024 * 1024))
                        {
                            ModelState.AddModelError("CustomeError", "The picture is greater than 4MB");
                            return View(blog);
                        }
                        if (!(PostedPicture.ContentType == "image/jpeg" || PostedPicture.ContentType == "image/png"))
                        {
                            ModelState.AddModelError("CustomeError", "The picture must be eithe jpeg or png");
                            return View(blog);
                        }
                        var FileName = Guid.NewGuid().ToString() + Path.GetExtension(PostedPicture.FileName);
                        var FolderToSaveFile = Server.MapPath("~/BlogImages");

                        var PathToSaveFile = Path.Combine(FolderToSaveFile, FileName);
                        PostedPicture.SaveAs(PathToSaveFile);
                        blog.BlogPicture = FileName;
                    }
                    else
                    {
                        ModelState.AddModelError("CustomError", "Picture can't be saved. pls check with your system adminstrator ");
                        return View(blog);
                    }
                    var writer = _repo.FindWriterByName(blog.Name);
                    if (writer != null)
                    {
                        _repo.BlogCreate(blog, user.Id);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("CustomError", "Make Sure the Name has already been created");
                        return View(blog);
                    }
                }
                else
                {
                    ModelState.AddModelError("CustomError", "Error you can't create this post as you are not a writer");
                    return View(blog);
                }
            }

            return View(blog);
        }

        // GET: Blog/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = _repo.FindBlogByID(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BlogID,Title,BlogContent")] Blog blog,HttpPostedFileBase PostedPicture)
        {

            if (ModelState.IsValid)
            {
                var user = _repo.FindUserByID(Session["UserId"].ToString());
                if (user.IsWriter == true)
                {
                    if (PostedPicture != null)
                    {
                        if (PostedPicture.ContentLength > (4 * 1024 * 1024))
                        {
                            ModelState.AddModelError("CustomErrors", "The picture must not be greater than 4MB");
                            return View(blog);
                        }
                        if (!(PostedPicture.ContentType == "imge/jpeg" || PostedPicture.ContentType == "imge/png"))
                        {
                            ModelState.AddModelError("CustomErrors", "This image format is not supported use either JPEG or PNG");
                            return View(blog);
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
                                    return View(blog);
                                }
                            }
                        }
                        var FileName = Guid.NewGuid().ToString() + Path.GetExtension(PostedPicture.FileName);
                        var folderToSaveFile = Server.MapPath("~/BlogImages");
                        var pathToSaveFile = Path.Combine(folderToSaveFile, FileName);
                        PostedPicture.SaveAs(pathToSaveFile);
                        blog.BlogPicture = FileName;
                    }

                    _repo.EditBlog(blog, user.Id);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("CustomErrors", "Error you can't update this post as you are not a writer");
                    return View(blog);
                }
            }
            return View(blog);
        }

        // GET: Blog/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = _repo.FindBlogByID(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Blog blog = _repo.FindBlogByID(id);
            _repo.DeleteBlog(blog);
            return RedirectToAction("Index");
        }

    }
}
