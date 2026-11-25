using CustomerDesk.Api;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerDesk.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20261123151000_AddUniquePhone")]
    public partial class AddUniquePhone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Customers_Phone",
                table: "Customers",
                column: "Phone",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_Phone",
                table: "Customers");
        }
    }
}
