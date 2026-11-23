using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EstatePulse.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    agent_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    license_number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    commission_rate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    assigned_branch = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    total_deals_closed = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    bio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    is_verified = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.id);
                    table.ForeignKey(
                        name: "FK_Agents_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    entity_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    entity_id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    old_values_json = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    new_values_json = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ip_address = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CustomerKYCDocuments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    document_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    document_front_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    document_back_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    verification_status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    verified_by_user_id = table.Column<int>(type: "int", nullable: true),
                    verified_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    rejection_reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    uploaded_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerKYCDocuments", x => x.id);
                    table.ForeignKey(
                        name: "FK_CustomerKYCDocuments_Users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerKYCDocuments_Users_verified_by_user_id",
                        column: x => x.verified_by_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    location_address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    city = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    latitude = table.Column<decimal>(type: "decimal(10,8)", nullable: true),
                    longitude = table.Column<decimal>(type: "decimal(11,8)", nullable: true),
                    thumbnail_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_published = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UnitTypes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    type_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    bedrooms = table.Column<int>(type: "int", nullable: false),
                    bathrooms = table.Column<int>(type: "int", nullable: false),
                    parking_slots = table.Column<int>(type: "int", nullable: false),
                    floors = table.Column<int>(type: "int", nullable: false),
                    building_size = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    land_size_recommended = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    floor_plan_image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    specification_json = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitTypes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserFollows",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    following_user_id = table.Column<int>(type: "int", nullable: false),
                    followed_user_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFollows", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserFollows_Users_followed_user_id",
                        column: x => x.followed_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserFollows_Users_following_user_id",
                        column: x => x.following_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgentLeads",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    agent_id = table.Column<int>(type: "int", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: true),
                    lead_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    lead_phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    lead_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    budget_min = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    budget_max = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentLeads", x => x.id);
                    table.ForeignKey(
                        name: "FK_AgentLeads_Agents_agent_id",
                        column: x => x.agent_id,
                        principalTable: "Agents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgentLeads_Users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProjectPhases",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    project_id = table.Column<int>(type: "int", nullable: false),
                    phase_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    slug = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    master_plan_svg_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    launch_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    expected_completion_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectPhases", x => x.id);
                    table.ForeignKey(
                        name: "FK_ProjectPhases_Projects_project_id",
                        column: x => x.project_id,
                        principalTable: "Projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    phase_id = table.Column<int>(type: "int", nullable: false),
                    unit_type_id = table.Column<int>(type: "int", nullable: false),
                    unit_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    street_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    land_area_sqm = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    built_area_sqm = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    base_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    hold_expires_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    current_hold_user_id = table.Column<int>(type: "int", nullable: true),
                    svg_element_id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    row_version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.id);
                    table.ForeignKey(
                        name: "FK_Units_ProjectPhases_phase_id",
                        column: x => x.phase_id,
                        principalTable: "ProjectPhases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Units_UnitTypes_unit_type_id",
                        column: x => x.unit_type_id,
                        principalTable: "UnitTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Units_Users_current_hold_user_id",
                        column: x => x.current_hold_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    unit_id = table.Column<int>(type: "int", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    agent_id = table.Column<int>(type: "int", nullable: true),
                    agreed_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discount_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    deposit_required = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    reservation_expires_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.id);
                    table.ForeignKey(
                        name: "FK_Bookings_Agents_agent_id",
                        column: x => x.agent_id,
                        principalTable: "Agents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Bookings_Units_unit_id",
                        column: x => x.unit_id,
                        principalTable: "Units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitMedia",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    unit_id = table.Column<int>(type: "int", nullable: true),
                    unit_type_id = table.Column<int>(type: "int", nullable: true),
                    media_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    media_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    is_featured = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitMedia", x => x.id);
                    table.ForeignKey(
                        name: "FK_UnitMedia_UnitTypes_unit_type_id",
                        column: x => x.unit_type_id,
                        principalTable: "UnitTypes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UnitMedia_Units_unit_id",
                        column: x => x.unit_id,
                        principalTable: "Units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Commissions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    agent_id = table.Column<int>(type: "int", nullable: false),
                    commission_percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    total_commission_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    payment_status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    approved_by_user_id = table.Column<int>(type: "int", nullable: true),
                    disbursed_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commissions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Commissions_Agents_agent_id",
                        column: x => x.agent_id,
                        principalTable: "Agents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commissions_Bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "Bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Commissions_Users_approved_by_user_id",
                        column: x => x.approved_by_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    contract_number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    generated_pdf_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    signed_pdf_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    signing_deadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.id);
                    table.ForeignKey(
                        name: "FK_Contracts_Bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "Bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    payment_reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    customer_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    payment_channel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    transaction_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    proof_slip_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    verified_by_user_id = table.Column<int>(type: "int", nullable: true),
                    paid_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.id);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "Bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Users_customer_id",
                        column: x => x.customer_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Users_verified_by_user_id",
                        column: x => x.verified_by_user_id,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "InstallmentSchedules",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    stage_number = table.Column<int>(type: "int", nullable: false),
                    milestone_title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    due_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    due_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_paid = table.Column<bool>(type: "bit", nullable: false),
                    payment_id = table.Column<int>(type: "int", nullable: true),
                    paid_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallmentSchedules", x => x.id);
                    table.ForeignKey(
                        name: "FK_InstallmentSchedules_Bookings_booking_id",
                        column: x => x.booking_id,
                        principalTable: "Bookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstallmentSchedules_Payments_payment_id",
                        column: x => x.payment_id,
                        principalTable: "Payments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_created_at",
                table: "Users",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_Users_full_name",
                table: "Users",
                column: "full_name");

            migrationBuilder.CreateIndex(
                name: "IX_AgentLeads_agent_id",
                table: "AgentLeads",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                name: "IX_AgentLeads_customer_id",
                table: "AgentLeads",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Agents_agent_code",
                table: "Agents",
                column: "agent_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agents_user_id",
                table: "Agents",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_user_id",
                table: "AuditLogs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_agent_id",
                table: "Bookings",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_booking_reference",
                table: "Bookings",
                column: "booking_reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_customer_id",
                table: "Bookings",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_unit_id",
                table: "Bookings",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_agent_id",
                table: "Commissions",
                column: "agent_id");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_approved_by_user_id",
                table: "Commissions",
                column: "approved_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Commissions_booking_id",
                table: "Commissions",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_booking_id",
                table: "Contracts",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_contract_number",
                table: "Contracts",
                column: "contract_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerKYCDocuments_customer_id",
                table: "CustomerKYCDocuments",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerKYCDocuments_verified_by_user_id",
                table: "CustomerKYCDocuments",
                column: "verified_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentSchedules_booking_id",
                table: "InstallmentSchedules",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_InstallmentSchedules_payment_id",
                table: "InstallmentSchedules",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_booking_id",
                table: "Payments",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_customer_id",
                table: "Payments",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_payment_reference",
                table: "Payments",
                column: "payment_reference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_verified_by_user_id",
                table: "Payments",
                column: "verified_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectPhases_project_id_slug",
                table: "ProjectPhases",
                columns: new[] { "project_id", "slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_slug",
                table: "Projects",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitMedia_unit_id",
                table: "UnitMedia",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "IX_UnitMedia_unit_type_id",
                table: "UnitMedia",
                column: "unit_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_Units_current_hold_user_id",
                table: "Units",
                column: "current_hold_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Units_phase_id_unit_code",
                table: "Units",
                columns: new[] { "phase_id", "unit_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_unit_type_id",
                table: "Units",
                column: "unit_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_UnitTypes_code",
                table: "UnitTypes",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_followed_user_id",
                table: "UserFollows",
                column: "followed_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserFollows_following_user_id_followed_user_id",
                table: "UserFollows",
                columns: new[] { "following_user_id", "followed_user_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentLeads");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Commissions");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "CustomerKYCDocuments");

            migrationBuilder.DropTable(
                name: "InstallmentSchedules");

            migrationBuilder.DropTable(
                name: "UnitMedia");

            migrationBuilder.DropTable(
                name: "UserFollows");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "ProjectPhases");

            migrationBuilder.DropTable(
                name: "UnitTypes");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Users_created_at",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_full_name",
                table: "Users");
        }
    }
}
