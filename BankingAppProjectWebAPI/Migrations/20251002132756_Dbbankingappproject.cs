using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BankingAppProjectWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class Dbbankingappproject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "main");

            migrationBuilder.CreateTable(
                name: "AccountInformation",
                schema: "main",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "varchar", maxLength: 254, nullable: false),
                    Password = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInformation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankAccountInformation",
                schema: "main",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Money = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    AccountId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccountInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccountInformation_AccountInformation_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "main",
                        principalTable: "AccountInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                schema: "main",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "Text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Revoked = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReplacedByToken = table.Column<string>(type: "text", nullable: true),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_AccountInformation_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "main",
                        principalTable: "AccountInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TransactionLogs",
                schema: "main",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TypeOfTransaction = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Money = table.Column<decimal>(type: "numeric", nullable: false),
                    DateTimeOfTransaction = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    BankAccountId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionLogs_BankAccountInformation_BankAccountId",
                        column: x => x.BankAccountId,
                        principalSchema: "main",
                        principalTable: "BankAccountInformation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccountInformation_AccountId",
                schema: "main",
                table: "BankAccountInformation",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_AccountId",
                schema: "main",
                table: "Tokens",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLogs_BankAccountId",
                schema: "main",
                table: "TransactionLogs",
                column: "BankAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tokens",
                schema: "main");

            migrationBuilder.DropTable(
                name: "TransactionLogs",
                schema: "main");

            migrationBuilder.DropTable(
                name: "BankAccountInformation",
                schema: "main");

            migrationBuilder.DropTable(
                name: "AccountInformation",
                schema: "main");
        }
    }
}
