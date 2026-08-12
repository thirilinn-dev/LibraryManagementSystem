using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryManagementSystem.ConsoleApp.Models;

namespace LibraryManagementSystem.ConsoleApp;

public static class BookConsoleApp
{
    public static async Task RunAsync(HttpClient client)
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=================================");
            Console.WriteLine("        BOOK MANAGEMENT");
            Console.WriteLine("=================================");
            Console.WriteLine("1. View Books");
            Console.WriteLine("2. Search Books");
            Console.WriteLine("3. View Book Details");
            Console.WriteLine("4. Add Book");
            Console.WriteLine("5. Update Book");
            Console.WriteLine("6. Delete Book");
            Console.WriteLine("7. Back");
            Console.Write("Select an option: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    await ViewBooksAsync(client);
                    break;
                case "2":
                    await SearchBooksAsync(client);
                    break;
                case "3":
                    await ViewBookDetailsAsync(client);
                    break;
                case "4":
                    await AddBookAsync(client);
                    break;
                case "5":
                    await UpdateBookAsync(client);
                    break;
                case "6":
                    await DeleteBookAsync(client);
                    break;
                case "7":
                    return;
                default:
                    Console.WriteLine("Invalid option. Press any key to try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private static async Task ViewBooksAsync(HttpClient client)
    {
        try
        {
            var books = await client.GetFromJsonAsync<List<BookDto>>("api/Book");
            if (books == null || books.Count == 0)
            {
                Console.WriteLine("No books found.");
                return;
            }

            Console.WriteLine("\n----------------------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-5} | {"Title",-30} | {"Author",-20} | {"Genre",-15} | {"Avail",-5}");
            Console.WriteLine("----------------------------------------------------------------------------------");
            foreach (var b in books)
            {
                Console.WriteLine($"{b.BookId,-5} | {Truncate(b.Title, 30),-30} | {Truncate(b.Author, 20),-20} | {Truncate(b.Genre, 15),-15} | {b.AvailableBooks}/{b.TotalBooks,-5}");
            }
            Console.WriteLine("----------------------------------------------------------------------------------");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving books: {ex.Message}");
        }
    }

    private static async Task SearchBooksAsync(HttpClient client)
    {
        Console.Write("Enter Title to filter (or leave blank): ");
        string? title = Console.ReadLine();
        Console.Write("Enter Author to filter (or leave blank): ");
        string? author = Console.ReadLine();
        Console.Write("Enter Genre to filter (or leave blank): ");
        string? genre = Console.ReadLine();

        try
        {
            string url = $"api/Book?title={Uri.EscapeDataString(title ?? "")}&author={Uri.EscapeDataString(author ?? "")}&genre={Uri.EscapeDataString(genre ?? "")}";
            var books = await client.GetFromJsonAsync<List<BookDto>>(url);
            if (books == null || books.Count == 0)
            {
                Console.WriteLine("No books matching criteria found.");
                return;
            }

            Console.WriteLine("\n----------------------------------------------------------------------------------");
            Console.WriteLine($"{"ID",-5} | {"Title",-30} | {"Author",-20} | {"Genre",-15} | {"Avail",-5}");
            Console.WriteLine("----------------------------------------------------------------------------------");
            foreach (var b in books)
            {
                Console.WriteLine($"{b.BookId,-5} | {Truncate(b.Title, 30),-30} | {Truncate(b.Author, 20),-20} | {Truncate(b.Genre, 15),-15} | {b.AvailableBooks}/{b.TotalBooks,-5}");
            }
            Console.WriteLine("----------------------------------------------------------------------------------");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching books: {ex.Message}");
        }
    }

    private static async Task ViewBookDetailsAsync(HttpClient client)
    {
        Console.Write("Enter Book ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            var response = await client.GetAsync($"api/Book/{id}");
            if (response.IsSuccessStatusCode)
            {
                var b = await response.Content.ReadFromJsonAsync<BookDto>();
                if (b != null)
                {
                    Console.WriteLine("\n-----------------------------------");
                    Console.WriteLine("          BOOK DETAILS");
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine($"ID:          {b.BookId}");
                    Console.WriteLine($"Title:       {b.Title}");
                    Console.WriteLine($"Author:      {b.Author}");
                    Console.WriteLine($"Genre:       {b.Genre}");
                    Console.WriteLine($"Language:    {b.Language}");
                    Console.WriteLine($"Description: {b.Description ?? "N/A"}");
                    Console.WriteLine($"Total Books: {b.TotalBooks}");
                    Console.WriteLine($"Available:   {b.AvailableBooks}");
                    Console.WriteLine("-----------------------------------");
                }
            }
            else
            {
                Console.WriteLine($"Book not found (Status: {response.StatusCode})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving book: {ex.Message}");
        }
    }

    private static async Task AddBookAsync(HttpClient client)
    {
        Console.Write("Enter Title: ");
        string title = Console.ReadLine() ?? "";
        Console.Write("Enter Author: ");
        string author = Console.ReadLine() ?? "";
        Console.Write("Enter Genre: ");
        string genre = Console.ReadLine() ?? "";
        Console.Write("Enter Language: ");
        string language = Console.ReadLine() ?? "";
        Console.Write("Enter Description: ");
        string description = Console.ReadLine() ?? "";
        Console.Write("Enter Total Copies: ");
        if (!int.TryParse(Console.ReadLine(), out int total) || total < 1)
        {
            Console.WriteLine("Invalid copy count.");
            return;
        }

        var dto = new CreateBookDto
        {
            Title = title,
            Author = author,
            Genre = genre,
            Language = language,
            Description = string.IsNullOrWhiteSpace(description) ? null : description,
            TotalBooks = total
        };

        try
        {
            var response = await client.PostAsJsonAsync("api/Book", dto);
            await HandleApiResponse(response, "Book added successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding book: {ex.Message}");
        }
    }

    private static async Task UpdateBookAsync(HttpClient client)
    {
        Console.Write("Enter Book ID to Update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        try
        {
            var response = await client.GetAsync($"api/Book/{id}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Book not found.");
                return;
            }

            var existing = await response.Content.ReadFromJsonAsync<BookDto>();
            if (existing == null) return;

            Console.WriteLine($"Updating Book: {existing.Title}");
            Console.Write($"Enter New Title [{existing.Title}]: ");
            string titleInput = Console.ReadLine() ?? "";
            string title = string.IsNullOrWhiteSpace(titleInput) ? existing.Title : titleInput;

            Console.Write($"Enter New Author [{existing.Author}]: ");
            string authorInput = Console.ReadLine() ?? "";
            string author = string.IsNullOrWhiteSpace(authorInput) ? existing.Author : authorInput;

            Console.Write($"Enter New Genre [{existing.Genre}]: ");
            string genreInput = Console.ReadLine() ?? "";
            string genre = string.IsNullOrWhiteSpace(genreInput) ? existing.Genre : genreInput;

            Console.Write($"Enter New Language [{existing.Language}]: ");
            string langInput = Console.ReadLine() ?? "";
            string language = string.IsNullOrWhiteSpace(langInput) ? existing.Language : langInput;

            Console.Write($"Enter New Description [{existing.Description ?? "None"}]: ");
            string descInput = Console.ReadLine() ?? "";
            string? description = string.IsNullOrWhiteSpace(descInput) ? existing.Description : descInput;

            Console.Write($"Enter New Total Copies [{existing.TotalBooks}]: ");
            string totalInput = Console.ReadLine() ?? "";
            int total = string.IsNullOrWhiteSpace(totalInput) ? existing.TotalBooks : (int.TryParse(totalInput, out int val) ? val : existing.TotalBooks);

            var dto = new UpdateBookDto
            {
                Title = title,
                Author = author,
                Genre = genre,
                Language = language,
                Description = description,
                TotalBooks = total
            };

            var patchResponse = await client.PatchAsJsonAsync($"api/Book/{id}", dto);
            await HandleApiResponse(patchResponse, "Book updated successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating book: {ex.Message}");
        }
    }

    private static async Task DeleteBookAsync(HttpClient client)
    {
        Console.Write("Enter Book ID to Delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Console.Write($"Are you sure you want to soft-delete book ID {id}? (y/n): ");
        if (Console.ReadLine()?.ToLower() != "y")
        {
            Console.WriteLine("Deletion cancelled.");
            return;
        }

        try
        {
            var response = await client.DeleteAsync($"api/Book/{id}");
            await HandleApiResponse(response, "Book deleted successfully (soft delete)!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting book: {ex.Message}");
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
