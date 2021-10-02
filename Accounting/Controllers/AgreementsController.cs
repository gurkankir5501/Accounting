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
using Excel = Microsoft.Office.Interop.Excel;
namespace Accounting.Controllers
{
    public class AgreementsController : Controller
    {
        private arisanerpEntities db = new arisanerpEntities();

        // GET: Agreements
        public ActionResult Index(bool type, bool buyingSales)
        {
            /////////////////////////
            var AccessControl = Session["AccessControl"];
            if (AccessControl == null)
            {
                return RedirectToAction("login", "Home");
            }
            /////////////////////////
            var item = db.Agreement.Where(s => s.agreementType == type && s.buyingSales == buyingSales).ToList();

            ViewBag.type = type;
            ViewBag.buyingSales = buyingSales;
            return View(item);
        }

        // alt klasörleri oluşturma fonksiyonu 
        public void subDirectoryCreate(String buyingSales, String agreementType, int agreementID)
        {
            // ana mutabakat sizinini veriyoruz
            String path = "/" + "DATA" + "/" + buyingSales + "/" + agreementType + "/" + agreementID.ToString();

            // böyle bir dizin var mı kontrol ediyoruz
            if (Directory.Exists(Server.MapPath(path)))
            {
                //Ana mutabakat altındaki alt mutabakatları buluyoruz
                var reconciliations = db.Reconciliations.Where(s => s.agreementID == agreementID).ToList();

                // her alt mutabakatı dolaşıyoruz 
                foreach (var item in reconciliations)
                {

                    //her alt mutabakat IDsi ile yeni bir klasör oluşturuyoruz
                    String reconDirectoryPath = path + "/" + item.reconID.ToString() + "/";
                    Directory.CreateDirectory(Server.MapPath(reconDirectoryPath));
                    // Oluşturduğumuz alt mutabakat klasörünün altına set-get klasörleri oluşturuyoruz.
                    Directory.CreateDirectory(Server.MapPath(reconDirectoryPath + "set" + "/"));
                    Directory.CreateDirectory(Server.MapPath(reconDirectoryPath + "get" + "/"));

                }
            }
        }


        //Alış-Cari Mutabakat
        public bool BuyingCariMutabakatCreate(Agreement agreement, HttpPostedFileBase document)
        {

            //Dosya ismi alınır
            var documentName = Path.GetFileName(document.FileName);

            //dosyanın nereye , hangi isimli kayıt edileceği alınır
            var documentPath = Path.Combine(Server.MapPath("/DATA/Buying/cariMutabakat"), documentName);

            //dosya silme kodu
            //System.IO.File.Delete(path);

            // Bu dizinde aynı isimde dosya var mı kontrol edilir.
            if (System.IO.File.Exists(documentPath))
            {

                return false;
            }
            else
            {

                // 52. satırdaki koda bir ayar çek
                agreement.documentname = "/DATA/Buying/cariMutabakat/" + documentName;

                db.Agreement.Add(agreement);
                db.SaveChanges();
                var item = db.Agreement.Find(agreement.agreementID).agreementID;
                //yeni mutabakat klasörü oluşturulur
                var newAgreement = Server.MapPath("/DATA/Buying/cariMutabakat/" + agreement.agreementID.ToString() + "/");

                Directory.CreateDirectory(newAgreement);
                //dosya path altına kayıt edilir
                document.SaveAs(documentPath);

                var directoryPath = Server.MapPath("/DATA/Buying/cariMutabakat/" + item.ToString());

                // dosya path altına kayıt edildikten sonra okunur
                Excel.Application application = new Excel.Application();
                Excel.Workbook workbook = application.Workbooks.Open(documentPath);
                Excel.Worksheet worksheet = workbook.ActiveSheet;
                Excel.Range range = worksheet.UsedRange;

                try
                {

                    for (int i = 5; i <= range.Rows.Count - 1; i++)
                    {
                        Reconciliations reconciliations = new Reconciliations();

                        reconciliations.title = ((Excel.Range)range.Cells[i, 1]).Text;
                        reconciliations.country = ((Excel.Range)range.Cells[i, 2]).Text;

                        reconciliations.taxNumber = ((Excel.Range)range.Cells[i, 3]).Text;


                        reconciliations.tcNumber = ((Excel.Range)range.Cells[i, 4]).Text;
                        reconciliations.documentNumber = Convert.ToInt32(((Excel.Range)range.Cells[i, 5]).Text);
                        String total = ((Excel.Range)range.Cells[i, 6]).Text;
                        String[] array = total.Split(',');
                        reconciliations.amountService = Convert.ToDouble(array[0]);
                        reconciliations.phoneNumber = ((Excel.Range)range.Cells[i, 7]).Text;
                        reconciliations.representative = ((Excel.Range)range.Cells[i, 8]).Text;
                        reconciliations.customer = ((Excel.Range)range.Cells[i, 9]).Text;

                        reconciliations.agreementID = agreement.agreementID;
                        reconciliations.status = 0;
                        reconciliations.processStatus = false;
                        reconciliations.sentStatus = false;

                        db.Reconciliations.Add(reconciliations);
                    }
                    db.SaveChanges();

                    workbook.Close();
                    application.Quit();

                    System.IO.File.Delete(documentPath);
                }
                catch (Exception)
                {
                    workbook.Close();
                    application.Quit();

                    if (System.IO.File.Exists(documentPath))
                    {
                        System.IO.File.Delete(documentPath);
                    }

                    if (Directory.Exists(directoryPath))
                    {
                        Directory.Delete(directoryPath, true);
                    }
                    db.Agreement.Remove(agreement);
                    db.SaveChanges();
                    return false;
                }
                //alt klasörleri oluşturuyoruz - alış,satış bilgisi , carimi faturamı bilgisi ve ana mutabakat Id verdik parametre olarak
                subDirectoryCreate("Buying", "cariMutabakat", item);
                return true;
            }
        }

