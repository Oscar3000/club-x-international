using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Club_X_International.Models;
using System.IO;
using Club_X_International.DataConnect;

namespace Club_X_International.Controllers
{
    [Authorize]
    [HandleError(View = "Error")]
    public class WriterController : Controller
    {
        private  Repository _repo = new Repository();

        // GET: Writer
        public ActionResult Index()
        {
            return View(_repo.GetWriters());
        }

        // GET: Writer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Writer writer = _repo.FindWriterById(id);
            if (writer == null)
            {
                return HttpNotFound();
            }
            return View(writer);
        }

        // GET: Writer/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Writer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WriterID,Name,Description")] Writer writer,HttpPostedFileBase PostedPicture)
        {
            if (ModelState.IsValid)
            {
                writer.WriterPic = Picture(PostedPicture);
                if (writer.WriterPic != null)
                {
                    var checker =_repo.FindWriterByName(writer.Name);
                    if (checker == null)
                    {
                        _repo.CreateWriter(writer,Session["UserId"].ToString());
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("CustomError", "An Error Occured! Pls make sure the Name is Unique");
                        return View(writer);
                    }
                }
                else
                {
                    ModelState.AddModelError("CustomError", "An Error Occured! Pls Try adding the picture again. Make sure the Picture is less than 4MB");
                    return View(writer);
                }
            }

            return View(writer);
        }

        // GET: Writer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Writer writer = _repo.FindWriterById(id);
            if (writer == null)
            {
                return HttpNotFound();
            }
            return View(writer);
        }

        // POST: Writer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WriterID,Name,Description")] Writer writer, HttpPostedFileBase PostedPicture)
        {
            if (ModelState.IsValid)
            {
                if (PostedPicture != null)
                {
                    if (PostedPicture.ContentLength > (4 * 1024 * 1024))
                    {
                        ModelState.AddModelError("CustomErrors", "The picture must not be greater than 4MB");
                        return View(writer);
                    }
                    if (!(PostedPicture.ContentType == "imge/jpeg" || PostedPicture.ContentType == "imge/png"))
                    {
                        ModelState.AddModelError("CustomErrors", "This image format is not supported use either JPEG or PNG");
                        return View(writer);
                    }
                    if (writer.WriterPic != null)
                    {
                        var Dir = Server.MapPath("~/WriterImages");
                        var fileToDel = Path.Combine(Dir, writer.WriterPic);
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
                                return View(writer);
                            }
                        }
                    }
                    var FileName = Guid.NewGuid().ToString() + Path.GetExtension(PostedPicture.FileName);
                    var folderToSaveFile = Server.MapPath("~/WriterImages");
                    var pathToSaveFile = Path.Combine(folderToSaveFile, FileName);
                    PostedPicture.SaveAs(pathToSaveFile);
                    writer.WriterPic = FileName;
                }
                _repo.EditWriter(writer);
                return RedirectToAction("Index");
            }
            return View(writer);
        }

        // GET: Writer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Writer writer = _repo.FindWriterById(id);
            if (writer == null)
            {
                return HttpNotFound();
            }
            return View(writer);
        }

        // POST: Writer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Writer writer = _repo.FindWriterById(id);

            _repo.DeleteWriter(writer, Session["UserId"].ToString());
            return RedirectToAction("Index");
        }

        public string Picture(HttpPostedFileBase PostedPicture)
        {
            if (PostedPicture != null)
            {
                if (PostedPicture.ContentLength > (4 * 1024 * 1024))
                {
                    ModelState.AddModelError("CustomeErrors", "The picture is greater than 4MB");
                    return null;
                }
                if (!(PostedPicture.ContentType == "image/jpeg" || PostedPicture.ContentType == "image/png"))
                {
                    ModelState.AddModelError("CustomeErrors", "The picture must be eithe jpeg or png");
                    return null;
                }
                var FileName = Guid.NewGuid().ToString() + Path.GetExtension(PostedPicture.FileName);
                var FolderToSaveFile = Server.MapPath("~/WriterImages");

                var PathToSaveFile = Path.Combine(FolderToSaveFile, FileName);
                PostedPicture.SaveAs(PathToSaveFile);

                return FileName;
            }else
            {
                return null;
            }
        }
    }
}
