using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using INF272Group11Project.Models;
using INF272Group11Project.ViewModels;

namespace INF272Group11Project.Controllers
{
    public class VotingStationController : Controller
    {
        VotingSystemProjectEntities2 db = new VotingSystemProjectEntities2();
        AddVotingStationVM AddVotingStationVM = new AddVotingStationVM();
        // GET: VotingStation
        public ActionResult AddVotingStation(string StaffGUID, string id)
        {
            ViewBag.message = TempData["message"];
            ViewBag.success = TempData["success"];
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUIDVM = new StaffGUIDControl();
                if (staffGUIDVM.IsLogedIn(db, StaffGUID))
                {
                    staffGUIDVM.RefreshGUID(db);
                    AddVotingStationVM AVM = new AddVotingStationVM();
                    AVM.StaffView = staffGUIDVM;
                    ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceID", "ProvinceName");
                    ViewBag.CityOrTownID = new SelectList(db.CityOrTowns, "CityOrTownID", "CityOrTownName");
                    ViewBag.SuburbID = new SelectList(db.Suburbs, "SuburbID", "SuburbName");
                    return View(AVM);
                }
                else
                {
                    TempData["message"] = "An Errorc Occured Please Try Again";
                    return RedirectToAction("StaffHomePage", "Staff", new { StaffGUID = StaffGUID });
                }
            }
            else
            {
                TempData["message"] = "Your Session Has Expired Please Login Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }

        }

