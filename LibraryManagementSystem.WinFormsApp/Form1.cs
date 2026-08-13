using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using LibraryManagementSystem.WinFormsApp.Models;

namespace LibraryManagementSystem.WinFormsApp;

public partial class Form1 : Form
{
    private readonly HttpClient _client;
    private int? _selectedBookId = null;
    private int? _selectedBorrowerId = null;
    private bool _isUpdatingSelection = false;
    private List<BorrowerDto> _allBorrowers = new();

    public Form1()
    {
        InitializeComponent();

        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://localhost:5288/");

        this.Load += Form1_Load;
    }

    private async void Form1_Load(object? sender, EventArgs e)
    {
        SwitchTab(0);
        await LoadBooksAsync();
    }

    private void SwitchTab(int index)
    {
        tabControlMain.SelectedIndex = index;

        // Visual feedback for selected tab button
        Color activeColor = Color.FromArgb(141, 110, 99);   // #8D6E63
        Color inactiveColor = Color.FromArgb(121, 85, 72); // #795548

        btnNavBooks.BackColor = index == 0 ? activeColor : inactiveColor;
        btnNavBorrowers.BackColor = index == 1 ? activeColor : inactiveColor;
        btnNavBorrowings.BackColor = index == 2 ? activeColor : inactiveColor;
        btnNavOverdue.BackColor = index == 3 ? activeColor : inactiveColor;
    }

    // ==========================================
    // Top Navigation Handlers
    // ==========================================
    private async void btnNavBooks_Click(object? sender, EventArgs e)
    {
        SwitchTab(0);
        await LoadBooksAsync();
    }

    private async void btnNavBorrowers_Click(object? sender, EventArgs e)
    {
        SwitchTab(1);
        await LoadBorrowersAsync();
    }

    private async void btnNavBorrowings_Click(object? sender, EventArgs e)
    {
        SwitchTab(2);
        await PrepareLendFormAsync();
        await LoadActiveBorrowingsAsync();
    }

    private async void btnNavOverdue_Click(object? sender, EventArgs e)
    {
        SwitchTab(3);
        await LoadOverdueBooksAsync();
    }

