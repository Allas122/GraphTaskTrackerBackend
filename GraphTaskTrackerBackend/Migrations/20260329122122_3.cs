using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraphTaskTrackerBackend.Migrations
{
    /// <inheritdoc />
    public partial class _3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Edges_Nodes_FromNodeId",
                table: "Edges");

            migrationBuilder.DropForeignKey(
                name: "FK_Edges_Nodes_ToNodeId",
                table: "Edges");

            migrationBuilder.DropForeignKey(
                name: "FK_Nodes_Nodes_NodeId",
                table: "Nodes");

            migrationBuilder.DropIndex(
                name: "IX_Nodes_NodeId",
                table: "Nodes");

            migrationBuilder.DropColumn(
                name: "NodeId",
                table: "Nodes");

            migrationBuilder.AddForeignKey(
                name: "FK_Edges_Nodes_FromNodeId",
                table: "Edges",
                column: "FromNodeId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Edges_Nodes_FromNodeId",
                table: "Edges");

            migrationBuilder.DropForeignKey(
                name: "FK_Edges_Nodes_ToNodeId",
                table: "Edges");

            migrationBuilder.AddColumn<Guid>(
                name: "NodeId",
                table: "Nodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Nodes_NodeId",
                table: "Nodes",
                column: "NodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Edges_Nodes_FromNodeId",
                table: "Edges",
                column: "FromNodeId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Edges_Nodes_ToNodeId",
                table: "Edges",
                column: "ToNodeId",
                principalTable: "Nodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Nodes_Nodes_NodeId",
                table: "Nodes",
                column: "NodeId",
                principalTable: "Nodes",
                principalColumn: "Id");
        }
    }
}