        public ActionResult doAddVotingStation(string StaffGUID, string id, string VotingStationName, [Bind(Include = "SuburbID, ProvinceID, CityOrTownID")] VotingStation vs, string StreetAddress, string Longitude, string Latitude, string OpeningTime, string ClosingTime)
        {
            if (StaffGUID != null && id != null && VotingStationName != null && StreetAddress != null && Longitude != null && Latitude != null && OpeningTime != null && ClosingTime != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    var searchVotingStation = db.VotingStations.Where(x => x.VotingStationName == VotingStationName).FirstOrDefault();
                    if (searchVotingStation == null)
                    {

                        VotingStation A = new VotingStation();
                        A.VotingStationName = VotingStationName;
                        A.VotingStationLongitude = Convert.ToInt32(Longitude);
                       A.VotingStationLatitude = Convert.ToInt32(Latitude);
                        A.VotingStationOpeningTime = Convert.ToDateTime(OpeningTime);
                        A.VotingStationClosingTime = Convert.ToDateTime(ClosingTime);
                        A.VotingStationStreetAddress = StreetAddress;
                        A.SuburbID = Convert.ToInt32(vs.SuburbID);
                        A.ProvinceID = Convert.ToInt32(vs.ProvinceID);
                        A.CityOrTownID = Convert.ToInt32(vs.CityOrTownID);
                        db.VotingStations.Add(A);
                        db.SaveChanges();

                        TempData["success"] = "The Voting Station Has Been Added Successfully";
                        return RedirectToAction("StaffHomePage", "Staff", new { StaffGUID = StaffGUID });
                    }
                    else
                    {
                        TempData["message"] = "The Voting Station Already Exists";
                        return RedirectToAction("AddVotingStation", "VotingStation", new { StaffGUID = StaffGUID, id = id });
                    }
                }
                else
                {
                    TempData["message"] = "Your Session Has Expired, Please Login Again!";
                    return RedirectToAction("StaffLogin","Staff");
                }
            }
            else
            {
                TempData["message"] = "Please Fill In All of Your Details";
                return RedirectToAction("AddVotingStation", "VotingStation", new { StaffGUID = StaffGUID, id = id });
            }

        }
        public ActionResult UpdateDeleteVotingStation(string StaffGUID, string id)
        {
            ViewBag.message = TempData["message"];
            ViewBag.success = TempData["success"];
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUIDVM = new StaffGUIDControl();
                if (staffGUIDVM.IsLogedIn(db, StaffGUID))
                {
                    staffGUIDVM.RefreshGUID(db);
                    AddVotingStationVM AVM = new AddVotingStationVM();
                    AVM.StaffView = staffGUIDVM;
                    ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceID", "ProvinceName");
                    ViewBag.CityOrTownID = new SelectList(db.CityOrTowns, "CityOrTownID", "CityOrTownName");
                    ViewBag.SuburbID = new SelectList(db.Suburbs, "SuburbID", "SuburbName");
                    AVM.VotingStation = TempData["TempSearch"] as VotingStation;
                    return View(AVM);
                }
                else
                {
                    TempData["message"] = "An Errorc Occured Please Try Again";
                    return RedirectToAction("StaffHomePage", "Staff", new { StaffGUID = StaffGUID });
                }
            }
            else
            {
                TempData["message"] = "Your Session Has Expired Please Login Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }
        }
        [HttpPost]
        public ActionResult SearchVotingStation(string StaffGUID, string id, string VotingStationName, string StreetAddress, [Bind(Include = "SuburbID, ProvinceID, CityOrTownID")] VotingStation vs)
        {
            ViewBag.message = TempData["message"];
            ViewBag.success = TempData["success"];

            if (StaffGUID != null)
            {
                if (VotingStationName != null && StreetAddress != null && vs.ProvinceID != null && vs.SuburbID != null && vs.CityOrTownID != null)
                {
                    var SearchVotingStation = db.VotingStations.Where(x => x.VotingStationName == VotingStationName && x.ProvinceID == vs.ProvinceID && x.SuburbID == vs.SuburbID && x.CityOrTownID == vs.CityOrTownID && x.VotingStationStreetAddress == StreetAddress).FirstOrDefault();
                    if (SearchVotingStation != null)
                    {
                        TempData["TempSearch"] = SearchVotingStation;
                        TempData["success"] = "The Station Was Successfully Found!";
                        return RedirectToAction("UpdateDeleteVotingStation", new { StaffGUID = StaffGUID, id = id });
                    }
                    else
                    {
                        TempData["message"] = "The Voting Station You Have Searched For, Does Not Exist! Please Ensure That All Information Is Correct";
                        return RedirectToAction("UpdateDeleteVotingStation", new { StaffGUID = StaffGUID, id = id });
                    }

                }
                else
                {
                    TempData["message"] = "Please Ensure Fill In All Fields!";
                    return RedirectToAction("UpdateDeleteVotingStation", new { StaffGUID = StaffGUID, id = id });
                }
            }
            else
            {
                TempData["message"] = "Your Session Has Expired Please Login Again";
                return RedirectToAction("StaffLogin","Staff");
            }
        }
        [HttpPost]
        public ActionResult UpdateVotingStation(string StaffGUID, string id, string VotingStationID)
        {
            ViewBag.message = TempData["message"];
            ViewBag.success = TempData["success"];
            if (StaffGUID != null && id != null)
            {
                if (VotingStationID != null)
                {

                
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                    if (staffGUID.IsLogedIn(db, StaffGUID))
                    {
                        staffGUID.RefreshGUID(db);
                        var vids = Convert.ToInt32(VotingStationID);
                        var searchVotingStation = db.VotingStations.Where(x => x.VotingStationID == vids).FirstOrDefault();
                        if (searchVotingStation != null)
                        {
                            AddVotingStationVM add = new AddVotingStationVM();
                            add.StaffView = staffGUID;
                            add.VotingStation = searchVotingStation;
                            ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceID", "ProvinceName");
                            ViewBag.CityOrTownID = new SelectList(db.CityOrTowns, "CityOrTownID", "CityOrTownName");
                            ViewBag.SuburbID = new SelectList(db.Suburbs, "SuburbID", "SuburbName");
                            return View(add);
                        }
                        else
                        {
                            TempData["message"] = "The Voting Station You Have Searched For, Does Not Exist!";
                            return RedirectToAction("UpdateDeleteVotingStation", "VotingStation", new { StaffGUID = StaffGUID, id = id });
                        }
                    }
                    else
                    {
                        TempData["message"] = "The Please Search for a Voting Station!";
                        return RedirectToAction("UpdateDeleteVotingStation", "VotingStation", new { StaffGUID = StaffGUID, id = id });
                    }
                

                }
                else
                {
                    TempData["message"] = "Your Session Has Expired, Please Login Again!";
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            else
            {
                TempData["message"] = "Your Session Has Expired, Please Login Again!";
                return RedirectToAction("StaffLogin","Staff");
            }
        }

        public ActionResult doVotingStationUpdate(string StaffGUID, string id, string VotingStationID, string VotingStationName, [Bind(Include = "SuburbID, ProvinceID, CityOrTownID")] VotingStation vs, string StreetAddress, string Longitude, string Latitude, string OpeningTime, string ClosingTime)
        {
            if (StaffGUID != null && id != null && VotingStationID != null && VotingStationName != null && StreetAddress != null && Longitude != null && Latitude != null && OpeningTime != null && ClosingTime != null)
            {
                var ids = Convert.ToInt32(VotingStationID);
                var search = db.VotingStations.Where(x => x.VotingStationID == ids).FirstOrDefault();
                if (search != null)
                {
                    var searchName = db.VotingStations.Where(j => j.VotingStationID != ids && j.VotingStationName == VotingStationName).FirstOrDefault();
                    if (searchName == null)
                    {
                        search.VotingStationName = VotingStationName;
                        search.VotingStationLongitude = Convert.ToInt32(Longitude);
                        search.VotingStationLatitude = Convert.ToInt32(Latitude);
                        search.VotingStationOpeningTime = Convert.ToDateTime(OpeningTime);
                        search.VotingStationClosingTime = Convert.ToDateTime(ClosingTime);
                        search.VotingStationStreetAddress = StreetAddress;
                        search.SuburbID = vs.SuburbID;
                        search.ProvinceID = vs.ProvinceID;
                        search.CityOrTownID = vs.CityOrTownID;
                        db.VotingStations.Add(search);
                        db.SaveChanges();
                        TempData["success"] = "The Voting Station Has Been Successfully Updated!";
                        return RedirectToAction("StaffHomePage", "Staff", new { StaffGUID = StaffGUID });
                    }
                    else
                    {
                        TempData["message"] = "Voting Sation Could Not be Found!";
                        return RedirectToAction("UpdateVotingStation", "VotingStation", new { StaffGUID = StaffGUID, id = id, VotingStationID = VotingStationID });
                    }
                }
                else
                {
                    TempData["message"] = "The Voting Station does not exist!";
                    return RedirectToAction("UpdateVotingStation", "VotingStation", new { StaffGUID = StaffGUID, id = id, VotingStationID = VotingStationID });
                }

            }
            else
            {
                TempData["message"] = "Please Fill In All Of The Fields";
                return RedirectToAction("UpdateVotingStation", "VotingStation", new { StaffGUID = StaffGUID, id = id, VotingStationID = VotingStationID });
            }


        }
        public ActionResult DeleteVotingStation(string StaffGUID, string id, string VotingStationID)
        {
            if(StaffGUID != null && id != null)
            {
                if (VotingStationID != null)
                {
                    var ids = Convert.ToInt32(VotingStationID);
                    VotingStation search = db.VotingStations.Where(x => x.VotingStationID == ids).FirstOrDefault();
                    if (search != null)
                    {

                        db.VotingStations.Remove(search);
                        db.SaveChanges();
                        TempData["success"] = "The Voting Station was Deleted Successfully!";
                        return RedirectToAction("StaffHomePage", "Staff", new { StaffGUID = StaffGUID });
                    }
                    else
                    {
                        TempData["message"] = "The Voting Station Could Not Be Found! Please Try Search Again!";
                        return RedirectToAction("UpdateDeleteVotingStation", "VotingStation", new { SraffGUID = StaffGUID, id = id });
                    }
                }
                else
                {
                    TempData["message"] = "The Please Search for a Voting Station!";
                    return RedirectToAction("UpdateDeleteVotingStation", "VotingStation", new { StaffGUID = StaffGUID, id = id });
                }
            
            }
            else
            {
                TempData["message"] = "Your Session Has Expired Please Login Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }
        }
    }
}