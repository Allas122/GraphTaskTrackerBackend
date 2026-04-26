using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraphTaskTrackerBackend.Migrations
{
    /// <inheritdoc />
    public partial class _1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Edges_Nodes_ToNodeId",
                table: "Edges");

            migrationBuilder.AddForeignKey(
                name: "FK_Edges_Nodes_ToNodeId",
                table: "Edges",
                column: "ToNodeId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Edges_Nodes_ToNodeId",
                table: "Edges");

            migrationBuilder.AddForeignKey(
                name: "FK_Edges_Nodes_ToNodeId",
                table: "Edges",
                column: "ToNodeId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
