using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using ZHungers.DB; 

namespace ZHungers.Controllers
{
    public class ZhungerController : Controller
    {

        // GET: Zhunger
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(string email, string pass, string usertype)
        {
            var db = new ZhungerEntities();

            if (usertype.Equals("Admin"))
            {

                var us = (from u in db.Admins
                          where u.email.Equals(email) && u.pass.Equals(pass)
                          select u).SingleOrDefault();
                if (us != null)
                { 
                    Session["id"] = us.Id;
                    var allCollectRequests = db.Food_callections.ToList();

                    foreach (var item in allCollectRequests)
                    {
                        var b = DateTime.Now;
                        if (((TimeSpan)(b - item.PlacingDate)).Days > 3 && item.status != "Delivered")
                        {
                            item.status = "Wasted";
                        }
                        // else { item.status = something(); }
                    }
                    db.SaveChanges();
                    return RedirectToAction("Admin");
                }
            }
            else if (usertype.Equals("Employee"))
            {

                var us = (from u in db.Employes
                          where u.email.Equals(email) && u.pass.Equals(pass)
                          select u).SingleOrDefault();
                if (us != null)
                {
                    Session["id"] = us.id;
                    return RedirectToAction("Employe");
                }
            }
            else if (usertype.Equals("Restaurant"))
            {
                var us = (from u in db.Restaurants
                          where u.email.Equals(email) && u.pass.Equals(pass)
                          select u).SingleOrDefault();
                if (us != null)
                {
                    Session["id"] = us.id;
                    return RedirectToAction("Restaurant");
                }
            }
            return View();
        }
        public ActionResult Admin()
        {
            var db = new ZhungerEntities();
            var col = db.Food_callections.ToList();
            
            return View(col);
        }

        [HttpGet]
        public ActionResult Aupdate(int id)
        {
            var db = new ZhungerEntities();
            var product = (from p in db.Food_callections
                           where p.id == id
                           select p).SingleOrDefault();
            return View(product);
        }

        [HttpPost]
        public ActionResult Aupdate(Food_callections pd)
        {
            var db = new ZhungerEntities();
            var ext = (from p in db.Food_callections
                       where p.id == pd.id
                       select p).SingleOrDefault();
            db.Entry(ext).CurrentValues.SetValues(pd);
            db.SaveChanges();
            return RedirectToAction("Admin");
        }
         public ActionResult DeleteProduct(int id)
        {
            var db = new ZhungerEntities();
            var product = (from p in db.Food_callections
                           where p.id == id
                           select p).SingleOrDefault();
            db.Food_callections.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Admin");
        }


        public ActionResult Restaurant()
        {
            var db = new ZhungerEntities();
            var col = db.Food_callections.ToList();
            var collections = (from c in col
                               where c.r_id == (int)Session["id"]
                               select c).ToList();
            return View(collections);
        }
        [HttpGet]
        public ActionResult createCollection()
        {
            return View();
        }
        [HttpPost]
        public ActionResult createCollection(Food_callections f)
        {
            f.r_id = (int)Session["id"];
            f.status = "Not Collected";
            var db = new ZhungerEntities();
            db.Food_callections.Add(f);
            db.SaveChanges();
            return RedirectToAction("Restaurant");
        }

        public ActionResult Employe()
        {
            var db = new ZhungerEntities();
            var col = db.Food_callections.ToList();
            var collections = (from c in col
                               where c.e_id== (int)Session["id"]
                               select c).ToList();
            return View(collections);
        }

        [HttpGet]
        public ActionResult Eupdate(int id)
        {
            var db = new ZhungerEntities();
            var product = (from p in db.Food_callections
                           where p.id == id
                           select p).SingleOrDefault();
            return View(product);
        }

        [HttpPost]
        public ActionResult Eupdate(Food_callections pd)
        {
            var db = new ZhungerEntities();
            var ext = (from p in db.Food_callections
                       where p.id == pd.id
                       select p).SingleOrDefault();
            db.Entry(ext).CurrentValues.SetValues(pd);
            db.SaveChanges();
            return RedirectToAction("Employe");
        }

        

        [HttpGet]
        public ActionResult AssignEmployees(int id)
        {
            var db = new ZhungerEntities();
            Session["CollectReqId"] = id;
            return View(db.Employes.ToList());
        }
        [HttpPost]
        public ActionResult AssignEmployees(int[] Employe)
        {
            var db = new ZhungerEntities();
            var collectreqid = Int32.Parse(Session["collectreqid"].ToString());
            var ext = (from p in db.Food_callections
                       where p.id == collectreqid
                       select p).SingleOrDefault();

            ext.e_id = Employe[0];
            db.SaveChanges();
            return RedirectToAction("Admin");
        }
        [HttpGet]
        public ActionResult Statusup(int id)
        {

           
            Session["CollectReqId"] = id;
            return View();
        }
        [HttpPost]
        public ActionResult Statusup(Food_callections pd)
        {
            var db = new ZhungerEntities();
            var collectreqid = Int32.Parse(Session["collectreqid"].ToString());
            var ext = (from p in db.Food_callections
                       where p.id == collectreqid
                       select p).SingleOrDefault();

            ext.status = pd.status;
            db.SaveChanges();
            return RedirectToAction("Employe");
        }




        public ActionResult EmptList()
        {
            var db = new ZhungerEntities();
            var em = db.Employes.ToList();
            return View(em);
        }

        [HttpGet]
        public ActionResult AddEmloyee()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEmloyee(Employe pd)
        {
            var db = new ZhungerEntities();
            db.Employes.Add(pd);
            db.SaveChanges();
            return RedirectToAction("EmptList");
        }

        public ActionResult RestList()
        {
            var db = new ZhungerEntities();
            var em = db.Restaurants.ToList();
            return View(em);
        }

        [HttpGet]
        public ActionResult AddRestaurent()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddRestaurent(Restaurant pd)
        {
            var db = new ZhungerEntities();
            db.Restaurants.Add(pd);
            db.SaveChanges();
            return RedirectToAction("RestList");
        }


    }
}