        //Alış-FaturaMutabakat
        public bool BuyingFaturaMutabakatCreate(Agreement agreement, HttpPostedFileBase document)
        {

            //Dosya ismi alınır
            var documentName = Path.GetFileName(document.FileName);

            //dosyanın nereye , hangi isimli kayıt edileceği alınır
            var documentPath = Path.Combine(Server.MapPath("/DATA/Buying/faturaMutabakat"), documentName);

            //dosya silme kodu
            //System.IO.File.Delete(path);

            // Bu dizinde aynı isimde dosya var mı kontrol edilir.
            if (System.IO.File.Exists(documentPath))
            {

                return false;
            }
            else
            {
                // 130 .satıra bir düzen çek 
                agreement.documentname = "/DATA/Buying/faturaMutabakat/" + documentName;

                db.Agreement.Add(agreement);
                db.SaveChanges();
                var item = db.Agreement.Find(agreement.agreementID).agreementID;
                //yeni mutabakat klasörü oluşturulur
                var newAgreement = Server.MapPath("/DATA/Buying/faturaMutabakat/" + agreement.agreementID + "/");

                Directory.CreateDirectory(newAgreement);
                //dosya path altına kayıt edilir
                document.SaveAs(documentPath);

                var directoryPath = Server.MapPath("/DATA/Buying/faturaMutabakat/" + item.ToString());

                // dosya path altına kayıt edildikten sonra okunur
                Excel.Application application = new Excel.Application();
                Excel.Workbook workbook = application.Workbooks.Open(documentPath);
                Excel.Worksheet worksheet = workbook.ActiveSheet;
                Excel.Range range = worksheet.UsedRange;

                try
                {

                    for (int i = 5; i <= range.Rows.Count - 1; i++)
                    {
                        Reconciliations reconciliations = new Reconciliations();

                        reconciliations.title = ((Excel.Range)range.Cells[i, 1]).Text;
                        reconciliations.country = ((Excel.Range)range.Cells[i, 2]).Text;

                        reconciliations.taxNumber = ((Excel.Range)range.Cells[i, 3]).Text;


                        reconciliations.tcNumber = ((Excel.Range)range.Cells[i, 4]).Text;
                        reconciliations.documentNumber = Convert.ToInt32(((Excel.Range)range.Cells[i, 5]).Text);
                        String total = ((Excel.Range)range.Cells[i, 6]).Text;
                        String[] array = total.Split(',');
                        reconciliations.amountService = Convert.ToDouble(array[0]);
                        reconciliations.phoneNumber = ((Excel.Range)range.Cells[i, 7]).Text;
                        reconciliations.representative = ((Excel.Range)range.Cells[i, 8]).Text;
                        reconciliations.customer = ((Excel.Range)range.Cells[i, 9]).Text;

                        reconciliations.agreementID = agreement.agreementID;
                        reconciliations.status = 0;
                        reconciliations.processStatus = false;
                        reconciliations.sentStatus = false;

                        db.Reconciliations.Add(reconciliations);
                    }
                    db.SaveChanges();
                    workbook.Close();
                    application.Quit();
                    System.IO.File.Delete(documentPath);


                }
                catch (Exception)
                {
                    workbook.Close();
                    application.Quit();
                    if (System.IO.File.Exists(documentPath))
                    {
                        System.IO.File.Delete(documentPath);
                    }


                    if (Directory.Exists(directoryPath))
                    {
                        Directory.Delete(directoryPath, true);
                    }
                    db.Agreement.Remove(agreement);
                    db.SaveChanges();
                    return false;

                }
                subDirectoryCreate("Buying", "faturaMutabakat", item);
                return true;
            }
        }

