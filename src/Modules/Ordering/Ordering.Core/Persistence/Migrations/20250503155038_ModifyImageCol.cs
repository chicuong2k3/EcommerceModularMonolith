using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Core.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyImageCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                schema: "ordering",
                table: "OrderItems",
                newName: "Image");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Image",
                schema: "ordering",
                table: "OrderItems",
                newName: "ImageUrl");
        }
    }
}
