using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryManagementSystem.WinFormsApp.Models;

namespace LibraryManagementSystem.WinFormsApp;

public partial class Form1 : Form
{
    private readonly HttpClient _client;

    public Form1()
    {
        InitializeComponent();

        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://localhost:5288/");

        // Setup Event Handlers
        this.Load += Form1_Load;
    }

    private async void Form1_Load(object? sender, EventArgs e)
    {
        // Default to Dashboard
        SwitchTab(0);
        await RefreshDashboardAsync();
    }

    private void SwitchTab(int index)
    {
        tabControlMain.SelectedIndex = index;
    }

    // Sidebar navigation clicks
    private async void btnDashboard_Click(object? sender, EventArgs e)
    {
        SwitchTab(0);
        await RefreshDashboardAsync();
    }

    private async void btnBooks_Click(object? sender, EventArgs e)
    {
        SwitchTab(1);
        await LoadBooksAsync();
    }

    private async void btnBorrowers_Click(object? sender, EventArgs e)
    {
        SwitchTab(2);
        await LoadBorrowersAsync();
    }

    private async void btnBorrowing_Click(object? sender, EventArgs e)
    {
        SwitchTab(3);
        await PrepareLendFormAsync();
    }

    private async void btnReturns_Click(object? sender, EventArgs e)
    {
        SwitchTab(4);
        await LoadActiveBorrowingsAsync();
    }

    private async void btnOverdue_Click(object? sender, EventArgs e)
    {
        SwitchTab(5);
        await LoadOverdueBooksAsync();
    }

