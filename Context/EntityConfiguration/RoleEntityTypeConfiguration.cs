﻿using IdealDiscuss.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdealDiscuss.Context.EntityConfiguration
{
    public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(r => r.RoleName);
            builder.Property(c => c.RoleName)
                .IsRequired()
                .HasMaxLength(15);
            builder.HasIndex(r => r.Users)
             .IsUnique();
        }
    }
}
