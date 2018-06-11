using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageServiceWeb.Models;
using ImageService.Infrastructure;

namespace ImageServiceWeb.Controllers
{
    public class HomeController : Controller
    {
        static HomePageModel homePageModel = new HomePageModel();
        static SettingsModel settingsModel = new SettingsModel();
        static LogsModel logsModel = new LogsModel();
        static PhotosModel photosModel = new PhotosModel();

        /// <summary>
        /// Get request for home view.
        /// </summary>
        /// <returns>The home view.</returns>
        public ActionResult HomeView()
        {
            return View(homePageModel);
        }

        /// <summary>
        /// Get request for config view.
        /// </summary>
        /// <returns>The config view.</returns>
        public ActionResult Config()
        {
            return View(settingsModel);
        }

        /// <summary>
        /// Get request for photos view.
        /// </summary>
        /// <returns>The photos view.</returns>
        public ActionResult Photos()
        {
            photosModel.UpdatePhotos();
            return View(photosModel);
        }

        /// <summary>
        /// Get request for logs view.
        /// Filters the list, if no param is given filters by all.
        /// </summary>
        /// <param name="type">The type to filter the list by.</param>
        /// <returns>The logs view.</returns>
        public ActionResult Logs(string type)
        {
            if (type != null)
            {
                logsModel.UpdateByType(type);
            } else {
                logsModel.UpdateByType(Consts.ALL);
            }
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

        /// <summary>
        /// Removes the handler given.
        /// </summary>
        /// <param name="handler">The handler to remove.</param>
        [HttpPost]
        public void RemoveHandlerConfirmation(string handler)
        {
            settingsModel.RemoveHandler(handler);
        }

        /// <summary>
        /// Get request for ViewPhoto view.
        /// Displays the image with the given path.
        /// </summary>
        /// <param name="imgPath">The img path.</param>
        /// <returns>The ViewPhoto view.</returns>
        public ActionResult ViewPhoto(string imgPath)
        {
            ViewBag.img = photosModel.FindImageByRelativeFullImgPath(imgPath);
            return View();
        }

        /// <summary>
        /// Get request for the DeletePhoto view.
        /// </summary>
        /// <param name="imgPath">The img path of the photo to remove.</param>
        /// <returns>The DeletePhoto view with the path given as a paramater.</returns>
        public ActionResult DeletePhoto(string imgPath)
        {
            ViewBag.img = photosModel.FindImageByRelativeFullImgPath(imgPath);
            return View();
        }

        /// <summary>
        /// Deletes the photo with the given path and redirects to the Photos View.
        /// </summary>
        /// <param name="imgPath">The img path.</param>
        /// <returns>The Photos View.</returns>
        public ActionResult DeletePhotoConfirmation(string imgPath)
        {
            photosModel.DeletePhotoByRelativeFullImgPath(imgPath);
            return RedirectToAction("Photos");
        }
    }
}
