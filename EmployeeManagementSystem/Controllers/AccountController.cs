using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace EmployeeManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        readonly EmpMgmtEntities db = new EmpMgmtEntities();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Models.Membership model)
        {
            using(var context = new EmpMgmtEntities())
            {
                bool isValid = context.Users.Any(x => x.UserName == model.UserName && x.Password == model.Password);
                if (isValid)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return RedirectToAction("Index", "Employees");
                }
                ModelState.AddModelError("", "Invalid Login credentials");
                return View();
            }
        }
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(User model)
        {
            using (var context = new EmpMgmtEntities())
            {
                try
                {
                    bool isValid = context.Users.Any(x => x.UserName == model.UserName);
                    if (isValid)
                    {
                        ViewBag.Notification = "User Already Exists";
                        return View();
                    }
                    else
                    {
                        context.Users.Add(model);
                        context.SaveChanges();
                        TempData["Id"] = model.Id.ToString();
                        TempData["UserName"] = model.UserName.ToString();
                        ViewBag.Notification = "Successfully Registered";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Notification = "Error occurred while saving user details: " + ex.Message;
                    return View();
                }
            }

            //using(var context = new EmpMgmtEntities())
            //{
            //    bool isValid = context.Users.Any(x => x.UserName == model.UserName);
            //    if (isValid)
            //    {
            //        ViewBag.Notification = "User Already Exists";
            //        return View();
            //    }
            //    else
            //    {
            //        context.Users.Add(model);
            //        context.SaveChanges();
            //        Session["Id"] = model.Id.ToString();
            //        Session["UserName"] = model.UserName.ToString();
            //        ViewBag.Notification = "Successfully Registered";
            //        return View();
            //    }

            //}
            /*return RedirectToAction("Login");*/
        }
        /*using(var context = new EmpMgmtEntities())
           {
               context.Users.Add(model);
               context.SaveChanges();
           }
       return RedirectToAction("Login");*/
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}