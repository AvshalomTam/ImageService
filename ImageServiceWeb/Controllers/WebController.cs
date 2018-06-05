using ImageServiceWeb.Communication;
using ImageServiceWeb.Models;
using Newtonsoft.Json.Linq;
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

        [HttpPost]
        public ActionResult ConfigView(string item)
        {
            return RedirectToAction("DeleteView", new { item = item });
        }

        [HttpGet]
        public ActionResult DeleteView(string item)
        {
            // that's the only way to send a string as parameter
            // (or create a class holding a string)
            return View("DeleteView", "_Layout", item);
        }

        [HttpPost]
        public JObject RemoveHandler(string name)
        {
            c_model.RemoveHandler(name);
            JObject data = new JObject();
            data["result"] = "Request to close " + name + " has been sent";            
            return data;            
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