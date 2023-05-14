using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineExaminationSystem.Data.Migrations
{
    public partial class changeDateToTimeInExam2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamTime",
                table: "Exams");

            migrationBuilder.AddColumn<int>(
                name: "ExamDuration",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "3439b822-6c08-45e1-9371-2475a0dbc754");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "8a4eccfc-5daa-4574-b33c-1c9371f0dd6b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "8a71bf51-5e7e-4200-bd31-8424098f594b");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamDuration",
                table: "Exams");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ExamTime",
                table: "Exams",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "b7921fab-e68b-424c-a5cd-8ac2168a24e6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2",
                column: "ConcurrencyStamp",
                value: "82175e79-4122-4aed-b5de-0650e98913fc");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3",
                column: "ConcurrencyStamp",
                value: "b8d8c2c0-c2de-44a5-b878-e6cb0c9735bb");
        }
    }
}
