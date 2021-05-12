using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyBlog.Data;
using MyBlog.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using MyBlog.Services;

namespace MyBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBlogImageService _imageService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBlogImageService imageService)
        {
            _logger = logger;
            _context = context;
            _imageService = imageService;
        }

        public async Task<IActionResult> IndexAsync(int? page)
        {
            var ImageData = _imageService.EncodeFileAsync("defaultBlogBackground");
            ViewData["HeaderImage"] = _imageService.DecodeImage(ImageData,"png");

            ViewData["HeaderText"] = "Landing Page";
            ViewData["SubText"] = "Landing Page";

            var pageNumber = page ?? 1;
            var pageSize = 5;
            
            //Load the view up 
            var allBlogs = await _context.Blogs.OrderByDescending(b => b.Created)
                                               .ToPagedListAsync(pageNumber, pageSize);

            return View(allBlogs);
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
