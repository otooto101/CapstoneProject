using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeAdvisor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixEmbeddingColumnToNvarcharMax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The TwinNarratives.Embedding column was originally created as a native SQL Server
            // "vector" type, but the model now stores embeddings as a JSON string in nvarchar(max)
            // (cosine similarity is computed in C#). Because the EF model snapshot already reflects
            // nvarchar(max), scaffolding produced no changes even though the physical column is still
            // a vector. The native vector type caps at 1998 dimensions, so inserting a 3072-dimension
            // embedding fails with "Given Vector size is not supported. Maximum allowed size '1998'".
            //
            // This guarded statement converts the physical column to nvarchar(max) only when it is not
            // already nvarchar, so it is idempotent and a no-op on databases created fresh from the
            // current model. The embedding is a regenerable cache (RelatedDecisionRetriever re-embeds
            // rows whose Embedding is null), so dropping the column data is safe.
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM sys.columns c
    INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
    WHERE c.object_id = OBJECT_ID(N'[dbo].[TwinNarratives]')
      AND c.name = N'Embedding'
      AND t.name <> N'nvarchar'
)
BEGIN
    ALTER TABLE [dbo].[TwinNarratives] DROP COLUMN [Embedding];
    ALTER TABLE [dbo].[TwinNarratives] ADD [Embedding] nvarchar(max) NULL;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: the EF model state is nvarchar(max) both before and after this migration.
            // This migration only repairs physical schema drift, so rolling it back leaves the
            // column as nvarchar(max), which is what the previous migration's model snapshot expects.
        }
    }
}
