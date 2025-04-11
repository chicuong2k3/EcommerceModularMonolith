using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductVariants",
                schema: "ordering");

            migrationBuilder.AddColumn<string>(
                name: "AttributesDescription",
                schema: "ordering",
                table: "Products",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "ordering",
                table: "Products",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                schema: "ordering",
                table: "Products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                schema: "ordering",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "SalePrice",
                schema: "ordering",
                table: "Products",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VariantId",
                schema: "ordering",
                table: "Products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttributesDescription",
                schema: "ordering",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "ordering",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                schema: "ordering",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Quantity",
                schema: "ordering",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SalePrice",
                schema: "ordering",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VariantId",
                schema: "ordering",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributesDescription = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ImageUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    SalePrice = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "ordering",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId",
                schema: "ordering",
                table: "ProductVariants",
                column: "ProductId");
        }
    }
}
