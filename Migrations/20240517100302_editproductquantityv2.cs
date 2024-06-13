using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopGiay.Migrations
{
    /// <inheritdoc />
    public partial class editproductquantityv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductQuantity_ProductSizes_ProductSizeId",
                table: "ProductQuantity");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductQuantity_Products_ProductId",
                table: "ProductQuantity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductQuantity",
                table: "ProductQuantity");

            migrationBuilder.RenameTable(
                name: "ProductQuantity",
                newName: "ProductQuantities");

            migrationBuilder.RenameIndex(
                name: "IX_ProductQuantity_ProductSizeId",
                table: "ProductQuantities",
                newName: "IX_ProductQuantities_ProductSizeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductQuantity_ProductId",
                table: "ProductQuantities",
                newName: "IX_ProductQuantities_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductQuantities",
                table: "ProductQuantities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductQuantities_ProductSizes_ProductSizeId",
                table: "ProductQuantities",
                column: "ProductSizeId",
                principalTable: "ProductSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductQuantities_Products_ProductId",
                table: "ProductQuantities",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductQuantities_ProductSizes_ProductSizeId",
                table: "ProductQuantities");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductQuantities_Products_ProductId",
                table: "ProductQuantities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductQuantities",
                table: "ProductQuantities");

            migrationBuilder.RenameTable(
                name: "ProductQuantities",
                newName: "ProductQuantity");

            migrationBuilder.RenameIndex(
                name: "IX_ProductQuantities_ProductSizeId",
                table: "ProductQuantity",
                newName: "IX_ProductQuantity_ProductSizeId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductQuantities_ProductId",
                table: "ProductQuantity",
                newName: "IX_ProductQuantity_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductQuantity",
                table: "ProductQuantity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductQuantity_ProductSizes_ProductSizeId",
                table: "ProductQuantity",
                column: "ProductSizeId",
                principalTable: "ProductSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductQuantity_Products_ProductId",
                table: "ProductQuantity",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
