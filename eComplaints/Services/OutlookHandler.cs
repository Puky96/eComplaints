using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Outlook;

namespace eComplaints.Services
{
    public class OutlookHandler
    {
        public void SendAppointment(string identificationNumber, DateTime dateTime, string receiver)
        {
            DateTime appoint = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 7, 30, 0);

            Application app = new Application();


            AppointmentItem newAppointment = (AppointmentItem)app.CreateItem(OlItemType.olAppointmentItem);

            newAppointment.MeetingStatus = OlMeetingStatus.olMeeting;
            newAppointment.Location = "MPD Complaint";
            newAppointment.Subject = "Verifica raspuns pentru " + identificationNumber;
            newAppointment.Body = "Plangerea cu numarul " + identificationNumber  + " asteapta rezolvarea. \nData programata de primire a raspunsului: " + dateTime.ToString("dd.MMMM.yyyy");
            newAppointment.Start = appoint;
            newAppointment.Duration = 5;
            Recipient recipient = newAppointment.Recipients.Add(receiver);
            recipient.Type = (int)OlMeetingRecipientType.olRequired;
            ((Microsoft.Office.Interop.Outlook._AppointmentItem)newAppointment).Send();
        }

        public void SendEmail(string phenomenaCategory, string problem, string complaintEmiter, string identificationNumber, string receiver, string accesUrl = null)
        {
            Application app = new Application();

            MailItem newMail = (MailItem)app.CreateItem(OlItemType.olMailItem);

            newMail.Subject = "Plangere " + phenomenaCategory + "/" + problem;
            newMail.Body = "Atentie! \n" + complaintEmiter + " a inregistrat plangerea cu numarul " + identificationNumber + " pentru problema:   " + phenomenaCategory + "/" + problem + ".\n\nIntrati in aplicatie pentru a o evalua!\n\nLink: " + (accesUrl ?? "no link available");
            newMail.Recipients.Add(receiver);
            newMail.Recipients.ResolveAll();
            newMail.Send();
        }
    }
}
