using MobileBackend.ViewModels;
using MobileBackEnd.DataAccess;
using MobileBackEnd.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MobileBackEnd.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult HoursPerWorkAssignment()
        {
            TimeSheetEntities entities = new TimeSheetEntities();
            try
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);
                //haetaan kaikki kuluvan päivän tuntikirjaukset
                List<Timesheet> allTimesheetsToday = (from ts in entities.Timesheet
                                                      where (ts.StartTime > today) &&
                                                      (ts.StartTime < tomorrow) && (ts.WorkComplete == true)
                                                      select ts).ToList();
                // ryhmitellään kirjaukset tehtävittäin, ja lasketaan kestot

                List<HoursPerWorkAssignmentModel> model = new List<HoursPerWorkAssignmentModel>();

                foreach (Timesheet timesheet in allTimesheetsToday)
                {
                    int assignmentId = timesheet.Id_WorkAssignment.Value;
                    HoursPerWorkAssignmentModel existing = model.Where(m => m.WorkAssignmentId == assignmentId).FirstOrDefault();

                    if (existing != null)
                    {
                        existing.TotalHours += (timesheet.StopTime.Value - timesheet.StartTime.Value).TotalHours;
                    }
                    else
                    {
                        existing = new HoursPerWorkAssignmentModel()

                        {
                            WorkAssignmentId = assignmentId,
                            WorkAssignmentName = timesheet.WorkAssignments.Title,
                            TotalHours = (timesheet.StopTime.Value - timesheet.StartTime.Value).TotalHours
                        };
                        model.Add(existing);
                    }
                }
                return View(model);
            }
            finally
            {
                entities.Dispose();
            }
          
        }
        public ActionResult HoursPerWorkAssignmentAsExcel()
        {
            //TODO:hae tiedot tietokanasta
            StringBuilder csv = new StringBuilder();

            //luodaan CSV-muotoinen tiedosto
            csv.AppendLine("MAtti;123,3");
            csv.AppendLine("Jesse;123,3");
            csv.AppendLine("Kaisa;123,3");
            // palautetaan CSV-tiedot selaimelle
            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
            return File(buffer, "text/csv", "Työtunnit.csv");
        }
        public ActionResult HoursPerWorkAssignmentAsExcel2()
        {
     
            StringBuilder csv = new StringBuilder();

            //luodaan CSV-muotoinen tiedosto
            TimeSheetEntities entities = new TimeSheetEntities();
            try
            {
                DateTime today = DateTime.Today;
                DateTime tomorrow = today.AddDays(1);
           
            //haetaan kuluvan päivän tuntikirjaukset
            List<Timesheet> allTimesheetsToday = (from ts in entities.Timesheet
                                                  where (ts.StartTime > today) &&
                                                  (ts.StartTime < tomorrow) && (ts.WorkComplete == true)
                                                  select ts).ToList();
                foreach (Timesheet timesheet in allTimesheetsToday)
                {
                    csv.AppendLine(timesheet.Id_Employee + "," + timesheet.StartTime + "," + timesheet.StopTime + ",");
                }
 }

           finally
            {
                entities.Dispose();
            }
            // palautetaan CSV-tiedot selaimelle
            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
            return File(buffer, "text/csv", "Työtunnit.csv");
        }
        public ActionResult GetTimesheetCounts(string onlyComplete)
        {
            ReportChartDataViewModel model = new ReportChartDataViewModel();

            TimeSheetEntities entities = new TimeSheetEntities();
            try
            {
                model.Labels = (from wa in entities.WorkAssignments
                                orderby wa.Id_WorkAssignment
                                select wa.Title).ToArray();

                if (onlyComplete == "1")
                {
                    model.Counts = (from ts in entities.Timesheet
                                    where (ts.WorkComplete == true)
                                    orderby ts.Id_WorkAssignment
                                    group ts by ts.Id_WorkAssignment into grp
                                    select grp.Count()).ToArray();
                }
                else
                {
                    model.Counts = (from ts in entities.Timesheet
                                    orderby ts.Id_WorkAssignment
                                    group ts by ts.Id_WorkAssignment into grp
                                    select grp.Count()).ToArray();
                }

            }
            finally
            {
                entities.Dispose();
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}