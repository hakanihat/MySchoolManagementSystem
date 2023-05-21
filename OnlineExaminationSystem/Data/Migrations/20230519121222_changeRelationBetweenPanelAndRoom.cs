using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExaminationSystem.Data.Migrations
{
    public partial class changeRelationBetweenPanelAndRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_ChatPanels_ChatPanelId",
                table: "ChatRooms");

            migrationBuilder.DropIndex(
                name: "IX_ChatRooms_ChatPanelId",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "ChatPanelId",
                table: "ChatRooms");

            migrationBuilder.CreateTable(
                name: "ChatPanelChatRoom",
                columns: table => new
                {
                    ChatPanelsId = table.Column<int>(type: "int", nullable: false),
                    ChatRoomsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatPanelChatRoom", x => new { x.ChatPanelsId, x.ChatRoomsId });
                    table.ForeignKey(
                        name: "FK_ChatPanelChatRoom_ChatPanels_ChatPanelsId",
                        column: x => x.ChatPanelsId,
                        principalTable: "ChatPanels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatPanelChatRoom_ChatRooms_ChatRoomsId",
                        column: x => x.ChatRoomsId,
                        principalTable: "ChatRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "9c007c4a-5bea-49bc-a7f2-ccfdc0545ed6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "30c0190c-cfcc-46b1-aecd-5205288c785e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "4ba08ccc-fea8-4034-a843-2e8bf3744792");

            migrationBuilder.CreateIndex(
                name: "IX_ChatPanelChatRoom_ChatRoomsId",
                table: "ChatPanelChatRoom",
                column: "ChatRoomsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatPanelChatRoom");

            migrationBuilder.AddColumn<int>(
                name: "ChatPanelId",
                table: "ChatRooms",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "e98fd0c9-9324-45d2-9bc2-c98ab85a281e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "3f09f879-10c0-431d-a6a2-323726614b43");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "47d10831-dc16-4831-9c14-76ca6d4039d1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_ChatPanelId",
                table: "ChatRooms",
                column: "ChatPanelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_ChatPanels_ChatPanelId",
                table: "ChatRooms",
                column: "ChatPanelId",
                principalTable: "ChatPanels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
