using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterViewV2.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentEnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentAzName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Vacancies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VacancyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Position_Id = table.Column<int>(type: "int", nullable: false),
                    Departmet_Id = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    AvailableVacantCount = table.Column<int>(type: "int", nullable: false),
                    EmploymentCv_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vacancies", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionEnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PositionAzName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Position_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FilesId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_File_Files_FilesId",
                        column: x => x.FilesId,
                        principalTable: "Files",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaim_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CvFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilesId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CvFiles", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_CvFiles_Files_FilesId",
                        column: x => x.FilesId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CvFiles_Position_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmploymentCv",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FamilyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FatherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaritalStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhysicalAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescribeIllness = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuringBrief = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnLanguage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AzeLanguage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RuLanguage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherLanguage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkExperienceYES = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnyOtherSkillsYouHave = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YourSkillsThatCanSupportYourApplication = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkingAtSeaYesNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImgName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImgPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CandidatePositions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Education = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatestPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonLeaving = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LanguageSkill = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilesId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BusinessType2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BusinessType3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAddress2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAddress3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyPosition2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyPosition3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndDate2 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate3 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LatestPosition2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatestPosition3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonLeaving2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonLeaving3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate2 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartDate3 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AttendTraining = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HaveValid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerformJob = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipyardOffshore = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WriteEmployer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WriteExperience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WriteAddInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CV_Interview_Process_Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interview_Application_Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmploymentCv", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_EmploymentCv_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmploymentCv_Files_FilesId",
                        column: x => x.FilesId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmploymentCv_Position_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Position",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RegisterDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_User_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Position_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Position",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Condidate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FinCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommonComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CvId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Condidate", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Condidate_EmploymentCv_CvId",
                        column: x => x.CvId,
                        principalTable: "EmploymentCv",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Cv_Eductions",
                columns: table => new
                {
                    Eduction_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EducationalInstitution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EducationDegree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EducationSpecialization = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EducationStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EducationEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmploymentCvId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cv_Eductions", x => x.Eduction_ID)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Cv_Eductions_EmploymentCv_EmploymentCvId",
                        column: x => x.EmploymentCvId,
                        principalTable: "EmploymentCv",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cv_OtherFiles",
                columns: table => new
                {
                    OtherFile_ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmploymentCvId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cv_OtherFiles", x => x.OtherFile_ID)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Cv_OtherFiles_EmploymentCv_EmploymentCvId",
                        column: x => x.EmploymentCvId,
                        principalTable: "EmploymentCv",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cv_ReferanceFrients",
                columns: table => new
                {
                    ReferanceFrient_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FriendReferenceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FriendReferenceSurname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FriendReferencePhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmploymentCvId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cv_ReferanceFrients", x => x.ReferanceFrient_ID)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Cv_ReferanceFrients_EmploymentCv_EmploymentCvId",
                        column: x => x.EmploymentCvId,
                        principalTable: "EmploymentCv",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cv_WorkExperiences",
                columns: table => new
                {
                    WorkExperience_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LatestPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobResponsibilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WorkEndtDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IAmCurrentlyWorking = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReasonForLeavingWork = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmploymentCvId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cv_WorkExperiences", x => x.WorkExperience_ID)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Cv_WorkExperiences_EmploymentCv_EmploymentCvId",
                        column: x => x.EmploymentCvId,
                        principalTable: "EmploymentCv",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaim_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogin_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserToken",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Interview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Interview1Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Interview1Result = table.Column<bool>(type: "bit", nullable: true),
                    Interview1Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interview2Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Interview2Result = table.Column<bool>(type: "bit", nullable: true),
                    Interview2Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interview3Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Interview3Result = table.Column<bool>(type: "bit", nullable: true),
                    Interview3Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    FamilySituation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Children = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChildrenSum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MilitaryStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Education = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qualification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfessionalLiscences = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousEmployer1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedDuties1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousEmployer2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedDuties2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousEmployer3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedDuties3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CDCReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CDCKnow = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplyingPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsideringPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OffshoreExperience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescribeOther = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SafetyExperience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StopSmart = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentLast = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencySituation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GreatestStrength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentJob = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescribeDecision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TeamPlayer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinimumEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WrittenEnglish = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CareerGoals = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CDCHire = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hobbies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnyQuestions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Presentation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Attitude = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Intelligence = table.Column<int>(type: "int", nullable: true),
                    Suitable = table.Column<int>(type: "int", nullable: true),
                    Intelligence1 = table.Column<int>(type: "int", nullable: true),
                    Suitable1 = table.Column<int>(type: "int", nullable: true),
                    Intelligence2 = table.Column<int>(type: "int", nullable: true),
                    Suitable2 = table.Column<int>(type: "int", nullable: true),
                    Intelligence3 = table.Column<int>(type: "int", nullable: true),
                    Suitable3 = table.Column<int>(type: "int", nullable: true),
                    ResultComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interviewer1ChatAdmin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interviewer2ChatAdmin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interviewer3ChatAdmin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewExam = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExamScore = table.Column<double>(type: "float", nullable: true),
                    ExamComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HireComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HireReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastSalary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InterviewWaive = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExamUserId = table.Column<int>(type: "int", nullable: true),
                    User1Id = table.Column<int>(type: "int", nullable: true),
                    User2Id = table.Column<int>(type: "int", nullable: true),
                    User3Id = table.Column<int>(type: "int", nullable: true),
                    HireUserId = table.Column<int>(type: "int", nullable: true),
                    CondidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentIntwId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FilesId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmploymentCv_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmploymentCvId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CV_Interview_Process_Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Interview_Application_Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interview", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Interview_Condidate_CondidateId",
                        column: x => x.CondidateId,
                        principalTable: "Condidate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interview_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Interview_Department_DepartmentIntwId",
                        column: x => x.DepartmentIntwId,
                        principalTable: "Department",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Interview_EmploymentCv_EmploymentCvId",
                        column: x => x.EmploymentCvId,
                        principalTable: "EmploymentCv",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interview_Files_FilesId",
                        column: x => x.FilesId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Interview_Position_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Position",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Interview_User_ExamUserId",
                        column: x => x.ExamUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Interview_User_HireUserId",
                        column: x => x.HireUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Interview_User_User1Id",
                        column: x => x.User1Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interview_User_User2Id",
                        column: x => x.User2Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interview_User_User3Id",
                        column: x => x.User3Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterviewPosition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewPosition", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_InterviewPosition_Interview_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interview",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewPosition_Position_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, "d9a3bd2b-0d19-4219-be5f-58323a3da37e", "Admin", "ADMIN" },
                    { 3, "e1b257d0-080e-4e00-b95a-a941344ba015", "Reviewer", "REVIEWER" },
                    { 4, "4f809ef9-8cf1-43f6-854a-e71d3158e5d8", "HR Coordinator", "HR COORDINATOR" },
                    { 5, "317e9bd6-2294-498f-a448-7bebbfb4b244", "Department Head", "DEPARTMENT HEAD" },
                    { 6, "0a380f5e-ebd0-4b1a-b018-8daad935d9b9", "Employment Manager", "EMPLOYMENT MANAGER" },
                    { 7, "8ca98553-5aa3-4000-bc84-17a7e58e744b", "HR Manager", "HR MANAGER" },
                    { 8, "88d6fd6e-1699-4f98-9443-1eecda4cbeca", "Interviewer 1", "INTERVIEWER 1" },
                    { 9, "6f243ba0-4a44-4b1f-bcef-0c4ff9304c49", "Interviewer 2", "INTERVIEWER 2" },
                    { 10, "be9f992c-c4ae-40f6-8061-4208fcf53003", "Finance_DepartmentHead", "FINANCE_DEPARTMENTHEAD" },
                    { 11, "54cc7648-0e81-48cc-a5c4-7fc87d6efb34", "HR_DepartmentHead", "HR_DEPARTMENTHEAD" },
                    { 12, "d5dc8f44-2ff1-4569-b73b-a107e91440d6", "Legal_DepartmentHead", "LEGAL_DEPARTMENTHEAD" },
                    { 13, "5782b036-22ac-4931-8ed4-bc7bb384a2de", "QHSET_DepartmentHead", "QHSET_DEPARTMENTHEAD" },
                    { 14, "471a02ef-ac8b-4117-ab0f-11474637f611", "Office_DepartmentHead", "OFFICE_DEPARTMENTHEAD" },
                    { 15, "f15721a5-8b0a-48aa-8aab-7654acb44a43", "Maintenance_DepartmentHead", "MAINTENANCE_DEPARTMENTHEAD" },
                    { 16, "5bea2be6-b493-4f69-8807-d954573cabf6", "Commercial_DepartmentHead", "COMMERCIAL_DEPARTMENTHEAD" },
                    { 17, "b86b2a26-eed2-4651-a28c-547084f3440b", "Procurement_DepartmentHead", "PROCUREMENT_DEPARTMENTHEAD" },
                    { 18, "5766e399-0f50-4b3e-a1bf-62c2f045b098", "Projects_DepartmentHead", "PROJECTS_DEPARTMENTHEAD" },
                    { 19, "d9cd7739-28f4-4b13-b0d2-623a10afe556", "RigOperations_DepartmentHead", "RIGOPERATIONS_DEPARTMENTHEAD" },
                    { 20, "30bf8ae1-f1f1-43a9-a3ff-fca78a400964", "MarineBase_DepartmentHead", "MARINEBASE_DEPARTMENTHEAD" },
                    { 21, "b1524be0-0de9-4c98-8ae0-4cb7dddd0d53", "Drilling_DepartmentHead", "DRILLING_DEPARTMENTHEAD" },
                    { 22, "f155a5e3-88a1-42d3-afb4-53b562690467", "Marine_DepartmentHead", "MARINE_DEPARTMENTHEAD" },
                    { 23, "7f31370b-0a29-473d-b70c-76c7ccd8faf2", "Materials_DepartmentHead", "MATERIALS_DEPARTMENTHEAD" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DepartmentId", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "PositionId", "RegisterDate", "SecurityStamp", "Surname", "TwoFactorEnabled", "UserName" },
                values: new object[] { 1, 0, "f5a1ebc8-cd88-4d26-8180-344716f9bcce", null, "admin@caspiandrilling.com", true, false, null, "Admin", "ADMIN@CASPIANDRILLING.COM", "ADMIN", "AQAAAAEAACcQAAAAEF5wCTAXMjI2VFblO0GyB7iAWx6K9KpYJt0X+y4qec975SRQ3I9o3YaNW7ZXjabhhA==", null, false, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "Admin", false, "admin" });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Condidate_CvId",
                table: "Condidate",
                column: "CvId");

            migrationBuilder.CreateIndex(
                name: "IX_Cv_Eductions_EmploymentCvId",
                table: "Cv_Eductions",
                column: "EmploymentCvId");

            migrationBuilder.CreateIndex(
                name: "IX_Cv_OtherFiles_EmploymentCvId",
                table: "Cv_OtherFiles",
                column: "EmploymentCvId");

            migrationBuilder.CreateIndex(
                name: "IX_Cv_ReferanceFrients_EmploymentCvId",
                table: "Cv_ReferanceFrients",
                column: "EmploymentCvId");

            migrationBuilder.CreateIndex(
                name: "IX_Cv_WorkExperiences_EmploymentCvId",
                table: "Cv_WorkExperiences",
                column: "EmploymentCvId");

            migrationBuilder.CreateIndex(
                name: "IX_CvFiles_FilesId",
                table: "CvFiles",
                column: "FilesId");

            migrationBuilder.CreateIndex(
                name: "IX_CvFiles_PositionId",
                table: "CvFiles",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentCv_DepartmentId",
                table: "EmploymentCv",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentCv_FilesId",
                table: "EmploymentCv",
                column: "FilesId");

            migrationBuilder.CreateIndex(
                name: "IX_EmploymentCv_PositionId",
                table: "EmploymentCv",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_File_FilesId",
                table: "File",
                column: "FilesId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_CondidateId",
                table: "Interview",
                column: "CondidateId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_DepartmentId",
                table: "Interview",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_DepartmentIntwId",
                table: "Interview",
                column: "DepartmentIntwId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_EmploymentCvId",
                table: "Interview",
                column: "EmploymentCvId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_ExamUserId",
                table: "Interview",
                column: "ExamUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_FilesId",
                table: "Interview",
                column: "FilesId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_HireUserId",
                table: "Interview",
                column: "HireUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_PositionId",
                table: "Interview",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_User1Id",
                table: "Interview",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_User2Id",
                table: "Interview",
                column: "User2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Interview_User3Id",
                table: "Interview",
                column: "User3Id");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewPosition_InterviewId",
                table: "InterviewPosition",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewPosition_PositionId",
                table: "InterviewPosition",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Position_DepartmentId",
                table: "Position",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Role",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaim_RoleId",
                table: "RoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_User_DepartmentId",
                table: "User",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_User_PositionId",
                table: "User",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "User",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_UserId",
                table: "UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_UserId",
                table: "UserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cv_Eductions");

            migrationBuilder.DropTable(
                name: "Cv_OtherFiles");

            migrationBuilder.DropTable(
                name: "Cv_ReferanceFrients");

            migrationBuilder.DropTable(
                name: "Cv_WorkExperiences");

            migrationBuilder.DropTable(
                name: "CvFiles");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropTable(
                name: "InterviewPosition");

            migrationBuilder.DropTable(
                name: "RoleClaim");

            migrationBuilder.DropTable(
                name: "UserClaim");

            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "UserToken");

            migrationBuilder.DropTable(
                name: "Vacancies");

            migrationBuilder.DropTable(
                name: "Interview");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Condidate");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "EmploymentCv");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Position");

            migrationBuilder.DropTable(
                name: "Department");
        }
    }
}
