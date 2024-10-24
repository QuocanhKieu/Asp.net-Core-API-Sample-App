using T2305M_API.Enums;
using T2305M_API.Entities;
using System.Linq;

public static class DatabaseSeeder
{
    public static void Seed(T2305mApiContext context)
    {
        // Check if any Tag data already exists to prevent duplication
        //if (context.Tag.Any()) return;

        //// Create a list of Tag entities using GenreConstants
        //var tags = new List<Tag>
        //{
        //    new Tag { Name = GenreConstants.BiographiesMemoirs },
        //    new Tag { Name = GenreConstants.CivilWar },
        //    new Tag { Name = GenreConstants.GlobalHistory },
        //    new Tag { Name = GenreConstants.PoliticsLaw },
        //    new Tag { Name = GenreConstants.Reference },
        //    new Tag { Name = GenreConstants.ReligionAncientWorld },
        //    new Tag { Name = GenreConstants.SocietyCulture },
        //    new Tag { Name = GenreConstants.Warfare },
        //    new Tag { Name = GenreConstants.WorldWarII },
        //    new Tag { Name = GenreConstants.GeneralInterest },
        //    new Tag { Name = GenreConstants.ScienceNature },
        //    new Tag { Name = GenreConstants.AmericanHistory },
        //    new Tag { Name = GenreConstants.RussianHistory },
        //    new Tag { Name = GenreConstants.TwentiethCenturyPresent },
        //    new Tag { Name = GenreConstants.EuropeanHistory },
        //    new Tag { Name = GenreConstants.FanFavorites }
        //};

        //// Add tags to the context and save them to the database
        //context.Tag.AddRange(tags);
        //context.SaveChanges();
    }
}
