using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrucibleBlog.Data;
using CrucibleBlog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CrucibleBlog.Services.Interfaces;
using CrucibleBlog.Services;
using CrucibleBlog.Helpers;
using X.PagedList;

namespace CrucibleBlog.Controllers
{
	[Authorize(Roles = "Admin")]
	public class BlogPostsController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IBlogService _blogService;
		private readonly UserManager<BlogUser> _userManager;

		public BlogPostsController(ApplicationDbContext context, UserManager<BlogUser> userManager, IBlogService? blogService)
		{
			_context = context;
			_userManager = userManager;
			_blogService = blogService;
		}

        // GET: BlogPosts
        public async Task<IActionResult> Index(int? pageNum)
        {
            int pageSize = 4;
            int page = pageNum ?? 1;

            IPagedList<BlogPost> blogPosts = (await _context.BlogPost.Include(blogPosts => blogPosts.Category).ToPagedListAsync(pageNum, pageSize));
            return View(blogPosts);
        }

        // GET: BlogPosts/Details/5
        [AllowAnonymous]
		public async Task<IActionResult> Details(string? slug)
		{
			if (string.IsNullOrEmpty(slug))
			{
				return NotFound();
			}

			var blogPost = await _context.BlogPost
										 .Include(b => b.Category)
										 .Include(b => b.Comments)
										 .ThenInclude(c => c.Author)
										 .Include(b => b.Tags)
										 .FirstOrDefaultAsync(m => m.Slug == slug);

			if (blogPost == null)
			{
				return NotFound();
			}

			return View(blogPost);
		}

		// GET: BlogPosts/Create
		public IActionResult Create()
		{
			ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
			return View();
		}

		// POST: BlogPosts/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Title,Abstract,Content,CreatedDate,UpdatedDate,Slug,IsPublished,CategoryId,ImageData,ImageType")] BlogPost blogPost, string? stringTags)
		{
			ModelState.Remove("Slug");

			if (ModelState.IsValid)
			{
				blogPost.CreatedDate = DateTime.UtcNow;

				_context.Add(blogPost);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));

			}

				if (!await _blogService.ValidSlugAsync(blogPost.Title, blogPost.Id))
				{
					ModelState.AddModelError("Title", "A similar Title/Slug is already in use.");

					ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
					return View(blogPost);
				}

			blogPost.Slug = StringHelper.BlogPostSlug(blogPost.Title);
			
			ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
			return View(blogPost);
		}



		// GET: BlogPosts/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.BlogPost == null)
			{
				return NotFound();
			}

			BlogPost? blogPost = await _context.BlogPost.Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == id);

			if (blogPost == null)
			{
				return NotFound();
			}

			List<string> tagNames = blogPost.Tags.Select(t => t.Name!).ToList();

			ViewData["Tags"] = string.Join(", ", tagNames);
			ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
			return View(blogPost);
		}

		// POST: BlogPosts/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,CreatedDate,UpdatedDate,Slug,IsPublished,CategoryId,ImageData,ImageType")] BlogPost blogPost)
		{
			if (id != blogPost.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(blogPost);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!BlogPostExists(blogPost.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
			return View(blogPost);
		}

		// GET: BlogPosts/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.BlogPost == null)
			{
				return NotFound();
			}

			var blogPost = await _context.BlogPost
				.Include(b => b.Category)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (blogPost == null)
			{
				return NotFound();
			}

			return View(blogPost);
		}

		// POST: BlogPosts/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.BlogPost == null)
			{
				return Problem("Entity set 'ApplicationDbContext.BlogPost'  is null.");
			}
			var blogPost = await _context.BlogPost.FindAsync(id);
			if (blogPost != null)
			{
				_context.BlogPost.Remove(blogPost);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool BlogPostExists(int id)
		{
			return (_context.BlogPost?.Any(e => e.Id == id)).GetValueOrDefault();
		}
	}
}
