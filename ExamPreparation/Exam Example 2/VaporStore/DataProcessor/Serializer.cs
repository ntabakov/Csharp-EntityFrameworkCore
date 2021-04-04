using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using VaporStore.DataProcessor.Dto.Export;
using VaporStore.XmlHelper;

namespace VaporStore.DataProcessor
{
	using System;
	using Data;

	public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var genres = context.Genres.ToList().Where(x => genreNames.Contains(x.Name)).Select(x => new
            {
                Id = x.Id,
                Genre = x.Name,
                Games = x.Games.Where(d => d.Purchases.Count > 0).Select(g => new
                    {
                        Id = g.Id,
                        Title = g.Name,
                        Developer = g.Developer.Name,
                        Tags = string.Join(", ", g.GameTags.Select(gt => gt.Tag.Name).ToList()),
                        Players = g.Purchases.Count

                    }).OrderByDescending(o => o.Players)
                    .ThenBy(t => t.Id)
                    .ToList(),
                TotalPlayers = x.Games.Sum(s=>s.Purchases.Count)
            }).ToList()
                .OrderByDescending(x => x.TotalPlayers)
                .ThenBy(x => x.Id).ToList();
            
            var result = JsonConvert.SerializeObject(genres,Formatting.Indented);
            
            
            return result;
        }

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            var users = context.Users.ToList().Where(x => x.Cards.Any(a => a.Purchases.Any(p=>p.Type.ToString()==storeType)))
                .Select(x => new UsersXmlExportModel()
                {
                    Username = x.Username,
                    Purchases = x.Cards.SelectMany(c => c.Purchases)
                        .Where(pp=>pp.Type.ToString()==storeType)
                        .Select(s => new PurchasesXmlOutputModel
                    {
                        Card = s.Card.Number,
                        Cvc = s.Card.Cvc,
                        Date = s.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                        Game = new GameXmlOutputModel()
                        {
                            Title = s.Game.Name,
                            Genre = s.Game.Genre.Name,
                            Price = s.Game.Price,
                        }
                    })
                        .OrderBy(o=>o.Date)
                        .ToArray(),
                    TotalSpent = x.Cards.Sum(s =>
                        s.Purchases.Where(p => p.Type.ToString() == storeType).Sum(su => su.Game.Price))
                })
                .OrderByDescending(x=>x.TotalSpent)
                .ThenBy(x=>x.Username)
                .ToList();


            //var users = context.Users.Where(x => x.Cards.Any(c => c.Purchases.Any()))
            //    .SelectMany(x=>x.Cards, (x,c) => new
            //    {
            //        x.Username,
            //        Purchases = c.Purchases.Where(p=>p.Type.ToString() == storeType).Select(s=> new
            //        {
            //            Card = s.Card.Number,
            //            Cvc = s.Card.Cvc,
            //            Date = s.Date.ToString("yyyy-MM-dd HH:mm",CultureInfo.InvariantCulture),
            //            Game = s.Game.Name,
            //            GameGenre = s.Game.Genre.Name,
            //            GamePrice = s.Game.Price
            //        }),
            //    }).ToList();
            //var usersList = new List<UsersXmlExportModel>();

            //foreach (var user in users)
            //{
            //    var newUser = new UsersXmlExportModel
            //    {
            //        Username = user.Username,
                    
            //    };

            //    var list = new List<PurchasesXmlOutputModel>();

            //    foreach (var purchase in user.Purchases)
            //    {
            //        var game = new GameXmlOutputModel()
            //        {
            //            Genre = purchase.GameGenre,
            //            Price = purchase.GamePrice,
            //            Title = purchase.Game
            //        };


            //        var purchaseForUser = new PurchasesXmlOutputModel
            //        {
            //            Card = purchase.Card,
            //            Game = game,
            //            Cvc = purchase.Cvc,
            //            Date = purchase.Date

            //        };

            //        list.Add(purchaseForUser);
            //    }

            //    newUser.Purchases = list.ToArray();
            //    newUser.TotalSpent = 69;

            //    usersList.Add(newUser);
            //}

            var result = XmlConverter.Serialize(users, "Users");
            

            return result;
        }
	}
}