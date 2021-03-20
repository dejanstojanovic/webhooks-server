using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Webhooks.Data.Infrastructure.Migrations
{
    public partial class Initial_Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "webhooks");

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                schema: "webhooks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Timeout = table.Column<double>(type: "float", nullable: false, defaultValue: 120.0),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Event = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionHeaders",
                schema: "webhooks",
                columns: table => new
                {
                    SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionHeaders", x => new { x.SubscriptionId, x.Key });
                    table.ForeignKey(
                        name: "FK_SubscriptionHeaders_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalSchema: "webhooks",
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Event_Endpoint",
                schema: "webhooks",
                table: "Subscriptions",
                columns: new[] { "Event", "Endpoint" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionHeaders",
                schema: "webhooks");

            migrationBuilder.DropTable(
                name: "Subscriptions",
                schema: "webhooks");
        }
    }
}
