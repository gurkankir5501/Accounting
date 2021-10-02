using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Accounting.Models;
using Newtonsoft.Json.Linq;

namespace Accounting.Controllers
{
    public class ReconciliationsController : Controller
    {
        private arisanerpEntities db = new arisanerpEntities();

        /*
         -------- Açıklamalar -----------
                Özellikleri
         status= 0 ise checkBox açık durumda
         status=1  && sentStatus= false ise gönderim yapıldı ama herhangibir sebepten dolayı gönderilemedi. checkbox açık durumda
         status=1 && sentStatus = true ise gönderim başarılı bir şekilde gerçekleşti. 
         
         processStatus =false ise işlem kapalı
         processStatus =true ise işlem açık   ------ gönderilen kişinin işlem yapıp yapmama ksııtlamasını kontrol eder.

         status=2 ise mutabık olundu. durum kısmı mutabık olundu yazılır. checkbox kapalı durumda


         status=3 ise mutabık olunmadı. durum kısmı mutabık olunmadı yazılır .Gönderim yapılan kişiden gelen mesaj ve belgeler varsa gözükür. ek olarak gönderim yapan kişi belge ve mesaj göndermek ister ise gönderir.
         tekrar status 0 konumuna gidilir
         

        *****************
        Fatura Mutabakatında 5000 Tlden alt olanlar için uyarı versin!!!
        status = 5 ise fatura 5000 altı
         */


        // GET: Reconciliations
        public ActionResult Index(int id)
        {

            /////////////////////////
            var AccessControl = Session["AccessControl"];
            if (AccessControl == null)
            {
                return RedirectToAction("login", "Home");
            }
            /////////////////////////


            ViewBag.customer = db.Customers.ToList();
            ViewBag.agreementID = id;
            var agreement = db.Agreement.Where(s => s.agreementID == id).FirstOrDefault();
            ViewBag.type = agreement.agreementType;
            ViewBag.buyingSales = agreement.buyingSales;
            var reconciliations = db.Reconciliations.Where(s => s.agreementID == id).ToList();
            return View(reconciliations);
        }

        // GET: Reconciliations/Details/5
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
            Reconciliations reconciliations = db.Reconciliations.Find(id);
            if (reconciliations == null)
            {
                return HttpNotFound();
            }
            ViewBag.agreementID = reconciliations.agreementID;

            if (reconciliations.taxNumber != null)
            {
                ViewBag.customer = db.Customers.Where(s => s.taxNumber == reconciliations.taxNumber).FirstOrDefault();
            }
            else if (reconciliations.tcNumber != null)
            {
                ViewBag.customer = db.Customers.Where(s => s.tcNumber == reconciliations.tcNumber).FirstOrDefault();
            }
            ViewBag.document = reconciliations.ReconciliationsDocument.ToList();

            return View(reconciliations);
        }

        // alt klasörleri oluşturma fonksiyonu 
        public void subDirectoryCreate(String buyingSales, String agreementType, Reconciliations recon, int agreementID)
        {
            // ana mutabakat sizinini veriyoruz
            String path = "/" + "DATA" + "/" + buyingSales + "/" + agreementType + "/" + agreementID.ToString();

            // böyle bir dizin var mı kontrol ediyoruz
            if (Directory.Exists(Server.MapPath(path)))
            {

                //yeni alt mutabakat IDsi ile yeni bir klasör oluşturuyoruz
                String reconDirectoryPath = path + "/" + recon.reconID.ToString() + "/";
                Directory.CreateDirectory(Server.MapPath(reconDirectoryPath));
                // Oluşturduğumuz alt mutabakat klasörünün altına set-get klasörleri oluşturuyoruz.
                Directory.CreateDirectory(Server.MapPath(reconDirectoryPath + "set" + "/"));
                Directory.CreateDirectory(Server.MapPath(reconDirectoryPath + "get" + "/"));

            }
        }

        // GET: Reconciliations/Create
        public ActionResult Create(int id)
        {

            /////////////////////////
            var AccessControl = Session["AccessControl"];
            if (AccessControl == null)
            {
                return RedirectToAction("login", "Home");
            }
            /////////////////////////

            ViewBag.agreementID = id;
            Reconciliations reconciliations = new Reconciliations() { agreementID = id };
            return View(reconciliations);
        }

        // POST: Reconciliations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "reconID,title,country,taxNumber,documentNumber,amountService,phoneNumber,tcNumber,representative,customer,status,processStatus,agreementID,sentStatus")] Reconciliations reconciliations)
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

