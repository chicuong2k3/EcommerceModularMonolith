using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Core.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModifyImageCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image_Url",
                schema: "catalog",
                table: "ProductVariant");

            migrationBuilder.RenameColumn(
                name: "Image_AltText",
                schema: "catalog",
                table: "ProductVariant",
                newName: "ImageAltText");

            migrationBuilder.AddColumn<string>(
                name: "ImageData",
                schema: "catalog",
                table: "ProductVariant",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                schema: "catalog",
                table: "ProductVariant");

            migrationBuilder.RenameColumn(
                name: "ImageAltText",
                schema: "catalog",
                table: "ProductVariant",
                newName: "Image_AltText");

            migrationBuilder.AddColumn<string>(
                name: "Image_Url",
                schema: "catalog",
                table: "ProductVariant",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
