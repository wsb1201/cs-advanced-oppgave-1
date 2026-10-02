using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystem.Api.Migrations
{
	/// <inheritdoc />
	public partial class InitialCreate : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Books",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Title = table.Column<string>(type: "text", nullable: false),
					Author = table.Column<string>(type: "text", nullable: false),
					IsBorrowed = table.Column<bool>(type: "boolean", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Books", x => x.Id);
				}
			);

			migrationBuilder.CreateTable(
				name: "Loans",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					BookId = table.Column<Guid>(type: "uuid", nullable: false),
					UserId = table.Column<Guid>(type: "uuid", nullable: false),
					ExpiryDate = table.Column<DateTimeOffset>(
						type: "timestamp with time zone",
						nullable: false
					),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Loans", x => x.Id);
				}
			);

			migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					Id = table.Column<Guid>(type: "uuid", nullable: false),
					Username = table.Column<string>(type: "text", nullable: false),
					Password = table.Column<string>(type: "text", nullable: false),
					IsLibrarian = table.Column<bool>(type: "boolean", nullable: false),
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Users", x => x.Id);
				}
			);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(name: "Books");

			migrationBuilder.DropTable(name: "Loans");

			migrationBuilder.DropTable(name: "Users");
		}
	}
}
