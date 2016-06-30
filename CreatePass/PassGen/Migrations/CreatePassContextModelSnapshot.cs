using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using CreatePass;

namespace CreatePass.Migrations
{
    [DbContext(typeof(CreatePassContext))]
    partial class CreatePassContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("CreatePass.Model.SiteKeyItem", b =>
                {
                    b.Property<int>("SiteKeyItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateAdded");

                    b.Property<string>("Url_Encrypted");

                    b.Property<string>("Url_Hashed");

                    b.Property<string>("Url_PlainText");

                    b.HasKey("SiteKeyItemId");

                    b.ToTable("SiteKeys");
                });
        }
    }
}
