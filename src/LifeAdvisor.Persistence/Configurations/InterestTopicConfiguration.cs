using LifeAdvisor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LifeAdvisor.Persistence.Configurations;

public class InterestTopicConfiguration : IEntityTypeConfiguration<InterestTopic>
{
    public void Configure(EntityTypeBuilder<InterestTopic> builder)
    {
        builder.HasKey(t => t.InterestTopicId);

        builder.Property(t => t.Title).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Description).IsRequired().HasMaxLength(400);
        builder.Property(t => t.Category).IsRequired().HasMaxLength(60);
        builder.Property(t => t.Emoji).HasMaxLength(16);
        builder.Property(t => t.IsActive).HasDefaultValue(true);
        builder.Property(t => t.DisplayOrder).IsRequired();

        // Five hand-picked starter topics (not internet slop). These map cleanly
        // onto real-world updates we can surface later.
        builder.HasData(
            new InterestTopic
            {
                InterestTopicId = 1,
                Title = "Technology & AI",
                Category = "Tech",
                Emoji = "🤖",
                DisplayOrder = 1,
                IsActive = true,
                Description = "Breakthroughs, new tools, and how AI is reshaping the way we live and work."
            },
            new InterestTopic
            {
                InterestTopicId = 2,
                Title = "Money & Investing",
                Category = "Finance",
                Emoji = "💸",
                DisplayOrder = 2,
                IsActive = true,
                Description = "Markets, saving strategies, and smart money moves that compound over time."
            },
            new InterestTopic
            {
                InterestTopicId = 3,
                Title = "Health & Longevity",
                Category = "Wellbeing",
                Emoji = "🌿",
                DisplayOrder = 3,
                IsActive = true,
                Description = "Fitness, nutrition, sleep, and the science of living a longer, better life."
            },
            new InterestTopic
            {
                InterestTopicId = 4,
                Title = "Science & Space",
                Category = "Science",
                Emoji = "🔭",
                DisplayOrder = 4,
                IsActive = true,
                Description = "Discoveries, the cosmos, and the ideas quietly rewriting what we know."
            },
            new InterestTopic
            {
                InterestTopicId = 5,
                Title = "Career & Hustle",
                Category = "Growth",
                Emoji = "🚀",
                DisplayOrder = 5,
                IsActive = true,
                Description = "Work, startups, side projects, and growing into the next version of you."
            });
    }
}
