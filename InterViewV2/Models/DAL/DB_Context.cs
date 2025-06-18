using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InterViewV2.Models.DAL
{
    public class DB_Context : IdentityDbContext<User, Role, int>
    {
        public DB_Context(DbContextOptions<DB_Context> options) : base(options)
        {
        }
        public DbSet<Role> Role { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Position> Position { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Condidate> Condidate { get; set; }
        public DbSet<File> File { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<CvFiles> CvFiles { get; set; }
        public DbSet<Interview> Interview { get; set; }
        public DbSet<EmploymentCv> EmploymentCv { get; set; }
        public DbSet<Cv_Eduction> Cv_Eductions { get; set; }
        public DbSet<Cv_OtherFile> Cv_OtherFiles { get; set; }
        public DbSet<Cv_ReferanceFrient> Cv_ReferanceFrients { get; set; }
        public DbSet<Cv_WorkExperience> Cv_WorkExperiences { get; set; }
        public DbSet<InterviewPosition> InterviewPosition { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }

        public async Task<SelectList> SelectList<T>(string valueField, string textField) where T : class
        {
            var items = await Set<T>().ToListAsync();
            return new SelectList(items, valueField, textField);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>(x => x.ToTable("User"));
            builder.Entity<Role>(x => x.ToTable("Role"));
            builder.Entity<IdentityUserRole<int>>(x => x.ToTable("UserRole"));
            builder.Entity<IdentityUserLogin<int>>(x => x.ToTable("UserLogin"));
            builder.Entity<IdentityRoleClaim<int>>(x => x.ToTable("RoleClaim"));
            builder.Entity<IdentityUserClaim<int>>(x => x.ToTable("UserClaim"));
            builder.Entity<IdentityUserToken<int>>(x => x.ToTable("UserToken"));

            builder.Entity<Interview>()
            .HasOne(i => i.User1)
            .WithMany()
            .HasForeignKey(i => i.User1Id)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Interview>()
                .HasOne(i => i.User2)
                .WithMany()
                .HasForeignKey(i => i.User2Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Interview>()
                .HasOne(i => i.User3)
                .WithMany()
                .HasForeignKey(i => i.User3Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Role>().HasKey(x => x.Id).IsClustered(false);
            builder.Entity<Department>().HasKey(x => x.Id).IsClustered(false);
            builder.Entity<Position>().HasKey(x => x.Id).IsClustered(false);
            builder.Entity<User>().HasKey(x => x.Id).IsClustered(false);
            builder.Entity<Condidate>().HasKey(x => x.Id).IsClustered(false);
            builder.Entity<Models.File>().HasKey(x => x.Id).IsClustered(false);
            builder.Entity<Files>().HasKey(x => x.Id).IsClustered(false);
            builder.Entity<CvFiles>().HasKey(x => x.Id).IsClustered(false);
            builder.Entity<Interview>().HasKey(x => x.Id).IsClustered(false);
            builder.Entity<EmploymentCv>().HasKey(x => x.Id).IsClustered(false);
            builder.Entity<Cv_WorkExperience>().HasKey(x => x.WorkExperience_ID).IsClustered(false);
            builder.Entity<Cv_ReferanceFrient>().HasKey(x => x.ReferanceFrient_ID).IsClustered(false);
            builder.Entity<Cv_OtherFile>().HasKey(x => x.OtherFile_ID).IsClustered(false);
            builder.Entity<Cv_Eduction>().HasKey(x => x.Eduction_ID).IsClustered(false);
            builder.Entity<InterviewPosition>().HasKey(x => x.Id).IsClustered(false);
            builder.Entity<Vacancy>().HasKey(x => x.Id).IsClustered(false);

            InitializeData(builder);
        }
        private void InitializeData(ModelBuilder builder)
        {
            builder.Entity<Role>().HasData(new Role { Id = 1, Name = "Admin", NormalizedName = "Admin".ToUpper() });
           // builder.Entity<Role>().HasData(new Role { Id = 2, Name = "Interviewer", NormalizedName = "Interviewer".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 3, Name = "Reviewer", NormalizedName = "Reviewer".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 4, Name = "HR Coordinator", NormalizedName = "HR Coordinator".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 5, Name = "Department Head", NormalizedName = "Department Head".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 6, Name = "Employment Manager", NormalizedName = "Employment Manager".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 7, Name = "HR Manager", NormalizedName = "HR Manager".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 8, Name = "Interviewer 1", NormalizedName = "Interviewer 1".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 9, Name = "Interviewer 2", NormalizedName = "Interviewer 2".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 10, Name = "Finance_DepartmentHead", NormalizedName = "Finance_DepartmentHead".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 11, Name = "HR_DepartmentHead", NormalizedName = "HR_DepartmentHead".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 12, Name = "Legal_DepartmentHead", NormalizedName = "Legal_DepartmentHead".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 13, Name = "QHSET_DepartmentHead", NormalizedName = "QHSET_DepartmentHead".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 14, Name = "Office_DepartmentHead", NormalizedName = "Office_DepartmentHead".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 15, Name = "Maintenance_DepartmentHead", NormalizedName = "Maintenance_DepartmentHead".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 16, Name = "Commercial_DepartmentHead", NormalizedName = "Commercial_DepartmentHead".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 17, Name = "Procurement_DepartmentHead", NormalizedName = "Procurement_DepartmentHead".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 18, Name = "Projects_DepartmentHead", NormalizedName = "Projects_DepartmentHead".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 19, Name = "RigOperations_DepartmentHead", NormalizedName = "RigOperations_DepartmentHead".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 20, Name = "MarineBase_DepartmentHead", NormalizedName = "MarineBase_DepartmentHead".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 21, Name = "Drilling_DepartmentHead", NormalizedName = "Drilling_DepartmentHead".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 22, Name = "Marine_DepartmentHead", NormalizedName = "Marine_DepartmentHead".ToUpper() });
            builder.Entity<Role>().HasData(new Role { Id = 23, Name = "Materials_DepartmentHead", NormalizedName = "Materials_DepartmentHead".ToUpper() });

            var hasher = new PasswordHasher<User>();
            var adminUser = new User
            {
                Id = 1,
                Name = "Admin",
                Surname = "Admin",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@caspiandrilling.com",
                NormalizedEmail = "admin@caspiandrilling.com".ToUpper(),
                PasswordHash = hasher.HashPassword(null, "Admin123@@"),
                EmailConfirmed = true,
                SecurityStamp = string.Empty
            };
            builder.Entity<User>().HasData(adminUser);

            builder.Entity<IdentityUserRole<int>>().HasData(new IdentityUserRole<int>
            {
                UserId = adminUser.Id,
                RoleId = 1
            });

        }
    }
}
