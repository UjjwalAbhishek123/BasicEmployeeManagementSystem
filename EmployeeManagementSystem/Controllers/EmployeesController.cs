using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using EmployeeManagementSystem;

namespace EmployeeManagementSystem.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        readonly EmpMgmtEntities db = new EmpMgmtEntities();

        // GET: Employees
        
        public ActionResult Index(string searching, string sortingEmp)
        {
            /*ViewData["GetEmployeeDetails"] = searching;*/
            //ViewData["EmployeeId"] = string.IsNullOrEmpty(sortingEmp) ? "EmployeeID" : "";
            ViewData["EmployeeFirstName"] = string.IsNullOrEmpty(sortingEmp) ? "FirstName" : "";
            ViewData["EmployeeLastName"] = string.IsNullOrEmpty(sortingEmp) ? "LastName" : "";

            var empQuery = from x in db.Employees select x;

            switch (sortingEmp)
            {
                //case "EmployeeID":
                //    empQuery = empQuery.OrderBy(x => x.Id);
                //    break;
                case "FirstName":
                    empQuery = empQuery.OrderByDescending(x => x.FirstName);
                    break;
                case "LastName":
                    empQuery = empQuery.OrderBy(x => x.LastName);
                    break;
                default:
                    empQuery = empQuery.OrderBy(x => x.FirstName);
                    break;
            }
            if (!String.IsNullOrEmpty(searching))
            {
                empQuery = empQuery.Where(x => x.FirstName.Contains(searching) || x.LastName.Contains(searching));
            }
            return View(empQuery.ToList());
        }
        // GET: Employees/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentCode = new SelectList(db.Departments, "DepartmentCode", "DepartmentName");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Designation,Salary,DepartmentCode,DateOfJoining,OfficeLocation")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentCode = new SelectList(db.Departments, "DepartmentCode", "DepartmentName", employee.DepartmentCode);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentCode = new SelectList(db.Departments, "DepartmentCode", "DepartmentName", employee.DepartmentCode);
            return View(employee);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,Designation,Salary,DepartmentCode,DateOfJoining,OfficeLocation,UserName")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentCode = new SelectList(db.Departments, "DepartmentCode", "DepartmentName", employee.DepartmentCode);
            return View(employee);
        }

        // GET
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employee employee = db.Employees.Find(id);
            db.Employees.Remove(employee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //to export employee detail in excel file
        [HttpPost]
        public FileResult Export()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[7] {
                new DataColumn("First Name"),
                new DataColumn("Last Name"),
                new DataColumn("Designation"),
                new DataColumn("Employee Salary"),
                new DataColumn("Department Name"),
                new DataColumn("Date of Joining"),
                new DataColumn("Office Location")
            });
            var emps = from employee in db.Employees.ToList() select employee;
            foreach(var employee in emps)
            {
                dt.Rows.Add(employee.FirstName, 
                    employee.LastName, 
                    employee.Designation, 
                    employee.Salary, 
                    employee.Department.DepartmentName, 
                    employee.DateOfJoining, 
                    employee.OfficeLocation);
            }
            using(XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using(MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }
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