                if (reconciliations.title != null && reconciliations.phoneNumber != null && (reconciliations.taxNumber != null || reconciliations.tcNumber != null))
                {
                    reconciliations.sentStatus = false;
                    reconciliations.processStatus = false;
                    reconciliations.status = 0;
                    db.Reconciliations.Add(reconciliations);
                    db.SaveChanges();

                    try
                    {
                        var newRecon = db.Reconciliations.Find(reconciliations.reconID);
                        var agreement = db.Agreement.Find(reconciliations.agreementID);

                        if (agreement.buyingSales == true && agreement.agreementType == true)
                        {
                            subDirectoryCreate("Buying", "cariMutabakat", newRecon, agreement.agreementID);
                        }
                        else if (agreement.buyingSales == true && agreement.agreementType == false)
                        {
                            subDirectoryCreate("Buying", "faturaMutabakat", newRecon, agreement.agreementID);
                        }
                        else if (agreement.buyingSales == false && agreement.agreementType == true)
                        {
                            subDirectoryCreate("Sales", "cariMutabakat", newRecon, agreement.agreementID);
                        }
                        else if (agreement.buyingSales == false && agreement.agreementType == false)
                        {
                            subDirectoryCreate("Sales", "faturaMutabakat", newRecon, agreement.agreementID);
                        }

                        return RedirectToAction("Index", new { id = reconciliations.agreementID });
                    }
                    catch (Exception)
                    {

                        db.Reconciliations.Remove(reconciliations);
                        db.SaveChanges();
                        ViewBag.res = 6;
                        ViewBag.agreementID = reconciliations.agreementID;
                        return View(reconciliations);
                    }
                }
                else
                {
                    ViewBag.res = 6;
                    ViewBag.agreementID = reconciliations.agreementID;
                    return View(reconciliations);
                }

            }
            ViewBag.res = 6;
            ViewBag.agreementID = reconciliations.agreementID;
            return View(reconciliations);
        }

        // GET: Reconciliations/Edit/5
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
            Reconciliations reconciliations = db.Reconciliations.Find(id);
            if (reconciliations == null)
            {
                return HttpNotFound();
            }
            ViewBag.agreementID = reconciliations.agreementID;
            return View(reconciliations);
        }

        // POST: Reconciliations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "reconID,title,country,taxNumber,documentNumber,amountService,phoneNumber,tcNumber,representative,customer,status,processStatus,agreementID,sentStatus")] Reconciliations reconciliations)
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
                if (reconciliations.title != null && reconciliations.phoneNumber != null && (reconciliations.taxNumber != null || reconciliations.tcNumber != null))
                {
                    var item = db.Reconciliations.Find(reconciliations.reconID);
                    item.title = reconciliations.title;
                    item.country = reconciliations.country;
                    item.taxNumber = reconciliations.taxNumber;
                    item.documentNumber = reconciliations.documentNumber;
                    item.amountService = reconciliations.amountService;
                    item.phoneNumber = reconciliations.phoneNumber;
                    item.tcNumber = reconciliations.tcNumber;
                    item.representative = reconciliations.representative;
                    item.customer = reconciliations.customer;
                    db.SaveChanges();

                    return RedirectToAction("Index", new { id = item.agreementID });
                }
                else
                {
                    ViewBag.res = 6;
                    ViewBag.agreementID = reconciliations.agreementID;
                    return View(reconciliations);
                }

            }
            ViewBag.res = 6;
            ViewBag.agreementID = reconciliations.agreementID;
            return View(reconciliations);
        }

        // GET: Reconciliations/Delete/5
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
            Reconciliations reconciliations = db.Reconciliations.Find(id);
            if (reconciliations == null)
            {
                return HttpNotFound();
            }
            return View(reconciliations);
        }

        // POST: Reconciliations/Delete/5
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

            Reconciliations reconciliations = db.Reconciliations.Find(id);

            //dosya silme kodu
            //System.IO.File.Delete(path);
            String subDirectoryPath = "";
            if (reconciliations.Agreement.agreementType == true && reconciliations.Agreement.buyingSales == true)
            {
                subDirectoryPath = Server.MapPath("/DATA/Buying/cariMutabakat/" + reconciliations.Agreement.agreementID.ToString() + "/" + reconciliations.reconID.ToString());
            }
            else if (reconciliations.Agreement.agreementType == false && reconciliations.Agreement.buyingSales == true)
            {
                subDirectoryPath = Server.MapPath("/DATA/Buying/faturaMutabakat/" + reconciliations.Agreement.agreementID.ToString() + "/" + reconciliations.reconID.ToString());
            }
            else if (reconciliations.Agreement.agreementType == true && reconciliations.Agreement.buyingSales == false)
            {
                subDirectoryPath = Server.MapPath("/DATA/Sales/cariMutabakat/" + reconciliations.Agreement.agreementID.ToString() + "/" + reconciliations.reconID.ToString());
            }
            else if (reconciliations.Agreement.agreementType == false && reconciliations.Agreement.buyingSales == false)
            {
                subDirectoryPath = Server.MapPath("/DATA/Sales/faturaMutabakat/" + reconciliations.Agreement.agreementID.ToString() + "/" + reconciliations.reconID.ToString());
            }

            // Bu dizinde aynı isimde dosya var mı kontrol edilir.
            //varsa siliniyor

            if (Directory.Exists(subDirectoryPath))
            {
                //varsa siliniyor
                Directory.Delete(subDirectoryPath, true);
            }
            db.Reconciliations.Remove(reconciliations);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = reconciliations.agreementID });
        }

        public void ReconSent(Customers customer, Reconciliations recon)
        {
            //mail adresi control edilir
            if (customer.customerDescription != null)
            {
                var ID = HttpUtility.HtmlEncode(recon.reconID);
                mailSent mail = new mailSent()
                {
                    agreementType = recon.Agreement.agreementType,
                    amountService = recon.amountService,
                    SentMailAdress = customer.customerDescription,
                    buyingSales = recon.Agreement.buyingSales,
                    documentNumber = recon.documentNumber,
                    month = recon.Agreement.month,
                    year = recon.Agreement.year,
                    reconID = ID,
                    taxNumber = recon.taxNumber,
                    tcNumber = recon.tcNumber,
                    title = recon.title
                };

                mail.mailInformation();
                //mail başarılı bir şekilde gönderildiyse
                if (mail.Sent() == true)
                {

                    recon.sentStatus = true;
                    recon.status = 1;
                    recon.processStatus = true;
                    db.SaveChanges();
                }
                else // gönderilemediyse
                {
                    recon.sentStatus = false;
                    recon.status = 1;
                    recon.processStatus = false;
                    db.SaveChanges();
                }
            }
            else
            {
                recon.sentStatus = false;
                recon.status = 1;
                recon.processStatus = false;
                db.SaveChanges();
            }
        }

        public JsonResult Sent(string data)
        {

            if (data != null)
            {
                dynamic getData = JObject.Parse(data);
                foreach (var item in getData.data)
                {
                    //mutabakat bulunur
                    Reconciliations recon = db.Reconciliations.Find(Convert.ToInt32(item));

                    //cari Mutabakat bütün faturalar gidecek
                    if (recon.Agreement.agreementType == true)
                    {
                        //vergi numarası varsa 
                        if (recon.taxNumber != null)
                        {
                            var customer = db.Customers.Where(s => s.taxNumber == recon.taxNumber).FirstOrDefault();
                            if (customer != null)
                            {
                                ReconSent(customer, recon);
                            }
                            else
                            {
                                recon.sentStatus = false;
                                recon.status = 1;
                                recon.processStatus = false;
                                db.SaveChanges();
                            }

                        }
                        else if (recon.tcNumber != null) // tc numarası varsa
                        {
                            var customer = db.Customers.Where(s => s.tcNumber == recon.tcNumber).FirstOrDefault();
                            if (customer != null)
                            {
                                ReconSent(customer, recon);
                            }
                            else
                            {
                                recon.sentStatus = false;
                                recon.status = 1;
                                recon.processStatus = false;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            recon.sentStatus = false;
                            recon.status = 1;
                            recon.processStatus = false;
                            db.SaveChanges();
                        }
                    }
                    else   //Fatura Mutabakatı 5.000 Tl üstü faturalar gidecek.
                    {
                        // toplam fatura 5000 TLden bütük yada eşit ise gönder
                        if (recon.amountService >= 5000)
                        {
                            //vergi numarası varsa 
                            if (recon.taxNumber != null)
                            {
                                var customer = db.Customers.Where(s => s.taxNumber == recon.taxNumber).FirstOrDefault();
                                if (customer != null)
                                {
                                    ReconSent(customer, recon);
                                }
                                else
                                {
                                    recon.sentStatus = false;
                                    recon.status = 1;
                                    recon.processStatus = false;
                                    db.SaveChanges();
                                }

                            }
                            else if (recon.tcNumber != null) // tc numarası varsa
                            {
                                var customer = db.Customers.Where(s => s.tcNumber == recon.tcNumber).FirstOrDefault();
                                if (customer != null)
                                {
                                    ReconSent(customer, recon);
                                }
                                else
                                {
                                    recon.sentStatus = false;
                                    recon.status = 1;
                                    recon.processStatus = false;
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                recon.sentStatus = false;
                                recon.status = 1;
                                recon.processStatus = false;
                                db.SaveChanges();
                            }
                        }
                        else
                        {
                            recon.status = 6;
                            recon.processStatus = false;
                            db.SaveChanges();
                        }

                    }



                }

                return Json(new { res = 1 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { res = 0 }, JsonRequestBehavior.AllowGet);
            }

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
