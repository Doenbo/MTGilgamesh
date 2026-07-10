using Microsoft.EntityFrameworkCore;
using MTG.Core.Cards;
using MTG.Core.Helper;
using MTG.Scryfall.Helper;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace MTG.DB;

[Table("CardsAsJson")]
public class CardCacheEntry
{
    [Key]
    [Column("Id")]
    public Guid Id { get; set; }

    [Column("Name")]
    public required string Name { get; set; }

    [Column("CollectorSet")]
    public required string CollectorSet { get; set; } //DB doesn't like "Set"

    [Column("CollectorNumber")]
    public required string CollectorNumber { get; set; }

    [Column("RawJson")]
    public required string RawJson { get; set; }

    [Column("AppVersion")]
    public required string AppVersion { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<CardCacheEntry> CardsAsJson { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //TODO Fix path
        string db_path = Path.GetFullPath(@"C:\Git\MTGilgamesh\MTG.DB\MTG.DB.mdf");
        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={db_path};Integrated Security=True;Connect Timeout=30";

        optionsBuilder.UseSqlServer(connectionString);
    }

    public static Result<JsonString> GetExact(CardRef cref)
    {
        try
        {
            using var dbContext = new AppDbContext();

            var cacheEntry = dbContext.CardsAsJson
            .FirstOrDefault(c => c.Name.ToLower().Contains(cref.Name.ToLower()));

            if (cacheEntry == null)
            {
                return Result<JsonString>.Failure($"Card with the name {cref.Name} was not found in DB!");
            }
            return Result<JsonString>.Success(new JsonString(cacheEntry.RawJson));
        }
        catch (Exception ex)
        {
            return Result<JsonString>.Failure($"Error reading DB: {ex.Message}");
        }
    }

    public static Result InsertCardIntoDb(JsonString json, CardRef cref)
    {
        try
        {
            using var dbContext = new AppDbContext();

            var existingEntry = dbContext.CardsAsJson.Find(cref.Id);
            var currentAppVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (currentAppVersion == null)
                return Result.Failure($"Cannot get AppVersion!");

            if (existingEntry == null)
            {
                var newEntry = new CardCacheEntry
                {
                    Id = cref.Id,
                    Name = cref.Name,
                    CollectorSet = cref.Set,
                    CollectorNumber = cref.CollectorNumber,
                    RawJson = json.Value,
                    AppVersion = currentAppVersion.ToString(),
                };
                dbContext.CardsAsJson.Add(newEntry);
            }
            else if (existingEntry.AppVersion != currentAppVersion.ToString())
            {
                existingEntry.RawJson = json.Value;
                existingEntry.AppVersion = currentAppVersion.ToString();

                dbContext.CardsAsJson.Update(existingEntry);
            }
            else
            {
                return Result.Success();
            }

            dbContext.SaveChanges();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error accessing DB: {ex}");
        }
    }
}
