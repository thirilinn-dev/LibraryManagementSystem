using System;
using System.Windows.Forms;
using LibraryManagementSystem.WinFormsApp.Models;

namespace LibraryManagementSystem.WinFormsApp;

public class BookDialog : Form
{
    private Label lblTitle;
    private TextBox txtTitle;
    private Label lblAuthor;
    private TextBox txtAuthor;
    private Label lblGenre;
    private TextBox txtGenre;
    private Label lblLanguage;
    private TextBox txtLanguage;
    private Label lblDescription;
    private TextBox txtDescription;
    private Label lblTotalBooks;
    private NumericUpDown numTotalBooks;
    private Button btnSave;
    private Button btnCancel;

    public string BookTitle => txtTitle.Text.Trim();
    public string Author => txtAuthor.Text.Trim();
    public string Genre => txtGenre.Text.Trim();
    public string Language => txtLanguage.Text.Trim();
    public string Description => txtDescription.Text.Trim();
    public int TotalBooks => (int)numTotalBooks.Value;

    public BookDialog(BookDto? existing = null)
    {
        InitializeComponent();
        
        if (existing != null)
        {
            this.Text = "Edit Book";
            txtTitle.Text = existing.Title;
            txtAuthor.Text = existing.Author;
            txtGenre.Text = existing.Genre;
            txtLanguage.Text = existing.Language;
            txtDescription.Text = existing.Description ?? "";
            numTotalBooks.Value = existing.TotalBooks;
        }
        else
        {
            this.Text = "Add Book";
            numTotalBooks.Value = 1;
        }
    }

    private void InitializeComponent()
    {
        this.lblTitle = new Label();
        this.txtTitle = new TextBox();
        this.lblAuthor = new Label();
        this.txtAuthor = new TextBox();
        this.lblGenre = new Label();
        this.txtGenre = new TextBox();
        this.lblLanguage = new Label();
        this.txtLanguage = new TextBox();
        this.lblDescription = new Label();
        this.txtDescription = new TextBox();
        this.lblTotalBooks = new Label();
        this.numTotalBooks = new NumericUpDown();
        this.btnSave = new Button();
        this.btnCancel = new Button();

        ((System.ComponentModel.ISupportInitialize)(this.numTotalBooks)).BeginInit();
        this.SuspendLayout();

        // 
        // Labels & Inputs Layout
        // 
        int labelX = 20, inputX = 140, startY = 20, gap = 40, width = 280;

        lblTitle.Text = "Title:";
        lblTitle.Location = new System.Drawing.Point(labelX, startY);
        lblTitle.Size = new System.Drawing.Size(100, 20);
        txtTitle.Location = new System.Drawing.Point(inputX, startY);
        txtTitle.Size = new System.Drawing.Size(width, 20);

        lblAuthor.Text = "Author:";
        lblAuthor.Location = new System.Drawing.Point(labelX, startY + gap);
        lblAuthor.Size = new System.Drawing.Size(100, 20);
        txtAuthor.Location = new System.Drawing.Point(inputX, startY + gap);
        txtAuthor.Size = new System.Drawing.Size(width, 20);

        lblGenre.Text = "Genre:";
        lblGenre.Location = new System.Drawing.Point(labelX, startY + gap * 2);
        lblGenre.Size = new System.Drawing.Size(100, 20);
        txtGenre.Location = new System.Drawing.Point(inputX, startY + gap * 2);
        txtGenre.Size = new System.Drawing.Size(width, 20);

        lblLanguage.Text = "Language:";
        lblLanguage.Location = new System.Drawing.Point(labelX, startY + gap * 3);
        lblLanguage.Size = new System.Drawing.Size(100, 20);
        txtLanguage.Location = new System.Drawing.Point(inputX, startY + gap * 3);
        txtLanguage.Size = new System.Drawing.Size(width, 20);

        lblDescription.Text = "Description:";
        lblDescription.Location = new System.Drawing.Point(labelX, startY + gap * 4);
        lblDescription.Size = new System.Drawing.Size(100, 20);
        txtDescription.Location = new System.Drawing.Point(inputX, startY + gap * 4);
        txtDescription.Size = new System.Drawing.Size(width, 60);
        txtDescription.Multiline = true;

        lblTotalBooks.Text = "Total Copies:";
        lblTotalBooks.Location = new System.Drawing.Point(labelX, startY + gap * 4 + 70);
        lblTotalBooks.Size = new System.Drawing.Size(100, 20);
        numTotalBooks.Location = new System.Drawing.Point(inputX, startY + gap * 4 + 70);
        numTotalBooks.Size = new System.Drawing.Size(100, 20);
        numTotalBooks.Minimum = 0;
        numTotalBooks.Maximum = 1000;

        // 
        // Buttons
        // 
        btnSave.Text = "Save";
        btnSave.Location = new System.Drawing.Point(230, startY + gap * 4 + 115);
        btnSave.Size = new System.Drawing.Size(90, 30);
        btnSave.Click += new EventHandler(btnSave_Click);
        btnSave.BackColor = System.Drawing.Color.FromArgb(13, 110, 253);
        btnSave.ForeColor = System.Drawing.Color.White;
        btnSave.FlatStyle = FlatStyle.Flat;

        btnCancel.Text = "Cancel";
        btnCancel.Location = new System.Drawing.Point(330, startY + gap * 4 + 115);
        btnCancel.Size = new System.Drawing.Size(90, 30);
        btnCancel.Click += new EventHandler(btnCancel_Click);
        btnCancel.BackColor = System.Drawing.Color.FromArgb(108, 117, 125);
        btnCancel.ForeColor = System.Drawing.Color.White;
        btnCancel.FlatStyle = FlatStyle.Flat;

        // 
        // Form Configuration
        // 
        this.ClientSize = new System.Drawing.Size(450, 370);
        this.Controls.Add(lblTitle);
        this.Controls.Add(txtTitle);
        this.Controls.Add(lblAuthor);
        this.Controls.Add(txtAuthor);
        this.Controls.Add(lblGenre);
        this.Controls.Add(txtGenre);
        this.Controls.Add(lblLanguage);
        this.Controls.Add(txtLanguage);
        this.Controls.Add(lblDescription);
        this.Controls.Add(txtDescription);
        this.Controls.Add(lblTotalBooks);
        this.Controls.Add(numTotalBooks);
        this.Controls.Add(btnSave);
        this.Controls.Add(btnCancel);
        
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;

        ((System.ComponentModel.ISupportInitialize)(this.numTotalBooks)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(BookTitle))
        {
            MessageBox.Show("Title is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(Author))
        {
            MessageBox.Show("Author is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(Genre))
        {
            MessageBox.Show("Genre is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(Language))
        {
            MessageBox.Show("Language is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private void btnCancel_Click(object? sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}
