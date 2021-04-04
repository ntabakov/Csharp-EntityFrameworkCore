using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using VaporStore.Data.Models;
using VaporStore.Data.Models.Enums;
using VaporStore.DataProcessor.Dto.Import;
using VaporStore.XmlHelper;

namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Data;

	public static class Deserializer
    {
        private const string ERROR_MSG = "Invalid Data";

		public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            // var sb = new StringBuilder();

            // var gameDtos = JsonConvert.DeserializeObject<List<GamesJsonInputModel>>(jsonString);

            // var games = new List<Game>();

            //// var developers = new List<Developer>();
            //// var genres = new List<Genre>();
            //// var tags = new List<Tag>();

            // foreach (var game in gameDtos)
            // {
            //     if (!IsValid(game))
            //     {
            //         sb.AppendLine("Invalid Data");

            //         continue;
            //     }



            //     var gameToAdd = new Game()
            //     {
            //         Name = game.Name,
            //         ReleaseDate = game.ReleaseDate.Value,
            //         Price = game.Price
            //     };

            //     var developer = context.Developers.FirstOrDefault(d => d.Name == game.Developer) ??
            //     new Developer()
            //     {
            //         Name = game.Developer
            //     };

            //     //developers.Add(developer);

            //     gameToAdd.Developer = developer;

            //     var genre = context.Genres.FirstOrDefault(g => g.Name == game.Genre) ??
            //     new Genre()
            //     {
            //         Name = game.Genre
            //     };

            //    // genres.Add(genre);

            //     gameToAdd.Genre = genre;

            //     foreach (var tag in game.Tags)
            //     {
            //         var tagToAdd = context.Tags.FirstOrDefault(t => t.Name == tag) ??
            //         new Tag()
            //         {
            //             Name = tag
            //         };

            //        // tags.Add(tagToAdd);

            //         gameToAdd.GameTags.Add(new GameTag()
            //         {
            //             Game = gameToAdd,
            //             Tag = tagToAdd
            //         });
            //     }

            //     if (gameToAdd.GameTags.Count == 0)
            //     {
            //         sb.AppendLine("Invalid Data");

            //         continue;
            //     }

            //     games.Add(gameToAdd);

            //     sb.AppendLine($"Added {game.Name} ({game.Genre}) with {game.Tags.Length} tags");
            // }

            // context.Games.AddRange(games);

            // context.SaveChanges();

            // return sb.ToString().Trim();
            var json = JsonConvert.DeserializeObject<GamesJsonInputModel[]>(jsonString);

            var gamesList = new List<Game>();

            var sb = new StringBuilder();

            foreach (var game in json)
            {
                if (!IsValid(game) || game.Tags.Length == 0)
                {
                    sb.AppendLine(ERROR_MSG);
                    continue;
                }

                var developer = context.Developers
                    .FirstOrDefault(x => x.Name == game.Developer) ?? new Developer()
                    {
                        Name = game.Developer,
                    };


                var genre = context.Genres
                    .FirstOrDefault(x => x.Name == game.Genre) ?? new Genre()
                    {
                        Name = game.Genre,
                    };


                var newGame = new Game
                {
                    Name = game.Name,
                    Developer = developer,
                    Genre = genre,
                    ReleaseDate = game.ReleaseDate.Value,
                    Price = game.Price,

                };

                foreach (var tag in game.Tags)
                {
                    Tag tagToAdd = context.Tags.FirstOrDefault(x => x.Name == tag) ?? new Tag { Name = tag };

                    newGame.GameTags.Add(new GameTag { Tag = tagToAdd });
                }

                sb.AppendLine($"Added {game.Name} ({game.Genre}) with {game.Tags.Length} tags");

                gamesList.Add(newGame);
                context.Games.Add(newGame);
                context.SaveChanges();

            }

            


            return sb.ToString();
        }

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var json = JsonConvert.DeserializeObject<UsersJsonImportModel[]>(jsonString);

            var sb = new StringBuilder();

            foreach (var user in json)
            {
                if (!IsValid(user) ||
                    !user.Cards.All(IsValid))
                {
                    sb.AppendLine(ERROR_MSG);
                    continue;
                }

                var userToAdd = new User()
                {
                    FullName = user.FullName,
                    Username = user.Username,
                    Email = user.Email,
                    Age = user.Age,
                    Cards = user.Cards.Select(x=> new Card()
                    {
                        Number = x.Number,
                        Cvc = x.Cvc,
                        Type = (CardType)Enum.Parse(typeof(CardType),x.Type)
                    }).ToList()
                };

                sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
                context.Users.Add(userToAdd);
                context.SaveChanges();
            }




            return sb.ToString().TrimEnd();
        }

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            var xml = XmlConverter.Deserializer<PurchasesXmlInputModel>(xmlString, "Purchases");

            var sb = new StringBuilder();

           

            foreach (var purchase in xml)
            {
                if (!IsValid(purchase))
                {
                    sb.AppendLine(ERROR_MSG);
                    continue;
                }

                var date = DateTime.ParseExact(purchase.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None);

                var card = context.Cards.FirstOrDefault(x => x.Number == purchase.Card);
                var game = context.Games.FirstOrDefault(x => x.Name == purchase.Title);

                var purchaseToAdd = new Purchase()
                {
                    Game = game,

                    Type = (PurchaseType)Enum.Parse(typeof(PurchaseType),purchase.Type),
                    ProductKey = purchase.Key,
                    Date = date,
                    Card = card,

                };

                var user = context.Users
                    .SelectMany(x=>x.Cards, (a, d) => new
                    {
                        a.Username,
                        d.Number,
                    }).FirstOrDefault(x => x.Number==purchase.Card);
                sb.AppendLine($"Imported {purchase.Title} for {user.Username}");

                context.Add(purchaseToAdd);
                context.SaveChanges();
            }

            return sb.ToString().TrimEnd();
        }

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}