using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrucibleBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSlugToRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_AuthorId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_BlogPost_BlogPostId1",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Categories_BlogPostId",
                table: "Tags");

            migrationBuilder.RenameColumn(
                name: "Updated",
                table: "Comments",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Comments",
                newName: "CreatedDate");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "BlogPost",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_AuthorId",
                table: "Comments",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_BlogPost_BlogPostId",
                table: "Tags",
                column: "BlogPostId",
                principalTable: "BlogPost",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Categories_BlogPostId1",
                table: "Tags",
                column: "BlogPostId1",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_AuthorId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_BlogPost_BlogPostId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Categories_BlogPostId1",
                table: "Tags");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Comments",
                newName: "Updated");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Comments",
                newName: "Created");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorId",
                table: "Comments",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "BlogPost",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_AuthorId",
                table: "Comments",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_BlogPost_BlogPostId1",
                table: "Tags",
                column: "BlogPostId1",
                principalTable: "BlogPost",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Categories_BlogPostId",
                table: "Tags",
                column: "BlogPostId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
