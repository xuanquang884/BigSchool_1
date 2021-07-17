using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        BigSchoolContext db = new BigSchoolContext();
        // GET: Courses
        public ActionResult Create()
        {
            Course objCourse = new Course();
            objCourse.listcategory = db.Categories.ToList();
            return View(objCourse);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course objCourse)
        {
            ModelState.Remove("LecturerId");
            if (!ModelState.IsValid)
            {
                objCourse.listcategory = db.Categories.ToList();
                return View("Create", objCourse);
            }

            // lấy login user id
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            objCourse.LecturerId = user.Id;

            //add vào csdl
            db.Courses.Add(objCourse);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Attending()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAttendance = db.Attendances.Where(p => p.Attendee == currentUser.Id).ToList();
            var course = new List<Course>();
            foreach(Attendance temp in listAttendance)
            {
                Course objcourse = temp.Course;
                objcourse.LecturerId = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                    .FindById(objcourse.LecturerId).Name;
                course.Add(objcourse);
            }
            return View(course);
        }
        public ActionResult Mine()
        {
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var course = new List<Course>();
            foreach (Course i in course)
            {
                i.LectureName = currentUser.Name;
            }
            return View(course);
        }
        public ActionResult LectureIamGoing()
        {
            ApplicationUser currentUser =
            System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
            .FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            BigSchoolContext context = new BigSchoolContext();
            var listFollwee = context.Followings.Where(p => p.FollowerId ==
            currentUser.Id).ToList();
            var listAttendances = context.Attendances.Where(p => p.Attendee ==
            currentUser.Id).ToList();
            var courses = new List<Course>();
            foreach (var course in listAttendances)
            {
                foreach (var item in listFollwee)
                {
                    if (item.FolloweeId == course.Course.LecturerId)
                    {
                        Course objCourse = course.Course;
                        objCourse.LectureName =
                        System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>()
                        .FindById(objCourse.LecturerId).Name;
                        courses.Add(objCourse);
                    }
                }
            }
            return View(courses);
        }
    }
}