using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_System.Migrations
{
    /// <inheritdoc />
    public partial class Trainerpencentage_Clintdiscount_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClintDiscounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClintId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClintDiscounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClintDiscounts_AspNetUsers_ClintId",
                        column: x => x.ClintId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainerPercentages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrainerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerPercentages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainerPercentages_AspNetUsers_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClintDiscounts_ClintId",
                table: "ClintDiscounts",
                column: "ClintId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerPercentages_TrainerId",
                table: "TrainerPercentages",
                column: "TrainerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClintDiscounts");

            migrationBuilder.DropTable(
                name: "TrainerPercentages");
        }
    }
}