    // 1. Dashboard Tab Methods
    private async Task RefreshDashboardAsync()
    {
        try
        {
            var dashboard = await _client.GetFromJsonAsync<DashboardDto>("api/Dashboard");
            if (dashboard != null)
            {
                lblValTotalBooks.Text = dashboard.TotalBooks.ToString();
                lblValAvailBooks.Text = dashboard.AvailableBooks.ToString();
                lblValActiveBorrowings.Text = dashboard.ActiveBorrowings.ToString();
                lblValOverdueBooks.Text = dashboard.OverdueBooks.ToString();
                lblValRegisteredBorrowers.Text = dashboard.RegisteredBorrowers.ToString();

                if (dashboard.OverdueBooks > 0)
                {
                    lblOverdueWarning.Text = $"⚠️ WARNING: There are {dashboard.OverdueBooks} overdue borrowings!";
                    lblOverdueWarning.Visible = true;
                }
                else
                {
                    lblOverdueWarning.Visible = false;
                }
            }

            // Load quick overdue list in dashboard grid
            var overdueList = await _client.GetFromJsonAsync<List<BorrowingDto>>("api/Borrowing/overdue");
            dgvDashboardOverdue.DataSource = overdueList;
            if (dgvDashboardOverdue.Columns.Count > 0)
            {
                dgvDashboardOverdue.Columns["BorrowId"].HeaderText = "Borrow ID";
                dgvDashboardOverdue.Columns["BorrowerId"].Visible = false;
                dgvDashboardOverdue.Columns["BorrowerName"].HeaderText = "Borrower";
                dgvDashboardOverdue.Columns["BookId"].Visible = false;
                dgvDashboardOverdue.Columns["BookTitle"].HeaderText = "Book Title";
                dgvDashboardOverdue.Columns["BorrowDate"].HeaderText = "Borrow Date";
                dgvDashboardOverdue.Columns["DueDate"].HeaderText = "Due Date";
                dgvDashboardOverdue.Columns["ReturnDate"].Visible = false;
                dgvDashboardOverdue.Columns["Status"].HeaderText = "Status";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not connect to Web API. Ensure it is running.\nDetails: {ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // 2. Books Tab Methods
    private async Task LoadBooksAsync(string? title = null, string? author = null, string? genre = null)
    {
        try
        {
            string url = $"api/Book?title={Uri.EscapeDataString(title ?? "")}&author={Uri.EscapeDataString(author ?? "")}&genre={Uri.EscapeDataString(genre ?? "")}";
            var books = await _client.GetFromJsonAsync<List<BookDto>>(url);
            dgvBooks.DataSource = books;
            if (dgvBooks.Columns.Count > 0)
            {
                dgvBooks.Columns["BookId"].HeaderText = "ID";
                dgvBooks.Columns["Title"].HeaderText = "Title";
                dgvBooks.Columns["Author"].HeaderText = "Author";
                dgvBooks.Columns["Genre"].HeaderText = "Genre";
                dgvBooks.Columns["Language"].HeaderText = "Language";
                dgvBooks.Columns["Description"].HeaderText = "Description";
                dgvBooks.Columns["TotalBooks"].HeaderText = "Total";
                dgvBooks.Columns["AvailableBooks"].HeaderText = "Available";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnSearchBooks_Click(object? sender, EventArgs e)
    {
        await LoadBooksAsync(txtSearchTitle.Text, txtSearchAuthor.Text, txtSearchGenre.Text);
    }

    private async void btnClearSearch_Click(object? sender, EventArgs e)
    {
        txtSearchTitle.Clear();
        txtSearchAuthor.Clear();
        txtSearchGenre.Clear();
        await LoadBooksAsync();
    }

    private async void btnAddBook_Click(object? sender, EventArgs e)
    {
        using var dlg = new BookDialog();
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            var dto = new CreateBookDto
            {
                Title = dlg.BookTitle,
                Author = dlg.Author,
                Genre = dlg.Genre,
                Language = dlg.Language,
                Description = string.IsNullOrWhiteSpace(dlg.Description) ? null : dlg.Description,
                TotalBooks = dlg.TotalBooks
            };

            var res = await _client.PostAsJsonAsync("api/Book", dto);
            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show("Book added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadBooksAsync();
            }
            else
            {
                string errMsg = await GetErrorMessage(res);
                MessageBox.Show($"Failed to add book: {errMsg}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void btnEditBook_Click(object? sender, EventArgs e)
    {
        if (dgvBooks.CurrentRow == null) return;
        var book = (BookDto)dgvBooks.CurrentRow.DataBoundItem;

        using var dlg = new BookDialog(book);
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            var dto = new UpdateBookDto
            {
                Title = dlg.BookTitle,
                Author = dlg.Author,
                Genre = dlg.Genre,
                Language = dlg.Language,
                Description = string.IsNullOrWhiteSpace(dlg.Description) ? null : dlg.Description,
                TotalBooks = dlg.TotalBooks
            };

            var res = await _client.PatchAsJsonAsync($"api/Book/{book.BookId}", dto);
            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show("Book updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadBooksAsync();
            }
            else
            {
                string errMsg = await GetErrorMessage(res);
                MessageBox.Show($"Failed to update book: {errMsg}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void btnDeleteBook_Click(object? sender, EventArgs e)
    {
        if (dgvBooks.CurrentRow == null) return;
        var book = (BookDto)dgvBooks.CurrentRow.DataBoundItem;

        var confirm = MessageBox.Show($"Are you sure you want to soft-delete '{book.Title}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm == DialogResult.Yes)
        {
            var res = await _client.DeleteAsync($"api/Book/{book.BookId}");
            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show("Book soft-deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadBooksAsync();
            }
            else
            {
                string errMsg = await GetErrorMessage(res);
                MessageBox.Show($"Failed to delete book: {errMsg}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // 3. Borrowers Tab Methods
    private async Task LoadBorrowersAsync()
    {
        try
        {
            var borrowers = await _client.GetFromJsonAsync<List<BorrowerDto>>("api/Borrower");
            dgvBorrowers.DataSource = borrowers;
            if (dgvBorrowers.Columns.Count > 0)
            {
                dgvBorrowers.Columns["BorrowerId"].HeaderText = "ID";
                dgvBorrowers.Columns["BorrowerName"].HeaderText = "Name";
                dgvBorrowers.Columns["Phone"].HeaderText = "Phone";
                dgvBorrowers.Columns["Email"].HeaderText = "Email";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading borrowers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnRegisterBorrower_Click(object? sender, EventArgs e)
    {
        using var dlg = new BorrowerDialog();
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            var dto = new CreateBorrowerDto
            {
                BorrowerName = dlg.BorrowerName,
                Phone = dlg.Phone,
                Email = string.IsNullOrWhiteSpace(dlg.Email) ? null : dlg.Email
            };

            var res = await _client.PostAsJsonAsync("api/Borrower", dto);
            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show("Borrower registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadBorrowersAsync();
            }
            else
            {
                string errMsg = await GetErrorMessage(res);
                MessageBox.Show($"Failed to register: {errMsg}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void btnEditBorrower_Click(object? sender, EventArgs e)
    {
        if (dgvBorrowers.CurrentRow == null) return;
        var borrower = (BorrowerDto)dgvBorrowers.CurrentRow.DataBoundItem;

        using var dlg = new BorrowerDialog(borrower);
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            var dto = new UpdateBorrowerDto
            {
                BorrowerName = dlg.BorrowerName,
                Phone = dlg.Phone,
                Email = string.IsNullOrWhiteSpace(dlg.Email) ? null : dlg.Email
            };

            var res = await _client.PatchAsJsonAsync($"api/Borrower/{borrower.BorrowerId}", dto);
            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show("Borrower updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadBorrowersAsync();
            }
            else
            {
                string errMsg = await GetErrorMessage(res);
                MessageBox.Show($"Failed to update: {errMsg}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void btnDeleteBorrower_Click(object? sender, EventArgs e)
    {
        if (dgvBorrowers.CurrentRow == null) return;
        var borrower = (BorrowerDto)dgvBorrowers.CurrentRow.DataBoundItem;

        var confirm = MessageBox.Show($"Are you sure you want to soft-delete '{borrower.BorrowerName}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm == DialogResult.Yes)
        {
            var res = await _client.DeleteAsync($"api/Borrower/{borrower.BorrowerId}");
            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show("Borrower soft-deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadBorrowersAsync();
            }
            else
            {
                string errMsg = await GetErrorMessage(res);
                MessageBox.Show($"Failed to delete borrower: {errMsg}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void btnViewHistory_Click(object? sender, EventArgs e)
    {
        if (dgvBorrowers.CurrentRow == null) return;
        var borrower = (BorrowerDto)dgvBorrowers.CurrentRow.DataBoundItem;

        using var dlg = new BorrowingHistoryDialog(borrower.BorrowerId, borrower.BorrowerName, _client);
        dlg.ShowDialog();
    }

    // 4. Lend Book (Borrowing) Tab Methods
    private async Task PrepareLendFormAsync()
    {
        try
        {
            // Load borrowers for combobox
            var borrowers = await _client.GetFromJsonAsync<List<BorrowerDto>>("api/Borrower");
            cmbLendBorrower.DataSource = borrowers;
            cmbLendBorrower.DisplayMember = "BorrowerName";
            cmbLendBorrower.ValueMember = "BorrowerId";

            // Load books for combobox
            var books = await _client.GetFromJsonAsync<List<BookDto>>("api/Book");
            var availableBooks = books?.FindAll(b => b.AvailableBooks > 0);
            
            // Format displays nicely
            List<KeyValuePair<int, string>> bookList = new();
            if (availableBooks != null)
            {
                foreach (var b in availableBooks)
                {
                    bookList.Add(new KeyValuePair<int, string>(b.BookId, $"{b.Title} (Available: {b.AvailableBooks})"));
                }
            }
            
            cmbLendBook.DataSource = bookList;
            cmbLendBook.DisplayMember = "Value";
            cmbLendBook.ValueMember = "Key";

            dtpLendBorrowDate.Value = DateTime.Today;
            dtpLendDueDate.Value = DateTime.Today.AddDays(14);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing lending form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnLendBook_Click(object? sender, EventArgs e)
    {
        if (cmbLendBorrower.SelectedValue == null)
        {
            MessageBox.Show("Please select a borrower.", "Lend Book", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (cmbLendBook.SelectedValue == null)
        {
            MessageBox.Show("Please select a book.", "Lend Book", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int borrowerId = (int)cmbLendBorrower.SelectedValue;
        int bookId = (int)cmbLendBook.SelectedValue;
        DateOnly borrowDate = DateOnly.FromDateTime(dtpLendBorrowDate.Value);
        DateOnly dueDate = DateOnly.FromDateTime(dtpLendDueDate.Value);

        var dto = new CreateBorrowingDto
        {
            BorrowerId = borrowerId,
            BookId = bookId,
            BorrowDate = borrowDate,
            DueDate = dueDate
        };

        var res = await _client.PostAsJsonAsync("api/Borrowing", dto);
        if (res.IsSuccessStatusCode)
        {
            MessageBox.Show("Book lent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await PrepareLendFormAsync(); // reload combobox counts
        }
        else
        {
            string errMsg = await GetErrorMessage(res);
            MessageBox.Show($"Failed to lend book: {errMsg}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // 5. Returns Tab Methods
    private async Task LoadActiveBorrowingsAsync()
    {
        try
        {
            var borrowings = await _client.GetFromJsonAsync<List<BorrowingDto>>("api/Borrowing");
            var active = borrowings?.FindAll(b => b.ReturnDate == null);
            dgvReturns.DataSource = active;
            if (dgvReturns.Columns.Count > 0)
            {
                dgvReturns.Columns["BorrowId"].HeaderText = "Borrow ID";
                dgvReturns.Columns["BorrowerId"].Visible = false;
                dgvReturns.Columns["BorrowerName"].HeaderText = "Borrower";
                dgvReturns.Columns["BookId"].Visible = false;
                dgvReturns.Columns["BookTitle"].HeaderText = "Book Title";
                dgvReturns.Columns["BorrowDate"].HeaderText = "Borrow Date";
                dgvReturns.Columns["DueDate"].HeaderText = "Due Date";
                dgvReturns.Columns["ReturnDate"].Visible = false;
                dgvReturns.Columns["Status"].HeaderText = "Status";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading active borrowings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnReturnBook_Click(object? sender, EventArgs e)
    {
        if (dgvReturns.CurrentRow == null) return;
        var borrowing = (BorrowingDto)dgvReturns.CurrentRow.DataBoundItem;

        var confirm = MessageBox.Show($"Are you sure you want to return book '{borrowing.BookTitle}' borrowed by '{borrowing.BorrowerName}'?", "Confirm Return", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm == DialogResult.Yes)
        {
            var res = await _client.PostAsync($"api/Borrowing/{borrowing.BorrowId}/return", null);
            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show("Book returned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadActiveBorrowingsAsync();
            }
            else
            {
                string errMsg = await GetErrorMessage(res);
                MessageBox.Show($"Failed to return book: {errMsg}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // 6. Overdue Tab Methods
    private async Task LoadOverdueBooksAsync()
    {
        try
        {
            var overdue = await _client.GetFromJsonAsync<List<BorrowingDto>>("api/Borrowing/overdue");
            dgvOverdue.DataSource = overdue;
            if (dgvOverdue.Columns.Count > 0)
            {
                dgvOverdue.Columns["BorrowId"].HeaderText = "Borrow ID";
                dgvOverdue.Columns["BorrowerId"].Visible = false;
                dgvOverdue.Columns["BorrowerName"].HeaderText = "Borrower";
                dgvOverdue.Columns["BookId"].Visible = false;
                dgvOverdue.Columns["BookTitle"].HeaderText = "Book Title";
                dgvOverdue.Columns["BorrowDate"].HeaderText = "Borrow Date";
                dgvOverdue.Columns["DueDate"].HeaderText = "Due Date";
                dgvOverdue.Columns["ReturnDate"].Visible = false;
                dgvOverdue.Columns["Status"].HeaderText = "Status";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading overdue: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnReturnOverdueBook_Click(object? sender, EventArgs e)
    {
        if (dgvOverdue.CurrentRow == null) return;
        var borrowing = (BorrowingDto)dgvOverdue.CurrentRow.DataBoundItem;

        var confirm = MessageBox.Show($"Are you sure you want to return overdue book '{borrowing.BookTitle}'?", "Confirm Return", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm == DialogResult.Yes)
        {
            var res = await _client.PostAsync($"api/Borrowing/{borrowing.BorrowId}/return", null);
            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show("Book returned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadOverdueBooksAsync();
            }
            else
            {
                string errMsg = await GetErrorMessage(res);
                MessageBox.Show($"Failed to return book: {errMsg}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void btnRefreshOverdue_Click(object? sender, EventArgs e)
    {
        await LoadOverdueBooksAsync();
    }

    private async void btnRefreshReturns_Click(object? sender, EventArgs e)
    {
        await LoadActiveBorrowingsAsync();
    }

    private async void btnRefreshBooks_Click(object? sender, EventArgs e)
    {
        await LoadBooksAsync();
    }

    private async void btnRefreshBorrowers_Click(object? sender, EventArgs e)
    {
        await LoadBorrowersAsync();
    }

    private async void btnRefreshDashboard_Click(object? sender, EventArgs e)
    {
        await RefreshDashboardAsync();
    }

    // Error helper
    private async Task<string> GetErrorMessage(HttpResponseMessage response)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        try
        {
            if (!string.IsNullOrEmpty(errorContent))
            {
                var doc = System.Text.Json.JsonDocument.Parse(errorContent);
                if (doc.RootElement.TryGetProperty("message", out var msgProp))
                {
                    return msgProp.GetString() ?? "An error occurred.";
                }
            }
        }
        catch { }
        return $"Server responded with {response.StatusCode}.";
    }
}
