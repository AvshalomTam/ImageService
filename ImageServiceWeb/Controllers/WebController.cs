using ImageServiceWeb.Communication;
using ImageServiceWeb.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ImageServiceWeb.Controllers
{
    public class WebController : Controller
    {
        static HomePageModel h_model = new HomePageModel();
        static LogModel l_model = new LogModel();
        static ConfigModel c_model = new ConfigModel();

        [HttpGet]
        public ActionResult HomePage()
        {
            ViewBag.status = h_model.status;
            ViewBag.numOfPics = h_model.numOfPics;

            return View(h_model.students);
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
        public ActionResult DeleteView()
        {
            return RedirectToAction("ConfigView");
        }

        [HttpPost]
        public void RemoveHandler(string name)
        {
            c_model.RemoveHandler(name);                       
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