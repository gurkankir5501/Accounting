using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Accounting.Models;

namespace Accounting.Controllers
{

    public class HomeController : Controller
    {
        private arisanerpEntities db = new arisanerpEntities();



        public ActionResult Index()
        {
            

            var AccessControl = Session["AccessControl"];

            if (AccessControl == null)
            {
                return RedirectToAction("login");
            }

            return View();
        }

        public ActionResult Buying()
        {
            var AccessControl = Session["AccessControl"];

            if (AccessControl == null)
            {
                return RedirectToAction("login");
            }
            return View();
        }

        public ActionResult Sales()
        {
            var AccessControl = Session["AccessControl"];

            if (AccessControl == null)
            {
                return RedirectToAction("login");
            }
            return View();
        }

        [HttpGet]
        public ActionResult login()
        {

            return View();
        }

        [HttpPost]
        public ActionResult login(String userName, String password)
        {
            var users = db.Users.Where(s => s.userName == userName && s.userPassword == password && s.userRoleID == 3).FirstOrDefault();

            if (users != null)
            {

                Session["AccessControl"] = users;
                return RedirectToAction("index");
            }
            ViewBag.userName = userName;
            ViewBag.password = password;
            ViewBag.res = 10;
            return View();
        }

        public ActionResult logOut()
        {
            Session["AccessControl"] = null;
            return RedirectToAction("index");
            
        }
    }
}