        //satış-CariMutabakat
        public bool SalesCariMutabakatCreate(Agreement agreement, HttpPostedFileBase document)
        {

            //Dosya ismi alınır
            var documentName = Path.GetFileName(document.FileName);

            //dosyanın nereye , hangi isimli kayıt edileceği alınır
            var documentPath = Path.Combine(Server.MapPath("/DATA/Sales/cariMutabakat"), documentName);

            //dosya silme kodu
            //System.IO.File.Delete(path);

            // Bu dizinde aynı isimde dosya var mı kontrol edilir.
            if (System.IO.File.Exists(documentPath))
            {

                return false;
            }
            else
            {
                // 202 .satıra bir düzen çek 
                // agreement.documentname = "/DATA/Sales/cariMutabakat/" + documentName;
                agreement.documentname = "";
                db.Agreement.Add(agreement);
                db.SaveChanges();

                var item = db.Agreement.Find(agreement.agreementID).agreementID;

                //yeni mutabakat klasörü oluşturulur
                var newAgreement = Server.MapPath("/DATA/Sales/cariMutabakat/" + agreement.agreementID + "/");
                Directory.CreateDirectory(newAgreement);
                //dosya path altına kayıt edilir
                document.SaveAs(documentPath);

                var directoryPath = Server.MapPath("/DATA/Sales/cariMutabakat/" + item.ToString());

                // dosya path altına kayıt edildikten sonra okunur
                Excel.Application application = new Excel.Application();
                Excel.Workbook workbook = application.Workbooks.Open(documentPath);
                Excel.Worksheet worksheet = workbook.ActiveSheet;
                Excel.Range range = worksheet.UsedRange;

                try
                {
                    for (int i = 5; i <= range.Rows.Count - 1; i++)
                    {
                        Reconciliations reconciliations = new Reconciliations();

                        reconciliations.title = ((Excel.Range)range.Cells[i, 1]).Text;
                        reconciliations.country = ((Excel.Range)range.Cells[i, 2]).Text;

                        reconciliations.taxNumber = ((Excel.Range)range.Cells[i, 3]).Text;


                        reconciliations.tcNumber = ((Excel.Range)range.Cells[i, 4]).Text;
                        reconciliations.documentNumber = Convert.ToInt32(((Excel.Range)range.Cells[i, 5]).Text);
                        String total = ((Excel.Range)range.Cells[i, 6]).Text;
                        String[] array = total.Split(',');
                        reconciliations.amountService = Convert.ToDouble(array[0]);
                        reconciliations.phoneNumber = ((Excel.Range)range.Cells[i, 7]).Text;
                        reconciliations.representative = ((Excel.Range)range.Cells[i, 8]).Text;
                        reconciliations.customer = ((Excel.Range)range.Cells[i, 9]).Text;

                        reconciliations.agreementID = agreement.agreementID;
                        reconciliations.status = 0;
                        reconciliations.processStatus = false;
                        reconciliations.sentStatus = false;

                        db.Reconciliations.Add(reconciliations);
                    }
                    db.SaveChanges();
                    workbook.Close();
                    application.Quit();
                    System.IO.File.Delete(documentPath);
                }
                catch (Exception)
                {
                    workbook.Close();
                    application.Quit();

                    if (System.IO.File.Exists(documentPath))
                    {
                        System.IO.File.Delete(documentPath);
                    }
                    if (Directory.Exists(directoryPath))
                    {
                        Directory.Delete(directoryPath, true);
                    }

                    db.Agreement.Remove(agreement);
                    db.SaveChanges();
                    return false;
                }
                subDirectoryCreate("Sales", "cariMutabakat", item);

                return true;
            }
        }

