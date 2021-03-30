using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using SoftJail.Data.Models;
using SoftJail.Data.Models.Enums;
using SoftJail.DataProcessor.ImportDto;
using SoftJail.XML_Helper;

namespace SoftJail.DataProcessor
{

    using Data;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var departments = JsonConvert.DeserializeObject<DepartmentJSONInputModel[]>(jsonString);

            var departmentsList = new List<Department>();

            var sb = new StringBuilder();

            foreach (var currDepartment in departments)
            {
                if (!IsValid(currDepartment) ||
                    currDepartment.Cells.Count == 0 ||
                    !currDepartment.Cells.All(IsValid)
                    )
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                foreach (var cells in currDepartment.Cells)
                {
                    
                }

                var department = new Department
                {
                    Name = currDepartment.Name,
                    Cells = currDepartment.Cells.Select(x=> new Cell
                    {
                        CellNumber = x.CellNumber,
                        HasWindow = x.HasWindow,
                    }).ToList(),
                };
                departmentsList.Add(department);
                sb.AppendLine($"Imported {department.Name} with {department.Cells.Count} cells");
            }

            context.Departments.AddRange(departmentsList);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var prisoners = JsonConvert.DeserializeObject<PrisonerJSONInputModel[]>(jsonString);

            var prisonersList = new List<Prisoner>();

            var sb = new StringBuilder();

            foreach (var prisoner in prisoners)
            {
                if (!IsValid(prisoner) ||
                    !prisoner.Mails.All(IsValid))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var incarcerationDate = DateTime.TryParseExact(
                    prisoner.IncarcerationDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var incarcerationResultDate);


                var releaseDate = DateTime.TryParseExact(
                    prisoner.IncarcerationDate,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var releaseResultDate);

                var newPrisoner = new Prisoner
                {
                    FullName = prisoner.FullName,
                    Nickname = prisoner.Nickname,
                    Age = prisoner.Age,
                    IncarcerationDate = incarcerationResultDate,
                    ReleaseDate = releaseResultDate,
                    Bail = prisoner.Bail,
                    CellId = prisoner.CellId,
                    Mails = prisoner.Mails.Select(x=> new Mail
                    {
                        Description = x.Description,
                        Sender = x.Sender,
                        Address = x.Address,
                    }).ToList()
                };
                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
                prisonersList.Add(newPrisoner);
            }
            context.Prisoners.AddRange(prisonersList);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var officers = XmlConverter.Deserializer<OfficerXMLInputModel>(xmlString,"Officers");

            var sb = new StringBuilder();

            var officersList = new List<Officer>();

            foreach (var officer in officers)
            {
                if (!IsValid(officer))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                    
                }

                var newOffcier = new Officer
                {
                    FullName = officer.Name,
                    Salary = officer.Money,
                    Position = (Position)Enum.Parse(typeof(Position),officer.Position),
                    Weapon = (Weapon)Enum.Parse(typeof(Weapon),officer.Weapon),
                    DepartmentId = officer.DepartmentId,
                    OfficerPrisoners = officer.Prisoners.Select(x=>new OfficerPrisoner
                    {
                        PrisonerId = x.Id,
                    }).ToList()
                };
                officersList.Add(newOffcier);
                sb.AppendLine($"Imported {officer.Name} ({officer.Prisoners.Length} prisoners)");
            }

            context.Officers.AddRange(officersList);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
            
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }
    }
}