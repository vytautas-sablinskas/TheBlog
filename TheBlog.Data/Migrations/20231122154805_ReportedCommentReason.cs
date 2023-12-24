using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBlog.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReportedCommentReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "ReportedComments");

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ReportedComments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ReportedComments");

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "ReportedComments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
