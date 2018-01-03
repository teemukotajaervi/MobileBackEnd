using MobileBackEnd.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MobileBackEnd.Controllers
{
    public class EmployeeController : ApiController
    {
        public string[] GetAll()
        {
            string[] employeeNames = null;
            TimeSheetEntities entities = new TimeSheetEntities();
            try
            {
              employeeNames = (from e in entities.Employees
                               where (e.Active == true)
                               select e.FirstName+" "+e.LastName).ToArray();
            }
            finally
            {
                entities.Dispose();
            }
            return employeeNames;    
        }
        
        public byte[] GetEmployeeImage(string employeeName)
        {
            TimeSheetEntities entities = new TimeSheetEntities();
            try
            {
                string[] nameParts = employeeName.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                string first = nameParts[0];
                string last = nameParts[1];
                byte[] bytes = (from e in entities.Employees
                                where (e.Active == true) &&
                                (e.FirstName == first) &&
                                (e.LastName == last)
                                select e.EmployeePicture).FirstOrDefault();

                return bytes;
            }
            finally
            {
                entities.Dispose();
            }
        }
        public string PutEmployeeImage ()
        {
            TimeSheetEntities entities = new TimeSheetEntities();
            try
            {
                Employees newEmployee = new Employees()
                {
                    FirstName = "Heebo",
                    LastName = "X",
                    EmployeePicture = File.ReadAllBytes(@"‪X:\Code\mopo2.png")
                };
                entities.Employees.Add(newEmployee);
                entities.SaveChanges();

                return "OK";
            }
            finally
            {
                entities.Dispose();
            }
            return "Error";
        }
    }
 }

