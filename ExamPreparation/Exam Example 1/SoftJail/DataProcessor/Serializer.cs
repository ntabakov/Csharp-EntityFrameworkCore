using System.Linq;
using Newtonsoft.Json;
using SoftJail.DataProcessor.ExportDto;
using SoftJail.XML_Helper;

namespace SoftJail.DataProcessor
{

    using Data;
    using System;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners
                .ToList()
                .Where(x => ids.Contains(x.Id))
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.FullName,
                    CellNumber = x.Cell.CellNumber,
                    Officers = x.PrisonerOfficers.Select(s => new
                        {
                            OfficerName = s.Officer.FullName,
                            Department = s.Officer.Department.Name,
                        })
                        .OrderBy(o=>o.OfficerName)
                        .ToList(),
                    TotalOfficerSalary = x.PrisonerOfficers.Sum(s => s.Officer.Salary),
                })
                .OrderBy(o=>o.Name)
                .ThenBy(t=>t.Id)
                .ToList();

            var result = JsonConvert.SerializeObject(prisoners,Formatting.Indented);

            return result;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var names = prisonersNames.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var prisoners = context.Prisoners
                .ToList()
                .Where(x => names.Contains(x.FullName))
                .Select(x => new PrisonerXmlOutputModel
                {
                    Id = x.Id,
                    Name = x.FullName,
                    IncarcerationDate = x.IncarcerationDate.ToString("yyyy-MM-dd"),
                    EncryptedMessages = x.Mails.Select(m => new EncryptedMailsModel()
                    {
                        Description = Reverse(m.Description),
                    }).ToArray(),
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.IncarcerationDate)
                .ToList();

            var result = XmlConverter.Serialize(prisoners,"Prisoners");

            return result;
        }
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }


}