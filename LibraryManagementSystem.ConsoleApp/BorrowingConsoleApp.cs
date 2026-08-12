using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryManagementSystem.ConsoleApp.Models;

namespace LibraryManagementSystem.ConsoleApp;

public static class BorrowingConsoleApp
{
    public static async Task RunAsync(HttpClient client)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("       BORROWING MANAGEMENT");
            Console.WriteLine("=================================");
            Console.WriteLine("1. View Active Borrowings");
            Console.WriteLine("2. Lend Book");
            Console.WriteLine("3. Return Book");
            Console.WriteLine("4. View Overdue Books");
            Console.WriteLine("5. Back");
            Console.Write("Select an option: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    await ViewActiveBorrowingsAsync(client);
                    break;
                case "2":
                    await LendBookAsync(client);
                    break;
                case "3":
                    await ReturnBookAsync(client);
                    break;
                case "4":
                    await ViewOverdueBooksAsync(client);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid option. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    public static async Task ViewActiveBorrowingsAsync(HttpClient client)
    {
        try
        {
            var borrowings = await client.GetFromJsonAsync<List<BorrowingDto>>("api/Borrowing");
            if (borrowings == null || borrowings.Count == 0)
            {
                Console.WriteLine("No borrowing records found.");
                return;
            }

            var active = borrowings.FindAll(b => b.ReturnDate == null);
            if (active.Count == 0)
            {
                Console.WriteLine("No active borrowings currently.");
                return;
            }

            Console.WriteLine("\n------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"{"BorrowID",-9} | {"Borrower Name",-25} | {"Book Title",-30} | {"Borrow Date",-12} | {"Due Date",-12}");
            Console.WriteLine("------------------------------------------------------------------------------------------------------");
            foreach (var b in active)
            {
                Console.WriteLine($"{b.BorrowId,-9} | {Truncate(b.BorrowerName, 25),-25} | {Truncate(b.BookTitle, 30),-30} | {b.BorrowDate:yyyy-MM-dd,-12} | {b.DueDate:yyyy-MM-dd,-12}");
            }
            Console.WriteLine("------------------------------------------------------------------------------------------------------");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving borrowings: {ex.Message}");
        }
    }

    public static async Task LendBookAsync(HttpClient client)
    {
        Console.Write("Enter Borrower ID: ");
        if (!int.TryParse(Console.ReadLine(), out int borrowerId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Console.Write("Enter Book ID: ");
        if (!int.TryParse(Console.ReadLine(), out int bookId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var defaultDue = today.AddDays(14);

        Console.Write($"Enter Borrow Date (yyyy-MM-dd) [{today:yyyy-MM-dd}]: ");
        string borrowInput = Console.ReadLine() ?? "";
        DateOnly borrowDate = string.IsNullOrWhiteSpace(borrowInput) ? today : (DateOnly.TryParse(borrowInput, out var val) ? val : today);

        Console.Write($"Enter Due Date (yyyy-MM-dd) [{defaultDue:yyyy-MM-dd}]: ");
        string dueInput = Console.ReadLine() ?? "";
        DateOnly dueDate = string.IsNullOrWhiteSpace(dueInput) ? defaultDue : (DateOnly.TryParse(dueInput, out var val2) ? val2 : defaultDue);

        var dto = new CreateBorrowingDto
        {
            BorrowerId = borrowerId,
            BookId = bookId,
            BorrowDate = borrowDate,
            DueDate = dueDate
        };

        try
        {
            var response = await client.PostAsJsonAsync("api/Borrowing", dto);
            await HandleApiResponse(response, "Book successfully lent!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error lending book: {ex.Message}");
        }
    }

    public static async Task ReturnBookAsync(HttpClient client)
    {
        Console.Write("Enter Borrow ID to Return: ");
        if (!int.TryParse(Console.ReadLine(), out int borrowId))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            var response = await client.PostAsync($"api/Borrowing/{borrowId}/return", null);
            await HandleApiResponse(response, "Book returned successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error returning book: {ex.Message}");
        }
    }

    public static async Task ViewOverdueBooksAsync(HttpClient client)
    {
        try
        {
            var response = await client.GetAsync("api/Borrowing/overdue");
            if (response.IsSuccessStatusCode)
            {
                var overdue = await response.Content.ReadFromJsonAsync<List<BorrowingDto>>();
                if (overdue == null || overdue.Count == 0)
                {
                    Console.WriteLine("No overdue books found.");
                    return;
                }

                Console.WriteLine("\n=======================================================");
                Console.WriteLine("                    OVERDUE BOOKS");
                Console.WriteLine("=======================================================");
                Console.WriteLine($"{"BorrowID",-9} | {"Borrower Name",-20} | {"Book Title",-25} | {"Due Date",-12}");
                Console.WriteLine("-------------------------------------------------------");
                foreach (var b in overdue)
                {
                    Console.WriteLine($"{b.BorrowId,-9} | {Truncate(b.BorrowerName, 20),-20} | {Truncate(b.BookTitle, 25),-25} | {b.DueDate:yyyy-MM-dd,-12}");
                }
                Console.WriteLine("-------------------------------------------------------");
            }
            else
            {
                Console.WriteLine($"Error retrieving overdue list (Status: {response.StatusCode})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving overdue list: {ex.Message}");
        }
    }

    private static async Task HandleApiResponse(HttpResponseMessage response, string successMessage)
    {
        if (response.IsSuccessStatusCode)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(successMessage);
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            var errorContent = await response.Content.ReadAsStringAsync();
            string message = "An error occurred.";
            try
            {
                if (!string.IsNullOrEmpty(errorContent))
                {
                    var doc = System.Text.Json.JsonDocument.Parse(errorContent);
                    if (doc.RootElement.TryGetProperty("message", out var msgProp))
                    {
                        message = msgProp.GetString() ?? message;
                    }
                }
            }
            catch { }
            Console.WriteLine($"Error ({response.StatusCode}): {message}");
            Console.ResetColor();
        }
    }

    private static string Truncate(string val, int length)
    {
        if (string.IsNullOrEmpty(val)) return "";
        return val.Length <= length ? val : val.Substring(0, length - 3) + "...";
    }
}
