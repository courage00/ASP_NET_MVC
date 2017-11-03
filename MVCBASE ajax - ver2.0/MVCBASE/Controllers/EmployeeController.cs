using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using MVCBASE.Models;
using System.Net;
using System.Data.Entity;
using System.Web.Services;
using Newtonsoft.Json;
using System.Globalization;

namespace MVCBASE.Controllers
{
    public class EmployeeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
 

        //抓搜尋 跟初始顯示的頁數
        public ActionResult GetEndpage(string name, string title)
        {
            using (NorthwindChineseEntities db = new NorthwindChineseEntities())
            {

                int endpage = 0;//總頁數 
                int pagelistnum = 5;//每頁顯示數
                var datacount = db.Employees.Count();//總比數

                if (!String.IsNullOrEmpty(name)&& String.IsNullOrEmpty(title))
                {
                    datacount = db.Employees.Count(x => x.EmployeeName.Contains(name)); 
                }
                if (!String.IsNullOrEmpty(title)&& String.IsNullOrEmpty(name))
                {
                    datacount = db.Employees.Count(x => x.Title.Contains(title));
                }
                if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(title))
                {
                    datacount = db.Employees.Count(x => x.EmployeeName.Contains(name) && x.Title.Contains(title));
                }

                //總共頁數  每五筆一頁
                if (datacount % pagelistnum == 0)//餘數為0
                {
                    endpage = datacount / pagelistnum;
                }
                else //餘數不等於 0 時
                {
                    endpage = datacount / pagelistnum + 1;
                }
                return Json(endpage);
            }


        }

        //得到資料
        public ActionResult GetData(int page, string name, string title)
        {
            int startid = (page - 1) * 5;
            

            using (NorthwindChineseEntities db = new NorthwindChineseEntities())
            {
                var query = db.Employees.AsQueryable();
                var pagelistnum = 5;
       
                if (!String.IsNullOrEmpty(name))
                {
                    query = query.Where(x=> x.EmployeeName.Contains(name));
                }
                if (!String.IsNullOrEmpty(title))
                {
                    query = query.Where(x => x.Title.Contains(title));
                }

                query = query.OrderBy(x => x.EmployeeID).Skip(startid).Take(pagelistnum);

                string str_json = JsonConvert.SerializeObject(query.ToArray());

                return Json(str_json);
                //return name;

            }
        }
        //儲存新增
        public  ActionResult New(string name, string title, string titlec, string bdate, string hdate, string address, string hphone, string ex, string photopath, string notes, int mgid, int salary)
        {
            using (NorthwindChineseEntities db = new NorthwindChineseEntities())
            {
                DateTime birthdate = DateTime.Parse(bdate);
                DateTime hiredate = DateTime.Parse(hdate);
                Employees newem = new Employees () { EmployeeName = name, Title = title, TitleOfCourtesy = titlec, BirthDate = birthdate, HireDate = hiredate, Address = address, HomePhone = hphone, Extension = ex, PhotoPath = photopath, Notes = notes, ManagerID = mgid, Salary = salary };

                // if (ModelState.IsValid)
                // {
                db.Employees.Add(newem);
                 db.SaveChanges();
                //}
               // string birthdate = DateTime.Parse(bdate).ToString("yyyy/MM/dd");
               // string hiredate = DateTime.Parse(hdate).ToString("yyyy/MM/dd");
            }

            return  Json("新增成功");

        }

        //儲存修改
        public ActionResult SaveEdit(int id, string name, string title, string titlec, string bdate, string hdate, string address, string hphone, string ex, string photopath, string notes, int mgid, int salary)
        {
            DateTime birthdate = DateTime.Parse(bdate);
            DateTime hiredate = DateTime.Parse(hdate);

            using (NorthwindChineseEntities db = new NorthwindChineseEntities())
            {
                Employees e = db.Employees.Find(id);
                e.EmployeeName = name;
                e.Title = title;
                e.TitleOfCourtesy = titlec;
                e.BirthDate = birthdate;
                e.HireDate = hiredate;
                e.Address = address;
                e.HomePhone = hphone;
                e.Extension = ex;
                e.PhotoPath = photopath;
                e.Notes = notes;
                e.ManagerID = mgid;
                e.Salary = salary;

                db.Entry(e).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json("成功儲存修改");

        }
        //刪除
        public ActionResult Delete(string id)
        {
            using (NorthwindChineseEntities db = new NorthwindChineseEntities())
            {
                Employees e = db.Employees.Find(id);
                db.Employees.Remove(e);
                db.SaveChanges();
            }
            return Json("成功刪除");
        }
        //修改
        public ActionResult Edit(int emid)
        {
            using (NorthwindChineseEntities db = new NorthwindChineseEntities())
            {
                //Employees e = db.Employees.Find(emid);
                var query = db.Employees.Where(x=> x.EmployeeID==emid).OrderBy(x => x.EmployeeID);

                string edit_json = JsonConvert.SerializeObject(query.ToArray());

                return Json(edit_json);
            }

        }

    }
}