    // ==========================================
    // 1. Books Management
    // ==========================================
    private async Task LoadBooksAsync(string? title = null)
    {
        try
        {
            string url = $"api/Book?title={Uri.EscapeDataString(title ?? "")}";
            var books = await _client.GetFromJsonAsync<List<BookDto>>(url);
            
            _isUpdatingSelection = true;
            dgvBooks.DataSource = null;
            dgvBooks.DataSource = books;
            SetupBookColumns();
            ClearBookFields();
            _isUpdatingSelection = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading books: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SetupBookColumns()
    {
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

    private void dgvBooks_SelectionChanged(object? sender, EventArgs e)
    {
        if (_isUpdatingSelection) return;
        if (dgvBooks.CurrentRow == null || dgvBooks.CurrentRow.DataBoundItem == null)
        {
            _selectedBookId = null;
            return;
        }

        if (dgvBooks.CurrentRow.DataBoundItem is BookDto book)
        {
            _selectedBookId = book.BookId;
            txtBookTitle.Text = book.Title;
            txtBookAuthor.Text = book.Author;
            txtBookGenre.Text = book.Genre;
            txtBookLanguage.Text = book.Language;
            txtBookDescription.Text = book.Description ?? "";
            numBookTotalCopies.Value = book.TotalBooks;
        }
    }

    private void ClearBookFields()
    {
        _selectedBookId = null;
        txtBookTitle.Clear();
        txtBookAuthor.Clear();
        txtBookGenre.Clear();
        txtBookLanguage.Clear();
        txtBookDescription.Clear();
        numBookTotalCopies.Value = 0;
        
        _isUpdatingSelection = true;
        dgvBooks.ClearSelection();
        _isUpdatingSelection = false;
    }

    private async void btnBookSearch_Click(object? sender, EventArgs e)
    {
        await LoadBooksAsync(txtBookSearch.Text);
    }

    private async void btnBookViewAll_Click(object? sender, EventArgs e)
    {
        txtBookSearch.Clear();
        await LoadBooksAsync();
    }

    private async void btnBookCreate_Click(object? sender, EventArgs e)
    {
        if (!ValidateBookInputs()) return;

        var dto = new CreateBookDto
        {
            Title = txtBookTitle.Text.Trim(),
            Author = txtBookAuthor.Text.Trim(),
            Genre = txtBookGenre.Text.Trim(),
            Language = txtBookLanguage.Text.Trim(),
            Description = string.IsNullOrWhiteSpace(txtBookDescription.Text) ? null : txtBookDescription.Text.Trim(),
            TotalBooks = (int)numBookTotalCopies.Value
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

    private async void btnBookUpdate_Click(object? sender, EventArgs e)
    {
        if (_selectedBookId == null)
        {
            MessageBox.Show("Please select a book from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!ValidateBookInputs()) return;

        var dto = new UpdateBookDto
        {
            Title = txtBookTitle.Text.Trim(),
            Author = txtBookAuthor.Text.Trim(),
            Genre = txtBookGenre.Text.Trim(),
            Language = txtBookLanguage.Text.Trim(),
            Description = string.IsNullOrWhiteSpace(txtBookDescription.Text) ? null : txtBookDescription.Text.Trim(),
            TotalBooks = (int)numBookTotalCopies.Value
        };

        var res = await _client.PatchAsJsonAsync($"api/Book/{_selectedBookId}", dto);
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

    private async void btnBookDelete_Click(object? sender, EventArgs e)
    {
        if (_selectedBookId == null)
        {
            MessageBox.Show("Please select a book from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show($"Are you sure you want to soft-delete the book '{txtBookTitle.Text}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm == DialogResult.Yes)
        {
            var res = await _client.DeleteAsync($"api/Book/{_selectedBookId}");
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

    private void btnBookClear_Click(object? sender, EventArgs e)
    {
        ClearBookFields();
    }

    private bool ValidateBookInputs()
    {
        if (string.IsNullOrWhiteSpace(txtBookTitle.Text))
        {
            MessageBox.Show("Title is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (string.IsNullOrWhiteSpace(txtBookAuthor.Text))
        {
            MessageBox.Show("Author is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (string.IsNullOrWhiteSpace(txtBookGenre.Text))
        {
            MessageBox.Show("Genre is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (string.IsNullOrWhiteSpace(txtBookLanguage.Text))
        {
            MessageBox.Show("Language is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    // ==========================================
    // 2. Borrowers Management
    // ==========================================
    private async Task LoadBorrowersAsync()
    {
        try
        {
            var borrowers = await _client.GetFromJsonAsync<List<BorrowerDto>>("api/Borrower");
            _allBorrowers = borrowers ?? new List<BorrowerDto>();
            
            _isUpdatingSelection = true;
            ApplyBorrowerFilter();
            ClearBorrowerFields();
            _isUpdatingSelection = false;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading borrowers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ApplyBorrowerFilter()
    {
        string search = txtBorrowerSearch.Text.Trim().ToLower();
        if (string.IsNullOrEmpty(search))
        {
            dgvBorrowers.DataSource = null;
            dgvBorrowers.DataSource = _allBorrowers;
        }
        else
        {
            var filtered = _allBorrowers.Where(b => 
                b.BorrowerName.ToLower().Contains(search) || 
                b.Phone.Contains(search) || 
                (b.Email ?? "").ToLower().Contains(search)
            ).ToList();
            dgvBorrowers.DataSource = null;
            dgvBorrowers.DataSource = filtered;
        }
        SetupBorrowerColumns();
    }

    private void SetupBorrowerColumns()
    {
        if (dgvBorrowers.Columns.Count > 0)
        {
            dgvBorrowers.Columns["BorrowerId"].HeaderText = "ID";
            dgvBorrowers.Columns["BorrowerName"].HeaderText = "Name";
            dgvBorrowers.Columns["Phone"].HeaderText = "Phone";
            dgvBorrowers.Columns["Email"].HeaderText = "Email";
        }
    }

    private void dgvBorrowers_SelectionChanged(object? sender, EventArgs e)
    {
        if (_isUpdatingSelection) return;
        if (dgvBorrowers.CurrentRow == null || dgvBorrowers.CurrentRow.DataBoundItem == null)
        {
            _selectedBorrowerId = null;
            return;
        }

        if (dgvBorrowers.CurrentRow.DataBoundItem is BorrowerDto borrower)
        {
            _selectedBorrowerId = borrower.BorrowerId;
            txtBorrowerName.Text = borrower.BorrowerName;
            txtBorrowerPhone.Text = borrower.Phone;
            txtBorrowerEmail.Text = borrower.Email ?? "";
        }
    }

    private void ClearBorrowerFields()
    {
        _selectedBorrowerId = null;
        txtBorrowerName.Clear();
        txtBorrowerPhone.Clear();
        txtBorrowerEmail.Clear();

        _isUpdatingSelection = true;
        dgvBorrowers.ClearSelection();
        _isUpdatingSelection = false;
    }

    private void btnBorrowerSearch_Click(object? sender, EventArgs e)
    {
        ApplyBorrowerFilter();
    }

    private void btnBorrowerViewAll_Click(object? sender, EventArgs e)
    {
        txtBorrowerSearch.Clear();
        ApplyBorrowerFilter();
    }

    private async void btnBorrowerCreate_Click(object? sender, EventArgs e)
    {
        if (!ValidateBorrowerInputs()) return;

        var dto = new CreateBorrowerDto
        {
            BorrowerName = txtBorrowerName.Text.Trim(),
            Phone = txtBorrowerPhone.Text.Trim(),
            Email = string.IsNullOrWhiteSpace(txtBorrowerEmail.Text) ? null : txtBorrowerEmail.Text.Trim()
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
            MessageBox.Show($"Failed to register borrower: {errMsg}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnBorrowerUpdate_Click(object? sender, EventArgs e)
    {
        if (_selectedBorrowerId == null)
        {
            MessageBox.Show("Please select a borrower from the list to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!ValidateBorrowerInputs()) return;

        var dto = new UpdateBorrowerDto
        {
            BorrowerName = txtBorrowerName.Text.Trim(),
            Phone = txtBorrowerPhone.Text.Trim(),
            Email = string.IsNullOrWhiteSpace(txtBorrowerEmail.Text) ? null : txtBorrowerEmail.Text.Trim()
        };

        var res = await _client.PatchAsJsonAsync($"api/Borrower/{_selectedBorrowerId}", dto);
        if (res.IsSuccessStatusCode)
        {
            MessageBox.Show("Borrower updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await LoadBorrowersAsync();
        }
        else
        {
            string errMsg = await GetErrorMessage(res);
            MessageBox.Show($"Failed to update borrower: {errMsg}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnBorrowerDelete_Click(object? sender, EventArgs e)
    {
        if (_selectedBorrowerId == null)
        {
            MessageBox.Show("Please select a borrower from the list to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirm = MessageBox.Show($"Are you sure you want to soft-delete borrower '{txtBorrowerName.Text}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm == DialogResult.Yes)
        {
            var res = await _client.DeleteAsync($"api/Borrower/{_selectedBorrowerId}");
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

    private void btnBorrowerClear_Click(object? sender, EventArgs e)
    {
        ClearBorrowerFields();
    }

    private void btnBorrowerHistory_Click(object? sender, EventArgs e)
    {
        if (_selectedBorrowerId == null)
        {
            MessageBox.Show("Please select a borrower to view history.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var dlg = new BorrowingHistoryDialog(_selectedBorrowerId.Value, txtBorrowerName.Text, _client);
        dlg.ShowDialog();
    }

    private bool ValidateBorrowerInputs()
    {
        if (string.IsNullOrWhiteSpace(txtBorrowerName.Text))
        {
            MessageBox.Show("Borrower Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (string.IsNullOrWhiteSpace(txtBorrowerPhone.Text))
        {
            MessageBox.Show("Phone number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    // ==========================================
    // 3. Borrowing & Returns
    // ==========================================
    private async Task PrepareLendFormAsync()
    {
        try
        {
            var borrowers = await _client.GetFromJsonAsync<List<BorrowerDto>>("api/Borrower");
            cmbLendBorrower.DataSource = borrowers;
            cmbLendBorrower.DisplayMember = "BorrowerName";
            cmbLendBorrower.ValueMember = "BorrowerId";

            var books = await _client.GetFromJsonAsync<List<BookDto>>("api/Book");
            var availableBooks = books?.FindAll(b => b.AvailableBooks > 0);

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

    private async Task LoadActiveBorrowingsAsync()
    {
        try
        {
            var borrowings = await _client.GetFromJsonAsync<List<BorrowingDto>>("api/Borrowing");
            var active = borrowings?.FindAll(b => b.ReturnDate == null);
            
            dgvReturns.DataSource = null;
            dgvReturns.DataSource = active;
            SetupReturnsColumns();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading active borrowings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SetupReturnsColumns()
    {
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
            await PrepareLendFormAsync();
            await LoadActiveBorrowingsAsync();
        }
        else
        {
            string errMsg = await GetErrorMessage(res);
            MessageBox.Show($"Failed to lend book: {errMsg}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void btnReturnBook_Click(object? sender, EventArgs e)
    {
        if (dgvReturns.CurrentRow == null || dgvReturns.CurrentRow.DataBoundItem == null)
        {
            MessageBox.Show("Please select an active borrowing record to return.", "Return Book", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        
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

    // ==========================================
    // 4. Overdue Loans
    // ==========================================
    private async Task LoadOverdueBooksAsync()
    {
        try
        {
            var overdue = await _client.GetFromJsonAsync<List<BorrowingDto>>("api/Borrowing/overdue");
            dgvOverdue.DataSource = null;
            dgvOverdue.DataSource = overdue;
            SetupOverdueColumns();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading overdue loans: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SetupOverdueColumns()
    {
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

    private async void btnReturnOverdueBook_Click(object? sender, EventArgs e)
    {
        if (dgvOverdue.CurrentRow == null || dgvOverdue.CurrentRow.DataBoundItem == null)
        {
            MessageBox.Show("Please select an overdue record to return.", "Return Book", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

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

    // ==========================================
    // Helpers
    // ==========================================
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
