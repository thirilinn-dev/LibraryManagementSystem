using System;
using System.Collections.Generic;
using System.Linq;
using LibraryManagementSystem.Database.AppDbContextModels;

namespace LibraryManagementSystem.WebApi;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        // Ensure database exists
        context.Database.EnsureCreated();

        // 1. Seed Books if empty
        if (!context.TblBooks.Any())
        {
            var books = new List<TblBook>
            {
                new TblBook { Title = "Moisten & Darken", Author = "J. K. Rowling", Genre = "Fantasy", Language = "English", Description = "A mysterious tale of shadows.", TotalBooks = 5, AvailableBooks = 4, IsDeleted = false },
                new TblBook { Title = "Missing Even Meeting", Author = "George Orwell", Genre = "Dystopian", Language = "English", Description = "A classic thriller of fate.", TotalBooks = 3, AvailableBooks = 3, IsDeleted = false },
                new TblBook { Title = "The Midnight Library", Author = "Matt Haig", Genre = "Fiction", Language = "English", Description = "Between life and death there is a library.", TotalBooks = 4, AvailableBooks = 4, IsDeleted = false },
                new TblBook { Title = "The Silent Patient", Author = "Alex Michaelides", Genre = "Mystery", Language = "English", Description = "A shock psychological thriller.", TotalBooks = 2, AvailableBooks = 2, IsDeleted = false },
                new TblBook { Title = "Where the Crawdads Sing", Author = "Delia Owens", Genre = "Drama", Language = "English", Description = "A beautiful story of survival.", TotalBooks = 6, AvailableBooks = 5, IsDeleted = false },
                new TblBook { Title = "Educated", Author = "Tara Westover", Genre = "Biography", Language = "English", Description = "An unforgettable memoir.", TotalBooks = 3, AvailableBooks = 3, IsDeleted = false },
                new TblBook { Title = "Becoming", Author = "Michelle Obama", Genre = "Autobiography", Language = "English", Description = "A deeply personal reckoning.", TotalBooks = 4, AvailableBooks = 3, IsDeleted = false },
                new TblBook { Title = "Sapiens", Author = "Yuval Noah Harari", Genre = "History", Language = "English", Description = "A brief history of humankind.", TotalBooks = 5, AvailableBooks = 5, IsDeleted = false },
                new TblBook { Title = "Atomic Habits", Author = "James Clear", Genre = "Self-Help", Language = "English", Description = "An easy way to build good habits.", TotalBooks = 10, AvailableBooks = 9, IsDeleted = false },
                new TblBook { Title = "Thinking, Fast and Slow", Author = "Daniel Kahneman", Genre = "Psychology", Language = "English", Description = "Two systems drive the way we think.", TotalBooks = 4, AvailableBooks = 4, IsDeleted = false }
            };

            context.TblBooks.AddRange(books);
            context.SaveChanges();
        }

        // 2. Seed Borrowers if empty
        if (!context.TblBorrowers.Any())
        {
            var borrowers = new List<TblBorrower>
            {
                new TblBorrower { BorrowerName = "Thiri", Phone = "09111222333", Email = "thiri@gmail.com", IsDeleted = false },
                new TblBorrower { BorrowerName = "Thein Htet", Phone = "09444555666", Email = "theinhtet@gmail.com", IsDeleted = false },
                new TblBorrower { BorrowerName = "Piti", Phone = "09777888999", Email = "piti@gmail.com", IsDeleted = false },
                new TblBorrower { BorrowerName = "Ko Lynn", Phone = "09222333444", Email = "kolynn@gmail.com", IsDeleted = false },
                new TblBorrower { BorrowerName = "Aye Aye", Phone = "09333444555", Email = "ayeaye@gmail.com", IsDeleted = false },
                new TblBorrower { BorrowerName = "Kyaw Kyaw", Phone = "09555666777", Email = "kyawkyaw@gmail.com", IsDeleted = false },
                new TblBorrower { BorrowerName = "Mya Mya", Phone = "09666777888", Email = "myamya@gmail.com", IsDeleted = false },
                new TblBorrower { BorrowerName = "Hla Hla", Phone = "09888999000", Email = "hlahla@gmail.com", IsDeleted = false },
                new TblBorrower { BorrowerName = "Nyi Nyi", Phone = "09999000111", Email = "nyinyi@gmail.com", IsDeleted = false },
                new TblBorrower { BorrowerName = "Zin Zin", Phone = "09111333555", Email = "zinzin@gmail.com", IsDeleted = false }
            };

            context.TblBorrowers.AddRange(borrowers);
            context.SaveChanges();
        }

        // 3. Seed Borrowings if empty (and we have books/borrowers)
        if (!context.TblBorrowings.Any())
        {
            var dbBooks = context.TblBooks.ToList();
            var dbBorrowers = context.TblBorrowers.ToList();

            if (dbBooks.Count >= 10 && dbBorrowers.Count >= 10)
            {
                var borrowings = new List<TblBorrowing>
                {
                    // Active loans (ReturnDate is null, status is "Borrowed")
                    new TblBorrowing { BorrowerId = dbBorrowers[0].BorrowerId, BookId = dbBooks[0].BookId, BorrowDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)), DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(9)), ReturnDate = null, Status = "Borrowed" },
                    new TblBorrowing { BorrowerId = dbBorrowers[1].BorrowerId, BookId = dbBooks[4].BookId, BorrowDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-2)), DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(12)), ReturnDate = null, Status = "Borrowed" },
                    new TblBorrowing { BorrowerId = dbBorrowers[2].BorrowerId, BookId = dbBooks[6].BookId, BorrowDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-3)), DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(11)), ReturnDate = null, Status = "Borrowed" },
                    new TblBorrowing { BorrowerId = dbBorrowers[3].BorrowerId, BookId = dbBooks[8].BookId, BorrowDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(13)), ReturnDate = null, Status = "Borrowed" },

                    // Returned loans (ReturnDate is set, status is "Returned")
                    new TblBorrowing { BorrowerId = dbBorrowers[4].BorrowerId, BookId = dbBooks[1].BookId, BorrowDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-15)), DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), ReturnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-2)), Status = "Returned" },
                    new TblBorrowing { BorrowerId = dbBorrowers[5].BorrowerId, BookId = dbBooks[2].BookId, BorrowDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)), DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(4)), ReturnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), Status = "Returned" },
                    new TblBorrowing { BorrowerId = dbBorrowers[6].BorrowerId, BookId = dbBooks[3].BookId, BorrowDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-8)), DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(6)), ReturnDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-3)), Status = "Returned" },

                    // Overdue loans (ReturnDate is null, DueDate is in the past, status is "Borrowed" or "Overdue")
                    new TblBorrowing { BorrowerId = dbBorrowers[7].BorrowerId, BookId = dbBooks[5].BookId, BorrowDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-20)), DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-6)), ReturnDate = null, Status = "Borrowed" },
                    new TblBorrowing { BorrowerId = dbBorrowers[8].BorrowerId, BookId = dbBooks[7].BookId, BorrowDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-25)), DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-11)), ReturnDate = null, Status = "Borrowed" },
                    new TblBorrowing { BorrowerId = dbBorrowers[9].BorrowerId, BookId = dbBooks[9].BookId, BorrowDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-30)), DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-16)), ReturnDate = null, Status = "Borrowed" }
                };

                // Adjust available book quantities for active and overdue loans (total 7 books checked out)
                dbBooks[0].AvailableBooks = dbBooks[0].TotalBooks - 1;
                dbBooks[4].AvailableBooks = dbBooks[4].TotalBooks - 1;
                dbBooks[6].AvailableBooks = dbBooks[6].TotalBooks - 1;
                dbBooks[8].AvailableBooks = dbBooks[8].TotalBooks - 1;
                dbBooks[5].AvailableBooks = dbBooks[5].TotalBooks - 1;
                dbBooks[7].AvailableBooks = dbBooks[7].TotalBooks - 1;
                dbBooks[9].AvailableBooks = dbBooks[9].TotalBooks - 1;

                context.TblBorrowings.AddRange(borrowings);
                context.SaveChanges();
            }
        }
    }
}
