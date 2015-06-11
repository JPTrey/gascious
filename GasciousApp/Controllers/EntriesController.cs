using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GasciousApp.Models;
using Microsoft.AspNet.Identity;

namespace GasciousApp.Controllers
{
    [Authorize]
    public class EntriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Entries
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                RedirectToAction("Login", "Account");
                return View();
            }
            
            string Username = User.Identity.GetUserName();
            Entry[] AllEntries = db.Entries.ToArray();
            Entry[] UserEntries = new Entry[AllEntries.Count()];  // entries belonging to user currently logged in
            int i = 0;
            foreach (var item in AllEntries)
            {
                if (item.Username == Username)
                {
                    UserEntries[i] = item;
                    i++;
                }
            }

            i = 0;
            int UserEntryCount = UserEntries.Count();
            decimal[] UserPrices = new decimal[UserEntryCount];
            decimal[] UserGallons = new decimal[UserEntryCount];
            decimal[] UserMiles = new decimal[UserEntryCount];
            string[] UserStations = new string[UserEntryCount];
            string[] UserVehicles = new string[UserEntryCount];
            
            Dictionary<string, int>     VehicleInstances    = new Dictionary<string, int>();            // number of entries per vehicle (for averaging)
            Dictionary<string, decimal> VehicleTotalPrice   = new Dictionary<string, decimal>();        // amount spent per vehicle
            Dictionary<string, decimal> VehicleTotalGallons = new Dictionary<string, decimal>();        // amount pumped per vehicle
            Dictionary<string, decimal> VehicleTotalMiles   = new Dictionary<string, decimal>();        // avg miles between fillups per vehicle
            Dictionary<string, decimal> VehicleMinMiles     = new Dictionary<string, decimal>();        // shortest trip traveled per vehicle
            Dictionary<string, decimal> VehicleMaxMiles     = new Dictionary<string, decimal>();            // longest trip traveled per vehicle
            Dictionary<string, int>     StationInstances    = new Dictionary<string, int>();            // number of entries per station (for averaging)
            Dictionary<string, decimal> StationTotalPrice   = new Dictionary<string, decimal>();        // amount paid to each station
            Dictionary<string, decimal> StationTotalGallons = new Dictionary<string, decimal>();        // amount filled at each station

            foreach (var item in UserEntries)
            {
                if (item != null)
                {
                   // for averaging
                    UserPrices[i] = item.Price;
                    UserGallons[i] = item.Gallons;
                    UserStations[i] = item.Station;
                    UserVehicles[i] = item.Vehicle;

                    // if: new vehicle in dictionary, initiate keys
                    if (!VehicleInstances.ContainsKey(item.Vehicle))
                    {
                        VehicleInstances.Add(item.Vehicle, 1);
                        VehicleTotalPrice.Add(item.Vehicle, item.Price);
                        VehicleTotalGallons.Add(item.Vehicle, item.Gallons);
                        VehicleTotalMiles.Add(item.Vehicle, item.Miles);
                        VehicleMinMiles.Add(item.Vehicle, item.Miles);
                        VehicleMaxMiles.Add(item.Vehicle, item.Miles);
                    }
                    else
                    {
                        VehicleInstances[item.Vehicle]++;
                        VehicleTotalPrice[item.Vehicle] += item.Price;
                        VehicleTotalGallons[item.Vehicle] += item.Gallons;
                        VehicleTotalMiles[item.Vehicle] += item.Miles;
                        
                        if (item.Miles < VehicleMinMiles[item.Vehicle])
                        {
                            VehicleMinMiles[item.Vehicle] = item.Miles;
                        }
                        else if (item.Miles > VehicleMaxMiles[item.Vehicle])
                        {
                            VehicleMaxMiles[item.Vehicle] = item.Miles;
                        }
                    }

                    // if: new station in dictionary, initiate keys
                    if (!StationTotalPrice.ContainsKey(item.Station))
                    {
                        StationInstances.Add(item.Station, 1);
                        StationTotalPrice.Add(item.Station, item.Price);
                        StationTotalGallons.Add(item.Station, item.Gallons);
                    }
                    else
                    {
                        StationInstances[item.Station]++;
                        StationTotalPrice[item.Station] += item.Price;
                        StationTotalGallons[item.Station] += item.Gallons;
                    }
                        i++;
                }
            }

            /* calculated value dictionaries */
            Dictionary<string, decimal> VehicleAvgPrices = new Dictionary<string,decimal>();     // avg transaction price per vehicle
            foreach (var item in VehicleInstances)
            {
                VehicleAvgPrices.Add(item.Key, VehicleTotalPrice[item.Key]/VehicleInstances[item.Key]);
            }

            Dictionary<string, decimal> VehicleAvgMiles = new Dictionary<string, decimal>();     // avg miles between fillups per vehicle
            foreach (var item in VehicleInstances)
            {
                VehicleAvgMiles.Add(item.Key, VehicleTotalMiles[item.Key] / VehicleInstances[item.Key]);
            }

            Dictionary<string, decimal> VehicleDPM = new Dictionary<string, decimal>();         // dollar/mile ratio
            foreach (var item in VehicleInstances)
            {
                if (VehicleTotalMiles[item.Key] > 0)
                {
                    VehicleDPM.Add(item.Key, VehicleTotalPrice[item.Key] / VehicleTotalMiles[item.Key]);
                }
            }
            
            Dictionary<string, decimal> StationAvgPrices = new Dictionary<string, decimal>();     // avg transaction price per station
            foreach (var item in StationInstances)
            {
                StationAvgPrices.Add(item.Key, StationTotalPrice[item.Key] / StationInstances[item.Key]);
            }

            Dictionary<string, decimal> StationDPG = new Dictionary<string, decimal>();     // avg price per gallon, each station
            foreach (var item in StationInstances)
            {
                StationDPG.Add(item.Key, StationTotalPrice[item.Key] / StationTotalGallons[item.Key]);
            }

            /* Dictionaries for graphs */
            ViewBag.VehicleTotalPrices = VehicleTotalPrice;
            ViewBag.StationTotalPrices = StationTotalPrice;
            ViewBag.VehicleTotalGallons = VehicleTotalGallons;
            ViewBag.StationTotalGallons = StationTotalGallons;
            ViewBag.VehicleAvgPrices = VehicleAvgPrices;
            ViewBag.StationAvgPrices = StationAvgPrices;
            ViewBag.VehicleAvgMiles = VehicleAvgMiles;
            ViewBag.VehicleDPM = VehicleDPM;
            ViewBag.StationDPG = StationDPG;
            ViewBag.VehicleMinMiles = VehicleMinMiles;
            ViewBag.VehicleMaxMiles = VehicleMaxMiles;
            
            return View();
        }

        // GET: Entries/Manage
        public ActionResult Manage()
        {
            ViewBag.Username = User.Identity.GetUserName();
            string HasEntries = "false";
            foreach (var entry in db.Entries.ToList())
            {
                if (entry.Username == User.Identity.GetUserName())
                {
                    HasEntries = "true";
                    break;
                }
            }

            ViewBag.HasEntries = HasEntries;
            List<Entry> EntryList = db.Entries.ToList();
            var OrderedList = from item in EntryList
                              orderby item.Date descending
                              select item;
            return View(OrderedList);
        }

        // GET: Entries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entry entry = db.Entries.Find(id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            return View(entry);
        }

        // GET: Entries/Create
        public ActionResult Create()
        {
            List<string> VehicleList = new List<string>();
            foreach (var entry in db.Entries.ToList())
            {
                if (entry.Username == User.Identity.GetUserName() && !VehicleList.Contains<string>(entry.Vehicle))  // if: vehicle belongs to user and isn't present
                {
                    VehicleList.Add(entry.Vehicle.ToString());
                }
            }
            ViewBag.VehicleList = VehicleList;
            return View();
        }

        // POST: Entries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,Price,Gallons,Station,Vehicle,Miles,Username")] Entry entry)
        {
            if (ModelState.IsValid)
            {
                if (entry.Vehicle == null)
                {
                    entry.Vehicle = Request.Form["vehicle-existing"];
                }
                entry.Date = DateTime.Now;
                entry.Username = User.Identity.GetUserName();
                db.Entries.Add(entry);
                db.SaveChanges();
                return RedirectToAction("Manage");
            }

            return View(entry);
        }

        // GET: Entries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entry entry = db.Entries.Find(id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            List<string> VehicleList = new List<string>();
            foreach (var item in db.Entries.ToList())
            {
                if (item.Username == User.Identity.GetUserName() && !VehicleList.Contains<string>(item.Vehicle))  // if: vehicle belongs to user and isn't present
                {
                    VehicleList.Add(item.Vehicle.ToString());
                }
            }
            ViewBag.VehicleList = VehicleList;
            return View(entry);
        }

        // POST: Entries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,Price,Gallons,Station,Vehicle,Miles,Username")] Entry entry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(entry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Manage");
            }
            return View(entry);
        }

        // GET: Entries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entry entry = db.Entries.Find(id);
            if (entry == null)
            {
                return HttpNotFound();
            }
            return View(entry);
        }

        // POST: Entries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Entry entry = db.Entries.Find(id);
            db.Entries.Remove(entry);
            db.SaveChanges();
            return RedirectToAction("Manage");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
