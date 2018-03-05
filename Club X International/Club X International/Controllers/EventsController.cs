using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Club_X_International.Models;
using Club_X_International.DataConnect;
using PagedList;
using System.IO;

namespace Club_X_International.Controllers
{
    [Authorize]
    [HandleError(View = "Error")]
    public class EventsController : Controller
    {
        private Repository _repo = new Repository();

        // GET: Events
        [AllowAnonymous]
        public ActionResult Index(string searchString, string CurrentFilter, int? page)
        {
            if(searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = CurrentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var Events = _repo.GetEvents(searchString);
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            ViewData["Events"] = Events.ToPagedList(pageNumber, pageSize);
            return View();
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = _repo.FindEventById(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "EventID,Title,EventTime,EventDescription")] Event @event, HttpPostedFileBase PostedPicture)
        {
            if (ModelState.IsValid)
            {
                if (PostedPicture != null)
                {
                    if (PostedPicture.ContentLength > (4 * 1024 * 1024))
                    {
                        ModelState.AddModelError("CustomeErrors", "The picture is greater than 4MB");
                        return View(@event);
                    }
                    if (!(PostedPicture.ContentType == "image/jpeg" || PostedPicture.ContentType == "image/png"))
                    {
                        ModelState.AddModelError("CustomeErrors", "The picture must be eithe jpeg or png");
                        return View(@event);
                    }
                    var FileName = Guid.NewGuid().ToString() + Path.GetExtension(PostedPicture.FileName);
                    var FolderToSaveFile = Server.MapPath("~/EventImages");

                    var PathToSaveFile = Path.Combine(FolderToSaveFile, FileName);
                    PostedPicture.SaveAs(PathToSaveFile);
                    @event.EventPicture = FileName;
                }
                else
                {
                    ModelState.AddModelError("CustomErrors", "Pls Make sure you are adding a picture that below 4MB. pls check with your system adminstrator ");
                    return View(@event);
                }
                _repo.EventCreate(@event);
                return RedirectToAction("Index");
            }

            return View(@event);
        }

        // GET: Events/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = _repo.FindEventById(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EventID,Title,EventTime,EventDescription")] Event @event, HttpPostedFileBase PostedPicture)
        {
            if (ModelState.IsValid)
            {
                if (PostedPicture != null)
                {
                    if (PostedPicture.ContentLength > (4 * 1024 * 1024))
                    {
                        ModelState.AddModelError("CustomErrors", "The picture must not be greater than 4MB");
                        return View(@event);
                    }
                    if (!(PostedPicture.ContentType == "image/jpeg" || PostedPicture.ContentType == "image/png"))
                    {
                        ModelState.AddModelError("CustomErrors", "This image format is not supported use either JPEG or PNG");
                        return View(@event);
                    }
                    if (@event.EventPicture != null)
                    {
                        var Dir = Server.MapPath("~/EventImages");
                        var fileToDel = Path.Combine(Dir, @event.EventPicture);
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
                                return View(@event);
                            }
                        }
                    }
                    var FileName = Guid.NewGuid().ToString() + Path.GetExtension(PostedPicture.FileName);
                    var folderToSaveFile = Server.MapPath("~/EventImages");
                    var pathToSaveFile = Path.Combine(folderToSaveFile, FileName);
                    PostedPicture.SaveAs(pathToSaveFile);
                    @event.EventPicture = FileName;
                    _repo.EditEvent(@event);
                    return RedirectToAction("Index");
                }
                else
                {
                    _repo.EditEvent(@event);
                    return RedirectToAction("Index");
                }
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = _repo.FindEventById(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Event @event = _repo.FindEventById(id);
            _repo.DeleteEvent(@event);
            return RedirectToAction("Index");
        }

    }
}
