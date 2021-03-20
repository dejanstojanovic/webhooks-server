using Microsoft.EntityFrameworkCore.Migrations;

namespace Webhooks.Data.Infrastructure.Migrations
{
    public partial class Removes__SubscriptionTimeout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timeout",
                schema: "webhooks",
                table: "Subscriptions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Timeout",
                schema: "webhooks",
                table: "Subscriptions",
                type: "float",
                nullable: false,
                defaultValue: 120.0);
        }
    }
}
