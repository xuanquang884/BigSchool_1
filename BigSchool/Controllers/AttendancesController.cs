using BigSchool.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BigSchool.Controllers
{
    public class AttendancesController : ApiController
    {
        BigSchoolContext db = new BigSchoolContext();
        public IHttpActionResult Attend (Course attendanceDto )
        {
            var userID = User.Identity.GetUserId();
            if(db.Attendances.Any(p=> p.Attendee == userID && p.CourseID==attendanceDto.Id))
            {
                return BadRequest("The attendance already exists!");
            }
            var attendance = new Attendance()
            {
                CourseID = attendanceDto.Id, Attendee = User.Identity.GetUserId()
            };
                db.Attendances.Add(attendance);
                db.SaveChanges();
            return Ok();
        }
    }
}
