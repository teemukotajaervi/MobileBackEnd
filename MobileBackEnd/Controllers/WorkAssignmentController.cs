using MobileBackEnd.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MobileBackEnd.Controllers
{
    public class WorkAssignmentController : ApiController
    {
        public string[] GetAll()
        {
            string[] assignmentNames = null;
            TimeSheetEntities entities = new TimeSheetEntities();
            try
            {
                assignmentNames = (from wa in entities.WorkAssignments
                                 where (wa.Active == true)
                                 select wa.Title).ToArray();
            }
            finally
            {
                entities.Dispose();
            }
            return assignmentNames;
        }
    }
}
