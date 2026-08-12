using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryManagementSystem.ConsoleApp.Models;

namespace LibraryManagementSystem.ConsoleApp;

public static class BorrowerConsoleApp
{
    public static async Task RunAsync(HttpClient client)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("       BORROWER MANAGEMENT");
            Console.WriteLine("=================================");
            Console.WriteLine("1. View Borrowers");
            Console.WriteLine("2. Register Borrower");
            Console.WriteLine("3. Update Borrower");
            Console.WriteLine("4. Delete Borrower");
            Console.WriteLine("5. View Borrowing History");
            Console.WriteLine("6. Back");
            Console.Write("Select an option: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    await ViewBorrowersAsync(client);
                    break;
                case "2":
                    await RegisterBorrowerAsync(client);
                    break;
                case "3":
                    await UpdateBorrowerAsync(client);
                    break;
                case "4":
                    await DeleteBorrowerAsync(client);
                    break;
                case "5":
                    await ViewHistoryAsync(client);
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Invalid option. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private static async Task ViewBorrowersAsync(HttpClient client)
    {
        try
        {
            var borrowers = await client.GetFromJsonAsync<List<BorrowerDto>>("api/Borrower");
            if (borrowers == null || borrowers.Count == 0)
            {
                Console.WriteLine("No borrowers found.");
                return;
            }

            Console.WriteLine("\n----------------------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-5} | {"Name",-30} | {"Phone",-20} | {"Email",-25}");
            Console.WriteLine("----------------------------------------------------------------------------------");
            foreach (var b in borrowers)
            {
                Console.WriteLine($"{b.BorrowerId,-5} | {Truncate(b.BorrowerName, 30),-30} | {Truncate(b.Phone, 20),-20} | {Truncate(b.Email ?? "N/A", 25),-25}");
            }
            Console.WriteLine("----------------------------------------------------------------------------------");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving borrowers: {ex.Message}");
        }
    }

    private static async Task RegisterBorrowerAsync(HttpClient client)
    {
        Console.Write("Enter Borrower Name: ");
        string name = Console.ReadLine() ?? "";
        Console.Write("Enter Phone: ");
        string phone = Console.ReadLine() ?? "";
        Console.Write("Enter Email: ");
        string email = Console.ReadLine() ?? "";

        var dto = new CreateBorrowerDto
        {
            BorrowerName = name,
            Phone = phone,
            Email = string.IsNullOrWhiteSpace(email) ? null : email
        };

        try
        {
            var response = await client.PostAsJsonAsync("api/Borrower", dto);
            await HandleApiResponse(response, "Borrower registered successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error registering borrower: {ex.Message}");
        }
    }

    private static async Task UpdateBorrowerAsync(HttpClient client)
    {
        Console.Write("Enter Borrower ID to Update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            var response = await client.GetAsync($"api/Borrower/{id}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Borrower not found.");
                return;
            }

            var existing = await response.Content.ReadFromJsonAsync<BorrowerDto>();
            if (existing == null) return;

            Console.WriteLine($"Updating Borrower: {existing.BorrowerName}");
            Console.Write($"Enter New Name [{existing.BorrowerName}]: ");
            string nameInput = Console.ReadLine() ?? "";
            string name = string.IsNullOrWhiteSpace(nameInput) ? existing.BorrowerName : nameInput;

            Console.Write($"Enter New Phone [{existing.Phone}]: ");
            string phoneInput = Console.ReadLine() ?? "";
            string phone = string.IsNullOrWhiteSpace(phoneInput) ? existing.Phone : phoneInput;

            Console.Write($"Enter New Email [{existing.Email ?? "None"}]: ");
            string emailInput = Console.ReadLine() ?? "";
            string? email = string.IsNullOrWhiteSpace(emailInput) ? existing.Email : emailInput;

            var dto = new UpdateBorrowerDto
            {
                BorrowerName = name,
                Phone = phone,
                Email = email
            };

            var patchResponse = await client.PatchAsJsonAsync($"api/Borrower/{id}", dto);
            await HandleApiResponse(patchResponse, "Borrower updated successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating borrower: {ex.Message}");
        }
    }

    private static async Task DeleteBorrowerAsync(HttpClient client)
    {
        Console.Write("Enter Borrower ID to Delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Console.Write($"Are you sure you want to soft-delete borrower ID {id}? (y/n): ");
        if (Console.ReadLine()?.ToLower() != "y")
        {
            Console.WriteLine("Deletion cancelled.");
            return;
        }

        try
        {
            var response = await client.DeleteAsync($"api/Borrower/{id}");
            await HandleApiResponse(response, "Borrower deleted successfully (soft delete)!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting borrower: {ex.Message}");
        }
    }

    private static async Task ViewHistoryAsync(HttpClient client)
    {
        Console.Write("Enter Borrower ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            var response = await client.GetAsync($"api/Borrower/{id}/history");
            if (response.IsSuccessStatusCode)
            {
                var history = await response.Content.ReadFromJsonAsync<List<BorrowingDto>>();
                if (history == null || history.Count == 0)
                {
                    Console.WriteLine("No borrowing history found for this borrower.");
                    return;
                }

                Console.WriteLine("\n-------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine($"{"BorrowID",-9} | {"Book Title",-30} | {"Borrow Date",-12} | {"Due Date",-12} | {"Return Date",-12} | {"Status",-10}");
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------");
                foreach (var h in history)
                {
                    string returnDateStr = h.ReturnDate.HasValue ? h.ReturnDate.Value.ToString("yyyy-MM-dd") : "Not Returned";
                    Console.WriteLine($"{h.BorrowId,-9} | {Truncate(h.BookTitle, 30),-30} | {h.BorrowDate:yyyy-MM-dd,-12} | {h.DueDate:yyyy-MM-dd,-12} | {returnDateStr,-12} | {h.Status,-10}");
                }
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------");
            }
            else
            {
                Console.WriteLine($"Error retrieving borrower history (Status: {response.StatusCode})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving borrower history: {ex.Message}");
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
