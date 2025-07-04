using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CentroTreinamento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCpfAndRoleToRecepcionistasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Instrutores_Cpf",
                table: "Instrutores");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Recepcionistas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "SenhaHash",
                table: "Recepcionistas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Recepcionistas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "Recepcionistas",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Recepcionistas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Instrutores",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Recepcionistas_Cpf",
                table: "Recepcionistas",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Instrutores_Cpf",
                table: "Instrutores",
                column: "Cpf",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Recepcionistas_Cpf",
                table: "Recepcionistas");

            migrationBuilder.DropIndex(
                name: "IX_Instrutores_Cpf",
                table: "Instrutores");

            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "Recepcionistas");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Recepcionistas");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Recepcionistas",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "SenhaHash",
                table: "Recepcionistas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Recepcionistas",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Instrutores",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(11)",
                oldMaxLength: 11);

            migrationBuilder.CreateIndex(
                name: "IX_Instrutores_Cpf",
                table: "Instrutores",
                column: "Cpf",
                unique: true,
                filter: "[Cpf] IS NOT NULL");
        }
    }
}
