using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using HWQueue.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace HWQueue.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        MyQueue myQueue=new MyQueue();
        List<string> mess=new List<string>();
        List<Lot> lots=new List<Lot>();


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Lots = await myQueue.ShowAllAsync();
            return View(mess);
        }
        //Index after choose currency
        [HttpPost]
        public async Task<IActionResult> Index(string id)
        {
            ViewBag.Lots = await myQueue.ShowTarget(id);
            return View();
        }
        //Create Lot
        public async Task<IActionResult> CreateLot() {
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateLot(Lot lot)
        {
            if (ModelState.IsValid)
            {
                myQueue.AddMessageAsync(JsonSerializer.Serialize(lot));
                return RedirectToAction("Index");
            }
           
            return View();
        }

        //Buy Lot
        public async Task<IActionResult> BuyLot(string id)
        {
            await myQueue.DeleteMessage(id);
            System.Threading.Thread.Sleep(2000);
            return RedirectToAction("Index");
        }
       

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}