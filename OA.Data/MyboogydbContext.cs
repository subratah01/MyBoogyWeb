using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace OA.Data;

public partial class MyboogydbContext : DbContext
{
    public MyboogydbContext()
    {
    }

    public MyboogydbContext(DbContextOptions<MyboogydbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Loginuser> Loginusers { get; set; }

    public virtual DbSet<Userrole> Userroles { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //    => optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=P@rrott@1234;database=myboogydb", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.32-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Loginuser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("loginuser");

            entity.HasIndex(e => e.UserName, "UserName_UNIQUE").IsUnique();

            entity.HasIndex(e => e.RoleId, "fk_loginuser_userrole _idx");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.DisplayName)
                .IsRequired()
                .HasMaxLength(45);
            entity.Property(e => e.IsActive).HasDefaultValueSql("'1'");
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(40);
            entity.Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(60);

            entity.HasOne(d => d.Role).WithMany(p => p.Loginusers)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("fk_loginuser_userrole");
        });

        modelBuilder.Entity<Userrole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PRIMARY");

            entity.ToTable("userrole");

            entity.Property(e => e.RoleId).ValueGeneratedNever();
            entity.Property(e => e.RoleName)
                .IsRequired()
                .HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
