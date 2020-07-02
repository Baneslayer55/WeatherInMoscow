using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeatherInMoscow.Models;

namespace WeatherInMoscow.Controllers
{
    public class HomeController : Controller
    {
        WeatherContext db = new WeatherContext();

        public ActionResult DisplayWeather(int? month, int? year, int? page)
        {
            IQueryable<Weather> weathers = db.Weathers;
            IQueryable<Weather> emptyWeathers = new List<Weather>().AsQueryable();

            WeatherViewModel weatherViewModel = new WeatherViewModel();

            weatherViewModel.YearFilter = year;
            weatherViewModel.MonthFilter = month;

            int pageSize = 16;
            int pageNumber = (page ?? 1);

            Dictionary<int, string> dictionaryToSelectList = new Dictionary<int , string>();
            List<int> yearsFromDatabase = (from yearInt in weathers
                select yearInt.WeatherDateTime.Year).Distinct().ToList();

            foreach (int y in yearsFromDatabase)
            {
                dictionaryToSelectList.Add(y, $"{y}");
            }
            dictionaryToSelectList.Add(0, "Все года");


            if (weathers.Count() == 0)
            {
                ViewBag.Message = "В базе отстуствуют данные. Пожалуйста, загрузите данные в базу.";
            }

            if (year == 0 && month != 0)
            {
                weathers = emptyWeathers;
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

            List<MonthSelectListModel> monthSelectList = new List<MonthSelectListModel>()
            {
                new MonthSelectListModel { MonthId = 0, MonthName = "Все месяца" },
                new MonthSelectListModel { MonthId = 1, MonthName = "Январь" },
                new MonthSelectListModel { MonthId = 2, MonthName = "Февраль" },
                new MonthSelectListModel { MonthId = 3, MonthName = "Март" },
                new MonthSelectListModel { MonthId = 4, MonthName = "Апрель" },
                new MonthSelectListModel { MonthId = 5, MonthName = "Май" },
                new MonthSelectListModel { MonthId = 6, MonthName = "Июнь" },
                new MonthSelectListModel { MonthId = 7, MonthName = "Июль" },
                new MonthSelectListModel { MonthId = 8, MonthName = "Август" },
                new MonthSelectListModel { MonthId = 9, MonthName = "Сентябрь" },
                new MonthSelectListModel { MonthId = 10, MonthName = "Октябрь" },
                new MonthSelectListModel { MonthId = 11, MonthName = "Ноябрь" },
                new MonthSelectListModel { MonthId = 12, MonthName = "Декабрь" },
            };           

            weatherViewModel.Weathers = weathers.OrderBy(w => w.WeatherDateTime).ToPagedList(pageNumber, pageSize);
            weatherViewModel.Month = new SelectList(monthSelectList, "MonthID", "MonthName");
            weatherViewModel.Year = new SelectList(dictionaryToSelectList, "Key", "Value");

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
                    XSSFWorkbook excelFile = new XSSFWorkbook(importedFile.InputStream); //Только для .xlsx. При не обходимости можно добавить xls
                    for (int i = 0; i < excelFile.NumberOfSheets; i++)
                    {
                        sheet = excelFile.GetSheetAt(i);
                        for (int row = 0; row <= sheet.LastRowNum; row++)
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
                ViewBag.Message = ex.Message;
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