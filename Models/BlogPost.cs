using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrucibleBlog.Models
{
	public class BlogPost
	{
		public int Id { get; set; }
		[Required]
		[StringLength(200, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
		public string? Title { get; set; }
		[StringLength(600, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
		public string? Abstract { get; set; }
		[Required]
		public string? Content { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime CreatedDate { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime UpdatedDate { get; set;}
		public string? Slug { get; set; }
		public bool IsPublished { get; set; }
		[Required]
		public int CategoryId { get; set; }

		[NotMapped]
		public IFormFile? ImageFile { get; set; }
		public byte[]? ImageData { get; set; }
		public string? ImageType { get; set; }

		//Navigation Properties
		public virtual Category? Category { get; set; }

		//1-to-many
		public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

		//Many-to-Many
		public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
	}
}
