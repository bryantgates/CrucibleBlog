﻿using CrucibleBlog.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CrucibleBlog.Data
{
	public class ApplicationDbContext : IdentityDbContext<BlogUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public virtual DbSet<BlogLike> BlogLikes { get; set; } = default!;
		public virtual DbSet<BlogUser> BlogUsers { get; set; } = default!;
		public virtual DbSet<Category> Categories { get; set; } = default!;
		public virtual DbSet<Comment> Comments { get; set; } = default!;
		public virtual DbSet<Tag> Tags { get; set; } = default!;
		public virtual DbSet<BlogPost> BlogPost { get; set; } = default!;
	}
}