using System.Linq;
using System.Text;
using Newtonsoft.Json;
using VaporStore.Data.Models;
using VaporStore.DataProcessor.Dto.Import;

namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Data;

	public static class Deserializer
	{
		public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var json = JsonConvert.DeserializeObject<GamesJsonInputModel[]>(jsonString);

            var gamesList = new List<Game>();

			var sb = new StringBuilder();

            foreach (var game in json)
            {
                if (!IsValid(game) || game.Tags.Length==0)
                {
                    sb.AppendLine("Invalid Data");
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
                    Tag tagToAdd = context.Tags.FirstOrDefault(x => x.Name == tag) ?? new Tag {Name = tag};

					newGame.GameTags.Add(new GameTag {Tag = tagToAdd});
                }

                sb.AppendLine($"Added {game.Name} ({game.Genre}) with {game.Tags.Length} tags");

				gamesList.Add(newGame);

            }

			context.Games.AddRange(gamesList);
            context.SaveChanges();


            return sb.ToString();
        }

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			throw new NotImplementedException();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			throw new NotImplementedException();
		}

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}