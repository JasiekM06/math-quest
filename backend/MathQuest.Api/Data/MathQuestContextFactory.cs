using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MathQuest.Api.Data;

public class MathQuestContextFactory : IDesignTimeDbContextFactory<MathQuestContext>
{
    public MathQuestContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<MathQuestContext>()
            .UseSqlite("Data Source=mathquest.db")
            .Options;

        return new MathQuestContext(options);      
    }
}