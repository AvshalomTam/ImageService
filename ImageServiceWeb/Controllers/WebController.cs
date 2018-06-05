using ImageServiceWeb.Communication;
using ImageServiceWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageServiceWeb.Controllers
{
    public class WebController : Controller
    {
        static List<Student> students = StudentModel.GetStudentList(@"App_Data/StudentsConfig.xml");

        static LogModel l_model = new LogModel();
        static ConfigModel c_model = new ConfigModel();

        [HttpGet]
        public ActionResult HomePage()
        {
            return View(students);
        }

        [HttpGet]
        public ActionResult LogsView()
        {
            return View(l_model.getList());
        }

        [HttpPost]
        public ActionResult LogsView(string type)
        {
            return View(l_model.getList(type));
        }

        [HttpGet]
        public ActionResult ConfigView()
        {
            return View(c_model.configuration);
        }

        // GET: First/Create
        public ActionResult PhotosView()
        {
            return View();
        }

        // POST: First/Create
        [HttpPost]
        public ActionResult PhotosView(string stam)
        {
            return View();
        }
    }
}