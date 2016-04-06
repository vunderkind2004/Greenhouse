using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GreenHouse.Interfaces.Repository;
using GreenHouse.Repository.DataModel;
using GreenHouse.Repository.Repository;
using GreenHouse.ViewModels;

namespace GreenHouse.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<SensorType> sensorRepository;
        public HomeController(IRepository<SensorType> sensorRepository)
        {
            this.sensorRepository = sensorRepository;
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AddSensorType(SensorTypeViewModel model)
        {
            var sensorType = new SensorType{TypeName = model.TypeName, Dimension = model.Dimension};
            sensorRepository.Create(sensorType);
            return RedirectToAction("Index");
        }

    }
}
