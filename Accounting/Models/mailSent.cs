using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace Accounting.Models
{
    public class mailSent
    {
        private int Port = 587;
        private string Host = "srvm09.trwww.com";
        private bool EnableSsl = true;
        private string userName = "mutabakat@arisanlastik.com";
        private string password = "arisanMutabakat55";
        private string Title = "Mesut SERDAR";
        public string SentMailAdress;
        private string Subject = "Mutabakat İşlemleri";
        private bool IsbodyHtml = true;
        private string Body = "";
        //server adresi
        public String url = "https://mutabakat.arisanlastik.com/ReconciliationView/Index";
        //local adres
        // public String url = "https://localhost:44336/ReconciliationView/Index";
        public string title;
        public int year;
        public int month;
        public string taxNumber;
        public string tcNumber;
        public int documentNumber;
        public double amountService;
        public string reconID;
        public bool agreementType;
        public bool buyingSales;

        public void mailInformation()
        {

            Body += " <div style='background-color: darkgrey;width: 600px;padding-bottom: 15px'>";
            Body += "<div style='margin-left: 100px;width: 400px;height: 600px;background-color: white;'>";
            Body += "<div style='background-color: orange;height: 10px'></div>";
            Body += "<div style='margin-top: 20px'>";
            if (agreementType == true)
            {
                Body += "<h2 style='margin-left:110px;color: orange'>Cari Mutabakat</h2>";
            }
            else
            {
                Body += "<h2 style='margin-left:110px;color: orange'>Fatura Mutabakat</h2>";
            }
            Body += "<hr>";
            Body += "</div>";
            Body += "<div style = 'margin-top: 20px;font-size: 15px;margin-left:100px;color: gray'> 31." + month + "." + year + " Tarihli Mutabakatınız.</div>";
            Body += "<div style='margin-top: 40px;margin-left: 20px;color: gray;margin-right: 20px'>";
            Body += "<span><b>Gönderen :</b> ArısanLastik</span>";
            Body += "<br>";
            Body += "<span><b>Gönderen VK/TC No : </b>0123456789</span>";
            Body += "<hr>";
            Body += "<span><b>Alıcı :</b>" + title + "</span>";
            Body += "<br>";

            if (taxNumber != null)
            {
                Body += "<span><b>Alıcı VK/TC No : </b> " + taxNumber + "</span>";
            }
            else if (tcNumber != null)
            {
                Body += "<span><b>Alıcı VK/TC No : </b> " + tcNumber + "</span>";
            }
            else
            {
                Body += "<span><b>Alıcı VK/TC No : </b> " + "Kayıtlı Değil!" + "</span>";
            }

            Body += "</div>";

            Body += "<div style='margin-top: 30px;margin-left: 40px'>";
            Body += "<table border='2px' style='border-color: orange'>";
            Body += "<thead>";
            Body += "<tr style='color: gray'>";
            Body += "<th style='padding:10px'>Fatura Adeti</th>";
            Body += "<th style='padding:10px;padding-left: 20px;padding-right: 20px'>Bakiye</th>";
            Body += "<th style='padding:10px'>Hesap Tipi</th>";
            Body += "</tr>";
            Body += "</thead>";
            Body += "<tbody>";
            Body += "<tr style='color: gray'>";
            Body += "<td>" + documentNumber + "</td>";
            Body += "<td>" + amountService.ToString("#,##0.00") + " TL</td>";
            if (buyingSales == true)
            {
                Body += "<td>Verecekli</td>";
            }
            else
            {
                Body += "<td>Alacaklı</td>";
            }
            Body += "</tr>";
            Body += "</tbody>";
            Body += "</table>";
            Body += "</div>";
            Body += "<div style='margin-left: 10px;margin-top: 25px;font-size: 10px;color: gray'>";
            Body += "<p>";
            Body += " 1- Mutabakatınızı bir ay içerisinde bildirmediğiniz takdirde T.T.K nun 92.Madddesi gereğince mutabık sayilacağımızı hatırlatırız.";
            Body += "</p>";
            Body += "<p>";
            Body += "2- Bakiyede mutabık olmadığınız takdirde hesap ekstrenizi tarafımıza acilen ileterek bu formu reddedebilirsiniz.";
            Body += "</p>";
            Body += "<p>";
            Body += "3- Hata ve unutma müstesnadır.";
            Body += "</p>";
            Body += "<p>";
            Body += "4- Bu mutabakat yazısı elektronik ortam gönderilmektedir.";
            Body += "</p>";
            Body += "<p>";
            Body += "5- Lütfen Adres ve iletişim bilgilerinizdeki değişiklilerinizi tarafimiza bildiriniz.";
            Body += "</p>";
            Body += "</div>";
            Body += "<div style='margin-left: 50px'>";
            Body += "<form action='" + url + "' method='Post'>";
            Body += "<input type='hidden' value='" + reconID + "' name='reconID'>";
            Body += "<button type='submit' style='float: left;padding: 10px;background-color:royalblue;color: white;margin-top: 20px;border-radius: 10px;width:300px;'>";
            Body += "Mutabakata Git";
            Body += "</button>";
            Body += "</form>";
            Body += "</div>";
            Body += "</div>";
            Body += "</div>";

        }

        public void messagesInformation()
        {

            Body += " <div style='background-color: darkgrey;width: 600px;padding-bottom: 15px'>";
            Body += "<div style='margin-left: 100px;width: 400px;height: 600px;background-color: white;'>";
            Body += "<div style='background-color: orange;height: 10px'></div>";
            Body += "<div style='margin-top: 20px'>";
            if (agreementType == true)
            {
                Body += "<h2 style='margin-left:110px;color: orange'>Cari Mutabakat</h2>";
            }
            else
            {
                Body += "<h2 style='margin-left:110px;color: orange'>Fatura Mutabakat</h2>";
            }
            Body += "<hr>";
            Body += "</div>";
            Body += "<div style = 'margin-top: 20px;font-size: 15px;margin-left:100px;color: gray'> 31." + month + "." + year + " Tarihli Mutabakatınız.</div>";
            Body += "<div style='margin-top: 40px;margin-left: 20px;color: gray;margin-right: 20px'>";
            Body += "<span><b>Gönderen :</b> ArısanLastik</span>";
            Body += "<br>";
            Body += "<span><b>Gönderen VK/TC No : </b>0123456789</span>";
            Body += "<hr>";
            Body += "<span><b>Alıcı :</b>" + title + "</span>";
            Body += "<br>";

            if (taxNumber != null)
            {
                Body += "<span><b>Alıcı VK/TC No : </b> " + taxNumber + "</span>";
            }
            else if (tcNumber != null)
            {
                Body += "<span><b>Alıcı VK/TC No : </b> " + tcNumber + "</span>";
            }
            else
            {
                Body += "<span><b>Alıcı VK/TC No : </b> " + "Kayıtlı Değil!" + "</span>";
            }

            Body += "</div>";

            Body += "<div style='margin-left: 10px;margin-top: 25px;font-size: 15px;color: gray'>";
            Body += "<p>";
            Body += "Merhabalar , yapmış olduğunuz geri bildirim incelenmiştir ve konu ile ilgili gerekli açıklamalar yapılmıştır. Açıklamaları görmek için Mutabakat'a Git Butonunu tıklayınız. </br> Geri bildiriminiz için teşekkür eder sağlıklı günler dileriz.";
            Body += "</p>";
            Body += "</div>";
            Body += "<div style='margin-left: 10px;margin-top: 25px;font-size: 10px;color: gray'>";
            Body += "<p>";
            Body += " 1- Mutabakatınızı bir ay içerisinde bildirmediğiniz takdirde T.T.K nun 92.Madddesi gereğince mutabık sayilacağımızı hatırlatırız.";
            Body += "</p>";
            Body += "<p>";
            Body += "2- Bakiyede mutabık olmadığınız takdirde hesap ekstrenizi tarafımıza acilen ileterek bu formu reddedebilirsiniz.";
            Body += "</p>";
            Body += "<p>";
            Body += "3- Hata ve unutma müstesnadır.";
            Body += "</p>";
            Body += "<p>";
            Body += "4- Bu mutabakat yazısı elektronik ortam gönderilmektedir.";
            Body += "</p>";
            Body += "<p>";
            Body += "5- Lütfen Adres ve iletişim bilgilerinizdeki değişiklilerinizi tarafimiza bildiriniz.";
            Body += "</p>";
            Body += "</div>";
            Body += "<div style='margin-left: 50px'>";
            Body += "<form action='" + url + "' method='Post'>";
            Body += "<input type='hidden' value='" + reconID + "' name='reconID'>";
            Body += "<button type='submit' style='float: left;padding: 10px;background-color:royalblue;color: white;margin-top: 20px;border-radius: 10px;width:300px;'>";
            Body += "Mutabakata Git";
            Body += "</button>";
            Body += "</form>";
            Body += "</div>";
            Body += "</div>";
            Body += "</div>";

        }

        public bool Sent()
        {


            try
            {

                SmtpClient sc = new SmtpClient();
                sc.Port = Port;
                sc.Host = Host;
                sc.EnableSsl = EnableSsl;

                sc.Credentials = new NetworkCredential(userName, password);

                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(userName, Title);

                mail.To.Add(SentMailAdress);

                mail.Subject = Subject; mail.IsBodyHtml = IsbodyHtml; mail.Body = Body;

                sc.Send(mail);

                return true;
            }
            catch (Exception)
            {

                return false;
            }


        }

    }
}