using System;
using System.Windows.Forms;
using LibraryManagementSystem.WinFormsApp.Models;

namespace LibraryManagementSystem.WinFormsApp;

public class BorrowerDialog : Form
{
    private Label lblName;
    private TextBox txtName;
    private Label lblPhone;
    private TextBox txtPhone;
    private Label lblEmail;
    private TextBox txtEmail;
    private Button btnSave;
    private Button btnCancel;

    public string BorrowerName => txtName.Text.Trim();
    public string Phone => txtPhone.Text.Trim();
    public string Email => txtEmail.Text.Trim();

    public BorrowerDialog(BorrowerDto? existing = null)
    {
        InitializeComponent();

        if (existing != null)
        {
            this.Text = "Edit Borrower";
            txtName.Text = existing.BorrowerName;
            txtPhone.Text = existing.Phone;
            txtEmail.Text = existing.Email ?? "";
        }
        else
        {
            this.Text = "Register Borrower";
        }
    }

    private void InitializeComponent()
    {
        this.lblName = new Label();
        this.txtName = new TextBox();
        this.lblPhone = new Label();
        this.txtPhone = new TextBox();
        this.lblEmail = new Label();
        this.txtEmail = new TextBox();
        this.btnSave = new Button();
        this.btnCancel = new Button();

        this.SuspendLayout();

        int labelX = 20, inputX = 140, startY = 20, gap = 40, width = 250;

        lblName.Text = "Name:";
        lblName.Location = new System.Drawing.Point(labelX, startY);
        lblName.Size = new System.Drawing.Size(100, 20);
        txtName.Location = new System.Drawing.Point(inputX, startY);
        txtName.Size = new System.Drawing.Size(width, 20);

        lblPhone.Text = "Phone:";
        lblPhone.Location = new System.Drawing.Point(labelX, startY + gap);
        lblPhone.Size = new System.Drawing.Size(100, 20);
        txtPhone.Location = new System.Drawing.Point(inputX, startY + gap);
        txtPhone.Size = new System.Drawing.Size(width, 20);

        lblEmail.Text = "Email (optional):";
        lblEmail.Location = new System.Drawing.Point(labelX, startY + gap * 2);
        lblEmail.Size = new System.Drawing.Size(100, 20);
        txtEmail.Location = new System.Drawing.Point(inputX, startY + gap * 2);
        txtEmail.Size = new System.Drawing.Size(width, 20);

        // Buttons
        btnSave.Text = "Save";
        btnSave.Location = new System.Drawing.Point(190, startY + gap * 3 + 10);
        btnSave.Size = new System.Drawing.Size(90, 30);
        btnSave.Click += new EventHandler(btnSave_Click);
        btnSave.BackColor = System.Drawing.Color.FromArgb(13, 110, 253);
        btnSave.ForeColor = System.Drawing.Color.White;
        btnSave.FlatStyle = FlatStyle.Flat;

        btnCancel.Text = "Cancel";
        btnCancel.Location = new System.Drawing.Point(290, startY + gap * 3 + 10);
        btnCancel.Size = new System.Drawing.Size(90, 30);
        btnCancel.Click += new EventHandler(btnCancel_Click);
        btnCancel.BackColor = System.Drawing.Color.FromArgb(108, 117, 125);
        btnCancel.ForeColor = System.Drawing.Color.White;
        btnCancel.FlatStyle = FlatStyle.Flat;

        // Form settings
        this.ClientSize = new System.Drawing.Size(420, 210);
        this.Controls.Add(lblName);
        this.Controls.Add(txtName);
        this.Controls.Add(lblPhone);
        this.Controls.Add(txtPhone);
        this.Controls.Add(lblEmail);
        this.Controls.Add(txtEmail);
        this.Controls.Add(btnSave);
        this.Controls.Add(btnCancel);

        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;

        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void btnSave_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(BorrowerName))
        {
            MessageBox.Show("Borrower Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (string.IsNullOrWhiteSpace(Phone))
        {
            MessageBox.Show("Phone number is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
