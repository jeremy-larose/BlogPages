﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BlogProject.Data;
using BlogProject.Enums;
using BlogProject.Models;
using BlogProject.Services;
using BlogProject.ViewModels;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace BlogProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private const int ItemsPerPage = 5;

        public HomeController(ILogger<HomeController> logger, IBlogEmailSender emailSender, ApplicationDbContext context)
        {
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        public async Task<IActionResult> Index( int? page )
        {
            var pageNumber = page ?? 1;

            // var blogs = _context.Blogs
            //     .Where(b => b.Posts.Any(p => p.ReadyStatus == ReadyStatus.ProductionReady))
            //     .OrderByDescending(b => b.Created)
            //     .ToPagedListAsync( pageNumber, ItemsPerPage );
            
            var blogs = _context.Blogs
                .Include( b=>b.BlogUser )
                .OrderByDescending(b => b.Created)
                .ToPagedListAsync( pageNumber, ItemsPerPage );

            return View(await blogs);
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMe model)
        {
            model.Message = $"{model.Message} <hr/> Phone: {model.Phone}";
            // This is where we will be emailing
           await _emailSender.SendContactEmailAsync(model.Email, model.Name, model.Subject, model.Message);
           return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
