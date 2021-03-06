﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MySQL.Data.EntityFrameworkCore.Extensions;
using Pomelo.EntityFrameworkCore.MySql.PerfTests.Models;

namespace Pomelo.EntityFrameworkCore.MySql.PerfTests
{
	public class AppDb : IdentityDbContext<AppIdentityUser>
	{
		// blog
		public DbSet<Blog> Blogs { get; set; }

		// crm
		public DbSet<CrmAdmin> CrmAdmins { get; set; }
		public DbSet<CrmRole> CrmRoles { get; set; }
		public DbSet<CrmMenu> CrmMenus { get; set; }

		// data types
		public DbSet<DataTypesSimple> DataTypesSimple { get; set; }
		public DbSet<DataTypesVariable> DataTypesVariable { get; set; }

		// generated data types
		public DbSet<GeneratedContact> GeneratedContacts { get; set; }

		private readonly bool _configured;

		public AppDb()
		{
			_configured = false;
		}

		public AppDb(DbContextOptions options) : base(options)
		{
			_configured = true;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!_configured)
			{
				if (AppConfig.EfProvider == "oracle")
				{
					// Oracle defines this with a case sensitive "MySQL" in MySQL.Data.EntityFrameworkCore.Extensions
					optionsBuilder.UseMySQL(AppConfig.Config["Data:ConnectionString"] + "ssl mode=none;");
				}
				else
				{
					// Pomelo defines this with a case sensitive "MySql" in Microsoft.EntityFrameworkCore
					optionsBuilder.UseMySql(AppConfig.Config["Data:ConnectionString"]);
				}
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<AppIdentityUser>(entity => entity.Property(m => m.Id).HasMaxLength(127));
			modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.Id).HasMaxLength(127));
			modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(127));
			modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(127));
			modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(127));
			modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(127));
			modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(127));
			modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(127));
			modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name).HasMaxLength(127));
			CrmMeta.OnModelCreating(modelBuilder);
			GeneratedContactMeta.OnModelCreating(modelBuilder);
		}

	}
}
