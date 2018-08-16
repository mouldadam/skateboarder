using SkateboarderMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Net;

namespace SkateboarderMVC.Controllers
{

    public class HomeController : Controller
    {

        private SkateboarderEntities db = new SkateboarderEntities();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

   
   
        public ActionResult Index(string searchString, string skateboarderStyle, string sortOrder)
        {

            // creating a Selectlist for the style dropdown list
            List<string> styleList = new List<string>();
            //Getting the styles from db in order 
            var styleQuery = from g in db.Skateboarders
                             orderby g.Style
                             select g.Style;
            //adding  unique styles to list
            styleList.AddRange(styleQuery.Distinct());
            // creating a selectlist putting it into view bag so view can find it
            ViewBag.skateboarderStyle = new SelectList(styleList);
            // LINQ query to select all  the records from the db

            var skateboarder = from d in db.Skateboarders select d;
            // filtering by style
            if (!String.IsNullOrEmpty(skateboarderStyle))
            {
                skateboarder = skateboarder.Where(x => x.Style == skateboarderStyle);
            }
            // is the search box is not empty filter by search box text . searching by Name
            if (!String.IsNullOrEmpty(searchString))
            {
         
                        skateboarder = skateboarder.Where(x => x.Name.Contains(searchString)); ;
            }

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.LikeSortParm = sortOrder == "likes" ? "likes_desc" : "likes";
           
            switch (sortOrder)
            {
                case "name_desc":
                    skateboarder = skateboarder.OrderByDescending(s => s.Name);
                    break;
                case "likes":
                    skateboarder = skateboarder.OrderBy(s => s.Likes);
                    break;
                case "likes_desc":
                    skateboarder = skateboarder.OrderByDescending(s => s.Likes);
                    break;
                default:
                    skateboarder = skateboarder.OrderBy(s => s.Name);
                    break;
            }
          
            return View(skateboarder.ToList());

        }

        [HttpPost]
        public ActionResult incrementLike(int id)
        {
            Skateboarder skateboarder = db.Skateboarders.Find(id);
            //var skateboarder = new Skateboarder { Id = 1 };
            skateboarder.Likes += 1;
            db.Entry(skateboarder).Property("Likes").IsModified = true;
            db.SaveChanges();
            return PartialView("likeView",skateboarder);
        }

        //[HttpPost]
        //public ActionResult Likes(int id)
        //{
        //    Skateboarder skateboarder = db.Skateboarders.Find(id);
        //    //var skateboarder = new Skateboarder { Id = 1 };
        //    skateboarder.Likes += 1;
        //    db.Entry(skateboarder).Property("Likes").IsModified = true;
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

            [HttpPost]
        public ActionResult incrementDislike(int id)
        {
            Skateboarder skateboarder = db.Skateboarders.Find(id);
            //var skateboarder = new Skateboarder { Id = 1 };
            skateboarder.Dislikes += 1;
            db.Entry(skateboarder).Property("Dislikes").IsModified = true;
            db.SaveChanges();
            return PartialView("dislikeView", skateboarder);
        }
        public ActionResult Create()
        {
           
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,HomeTown,DateOfBirth,Stance,Status,Style,ImageUrl,VideoHighlightUrl,Likes,Dislike,WheelSponsor,BoardSponsor")]Skateboarder skateboarder)
        {
            if(skateboarder.ImageUrl == null)
            {
                skateboarder.ImageUrl = "http://www.getdrawings.com/img/skateboarder-silhouette-clip-art-19.jpg";
            }
            if (ModelState.IsValid)
            {
                db.Skateboarders.Add(skateboarder);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(skateboarder);
        }

        public ActionResult Edit(int? id)
        {
            // if a null id is passed display an HTML error message
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Skateboarder skateboarder = db.Skateboarders.Find(id);
            // if an invalid id was passed in and the record does not 
            // exist in the database, display an HTML error message
            if (skateboarder == null)
            {
                return HttpNotFound();
            }
            return View(skateboarder);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,HomeTown,DateOfBirth,Stance,Status,Style,ImageUrl,VideoHighlightUrl,Likes,Dislike,WheelSponsor,BoardSponsor")]Skateboarder skateboarder)
        {
            if (skateboarder.ImageUrl == null)
            {
                skateboarder.ImageUrl = "http://getdrawings.com/img/skateboarder-silhouette-clip-art-19.jpg";
            }
            if (ModelState.IsValid)
            {
                db.Entry(skateboarder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(skateboarder);
        }

        public ActionResult Details(int? id)
        {
            // if a null id is passed display an HTML error message
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // gets the record from the database using its id
            Skateboarder skateboarder = db.Skateboarders.Find(id);
            // if an invalid id was passed in and the record does not 
            // exist in the database, display an HTML error message
            if (skateboarder == null)
            {
                return HttpNotFound();
            }
            //pass the data to details view to be displayed
            return View(skateboarder);
        }

        public ActionResult Delete(int? id)
        {
            // if a null id is passed display an HTML error message
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Skateboarder skateboarder = db.Skateboarders.Find(id);
            // if an invalid id was passed in and the record does not 
            // exist in the database, display an HTML error message
            if (skateboarder == null)
            {
                return HttpNotFound();
            }
            return View(skateboarder);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Skateboarder skateboarder = db.Skateboarders.Find(id);
            db.Skateboarders.Remove(skateboarder);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }


}