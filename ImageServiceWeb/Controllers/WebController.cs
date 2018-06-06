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
            ViewBag.handler = item;
            return View();
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
            try
            {
                string[] pics = Directory.GetFiles(c_model.configuration.OutputDir + @"\Thumbnails", "*.*", SearchOption.AllDirectories);
                string[] seperator = new string[] { "output" };
                for (int i = 0; i < pics.Length; i++)
                {
                    pics[i] = pics[i].Split(seperator, StringSplitOptions.None)[1];
                    pics[i] = @"..\output" + pics[i];
                }
                return View(pics);
            }
            catch
            {
                return View(new string[0]);
            }
            
        }

        // POST: First/Create
        [HttpPost]
        public ActionResult PhotosView(string stam)
        {
            return View();
        }
    }
}