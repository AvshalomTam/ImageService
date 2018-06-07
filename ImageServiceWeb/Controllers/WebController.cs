using ImageServiceWeb.Communication;
using ImageServiceWeb.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ImageServiceWeb.Controllers
{
    public class WebController : Controller
    {
        static LogModel l_model = new LogModel();
        static ConfigModel c_model = new ConfigModel();
        static HomePageModel h_model = new HomePageModel(c_model.configuration);
        static PhotosModel p_model = new PhotosModel(c_model.configuration);

        [HttpGet]
        public ActionResult HomePage()
        {
            return View(h_model.students);
        }

        public string NumberOfPics()
        {
            return h_model.numOfPics.ToString();
        }

        public string ServiceStatus()
        {
            return h_model.status;
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
            return RedirectToAction("RemoveHandlerView", new { item });
        }

        [HttpGet]
        public ActionResult RemoveHandlerView(string item)
        {
            ViewBag.handler = item;
            return View();
        }

        [HttpPost]
        public ActionResult RemoveHandlerView()
        {
            return RedirectToAction("ConfigView");
        }

        [HttpPost]
        public void RemoveHandler(string name)
        {
            c_model.RemoveHandler(name);                       
        }

        [HttpGet]
        public ActionResult PhotosView()
        {
            return View(p_model.PhotoList);
        }

        [HttpPost]
        public ActionResult DeletePicView(string path)
        {
            return View(new Photo(path));
        }

        [HttpPost]
        public void DeletePic(string path, string t_path)
        {
            // delete pics
        }

        [HttpPost]
        public ActionResult PicView(string path)
        {
            return View(new Photo(path));
        }
    }
}