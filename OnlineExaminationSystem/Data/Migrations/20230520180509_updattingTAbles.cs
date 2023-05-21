using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExaminationSystem.Data.Migrations
{
    public partial class updattingTAbles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRoomUsers_ChatRooms_ChatRoomId",
                table: "ChatRoomUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatRoomUsers",
                table: "ChatRoomUsers");

            migrationBuilder.DropIndex(
                name: "IX_ChatRoomUsers_ChatRoomId",
                table: "ChatRoomUsers");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ChatRoomUsers");

            migrationBuilder.RenameColumn(
                name: "ChatRoomId",
                table: "ChatRoomUsers",
                newName: "RoomId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatRoomUsers",
                table: "ChatRoomUsers",
                columns: new[] { "RoomId", "UserId" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "1f2464c2-eb7b-43d6-b9ce-c3d960ed143f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "713079b8-70e8-4e8b-8dc8-613ed8e0c319");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "1ac50c65-d81e-4abf-8982-aaad70494350");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRoomUsers_ChatRooms_RoomId",
                table: "ChatRoomUsers",
                column: "RoomId",
                principalTable: "ChatRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRoomUsers_ChatRooms_RoomId",
                table: "ChatRoomUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatRoomUsers",
                table: "ChatRoomUsers");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "ChatRoomUsers",
                newName: "ChatRoomId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ChatRoomUsers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatRoomUsers",
                table: "ChatRoomUsers",
                column: "Id");

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
                name: "IX_ChatRoomUsers_ChatRoomId",
                table: "ChatRoomUsers",
                column: "ChatRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRoomUsers_ChatRooms_ChatRoomId",
                table: "ChatRoomUsers",
                column: "ChatRoomId",
                principalTable: "ChatRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
