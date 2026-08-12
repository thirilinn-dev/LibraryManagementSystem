using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows.Forms;
using LibraryManagementSystem.WinFormsApp.Models;

namespace LibraryManagementSystem.WinFormsApp;

public class BorrowingHistoryDialog : Form
{
    private DataGridView dgvHistory;
    private Button btnClose;
    private int _borrowerId;
    private string _borrowerName;
    private HttpClient _client;

    public BorrowingHistoryDialog(int borrowerId, string borrowerName, HttpClient client)
    {
        _borrowerId = borrowerId;
        _borrowerName = borrowerName;
        _client = client;
        
        InitializeComponent();
        this.Text = $"Borrowing History - {_borrowerName} (ID: {_borrowerId})";
        
        this.Load += async (s, e) => await LoadHistoryAsync();
    }

    private void InitializeComponent()
    {
        this.dgvHistory = new DataGridView();
        this.btnClose = new Button();
        
        this.SuspendLayout();

        // 
        // dgvHistory
        // 
        dgvHistory.Dock = DockStyle.Top;
        dgvHistory.Height = 280;
        dgvHistory.AllowUserToAddRows = false;
        dgvHistory.AllowUserToDeleteRows = false;
        dgvHistory.ReadOnly = true;
        dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvHistory.BackgroundColor = System.Drawing.Color.White;

        // 
        // btnClose
        // 
        btnClose.Text = "Close";
        btnClose.Size = new System.Drawing.Size(90, 30);
        btnClose.Location = new System.Drawing.Point(480, 295);
        btnClose.Click += (s, e) => this.Close();
        btnClose.FlatStyle = FlatStyle.Flat;
        btnClose.BackColor = System.Drawing.Color.FromArgb(108, 117, 125);
        btnClose.ForeColor = System.Drawing.Color.White;

        // 
        // Form Configuration
        // 
        this.ClientSize = new System.Drawing.Size(600, 340);
        this.Controls.Add(dgvHistory);
        this.Controls.Add(btnClose);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        
        this.ResumeLayout(false);
    }

    private async System.Threading.Tasks.Task LoadHistoryAsync()
    {
        try
        {
            var history = await _client.GetFromJsonAsync<List<BorrowingDto>>($"api/Borrower/{_borrowerId}/history");
            dgvHistory.DataSource = history;
            if (dgvHistory.Columns.Count > 0)
            {
                dgvHistory.Columns["BorrowId"].HeaderText = "Borrow ID";
                dgvHistory.Columns["BorrowerId"].Visible = false;
                dgvHistory.Columns["BorrowerName"].Visible = false;
                dgvHistory.Columns["BookId"].Visible = false;
                dgvHistory.Columns["BookTitle"].HeaderText = "Book Title";
                dgvHistory.Columns["BorrowDate"].HeaderText = "Borrow Date";
                dgvHistory.Columns["DueDate"].HeaderText = "Due Date";
                dgvHistory.Columns["ReturnDate"].HeaderText = "Return Date";
                dgvHistory.Columns["Status"].HeaderText = "Status";
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
