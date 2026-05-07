using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIsProfileComplete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProfileComplete",
                table: "Students",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsProfileComplete",
                table: "Instructors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 5, 6, 14, 18, 21, 80, DateTimeKind.Utc).AddTicks(1915), "$2a$11$LB00E/LaLxCJ/218wrnmEuQfRLCJjDJ9ag9RZ0xMh.jHKRnn9eAnu" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProfileComplete",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsProfileComplete",
                table: "Instructors");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 4, 1, 22, 40, 36, 134, DateTimeKind.Utc).AddTicks(1042), "$2a$11$mQiD1iKlXjo2dc4P.1.Acuq50mlRXaF9X3VYo4XgFoX/jh7Pq6XT." });
        }
    }
}
