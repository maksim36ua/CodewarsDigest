using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using cw_itkpi.Models;

namespace cwitkpi.Migrations
{
    [DbContext(typeof(UserContext))]
    partial class UserContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("cw_itkpi.Models.UserInfo", b =>
                {
                    b.Property<string>("username");

                    b.Property<string>("clan");

                    b.Property<int>("honor");

                    b.Property<int>("lastWeekHonor");

                    b.Property<int>("thisWeekHonor");

                    b.Property<string>("vkLink");

                    b.HasKey("username");
                });
        }
    }
}
