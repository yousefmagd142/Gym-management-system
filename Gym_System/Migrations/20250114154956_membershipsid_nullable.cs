using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_System.Migrations
{
    /// <inheritdoc />
    public partial class membershipsid_nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Membrtships_MembrtshipsId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "MembrtshipsId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Membrtships_MembrtshipsId",
                table: "AspNetUsers",
                column: "MembrtshipsId",
                principalTable: "Membrtships",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Membrtships_MembrtshipsId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "MembrtshipsId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Membrtships_MembrtshipsId",
                table: "AspNetUsers",
                column: "MembrtshipsId",
                principalTable: "Membrtships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
