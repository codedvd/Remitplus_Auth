using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Remitplus_Authentication.Models;

public partial class E2epaymetsContext : DbContext
{
    public E2epaymetsContext()
    {
    }

    public E2epaymetsContext(DbContextOptions<E2epaymetsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FailedRequest> FailedRequests { get; set; }

    public virtual DbSet<IpWhitelist> IpWhitelists { get; set; }

    public virtual DbSet<IpblackList> IpblackLists { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserApiKey> UserApiKeys { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<WhitelistedIpLog> WhitelistedIpLogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=dpg-d3j6st1r0fns73aldth0-a.oregon-postgres.render.com;Database=e2epaymets;Username=e2epaymets_user;Password=ymcIjrrF964Mzjfi24GYHLwgOCsbk7Wm;Port=5432;SSL Mode=Require;Trust Server Certificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FailedRequest>(entity =>
        {
            entity.HasKey(e => e.FailId);

            entity.HasIndex(e => e.TransactionId, "IX_FailedRequests_TransactionId");

            entity.Property(e => e.FailId).ValueGeneratedNever();

            entity.HasOne(d => d.Transaction).WithMany(p => p.FailedRequests).HasForeignKey(d => d.TransactionId);
        });

        modelBuilder.Entity<IpWhitelist>(entity =>
        {
            entity.HasIndex(e => e.ApplicationUserId, "IX_IpWhitelists_ApplicationUserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.ApplicationUser).WithMany(p => p.IpWhitelists).HasForeignKey(d => d.ApplicationUserId);
        });

        modelBuilder.Entity<IpblackList>(entity =>
        {
            entity.ToTable("IPBlackLists");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Ipaddress).HasColumnName("IPAddress");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.Property(e => e.TransactionId).ValueGeneratedNever();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<UserApiKey>(entity =>
        {
            entity.HasIndex(e => e.ApplicationUserId, "IX_UserApiKeys_ApplicationUserId");

            entity.HasOne(d => d.ApplicationUser).WithMany(p => p.UserApiKeys).HasForeignKey(d => d.ApplicationUserId);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("ApplicationUserRoles_pkey");

            entity.Property(e => e.RoleId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<WhitelistedIpLog>(entity =>
        {
            entity.ToTable("WhitelistedIpLog");

            entity.HasIndex(e => e.ApplicationUserId, "IX_WhitelistedIpLog_ApplicationUserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.ApplicationUser).WithMany(p => p.WhitelistedIpLogs).HasForeignKey(d => d.ApplicationUserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
