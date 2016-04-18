using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace cwitkpi.Migrations
{
    public partial class ChangedWeeklyRatingScheme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "pointsHistory", table: "UserInfo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pointsHistory",
                table: "UserInfo",
                nullable: true);
        }
    }
}