        //satış-faturaMutabakatı
        public bool SalesfaturaMutabakatCreate(Agreement agreement, HttpPostedFileBase document)
        {
            //Dosya ismi alınır
            var documentName = Path.GetFileName(document.FileName);

            //dosyanın nereye , hangi isimli kayıt edileceği alınır
            var documentPath = Path.Combine(Server.MapPath("/DATA/Sales/faturaMutabakat"), documentName);

            //dosya silme kodu
            //System.IO.File.Delete(path);

            // Bu dizinde aynı isimde dosya var mı kontrol edilir.
            if (System.IO.File.Exists(documentPath))
            {

                return false;
            }
            else
            {
                //271.satıra düzen çek 

                agreement.documentname = "/DATA/Sales/faturaMutabakat/" + documentName;
                db.Agreement.Add(agreement);
                db.SaveChanges();
                var item = db.Agreement.Find(agreement.agreementID).agreementID;
                //yeni mutabakat klasörü oluşturulur
                var newAgreement = Server.MapPath("/DATA/Sales/faturaMutabakat/" + agreement.agreementID + "/");
                Directory.CreateDirectory(newAgreement);
                //dosya path altına kayıt edilir
                document.SaveAs(documentPath);

                var directoryPath = Server.MapPath("/DATA/Sales/faturaMutabakat/" + item.ToString());

                // dosya path altına kayıt edildikten sonra okunur
                Excel.Application application = new Excel.Application();
                Excel.Workbook workbook = application.Workbooks.Open(documentPath);
                Excel.Worksheet worksheet = workbook.ActiveSheet;
                Excel.Range range = worksheet.UsedRange;

                try
                {

                    for (int i = 5; i <= range.Rows.Count - 1; i++)
                    {
                        Reconciliations reconciliations = new Reconciliations();

                        reconciliations.title = ((Excel.Range)range.Cells[i, 1]).Text;
                        reconciliations.country = ((Excel.Range)range.Cells[i, 2]).Text;

                        reconciliations.taxNumber = ((Excel.Range)range.Cells[i, 3]).Text;


                        reconciliations.tcNumber = ((Excel.Range)range.Cells[i, 4]).Text;
                        reconciliations.documentNumber = Convert.ToInt32(((Excel.Range)range.Cells[i, 5]).Text);
                        String total = ((Excel.Range)range.Cells[i, 6]).Text;
                        String[] array = total.Split(',');
                        reconciliations.amountService = Convert.ToDouble(array[0]);
                        reconciliations.phoneNumber = ((Excel.Range)range.Cells[i, 7]).Text;
                        reconciliations.representative = ((Excel.Range)range.Cells[i, 8]).Text;
                        reconciliations.customer = ((Excel.Range)range.Cells[i, 9]).Text;

                        reconciliations.agreementID = agreement.agreementID;
                        reconciliations.status = 0;
                        reconciliations.processStatus = false;
                        reconciliations.sentStatus = false;

                        db.Reconciliations.Add(reconciliations);
                    }
                    db.SaveChanges();
                    workbook.Close();
                    application.Quit();

                    System.IO.File.Delete(documentPath);

                }
                catch (Exception)
                {

                    workbook.Close();
                    application.Quit();

                    if (Directory.Exists(directoryPath))
                    {
                        Directory.Delete(directoryPath, true);
                    }

                    if (System.IO.File.Exists(documentPath))
                    {
                        System.IO.File.Delete(documentPath);
                    }

                    db.Agreement.Remove(agreement);
                    db.SaveChanges();
                    return false;
                }

                subDirectoryCreate("Sales", "faturaMutabakat", item);
                return true;
            }
        }

        // GET: Agreements/Create
        public ActionResult Create(bool type, bool buyingSales)
        {

            /////////////////////////
            var AccessControl = Session["AccessControl"];
            if (AccessControl == null)
            {
                return RedirectToAction("login", "Home");
            }
            /////////////////////////

            ViewBag.type = type;
            ViewBag.buyingSales = buyingSales;
            Agreement agreement = new Agreement() { buyingSales = buyingSales, agreementType = type };
            return View(agreement);
        }

        // POST: Agreements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "agreementID,year,month,agreementType,buyingSales,documentname")] Agreement agreement, HttpPostedFileBase documentname)
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

