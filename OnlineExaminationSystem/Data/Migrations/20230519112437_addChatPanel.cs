using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExaminationSystem.Data.Migrations
{
    public partial class addChatPanel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatPanelId",
                table: "ChatRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ChatPanels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatPanels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatPanels_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "c1925b2d-668c-42c4-a9c7-78803429a9c0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "c0726aba-0833-40f8-aa9a-ae61441ad0e3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "7f9d19f0-3a8b-4f4c-acf2-287859420fbe");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_ChatPanelId",
                table: "ChatRooms",
                column: "ChatPanelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatPanels_ApplicationUserId",
                table: "ChatPanels",
                column: "ApplicationUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_ChatPanels_ChatPanelId",
                table: "ChatRooms",
                column: "ChatPanelId",
                principalTable: "ChatPanels",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_ChatPanels_ChatPanelId",
                table: "ChatRooms");

            migrationBuilder.DropTable(
                name: "ChatPanels");

            migrationBuilder.DropIndex(
                name: "IX_ChatRooms_ChatPanelId",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "ChatPanelId",
                table: "ChatRooms");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "5c5339ed-6f92-471c-901c-03e69920da5f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "bc7fb507-7a48-434b-a5dc-d2fac8274347");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "a60949a5-dab9-43d2-a652-0a6493660e13");
        }
    }
}
