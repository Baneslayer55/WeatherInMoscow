﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeatherInMoscow.Models;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Globalization;

namespace WeatherInMoscow.Controllers
{
    public class HomeController : Controller
    {
        WeatherContext db = new WeatherContext();

        public ActionResult DisplayWeather(int? month, int? year)
        {
            IQueryable<Weather> weathers = db.Weathers;

            List<int> years = (from yearInt in weathers
                               select yearInt.WeatherDateTime.Year).Distinct().ToList();

            years.Insert(0, 0);

            if (weathers.Count() == 0)
            {
                ViewBag.Message = "Упс, в базе отстуствуют данные. Пожалуйста, загрузите данные в базу.";
            }

            if (year == 0  && month != 0)
            {
                ViewBag.Message = "Пожалуйста, выберите год";
            }      
            else if (year != 0 && month != 0)
            {
                weathers = weathers.Where(w => w.WeatherDateTime.Year == year &&
                                               w.WeatherDateTime.Month == month);
            }
            else if (year != 0 && month == 0)
            {
                weathers = weathers.Where(w => w.WeatherDateTime.Year == year);
            }
            
            List<MonthOfYears> monthOfYears = new List<MonthOfYears>(){
                    new MonthOfYears { MonthId = 0, MonthName = "Все месяца" },
                    new MonthOfYears { MonthId = 1, MonthName = "Январь" },
                    new MonthOfYears { MonthId = 2, MonthName = "Февраль" },
                    new MonthOfYears { MonthId = 3, MonthName = "Март" },
                    new MonthOfYears { MonthId = 4, MonthName = "Апрель" },
                    new MonthOfYears { MonthId = 5, MonthName = "Май" },
                    new MonthOfYears { MonthId = 6, MonthName = "Июнь" },
                    new MonthOfYears { MonthId = 7, MonthName = "Июль" },
                    new MonthOfYears { MonthId = 8, MonthName = "Август" },
                    new MonthOfYears { MonthId = 9, MonthName = "Сентябрь" },
                    new MonthOfYears { MonthId = 10, MonthName = "Октябрь" },
                    new MonthOfYears { MonthId = 11, MonthName = "Ноябрь" },
                    new MonthOfYears { MonthId = 12, MonthName = "Декабрь" },
                };

            WeatherViewModel weatherViewModel = new WeatherViewModel
            {
                Weathers = weathers.ToList(),
                Month = new SelectList(monthOfYears, "MonthID", "MonthName"),
                Year = new SelectList(years)
            };
            return View(weatherViewModel);
        }
        
        public ActionResult UploadWeatherData()
        {
             return View();
        }

        [HttpPost]
        public ActionResult UploadWeatherData(HttpPostedFileBase[] importedFiles)
        {
            WeatherContext db = new WeatherContext();
            try
            {            
                foreach (var importedFile in importedFiles)
                {
                    ISheet sheet;
                    XSSFWorkbook hssfwb = new XSSFWorkbook(importedFile.InputStream); //Только для .xlsx. При не обходимости можно добавить xls
                    for (int i = 0; i < 12; i++)
                    {
                        sheet = hssfwb.GetSheetAt(i);
                        for (int row = 4; row <= sheet.LastRowNum; row++)
                        {
                            try
                            {
                                db.Weathers.Add
                                (new Weather
                                {
                                    WeatherDateTime = DateTime.ParseExact($"{sheet.GetRow(row).GetCell(0)} {sheet.GetRow(row).GetCell(1)}", "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture),
                                    Temperature = Double.TryParse($"{sheet.GetRow(row).GetCell(2)}", out var tempT) ? tempT : (double?)null,
                                    Humidity = Double.TryParse($"{sheet.GetRow(row).GetCell(3)}", out var tempH) ? tempH : (double?)null,
                                    Dewpoint = Double.TryParse($"{sheet.GetRow(row).GetCell(4)}", out var tempD) ? tempD : (double?)null,
                                    Pressure = Int32.TryParse($"{sheet.GetRow(row).GetCell(5)}", out var tempP) ? tempP : (int?)null,
                                    WindDirection = $"{sheet.GetRow(row).GetCell(6)}",
                                    WindSpeed = Int32.TryParse($"{sheet.GetRow(row).GetCell(7)}", out var tempWS) ? tempWS : (int?)null,
                                    Cloudiness = Int32.TryParse($"{sheet.GetRow(row).GetCell(8)}", out var tempC) ? tempC : (int?)null,
                                    LowCloudCover = Int32.TryParse($"{sheet.GetRow(row).GetCell(9)}", out var tempLCC) ? tempLCC : (int?)null,
                                    HorizontalVisibility = Int32.TryParse($"{sheet.GetRow(row).GetCell(10)}", out var tempHV) ? tempHV : (int?)null,
                                    WeatherConditions = $"{sheet.GetRow(row).GetCell(11)}"
                                }
                                );

                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                }
                ViewBag.Message = "Данные успешно загружены";
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message; //Вероятно, стоило написать информацию для пользователя
            }
            db.SaveChanges();
            return View(); 
        }
        public ActionResult Index()
        {
            ViewBag.Message = "IndexPage";
            return View();
        }
    }
}