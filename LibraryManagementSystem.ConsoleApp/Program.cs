using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryManagementSystem.ConsoleApp.Models;

namespace LibraryManagementSystem.ConsoleApp;

class Program
{
    static async Task Main(string[] args)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:5288/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        while (true)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=================================================");
            Console.WriteLine("           LIBRARY MANAGEMENT SYSTEM");
            Console.WriteLine("=================================================");
            Console.ResetColor();

            try
            {
                var dashboard = await client.GetFromJsonAsync<DashboardDto>("api/Dashboard");
                if (dashboard != null)
                {
                    Console.WriteLine($"Total Books: {dashboard.TotalBooks,-10} | Registered Borrowers: {dashboard.RegisteredBorrowers}");
                    Console.WriteLine($"Available:   {dashboard.AvailableBooks,-10} | Active Borrowings:    {dashboard.ActiveBorrowings}");
                    if (dashboard.OverdueBooks > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"WARNING: There are {dashboard.OverdueBooks} overdue borrowings! Please check them.");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("Overdue Books: 0");
                    }
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Could not load dashboard metrics. Ensure the Web API is running.");
                Console.ResetColor();
            }

            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("1. Book Management");
            Console.WriteLine("2. Borrower Management");
            Console.WriteLine("3. Borrowing Management");
            Console.WriteLine("4. Return Book");
            Console.WriteLine("5. View Overdue Books");
            Console.WriteLine("6. Exit");
            Console.WriteLine("=================================================");
            Console.Write("Select an option: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    await BookConsoleApp.RunAsync(client);
                    break;
                case "2":
                    await BorrowerConsoleApp.RunAsync(client);
                    break;
                case "3":
                    await BorrowingConsoleApp.RunAsync(client);
                    break;
                case "4":
                    await BorrowingConsoleApp.ReturnBookAsync(client);
                    Console.WriteLine("\nPress any key to return to main menu.");
                    Console.ReadKey();
                    break;
                case "5":
                    await BorrowingConsoleApp.ViewOverdueBooksAsync(client);
                    Console.WriteLine("\nPress any key to return to main menu.");
                    Console.ReadKey();
                    break;
                case "6":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid option. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }
}
