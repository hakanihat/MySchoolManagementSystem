using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExaminationSystem.Data.Migrations
{
    public partial class addIntermediateTablePanelRoom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatPanelChatRoom");

            migrationBuilder.CreateTable(
                name: "ChatPanelRooms",
                columns: table => new
                {
                    ChatPanelId = table.Column<int>(type: "int", nullable: false),
                    ChatRoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatPanelRooms", x => new { x.ChatPanelId, x.ChatRoomId });
                    table.ForeignKey(
                        name: "FK_ChatPanelRooms_ChatPanels_ChatPanelId",
                        column: x => x.ChatPanelId,
                        principalTable: "ChatPanels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatPanelRooms_ChatRooms_ChatRoomId",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "44ed0136-0462-4d48-b25c-3134fa6aacb2");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "b8560391-4d05-4106-b3af-4d9381f1f950");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "786e366a-bfd7-433d-8fb8-0ca727e606e1");

            migrationBuilder.CreateIndex(
                name: "IX_ChatPanelRooms_ChatRoomId",
                table: "ChatPanelRooms",
                column: "ChatRoomId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatPanelRooms");

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
    }
}
