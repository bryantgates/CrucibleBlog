using System.Diagnostics;
using CrucibleBlog.Data;
using CrucibleBlog.Models;
using CrucibleBlog.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CrucibleBlog.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _context;
		private readonly IBlogService _blogService;

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBlogService blogService)
		{
			_logger = logger;
			_context = context;
			_blogService = blogService;
		}

		public async Task<IActionResult> Index(int? pageNum)
		{
			int pageSize = 4;
			int page = pageNum ?? 1;

			IPagedList<BlogPost> blogPosts = (await _context.BlogPost.Include(blogPosts => blogPosts.Category).ToPagedListAsync(pageNum, pageSize));
			return View(blogPosts);
		}

		public async Task<IActionResult> SearchIndexAsync(int? pageNum, string? searchString)
		{
			int pageSize = 4;
			int page = pageNum ?? 1;

			IPagedList<BlogPost> blogPosts = await _blogService.SearchBlogPosts(searchString).ToPagedListAsync(page, pageSize);
			ViewData["ActionName"] = "SearchIndex";

			return View(blogPosts);
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