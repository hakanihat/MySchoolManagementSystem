using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExaminationSystem.Data.Migrations
{
    public partial class replaceExamResultWithSubmission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_ExamResults_ExamResultId",
                table: "StudentAnswers");

            migrationBuilder.AlterColumn<int>(
                name: "ExamResultId",
                table: "StudentAnswers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SubmissionId",
                table: "StudentAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "bf8049aa-4ee4-4219-acab-914fa083baa3");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "14c9ba69-41a7-494b-b0e9-ad2e969d600e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "e6ef63ca-d4cf-486f-a145-a9efbf09f612");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_SubmissionId",
                table: "StudentAnswers",
                column: "SubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_ExamResults_ExamResultId",
                table: "StudentAnswers",
                column: "ExamResultId",
                principalTable: "ExamResults",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_Submissions_SubmissionId",
                table: "StudentAnswers",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_ExamResults_ExamResultId",
                table: "StudentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAnswers_Submissions_SubmissionId",
                table: "StudentAnswers");

            migrationBuilder.DropIndex(
                name: "IX_StudentAnswers_SubmissionId",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "SubmissionId",
                table: "StudentAnswers");

            migrationBuilder.AlterColumn<int>(
                name: "ExamResultId",
                table: "StudentAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "8927eecc-3055-4d4d-a77b-7d25fd975eb0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "715fb31f-19da-46d4-a1fb-d6e721d439fb");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "0395c8f0-7adf-4b20-b3d3-6cb4f547aa9d");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAnswers_ExamResults_ExamResultId",
                table: "StudentAnswers",
                column: "ExamResultId",
                principalTable: "ExamResults",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
