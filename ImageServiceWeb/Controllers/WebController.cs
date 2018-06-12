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
            HasConnection();
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
            if (!HasConnection()) { return RedirectToAction("NoConnection"); }
            return View(l_model.logs);
        }

        [HttpGet]
        public ActionResult ConfigView()
        {
            if (!HasConnection()) { return RedirectToAction("NoConnection"); }
            return View(c_model.configuration);
        }

        [HttpPost]
        public ActionResult ConfigView(string item)
        {
            if (!HasConnection()) { return RedirectToAction("NoConnection"); }
            return RedirectToAction("RemoveHandlerView", new { item });
        }

        [HttpGet]
        public ActionResult RemoveHandlerView(string item)
        {
            if (!HasConnection()) { return RedirectToAction("NoConnection"); }
            ViewBag.handler = item;
            return View();
        }

        [HttpPost]
        public void RemoveHandler(string name)
        {
            c_model.RemoveHandler(name);
        }

        [HttpGet]
        public ActionResult PhotosView()
        {
            if (!HasConnection()) { return RedirectToAction("NoConnection"); }
            return View(p_model.PhotoList);
        }

        [HttpPost]
        public ActionResult DeletePicView(string path)
        {
            if (!HasConnection()) { return RedirectToAction("NoConnection"); }
            return View(new Photo(path));
        }

        [HttpPost]
        public void DeletePic(string path, string t_path)
        {
            // delete 2 pics, thumbnail and regular
            p_model.DeletePic(path);
            p_model.DeletePic(t_path);
        }

        [HttpPost]
        public ActionResult PicView(string path)
        {
            if (!HasConnection()) { return RedirectToAction("NoConnection"); }
            return View(new Photo(path));
        }

        public ActionResult NoConnection()
        {
            return View();
        }

        public bool HasConnection()
        {
            if (CommunicationSingleton.Instance.HasConnection) { return true; }

            // no connection
            if (CommunicationSingleton.Instance.connectToService() == 0)
            {
                // reproduce the (static) models
                l_model = new LogModel();
                c_model = new ConfigModel();
                h_model = new HomePageModel(c_model.configuration);
                p_model = new PhotosModel(c_model.configuration);
                return true;
            }
            else { return false; }
        }
    }
}