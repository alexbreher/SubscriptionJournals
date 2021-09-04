using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SubscriptionJournals.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    lastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    email = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    token = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    creationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    followers = table.Column<int>(type: "int", nullable: false),
                    following = table.Column<int>(type: "int", nullable: false),
                    journalsPublished = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_Id);
                });

            migrationBuilder.CreateTable(
                name: "Journals",
                columns: table => new
                {
                    journal_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    author = table.Column<int>(type: "int", nullable: false),
                    creationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    path = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Journals", x => x.journal_Id);
                    table.ForeignKey(
                        name: "FK_Journals_Users_author",
                        column: x => x.author,
                        principalTable: "Users",
                        principalColumn: "user_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    subscription_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    subscriptor = table.Column<int>(type: "int", nullable: false),
                    subscribesTo = table.Column<int>(type: "int", nullable: false),
                    subscriptionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.subscription_Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_subscribesTo",
                        column: x => x.subscribesTo,
                        principalTable: "Users",
                        principalColumn: "user_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Journals_author",
                table: "Journals",
                column: "author");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_subscribesTo",
                table: "Subscriptions",
                column: "subscribesTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Journals");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
