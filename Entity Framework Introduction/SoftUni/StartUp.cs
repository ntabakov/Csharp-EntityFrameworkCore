using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var db = new SoftUniContext();
            string result = DeleteProjectById(db);
            Console.WriteLine(result);

        }

        public static string RemoveTown(SoftUniContext context)
        {
            var addressesToDelete = context.Addresses
                .Where(x => x.Town.Name == "Seattle");
            int addressesCount = context.Addresses
                .Where(x => x.Town.Name == "Seattle").Count();

            var changeEmployeesAddressId = context.Employees
                .Where(x => x.Address.Town.Name == "Seattle");

            foreach (var e in changeEmployeesAddressId)
            {
                e.AddressId = null;
                
            }

            context.SaveChanges();
            foreach (var a in addressesToDelete)
            {
                context.Addresses.Remove(a);
            }

            var town = context.Towns.FirstOrDefault(x => x.Name == "Seattle");
            context.Towns.Remove(town);
            context.SaveChanges();


            return $"{addressesCount} addresses in Seattle were deleted";
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var project = context.Projects.Find(2);

            var projectsToDelete = context.EmployeesProjects
                .Where(x => x.ProjectId == 2);

            foreach (var p in projectsToDelete)
            {
                context.EmployeesProjects.Remove(p);
            }

            context.Projects.Remove(project);
            context.SaveChanges();
            var sb = new StringBuilder();
            var projects = context.Projects
                .Select(p => p.Name)
                .Take(10)
                .ToList();

            foreach (var p in projects)
            {
                sb.AppendLine($"{p}");
            }

            return sb.ToString().Trim();
        }
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.FirstName.StartsWith("Sa"))
                .OrderBy(x=>x.FirstName)
                .ThenBy(x=>x.LastName);

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.Department.Name == "Engineering" ||
                            x.Department.Name == "Tool Design" ||
                            x.Department.Name == "Marketing" ||
                            x.Department.Name == "Information Services")
                //.Select(x => new
                //{
                //    x.FirstName,
                //    x.LastName,
                //    x.Salary
                //})
                .OrderBy(x=>x.FirstName)
                .ThenBy(x=>x.LastName);

            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                e.Salary *= 1.12M;
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }


            return sb.ToString().TrimEnd();


        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(x => x.StartDate)
                .Select(x => new
                {
                    x.Name,
                    x.Description,
                    x.StartDate
                })
                
                .Take(10);
            var sb = new StringBuilder();

            foreach (var p in projects.OrderBy(x=>x.Name))
            {
                sb.AppendLine($"{p.Name}");
                sb.AppendLine($"{p.Description}");
                sb.AppendLine($"{p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");

            }

            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(x => x.Employees.Count > 5)
                .OrderBy(x => x.Employees.Count)
                .ThenBy(x => x.Name)
                .Select(x => new
                {
                    DepartmentName = x.Name,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Employees = x.Employees.Select(x=>new
                    {
                        EmployeeFirstName = x.FirstName,
                        EmployeeLastName = x.LastName,
                        EmplooyeeJobTitle = x.JobTitle
                    }),
                });
            var sb = new StringBuilder();

            foreach (var department in departments)
            {
                sb.AppendLine(
                    $"{department.DepartmentName} - {department.ManagerFirstName} {department.ManagerLastName}");
                foreach (var e in department.Employees.OrderBy(x=>x.EmployeeFirstName).ThenBy(x=>x.EmployeeLastName))
                {
                    sb.AppendLine($"{e.EmployeeFirstName} {e.EmployeeLastName} - {e.EmplooyeeJobTitle}");
                }

            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees
                .Select(x=>new
                {
                    x.EmployeeId,
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    Project = x.EmployeesProjects.Select(x=>
                        x.Project),
                })
                .FirstOrDefault(x => x.EmployeeId == 147);
            var sb = new StringBuilder();
            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
            foreach (var e in employee.Project.OrderBy(x => x.Name))
            {
                sb.AppendLine(e.Name);
            }

            return sb.ToString().TrimEnd();


        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var adresses = context.Addresses
                .Select(x=>new
                {
                    x.Employees,
                    x.Town.Name,
                    x.AddressText,
                    
                })
                .OrderByDescending(x => x.Employees.Count)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.AddressText);
            var sb = new StringBuilder();
            foreach (var e in adresses)
            {
                sb.AppendLine($"{e.AddressText}, {e.Name} - {e.Employees.Count} employees");
            }

            return sb.ToString().TrimEnd();

        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.EmployeesProjects.
                    Any(c => c.Project.StartDate.Year >= 2001
                    && c.Project.StartDate.Year <= 2003))
                .Select(x => new
                {
                    EmployeeFirstName = x.FirstName,
                    EmployeeLastName = x.LastName,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Project = x.EmployeesProjects.Select(x=>new{
                        x.Project.Name,
                        x.Project.StartDate,
                        x.Project.EndDate
                        })
                })
                .Take(10);

            var sb = new StringBuilder();

            foreach (var emp in employees)
            {
                sb.AppendLine(
                    $"{emp.EmployeeFirstName} {emp.EmployeeLastName} - Manager: {emp.ManagerFirstName} {emp.ManagerLastName}");
                foreach (var proj in emp.Project)
                {
                    var endDate =
                        proj.EndDate.HasValue
                            ? proj.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                            : "not finished";

                    sb.AppendLine($"--{proj.Name} - {proj.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - {endDate}");

                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees.Select(x => new
            {
                x.EmployeeId,
                x.FirstName,
                x.LastName,
                x.MiddleName,
                x.JobTitle,
                x.Salary
            })
                .OrderBy(x => x.EmployeeId);

            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();

        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees.Select(x => new
            {
                x.FirstName,
                x.Salary
            })
                .Where(x => x.Salary > 50000)
                .OrderBy(x => x.FirstName);

            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(x => x.Department.Name == "Research and Development")
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.Department.Name,
                    x.Salary
                })
                .OrderBy(x => x.Salary).ThenByDescending(x => x.FirstName);

            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.Name} - ${e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var nakov = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");


            context.Addresses.Add(address);
            context.SaveChanges();

            nakov.Address = address;
            context.SaveChanges();

            var adresses = context.Employees
                    .Select(x => new
                    {
                        x.Address.AddressText,
                        x.Address.AddressId
                    })
                    .OrderByDescending(x => x.AddressId)
                    .Take(10)
                ;
            var sb = new StringBuilder();

            foreach (var adr in adresses)
            {
                sb.AppendLine(adr.AddressText);
            }

            return sb.ToString().TrimEnd();

        }
    }
}
