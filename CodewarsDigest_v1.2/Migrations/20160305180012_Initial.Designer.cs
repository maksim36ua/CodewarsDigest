using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using cw_itkpi.Models;

namespace cwitkpi.Migrations
{
    [DbContext(typeof(UserContext))]
    [Migration("20160305180012_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
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

                    b.Property<string>("pointsHistory");

                    b.Property<int>("thisWeekHonor");

                    b.Property<string>("vkLink");

                    b.HasKey("username");
                });
        }
    }
}
