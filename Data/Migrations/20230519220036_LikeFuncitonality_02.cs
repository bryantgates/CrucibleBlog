using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrucibleBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class LikeFuncitonality_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogLike_AspNetUsers_BlogUserId",
                table: "BlogLike");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogLike_BlogPost_BlogPostId",
                table: "BlogLike");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogLike",
                table: "BlogLike");

            migrationBuilder.RenameTable(
                name: "BlogLike",
                newName: "BlogLikes");

            migrationBuilder.RenameIndex(
                name: "IX_BlogLike_BlogUserId",
                table: "BlogLikes",
                newName: "IX_BlogLikes_BlogUserId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogLike_BlogPostId",
                table: "BlogLikes",
                newName: "IX_BlogLikes_BlogPostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogLikes",
                table: "BlogLikes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogLikes_AspNetUsers_BlogUserId",
                table: "BlogLikes",
                column: "BlogUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogLikes_BlogPost_BlogPostId",
                table: "BlogLikes",
                column: "BlogPostId",
                principalTable: "BlogPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogLikes_AspNetUsers_BlogUserId",
                table: "BlogLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogLikes_BlogPost_BlogPostId",
                table: "BlogLikes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogLikes",
                table: "BlogLikes");

            migrationBuilder.RenameTable(
                name: "BlogLikes",
                newName: "BlogLike");

            migrationBuilder.RenameIndex(
                name: "IX_BlogLikes_BlogUserId",
                table: "BlogLike",
                newName: "IX_BlogLike_BlogUserId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogLikes_BlogPostId",
                table: "BlogLike",
                newName: "IX_BlogLike_BlogPostId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogLike",
                table: "BlogLike",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogLike_AspNetUsers_BlogUserId",
                table: "BlogLike",
                column: "BlogUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogLike_BlogPost_BlogPostId",
                table: "BlogLike",
                column: "BlogPostId",
                principalTable: "BlogPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
