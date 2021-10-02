using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Accounting.Models;
namespace Accounting.Controllers
{
    public class ReconciliationViewController : Controller
    {
        private arisanerpEntities db = new arisanerpEntities();

        [HttpPost]
        public ActionResult Index(String reconID)
        {
         


            var recon = db.Reconciliations.Find(Convert.ToInt32(reconID));

            if (recon.processStatus == true)
            {
                if (recon.taxNumber != null)
                {
                    ViewBag.customer = db.Customers.Where(s => s.taxNumber == recon.taxNumber).FirstOrDefault();
                }
                else if (recon.tcNumber != null)
                {
                    ViewBag.customer = db.Customers.Where(s => s.tcNumber == recon.tcNumber).FirstOrDefault();
                }
                ViewBag.document = recon.ReconciliationsDocument.ToList();
                return View(recon);
            }
            else
            {

                return RedirectToAction("confirm", new { result = "res" });
            }
        }
        public JsonResult Approve(String reconID)
        {
            var recon = db.Reconciliations.Find(Convert.ToInt32(reconID));
            recon.status = 2;
            recon.processStatus = false;
            db.SaveChanges();

            return Json(new { res = 1 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult notApprove(int reconID)
        {
           

            ViewBag.reconID = reconID;
            return PartialView("/Views/Shared/_notApproveView.cshtml");
        }

        [HttpPost]
        public ActionResult notApproveSent(HttpPostedFileBase[] files, String messages, String reconID)
        {

            

            var recon = db.Reconciliations.Find(Convert.ToInt32(reconID));
            var path = "";

            if (recon.Agreement.buyingSales == true && recon.Agreement.agreementType == true)
            {
                path = "/DATA/Buying/cariMutabakat/" + recon.Agreement.agreementID.ToString() + "/" + recon.reconID.ToString() + "/get";
            }
            else if (recon.Agreement.buyingSales == true && recon.Agreement.agreementType == false)
            {
                path = "/DATA/Buying/faturaMutabakat/" + recon.Agreement.agreementID.ToString() + "/" + recon.reconID.ToString() + "/get";
            }
            else if (recon.Agreement.buyingSales == false && recon.Agreement.agreementType == true)
            {
                path = "/DATA/Sales/cariMutabakat/" + recon.Agreement.agreementID.ToString() + "/" + recon.reconID.ToString() + "/get";
            }
            else if (recon.Agreement.buyingSales == false && recon.Agreement.agreementType == false)
            {
                path = "/DATA/Sales/faturaMutabakat/" + recon.Agreement.agreementID.ToString() + "/" + recon.reconID.ToString() + "/get";
            }

            if (Directory.Exists(Server.MapPath(path)))
            {

                ReconciliationsDocument redoc = new ReconciliationsDocument() { reconID = recon.reconID, messages = messages, setGetStatus = true, operationDate = DateTime.Now };
                db.ReconciliationsDocument.Add(redoc);
                db.SaveChanges();
                foreach (var item in files)
                {
                    if (item != null)
                    {
                        if (item.ContentLength > 0)
                        {
                            Document document = new Document();
                            document.reconciliationsDocumentID = redoc.reconciliationsDocumentID;
                            document.documentPath = "";
                            db.Document.Add(document);
                            db.SaveChanges();

                            var documentEdit = db.Document.Find(document.documentID);
                            // var documentname = Path.GetFileName(item.FileName);

                            if (item.FileName.EndsWith("xls") || item.FileName.EndsWith("xlsx"))
                            {
                                var documentPath = Path.Combine(Server.MapPath(path), documentEdit.documentID.ToString() + "." + "xls");

                                item.SaveAs(documentPath);
                                documentEdit.documentPath = path + "/" + documentEdit.documentID.ToString() + "." + "xls";
                                db.SaveChanges();
                            }
                            else if (item.FileName.EndsWith("jpg") || item.FileName.EndsWith("jpeg") || item.FileName.EndsWith("JPG") || item.FileName.EndsWith("Jpeg"))
                            {
                                var documentPath = Path.Combine(Server.MapPath(path), documentEdit.documentID.ToString() + "." + "jpg");

                                item.SaveAs(documentPath);
                                documentEdit.documentPath = path + "/" + documentEdit.documentID.ToString() + "." + "jpg";
                                db.SaveChanges();
                            }
                            else if (item.FileName.EndsWith("pdf") || item.FileName.EndsWith("PDF"))
                            {
                                var documentPath = Path.Combine(Server.MapPath(path), documentEdit.documentID.ToString() + "." + "pdf");

                                item.SaveAs(documentPath);
                                documentEdit.documentPath = path + "/" + documentEdit.documentID.ToString() + "." + "pdf";
                                db.SaveChanges();
                            }
                            else
                            {
                                db.Document.Remove(documentEdit);
                                db.SaveChanges();
                            }


                        }
                    }

                }

                recon.processStatus = false;
                recon.status = 3;
                db.SaveChanges();
            }

            return RedirectToAction("confirm");
        }


        [HttpPost]
        public ActionResult resultSet(HttpPostedFileBase[] files, String messages, String reconID)
        {
          

            var recon = db.Reconciliations.Find(Convert.ToInt32(reconID));

            Customers customer = null;

            if (recon.taxNumber != null)
            {
                customer = db.Customers.Where(s => s.taxNumber == recon.taxNumber).FirstOrDefault();
            }
            else if (recon.tcNumber != null)
            {
                customer = db.Customers.Where(s => s.tcNumber == recon.tcNumber).FirstOrDefault();
            }

            if (customer != null)
            {
                if (customer.customerDescription != null)
                {
                    if (recon.Agreement.agreementType == false)
                    {
                        if (recon.amountService >= 5000)
                        {
                            var path = "";

                            if (recon.Agreement.buyingSales == true && recon.Agreement.agreementType == true)
                            {
                                path = "/DATA/Buying/cariMutabakat/" + recon.Agreement.agreementID.ToString() + "/" + recon.reconID.ToString() + "/set";
                            }
                            else if (recon.Agreement.buyingSales == true && recon.Agreement.agreementType == false)
                            {
                                path = "/DATA/Buying/faturaMutabakat/" + recon.Agreement.agreementID.ToString() + "/" + recon.reconID.ToString() + "/set";
                            }
                            else if (recon.Agreement.buyingSales == false && recon.Agreement.agreementType == true)
                            {
                                path = "/DATA/Sales/cariMutabakat/" + recon.Agreement.agreementID.ToString() + "/" + recon.reconID.ToString() + "/set";
                            }
                            else if (recon.Agreement.buyingSales == false && recon.Agreement.agreementType == false)
                            {
                                path = "/DATA/Sales/faturaMutabakat/" + recon.Agreement.agreementID.ToString() + "/" + recon.reconID.ToString() + "/set";
                            }

                            if (Directory.Exists(Server.MapPath(path)))
                            {

                                ReconciliationsDocument redoc = new ReconciliationsDocument() { reconID = recon.reconID, messages = messages, setGetStatus = false, operationDate = DateTime.Now };
                                db.ReconciliationsDocument.Add(redoc);
                                db.SaveChanges();

                                if (files.Length != 0)
                                {
                                    foreach (var item in files)
                                    {
                                        if (item != null)
                                        {
                                            if (item.ContentLength > 0)
                                            {
                                                Document document = new Document();
                                                document.reconciliationsDocumentID = redoc.reconciliationsDocumentID;
                                                document.documentPath = "";
                                                db.Document.Add(document);
                                                db.SaveChanges();

                                                var documentEdit = db.Document.Find(document.documentID);
                                                // var documentname = Path.GetFileName(item.FileName);

                                                if (item.FileName.EndsWith("xls") || item.FileName.EndsWith("xlsx"))
                                                {
                                                    var documentPath = Path.Combine(Server.MapPath(path), documentEdit.documentID.ToString() + "." + "xls");

                                                    item.SaveAs(documentPath);
                                                    documentEdit.documentPath = path + "/" + documentEdit.documentID.ToString() + "." + "xls";
                                                    db.SaveChanges();
                                                }
                                                else if (item.FileName.EndsWith("jpg") || item.FileName.EndsWith("jpeg") || item.FileName.EndsWith("JPG") || item.FileName.EndsWith("Jpeg"))
                                                {
                                                    var documentPath = Path.Combine(Server.MapPath(path), documentEdit.documentID.ToString() + "." + "jpg");

                                                    item.SaveAs(documentPath);
                                                    documentEdit.documentPath = path + "/" + documentEdit.documentID.ToString() + "." + "jpg";
                                                    db.SaveChanges();
                                                }
                                                else if (item.FileName.EndsWith("pdf") || item.FileName.EndsWith("PDF"))
                                                {
                                                    var documentPath = Path.Combine(Server.MapPath(path), documentEdit.documentID.ToString() + "." + "pdf");

                                                    item.SaveAs(documentPath);
                                                    documentEdit.documentPath = path + "/" + documentEdit.documentID.ToString() + "." + "pdf";
                                                    db.SaveChanges();
                                                }
                                                else
                                                {
                                                    db.Document.Remove(documentEdit);
                                                    db.SaveChanges();
                                                }


                                            }
                                        }

                                    }
                                }


                                mailSent mail = new mailSent()
                                {
                                    agreementType = recon.Agreement.agreementType,
                                    amountService = recon.amountService,
                                    SentMailAdress = customer.customerDescription,
                                    buyingSales = recon.Agreement.buyingSales,
                                    documentNumber = recon.documentNumber,
                                    month = recon.Agreement.month,
                                    year = recon.Agreement.year,
                                    reconID = recon.reconID.ToString(),
                                    taxNumber = recon.taxNumber,
                                    tcNumber = recon.tcNumber,
                                    title = recon.title
                                };
                                mail.messagesInformation();
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
                        }
                        else
                        {
                            recon.sentStatus = false;
                            recon.status = 6;
                            recon.processStatus = false;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var path = "";

                        if (recon.Agreement.buyingSales == true && recon.Agreement.agreementType == true)
                        {
                            path = "/DATA/Buying/cariMutabakat/" + recon.Agreement.agreementID.ToString() + "/" + recon.reconID.ToString() + "/set";
                        }
                        else if (recon.Agreement.buyingSales == true && recon.Agreement.agreementType == false)
                        {
                            path = "/DATA/Buying/faturaMutabakat/" + recon.Agreement.agreementID.ToString() + "/" + recon.reconID.ToString() + "/set";
                        }
                        else if (recon.Agreement.buyingSales == false && recon.Agreement.agreementType == true)
                        {
                            path = "/DATA/Sales/cariMutabakat/" + recon.Agreement.agreementID.ToString() + "/" + recon.reconID.ToString() + "/set";
                        }
                        else if (recon.Agreement.buyingSales == false && recon.Agreement.agreementType == false)
                        {
                            path = "/DATA/Sales/faturaMutabakat/" + recon.Agreement.agreementID.ToString() + "/" + recon.reconID.ToString() + "/set";
                        }

                        if (Directory.Exists(Server.MapPath(path)))
                        {

                            ReconciliationsDocument redoc = new ReconciliationsDocument() { reconID = recon.reconID, messages = messages, setGetStatus = false, operationDate = DateTime.Now };
                            db.ReconciliationsDocument.Add(redoc);
                            db.SaveChanges();

                            if (files.Length != 0)
                            {
                                foreach (var item in files)
                                {
                                    if (item != null)
                                    {
                                        if (item.ContentLength > 0)
                                        {
                                            Document document = new Document();
                                            document.reconciliationsDocumentID = redoc.reconciliationsDocumentID;
                                            document.documentPath = "";
                                            db.Document.Add(document);
                                            db.SaveChanges();

                                            var documentEdit = db.Document.Find(document.documentID);
                                            // var documentname = Path.GetFileName(item.FileName);

                                            if (item.FileName.EndsWith("xls") || item.FileName.EndsWith("xlsx"))
                                            {
                                                var documentPath = Path.Combine(Server.MapPath(path), documentEdit.documentID.ToString() + "." + "xls");

                                                item.SaveAs(documentPath);
                                                documentEdit.documentPath = path + "/" + documentEdit.documentID.ToString() + "." + "xls";
                                                db.SaveChanges();
                                            }
                                            else if (item.FileName.EndsWith("jpg") || item.FileName.EndsWith("jpeg") || item.FileName.EndsWith("JPG") || item.FileName.EndsWith("Jpeg"))
                                            {
                                                var documentPath = Path.Combine(Server.MapPath(path), documentEdit.documentID.ToString() + "." + "jpg");

                                                item.SaveAs(documentPath);
                                                documentEdit.documentPath = path + "/" + documentEdit.documentID.ToString() + "." + "jpg";
                                                db.SaveChanges();
                                            }
                                            else if (item.FileName.EndsWith("pdf") || item.FileName.EndsWith("PDF"))
                                            {
                                                var documentPath = Path.Combine(Server.MapPath(path), documentEdit.documentID.ToString() + "." + "pdf");

                                                item.SaveAs(documentPath);
                                                documentEdit.documentPath = path + "/" + documentEdit.documentID.ToString() + "." + "pdf";
                                                db.SaveChanges();
                                            }
                                            else
                                            {
                                                db.Document.Remove(documentEdit);
                                                db.SaveChanges();
                                            }


                                        }
                                    }

                                }
                            }


                            mailSent mail = new mailSent()
                            {
                                agreementType = recon.Agreement.agreementType,
                                amountService = recon.amountService,
                                SentMailAdress = customer.customerDescription,
                                buyingSales = recon.Agreement.buyingSales,
                                documentNumber = recon.documentNumber,
                                month = recon.Agreement.month,
                                year = recon.Agreement.year,
                                reconID = recon.reconID.ToString(),
                                taxNumber = recon.taxNumber,
                                tcNumber = recon.tcNumber,
                                title = recon.title
                            };
                            mail.messagesInformation();
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
                    }

                }
                else
                {
                    recon.processStatus = false;
                    recon.status = 1;
                    recon.sentStatus = false;
                    db.SaveChanges();
                }

            }
            else
            {
                recon.processStatus = false;
                recon.status = 1;
                recon.sentStatus = false;
                db.SaveChanges();
            }

            return RedirectToAction("Details", "Reconciliations", new { id = recon.reconID });
        }


        public ActionResult confirm(string result)
        {
           

            if (result != null)
            {
                ViewBag.Messages = "Form Kullanıma Kapandı !!!";
            }
            else
            {
                ViewBag.Messages = "İslem Tamamlandı !!!";
            }
            return View();
        }
    }
}