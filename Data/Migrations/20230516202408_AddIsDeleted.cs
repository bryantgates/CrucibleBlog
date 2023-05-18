using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrucibleBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "BlogPost",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BlogPost",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BlogPost");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "BlogPost",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
