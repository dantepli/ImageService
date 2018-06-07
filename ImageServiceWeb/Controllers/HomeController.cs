using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageServiceWeb.Models;

namespace ImageServiceWeb.Controllers
{
    public class HomeController : Controller
    {
        static HomePageModel homePageModel = new HomePageModel();
        static SettingsModel settingsModel = new SettingsModel();
        static LogsModel logsModel = new LogsModel();

        // GET: HomePage
        public ActionResult HomeView()
        {
            return View(homePageModel);
        }

        //GET: ConfigPage
        public ActionResult Config()
        {
            return View(settingsModel);
        }

        //GET: LogsPage
        public ActionResult Logs()
        {
            return View(logsModel);
        }

        /// <summary>
        /// Removes the handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        public ActionResult RemoveHandler(string handler)
        {
            if (handler != null)
            {
                ViewBag.handlerVerbatimString = handler;
                ViewBag.handler = handler.Replace("\\", "\\\\");
            }
            return View();
        }

        [HttpPost]
        public void RemoveHandlerConfirmation(string handler)
        {
            settingsModel.RemoveHandler(handler);
            //TODO: REMOVE
            System.Threading.Thread.Sleep(2000);
        }

        // GET: First
        public ActionResult Index()
        {
            return View();
        }
    }
}
