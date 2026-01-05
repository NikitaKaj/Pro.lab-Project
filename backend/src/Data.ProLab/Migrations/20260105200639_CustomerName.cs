using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class CustomerName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e5334cb2-307d-44f1-a206-f5d4fb36e6e4", "AQAAAAIAAYagAAAAEK7qrKe6EmJH53oX4FnPSQC/7F8ONznxSjPOiUt3qINGzlvyqCidBJjZSLn7juHSlQ==", "d1109c73-43ef-48e6-8908-f1f5b4b92948" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "945bfd2a-ba75-4db0-a377-d12a5d363b65", "AQAAAAIAAYagAAAAED+e3pQmDNoPQzBbGEbE0YMlQhQQC04ZZwu9+t7BXNjTy4mJKtxPSAPmlaqWN7w/lg==", "04093d03-cafb-401b-929b-28cbe8fac675" });
        }
    }
}
