using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CreatePass.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteKeys",
                columns: table => new
                {
                    SiteKeyItemId = table.Column<int>(nullable: false)
                        .Annotation("Autoincrement", true),
                    DateAdded = table.Column<DateTime>(nullable: false),
                    Url_Encrypted = table.Column<string>(nullable: true),
                    Url_Hashed = table.Column<string>(nullable: true),
                    Url_PlainText = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteKeys", x => x.SiteKeyItemId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteKeys");
        }
    }
}
