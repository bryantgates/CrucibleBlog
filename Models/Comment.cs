using System.ComponentModel.DataAnnotations;

namespace CrucibleBlog.Models
{
	public class Comment
	{
		public int Id { get; set; }
		[Required]
		[Display(Name = "Comment")]
		[StringLength(5000, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
		public string? Body { get; set; }
		[DataType(DataType.DateTime)]
		[Required]
		public DateTime CreatedDate { get; set; }
		[DataType(DataType.DateTime)]
		public DateTime? UpdatedDate { get; set;}
		[StringLength(200, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
		public string? UpdatedReason { get; set; }
		public int BlogPostId { get; set; }
		public string? AuthorId { get; set; }

		//Navigation Properties
		public virtual BlogPost? BlogPost { get; set; }
		public virtual BlogUser? Author { get; set; }
	}
}
