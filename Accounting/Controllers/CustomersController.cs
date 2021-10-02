using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Accounting.Models;

namespace Accounting.Controllers
{
    public class CustomersController : Controller
    {
        private arisanerpEntities db = new arisanerpEntities();

        // GET: Customers
        public ActionResult Index()
        {
            /////////////////////////
            var AccessControl = Session["AccessControl"];
            if (AccessControl == null)
            {
                return RedirectToAction("login", "Home");
            }
            /////////////////////////

            var customers = db.Customers.Include(c => c.Cities).Include(c => c.Countries);
            return View(customers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {

            /////////////////////////
            var AccessControl = Session["AccessControl"];
            if (AccessControl == null)
            {
                return RedirectToAction("login", "Home");
            }
            /////////////////////////

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = db.Customers.Find(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

    
        // GET: Customers/Create
        public ActionResult Create(String id)
        {


            /////////////////////////
            var AccessControl = Session["AccessControl"];
            if (AccessControl == null)
            {
                return RedirectToAction("login", "Home");
            }
            /////////////////////////

            if (id == null)
            {
                ViewBag.cityID = new SelectList(db.Cities, "cityID", "cityName");
                ViewBag.countryID = new SelectList(db.Countries, "countryID", "countryName");
                return View();
            }
            else
            {
                var item = db.Reconciliations.Find(Convert.ToInt32(id));
                Customers customers = new Customers();
                customers.customerCompanyName = item.title;
                customers.customerGSM = item.phoneNumber;
                customers.taxNumber = item.taxNumber;
                customers.tcNumber = item.tcNumber;

                ViewBag.agreementID = item.agreementID;
                ViewBag.cityID = new SelectList(db.Cities, "cityID", "cityName");
                ViewBag.countryID = new SelectList(db.Countries, "countryID", "countryName");
                return View(customers);
            }
           

           
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "customerID,customerCompanyName,customerGSM,customerPhone,customerName,customerAddress,customerDescription,cityID,countryID,taxNumber,tcNumber")] Customers customers , string agreementID)
        {


            /////////////////////////
            var AccessControl = Session["AccessControl"];
            if (AccessControl == null)
            {
                return RedirectToAction("login", "Home");
            }
            /////////////////////////

            if (ModelState.IsValid)
            {
                if (customers.customerCompanyName != null && customers.customerGSM != null && customers.customerName != null && customers.customerDescription != null && customers.taxNumber != null && customers.tcNumber != null)
                {
                    db.Customers.Add(customers);
                    db.SaveChanges();
                    if(agreementID != null)
                    {
                        return RedirectToAction("Index","Reconciliations",new {id=(Convert.ToInt32(agreementID)) });
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                   
                }
                else
                {
                    ViewBag.res = 6;
                    ViewBag.agreementID = agreementID;
                    ViewBag.cityID = new SelectList(db.Cities, "cityID", "cityName", customers.cityID);
                    ViewBag.countryID = new SelectList(db.Countries, "countryID", "countryName", customers.countryID);
                    return View(customers);
                }
            }
            ViewBag.res = 6;
            ViewBag.agreementID = agreementID;
            ViewBag.cityID = new SelectList(db.Cities, "cityID", "cityName", customers.cityID);
            ViewBag.countryID = new SelectList(db.Countries, "countryID", "countryName", customers.countryID);
            return View(customers);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {

            /////////////////////////
            var AccessControl = Session["AccessControl"];
            if (AccessControl == null)
            {
                return RedirectToAction("login", "Home");
            }
            /////////////////////////

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = db.Customers.Find(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            ViewBag.cityID = new SelectList(db.Cities, "cityID", "cityName", customers.cityID);
            ViewBag.countryID = new SelectList(db.Countries, "countryID", "countryName", customers.countryID);
            return View(customers);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "customerID,customerCompanyName,customerGSM,customerPhone,customerName,customerAddress,customerDescription,cityID,countryID,taxNumber,tcNumber")] Customers customers)
        {

            /////////////////////////
            var AccessControl = Session["AccessControl"];
            if (AccessControl == null)
            {
                return RedirectToAction("login", "Home");
            }
            /////////////////////////

            if (ModelState.IsValid)
            {
                if (customers.customerCompanyName != null && customers.customerGSM != null && customers.customerName != null && customers.customerDescription != null && customers.taxNumber != null && customers.tcNumber != null)
                {
                    var item = db.Customers.Find(customers.customerID);
                    item.customerCompanyName = customers.customerCompanyName;
                    item.customerGSM = customers.customerGSM;
                    item.customerName = customers.customerName;
                    item.customerDescription = customers.customerDescription;
                    item.taxNumber = customers.taxNumber;
                    item.tcNumber = customers.tcNumber;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.res = 6;
                    ViewBag.cityID = new SelectList(db.Cities, "cityID", "cityName", customers.cityID);
                    ViewBag.countryID = new SelectList(db.Countries, "countryID", "countryName", customers.countryID);
                    return View(customers);
                }
           
               
            }
            ViewBag.cityID = new SelectList(db.Cities, "cityID", "cityName", customers.cityID);
            ViewBag.countryID = new SelectList(db.Countries, "countryID", "countryName", customers.countryID);
            return View(customers);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {

            /////////////////////////
            var AccessControl = Session["AccessControl"];
            if (AccessControl == null)
            {
                return RedirectToAction("login", "Home");
            }
            /////////////////////////

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customers customers = db.Customers.Find(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {


            /////////////////////////
            var AccessControl = Session["AccessControl"];
            if (AccessControl == null)
            {
                return RedirectToAction("login", "Home");
            }
            /////////////////////////

            Customers customers = db.Customers.Find(id);
            db.Customers.Remove(customers);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
