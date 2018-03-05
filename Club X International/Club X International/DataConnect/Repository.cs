using Club_X_International.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace Club_X_International.DataConnect
{
    public class Repository
    {
        public List<Blog> GetBlogs(string SearchString)
        {
            using(var context = new DataContext())
            {
                if (!(string.IsNullOrEmpty(SearchString)))
                {
                    return context.Blog.AsNoTracking().Include(n => n.writer).Where(n => n.Title.Contains(SearchString) || n.Name.Contains(SearchString)).ToList();
                }

                return context.Blog.AsNoTracking().Include(n => n.writer).OrderByDescending(n=>n.WrittenDate).ToList();
            }
        }

        public ApplicationUser FindUserByID(string id)
        {
            using(var context = new DataContext())
            {
                return context.Users.AsNoTracking().Include(n => n.writer).FirstOrDefault(n => n.Id == id);
            }
        }

        public List<Blog> getBlogByWriter(string writerName,string title)
        {
            using(var context = new DataContext())
            {
                return context.Blog.AsNoTracking().Include(n => n.writer).Where(n => n.Name == writerName && n.Title != title).OrderByDescending(n=>n.WrittenDate).Take(5).ToList();
            }
        }

        public List<Event> GetEvents(string searchString)
        {
            using(var context = new DataContext())
            {
                if (!(string.IsNullOrEmpty(searchString)))
                {
                    return context.Events.AsNoTracking().Where(n => n.Title.Contains(searchString) || n.EventDescription.Contains(searchString)).ToList();
                }
                return context.Events.AsNoTracking().OrderByDescending(n => n.EventID).ToList();
            }
        }

        public List<ApplicationRole> GetRoles()
        {
            var roles = new List<ApplicationRole>();
            using (var context = new DataContext())
            {
                foreach (var role in context.Roles)
                {
                    roles.Add(role as ApplicationRole);
                }
            }
            return roles;
        }

        public List<Writer> GetWriters()
        {
            using(var context = new DataContext())
            {
                return context.Writers.AsNoTracking().ToList();
            }
        }

        public void BlogCreate(Blog blog, string id)
        {
            using(var context = new DataContext())
            {
                var user = FindUserByID(id);
                var writer = FindWriterById(user.writer.WriterID);
                blog.WriterID = writer.WriterID;
                blog.WrittenDate = DateTime.Now;
                context.Blog.Add(blog);
                 context.SaveChanges();
            }
        }

        public void EventCreate(Event @event)
        {
            using(var context = new DataContext())
            {
                context.Events.Add(@event);
                context.SaveChanges();
            }
        }

        public void CreateWriter(Writer writer,string id)
        {
            using(var context = new DataContext())
            {
                var user = FindUserByID(id);
                user.IsWriter = true;
                user.writer = writer;
                context.Writers.Add(writer);
                context.SaveChanges();
            }
        }

        public void EditBlog(Blog blog, string id)
        {
            using(var context = new DataContext())
            {
                var user = FindUserByID(id);
                var writer = FindWriterById(user.writer.WriterID);
                blog.Name = writer.Name;
                blog.WriterID = writer.WriterID;
                //blog.writer = writer;
                context.Entry(blog).State = EntityState.Modified;
                if(blog.BlogPicture == null)
                {
                    context.Entry(blog).Property(n => n.BlogPicture).IsModified = false;
                }
                context.Entry(blog).Property(n => n.WrittenDate).IsModified = false;
                context.SaveChanges();
            }
        }

        public void EditEvent(Event @event)
        {
            using (var context = new DataContext())
            {
                context.Entry(@event).State = EntityState.Modified;
                if (@event.EventPicture == null)
                {
                    context.Entry(@event).Property(n => n.EventPicture).IsModified = false;
                }
                context.SaveChanges();
            }
        }

        public void EditWriter(Writer writer)
        {
            using (var context = new DataContext())
            {
                context.Entry(writer).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public Writer FindWriterByName(string name)
        {
            using(var context = new DataContext())
            {
                return context.Writers.AsNoTracking().FirstOrDefault(n => n.Name == name);
            }
        }

        public Writer FindWriterById(int? id)
        {
            using(var context = new DataContext())
            {
                return context.Writers.AsNoTracking().FirstOrDefault(n => n.WriterID == id);
            }
        }

        public Blog FindBlogByTitle(string title, int? id)
        {
            using(var context = new DataContext())
            {
               return context.Blog.Include(n => n.writer).FirstOrDefault(n => n.Title == title && n.BlogID == id);
            }
        }

        public Blog FindBlogByID( int? id)
        {
            using (var context = new DataContext())
            {
                return context.Blog.Include(n => n.writer).FirstOrDefault(n => n.BlogID == id);
            }
        }

        public Event FindEventById(int? id)
        {
            using(var context = new DataContext())
            {
                return context.Events.FirstOrDefault(n => n.EventID == id);
            }
        }

        public void DeleteEvent(Event @event)
        {
            using(var context = new DataContext())
            {
                context.Entry(@event).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public void DeleteBlog(Blog blog)
        {
            using (var context = new DataContext())
            {
                context.Entry(blog).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }

        public void DeleteWriter(Writer writer,string id)
        {
            using (var context = new DataContext())
            {
                var user = FindUserByID(id);
                user.IsWriter = false;
                user.writer = null;
                context.Entry(writer).State = EntityState.Deleted;
                context.SaveChanges();
            }
        }


    }
}