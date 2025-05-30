using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteBook.API.Migrations
{
    public partial class wuhu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountInfo",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Account = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInfo", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "NewMemoInfo",
                columns: table => new
                {
                    MemoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountInfoId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewMemoInfo", x => x.MemoID);
                    table.ForeignKey(
                        name: "FK_NewMemoInfo_AccountInfo_AccountInfoId",
                        column: x => x.AccountInfoId,
                        principalTable: "AccountInfo",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaitInfo",
                columns: table => new
                {
                    Waitid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountInfoId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitInfo", x => x.Waitid);
                    table.ForeignKey(
                        name: "FK_WaitInfo_AccountInfo_AccountInfoId",
                        column: x => x.AccountInfoId,
                        principalTable: "AccountInfo",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewMemoInfo_AccountInfoId",
                table: "NewMemoInfo",
                column: "AccountInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitInfo_AccountInfoId",
                table: "WaitInfo",
                column: "AccountInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewMemoInfo");

            migrationBuilder.DropTable(
                name: "WaitInfo");

            migrationBuilder.DropTable(
                name: "AccountInfo");
        }
    }
}
