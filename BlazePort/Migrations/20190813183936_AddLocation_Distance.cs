using Microsoft.EntityFrameworkCore.Migrations;

namespace BlazePort.Migrations
{
    public partial class AddLocation_Distance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Distance",
                table: "Locations",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 7,
                column: "Distance",
                value: 0.239f);

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 8,
                column: "Distance",
                value: 140f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Locations");
        }
    }
}
