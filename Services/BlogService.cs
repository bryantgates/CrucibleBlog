using System.Collections.Immutable;
using System.ComponentModel;
using CrucibleBlog.Data;
using CrucibleBlog.Helpers;
using CrucibleBlog.Models;
using CrucibleBlog.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CrucibleBlog.Services
{
	public class BlogService : IBlogService
	{

		private readonly ApplicationDbContext _context;
		

		public BlogService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task AddTagsToBlogPostAsync(string tagNames, int? blogPostId)
		{
			try
			{
				BlogPost? blogPost = await _context.BlogPost.FirstOrDefaultAsync(b => b.Id == blogPostId);
				if (blogPost == null)
				{
					return;
				}

				List<string> tags = tagNames.Split(',').ToList();

				foreach (string? tagName in tags) 
				{
					if (string.IsNullOrWhiteSpace(tagName)) continue;

					Tag? tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name!.Trim() == tagName.Trim().ToLower());
				
					if (tag == null)
					{
						tag = new Tag()
						{
							Name = tagName.Trim(),
						};

						_context.Tags.Add(tag);
					}
					blogPost.Tags.Add(tag);
				
				}
				await _context.SaveChangesAsync();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public Task AddTagsToBlogPostAsync(IEnumerable<int>? tagIds, int? blogPostId)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<Category>> GetCategoriesAsync()
		{
			try
			{
				IEnumerable<Category> categories = new List<Category>();


				categories=await _context.Categories.ToListAsync();
					return categories;
				
			}
			catch (Exception)
			{

				throw;
			}
		}

		public Task<IEnumerable<BlogPost>> GetPopularBlogPostsAsync()
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<BlogPost>> GetPopularBlogPostsAsync(int? count)
		{
			try
			{
				IEnumerable<BlogPost> blogPosts = await _context.BlogPost
																.Where(b => b.IsDeleted == false && b.IsPublished == true)
																.Include(b => b.Category)
																.Include(b => b.Comments)
																	.ThenInclude(c=>c.Author)
																.Include(b => b.Tags)
																.ToListAsync();

				return blogPosts.OrderByDescending(b=>b.Comments.Count).Take(count!.Value);
			}
			catch (Exception)
			{

				throw;
			}
		}

		public Task<IEnumerable<BlogPost>> GetRecentBlogPostAsync()
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<BlogPost>> GetRecentBlogPostAsync(int? count)
		{
			throw new NotImplementedException();
		}

		public Task<bool> IsTagOnBlogPostAsync(int? tagId, int? blogPostId)
		{
			throw new NotImplementedException();
		}

		public async Task RemoveAllBlogPostTagsAsync(int? blogPostId)
		{
			try
			{
				BlogPost? blogPost = await _context.BlogPost.Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == blogPostId);

				if (blogPost == null) return;

				blogPost.Tags.Clear();

				_context.Update(blogPost);
				await _context.SaveChangesAsync();
			}
			catch (Exception)
			{

				throw;
			}
		}

		public IEnumerable<BlogPost> SearchBlogPosts(string? searchString)
		{
			try
			{
				IEnumerable<BlogPost> blogPosts = new List<BlogPost>();

				if (string.IsNullOrEmpty(searchString))
				{
					return blogPosts;
				}
				else
				{
					searchString = searchString.Trim().ToLower();

					blogPosts = _context.BlogPost.Where(b => b.Title!.ToLower().Contains(searchString) ||
														b.Abstract!.ToLower().Contains(searchString) ||
														b.Content!.ToLower().Contains(searchString) ||
														b.Category!.Name!.ToLower().Contains(searchString) ||
														b.Comments!.Any(c => c.Body!.ToLower().Contains(searchString) ||
																			c.Author!.FirstName.ToLower().Contains(searchString) ||
																			c.Author!.LastName.ToLower().Contains(searchString)) ||
														b.Tags.Any(t => t.Name!.ToLower().Contains(searchString)))
												 .Include(b => b.Comments)
													.ThenInclude(c => c.Author)
												 .Include(b => b.Category)
												 .Include(b => b.Tags)
												 .Where(b => b.IsDeleted == false && b.IsPublished == true)
												 .AsNoTracking()
												 .OrderByDescending(b => b.CreatedDate)
												 .AsEnumerable();


					return blogPosts;
				}
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<bool> ValidSlugAsync(string? title, int? blogPostId)
		{
			try
			{
				string? newSlug = StringHelper.BlogPostSlug(title);

				if(blogPostId == null || blogPostId == 0)
				{
					//Creating BlogPost
					return !await _context.BlogPost.AnyAsync(b => b.Slug == newSlug);
				}
				else
				{
					//Editing BlogPost
					BlogPost? blogPost = await _context.BlogPost.AsNoTracking().FirstOrDefaultAsync(b => b.Id == blogPostId);

					string? oldSlug = blogPost?.Slug;

					if(!string.Equals(oldSlug, newSlug)) 
					{
						return !await _context.BlogPost.AnyAsync(b => b.Id != blogPost!.Id && b.Slug == newSlug);
					}
				}
				return true;
			}
			catch (Exception)
			{

				throw;
			}
			throw new NotImplementedException();
		}

		IEnumerable<BlogPost> IBlogService.SearchBlogPosts(string? searchString)
		{
			throw new NotImplementedException();
		}

		public async Task<List<Tag>> GetTagsAsync()
		{
			List<Tag> tags = await _context.Tags.ToListAsync();

			return tags;
		}

		public async Task<bool> UserLikedBlogAsync(int blogPostId, string blogUserId)
		{
			try
			{
				return await _context.BlogLikes.AnyAsync(bl => bl.BlogPostId == blogPostId && bl.IsLiked == true && bl.BlogUserId == blogUserId);
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<IEnumerable<BlogPost>> GetFavoriteBlogPostsAsync(string? blogUserId)
		{
			try
			{
				List<BlogPost> blogPosts = new();
				if (!string.IsNullOrEmpty(blogUserId))
				{
					BlogUser? blogUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == blogUserId);
					if (blogUser != null)
					{
						//List<int> blogPostIds = _context.BlogLikes.Where(bl => bl.BlogUserId == blogUserId && bl.IsLiked == true).Select(b => b.BlogPostId).ToList();
						blogPosts = await _context.BlogPost.Where(b => b.Likes.Any(l => l.BlogUserId == blogUserId && l.IsLiked == true) &&
																					b.IsPublished == true &&
																					b.IsDeleted == false)
															.Include(b => b.Likes)
															.Include(b => b.Comments)
															.Include(b => b.Category)
															.Include(b => b.Tags)
															.OrderByDescending(b => b.CreatedDate)
															.ToListAsync();
					}
				}
				return blogPosts;
			}
			catch (Exception)
			{

				throw;
			}
		}
	}
}