                if (documentname != null)
                {

                    //Dosyanın uzantısı xls ya da xlsx ise;
                    if (documentname.FileName.EndsWith("xls") || documentname.FileName.EndsWith("xlsx"))
                    {
                        ViewBag.res = 3;
                        // Alıs-CariMutabakat
                        if (agreement.agreementType == true && agreement.buyingSales == true)
                        {
                            var result = BuyingCariMutabakatCreate(agreement, documentname);
                            if (result == false)
                            {
                                ViewBag.res = 5;

                                ViewBag.type = agreement.agreementType;
                                ViewBag.buyingSales = agreement.buyingSales;
                                return View(agreement);

                            }



                        }
                        // Alıs-FaturaMutabakat
                        else if (agreement.agreementType == false && agreement.buyingSales == true)
                        {
                            var result = BuyingFaturaMutabakatCreate(agreement, documentname);
                            if (result == false)
                            {
                                ViewBag.type = agreement.agreementType;
                                ViewBag.buyingSales = agreement.buyingSales;
                                ViewBag.res = 5;
                                return View(agreement);

                            }
                        }
                        // Satıs-CariMutabakat
                        else if (agreement.agreementType == true && agreement.buyingSales == false)
                        {
                            var result = SalesCariMutabakatCreate(agreement, documentname);
                            if (result == false)
                            {
                                ViewBag.res = 5;
                                ViewBag.type = agreement.agreementType;
                                ViewBag.buyingSales = agreement.buyingSales;
                                return View(agreement);

                            }
                        }
                        // Satıs-FaturaMutabakat
                        else if (agreement.agreementType == false && agreement.buyingSales == false)
                        {
                            var result = SalesfaturaMutabakatCreate(agreement, documentname);
                            if (result == false)
                            {
                                ViewBag.res = 5;
                                ViewBag.type = agreement.agreementType;
                                ViewBag.buyingSales = agreement.buyingSales;
                                return View(agreement);

                            }
                        }

                        ViewBag.res = 4;
                        return RedirectToAction("Index", new { type = agreement.agreementType, buyingSales = agreement.buyingSales });


                    }
                    else
                    {
                        ViewBag.res = 3;
                        ViewBag.type = agreement.agreementType;
                        ViewBag.buyingSales = agreement.buyingSales;
                        return View(agreement);

                    }

                }
                else
                {

                    ViewBag.res = 2;
                    ViewBag.type = agreement.agreementType;
                    ViewBag.buyingSales = agreement.buyingSales;
                    return View(agreement);

                }


            }
            ViewBag.res = 1;
            ViewBag.type = agreement.agreementType;
            ViewBag.buyingSales = agreement.buyingSales;
            return View(agreement);
        }

        // GET: Agreements/Edit/5
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
            Agreement agreement = db.Agreement.Find(id);
            if (agreement == null)
            {
                return HttpNotFound();
            }
            return View(agreement);
        }

        // POST: Agreements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "agreementID,year,month,agreementType,buyingSales")] Agreement agreement)
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
                var item = db.Agreement.Find(agreement.agreementID);
                item.year = agreement.year;
                item.month = agreement.month;
                db.SaveChanges();

                return RedirectToAction("Index", new { type = agreement.agreementType, buyingSales = agreement.buyingSales });
            }
            ViewBag.res = 1;
            return View(agreement);
        }

        // GET: Agreements/Delete/5
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
            Agreement agreement = db.Agreement.Find(id);

            if (agreement == null)
            {
                return HttpNotFound();
            }
            return View(agreement);
        }

        // POST: Agreements/Delete/5
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

            Agreement agreement = db.Agreement.Find(id);

            //dosya silme kodu
            //System.IO.File.Delete(path);
            String documentPath = "";
            if (agreement.agreementType == true && agreement.buyingSales == true)
            {
                documentPath = Server.MapPath("/DATA/Buying/cariMutabakat/" + agreement.agreementID.ToString());
            }
            else if (agreement.agreementType == false && agreement.buyingSales == true)
            {
                documentPath = Server.MapPath("/DATA/Buying/faturaMutabakat/" + agreement.agreementID.ToString());
            }
            else if (agreement.agreementType == true && agreement.buyingSales == false)
            {
                documentPath = Server.MapPath("/DATA/Sales/cariMutabakat/" + agreement.agreementID.ToString());
            }
            else if (agreement.agreementType == false && agreement.buyingSales == false)
            {
                documentPath = Server.MapPath("/DATA/Sales/faturaMutabakat/" + agreement.agreementID.ToString());
            }

            // Bu dizinde aynı isimde dosya var mı kontrol edilir.
            //varsa siliniyor

            if (Directory.Exists(documentPath))
            {
                //varsa siliniyor
                Directory.Delete(documentPath, true);
            }

            //yüklenen excel kontrol ediliyor
            var excelPath = Server.MapPath(agreement.documentname);
            if (System.IO.File.Exists(excelPath))
            {
                //varsa siliniyor
                System.IO.File.Delete(excelPath);
            }

            db.Agreement.Remove(agreement);
            db.SaveChanges();
            return RedirectToAction("Index", new { type = agreement.agreementType, buyingSales = agreement.buyingSales });
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
