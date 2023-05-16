using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExaminationSystem.Data.Migrations
{
    public partial class boolsToSubmissionAndAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserCourse");

            migrationBuilder.DropIndex(
                name: "IX_TeacherCourses_ApplicationUserId",
                table: "TeacherCourses");

            migrationBuilder.AddColumn<bool>(
                name: "isResultChecked",
                table: "Submissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsExamSubmited",
                table: "Assignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TeacherCourses",
                table: "TeacherCourses",
                columns: new[] { "ApplicationUserId", "CourseId" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "ee32a477-ff14-4c08-af14-a22f29170e38");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "bdd44567-9af3-4ef2-b6a2-d02ef005ac88");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "2598741d-f5cf-4a6b-a273-643b34fe164f");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TeacherCourses",
                table: "TeacherCourses");

            migrationBuilder.DropColumn(
                name: "isResultChecked",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "IsExamSubmited",
                table: "Assignments");

            migrationBuilder.CreateTable(
                name: "ApplicationUserCourse",
                columns: table => new
                {
                    CoursesId = table.Column<int>(type: "int", nullable: false),
                    TeachersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserCourse", x => new { x.CoursesId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserCourse_AspNetUsers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserCourse_Courses_CoursesId",
                        column: x => x.CoursesId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "cc0af068-c74c-4e56-8441-25c18940755f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "e218271d-0d66-453a-a9c4-29c927544d7f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "3a06ef76-2949-48e7-8997-9aa235beb50f");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherCourses_ApplicationUserId",
                table: "TeacherCourses",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserCourse_TeachersId",
                table: "ApplicationUserCourse",
                column: "TeachersId");
        }
    }
}
