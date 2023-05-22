using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CrucibleBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class LikeFunctionality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLiked",
                table: "BlogPost");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "BlogPost");

            migrationBuilder.CreateTable(
                name: "BlogLike",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BlogPostId = table.Column<int>(type: "integer", nullable: false),
                    IsLiked = table.Column<bool>(type: "boolean", nullable: false),
                    BlogUserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogLike", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogLike_AspNetUsers_BlogUserId",
                        column: x => x.BlogUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogLike_BlogPost_BlogPostId",
                        column: x => x.BlogPostId,
                        principalTable: "BlogPost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogLike_BlogPostId",
                table: "BlogLike",
                column: "BlogPostId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogLike_BlogUserId",
                table: "BlogLike",
                column: "BlogUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogLike");

            migrationBuilder.AddColumn<bool>(
                name: "IsLiked",
                table: "BlogPost",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "BlogPost",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
