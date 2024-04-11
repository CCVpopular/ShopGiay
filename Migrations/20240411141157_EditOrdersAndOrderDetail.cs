using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopGiay.Migrations
{
    /// <inheritdoc />
    public partial class EditOrdersAndOrderDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Orders",
                newName: "Total");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Total",
                table: "Orders",
                newName: "TotalPrice");
        }
    }
}
