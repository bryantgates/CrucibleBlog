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
		private readonly IImageService _imageService;

        public BlogPostsController(ApplicationDbContext context, UserManager<BlogUser> userManager, IBlogService? blogService, IImageService imageService)
        {
            _context = context;
            _userManager = userManager;
            _blogService = blogService;
            _imageService = imageService;
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

			BlogPost? blogPost = await _context.BlogPost
										 .Include(b => b.Category)
										 .Include(b => b.Comments)
										 .ThenInclude(c => c.Author)
										 .Include(b => b.Tags)
										 .Include(b => b.Likes)
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
			BlogPost blogPost = new BlogPost();
			return View(blogPost);
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

				blogPost.Slug = StringHelper.BlogPostSlug(blogPost.Title);

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

            if (!string.IsNullOrEmpty(stringTags))
            {
                await _blogService.AddTagsToBlogPostAsync(stringTags, blogPost.Id);
            }

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
		public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,Content,CreatedDate,IsPublished,CategoryId,ImageFile,ImageData,ImageType")] BlogPost blogPost, string? stringTags)
		{
			if (id != blogPost.Id)
			{
				return NotFound();
			}

			ModelState.Remove("Slug");

			if (ModelState.IsValid)
			{
				try
				{
                    if (!await _blogService.ValidSlugAsync(blogPost.Title, blogPost.Id))
                    {
                        ModelState.AddModelError("Title", "A similar Title/Slug is already in use.");

						ViewData["Tags"] = stringTags != null && stringTags.Trim().EndsWith(",") ? stringTags : stringTags;
                        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
                        return View(blogPost);
                    }

					blogPost.Slug = StringHelper.BlogPostSlug(blogPost.Title);

					if(blogPost.ImageFile != null)
					{
						blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.ImageFile);
						blogPost.ImageType = blogPost.ImageFile.ContentType;
					}

					blogPost.UpdatedDate = DateTime.UtcNow;
                    blogPost.CreatedDate = DateTime.SpecifyKind(blogPost.CreatedDate, DateTimeKind.Utc);

                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();

                    await _blogService.RemoveAllBlogPostTagsAsync(blogPost.Id);

					if (!string.IsNullOrEmpty(stringTags)) 
					{
						await _blogService.AddTagsToBlogPostAsync(stringTags, blogPost.Id);
					}
					
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
				return RedirectToAction(nameof(Details), new { slug = blogPost.Slug });
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

		public async Task<IActionResult> LikeBlogPost(int blogPostId, string blogUserId)
		{
			//check if user has already liked this blog
			//1. get the user
			BlogUser? blogUser = await _context.Users.Include(u => u.BlogLikes).FirstOrDefaultAsync(u => u.Id == blogUserId);
			bool result = false;
			BlogLike? blogLike = new();

			if (blogUser != null)
			{
				if (!blogUser.BlogLikes.Any(bl => bl.BlogPostId == blogPostId)) 
				{
					blogLike = new BlogLike()
					{
						BlogPostId = blogPostId,
						IsLiked = true
					};

					blogUser.BlogLikes.Add(blogLike);
				}
				result = blogLike.IsLiked;
				await _context.SaveChangesAsync();
			}

			return Json(new
			{
				isLiked = result,
				count = _context.BlogLikes.Where(bl => bl.BlogPostId == blogPostId && bl.IsLiked == true).Count()
			});
			
		}
	}
}
