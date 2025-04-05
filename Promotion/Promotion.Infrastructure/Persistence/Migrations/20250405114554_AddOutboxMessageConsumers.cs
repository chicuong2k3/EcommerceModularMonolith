using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotion.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxMessageConsumers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutboxMessageConsumers",
                schema: "promotion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OutboxMessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessageConsumers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessageConsumers",
                schema: "promotion");
        }
    }
}
