using System.Windows.Forms;
using System.Drawing;

namespace LibraryManagementSystem.WinFormsApp;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    // Sidebar
    private Panel panelSidebar;
    private Label lblBrand;
    private Button btnDashboard;
    private Button btnBooks;
    private Button btnBorrowers;
    private Button btnBorrowing;
    private Button btnReturns;
    private Button btnOverdue;

    // Header
    private Panel panelHeader;
    private Label lblHeaderTitle;

    // Tab Control
    private TabControl tabControlMain;
    
    // Tab 1: Dashboard
    private TabPage tabDashboard;
    private Panel panelCardTotalBooks;
    private Label lblValTotalBooks;
    private Label lblTitleTotalBooks;
    private Panel panelCardAvailBooks;
    private Label lblValAvailBooks;
    private Label lblTitleAvailBooks;
    private Panel panelCardActiveBorrowings;
    private Label lblValActiveBorrowings;
    private Label lblTitleActiveBorrowings;
    private Panel panelCardOverdueBooks;
    private Label lblValOverdueBooks;
    private Label lblTitleOverdueBooks;
    private Panel panelCardRegisteredBorrowers;
    private Label lblValRegisteredBorrowers;
    private Label lblTitleRegisteredBorrowers;
    private Label lblOverdueWarning;
    private Label lblOverdueSectionTitle;
    private DataGridView dgvDashboardOverdue;
    private Button btnRefreshDashboard;

    // Tab 2: Books
    private TabPage tabBooks;
    private DataGridView dgvBooks;
    private TextBox txtSearchTitle;
    private TextBox txtSearchAuthor;
    private TextBox txtSearchGenre;
    private Label lblSearchTitle;
    private Label lblSearchAuthor;
    private Label lblSearchGenre;
    private Button btnSearchBooks;
    private Button btnClearSearch;
    private Button btnAddBook;
    private Button btnEditBook;
    private Button btnDeleteBook;
    private Button btnRefreshBooks;

    // Tab 3: Borrowers
    private TabPage tabBorrowers;
    private DataGridView dgvBorrowers;
    private Button btnRegisterBorrower;
    private Button btnEditBorrower;
    private Button btnDeleteBorrower;
    private Button btnViewHistory;
    private Button btnRefreshBorrowers;

    // Tab 4: Lend Book
    private TabPage tabBorrowing;
    private ComboBox cmbLendBorrower;
    private ComboBox cmbLendBook;
    private DateTimePicker dtpLendBorrowDate;
    private DateTimePicker dtpLendDueDate;
    private Label lblLendBorrower;
    private Label lblLendBook;
    private Label lblLendBorrowDate;
    private Label lblLendDueDate;
    private Button btnLendBook;

    // Tab 5: Returns
    private TabPage tabReturns;
    private DataGridView dgvReturns;
    private Button btnReturnBook;
    private Button btnRefreshReturns;

    // Tab 6: Overdue
    private TabPage tabOverdue;
    private DataGridView dgvOverdue;
    private Button btnReturnOverdueBook;
    private Button btnRefreshOverdue;

    private void InitializeComponent()
    {
        this.panelSidebar = new Panel();
        this.lblBrand = new Label();
        this.btnDashboard = new Button();
        this.btnBooks = new Button();
        this.btnBorrowers = new Button();
        this.btnBorrowing = new Button();
        this.btnReturns = new Button();
        this.btnOverdue = new Button();

        this.panelHeader = new Panel();
        this.lblHeaderTitle = new Label();

        this.tabControlMain = new TabControl();

        // Tabs
        this.tabDashboard = new TabPage();
        this.tabBooks = new TabPage();
        this.tabBorrowers = new TabPage();
        this.tabBorrowing = new TabPage();
        this.tabReturns = new TabPage();
        this.tabOverdue = new TabPage();

        this.SuspendLayout();

        // 
        // panelSidebar
        // 
        this.panelSidebar.BackColor = Color.FromArgb(33, 37, 41);
        this.panelSidebar.Dock = DockStyle.Left;
        this.panelSidebar.Width = 220;
        this.panelSidebar.Controls.Add(lblBrand);
        this.panelSidebar.Controls.Add(btnDashboard);
        this.panelSidebar.Controls.Add(btnBooks);
        this.panelSidebar.Controls.Add(btnBorrowers);
        this.panelSidebar.Controls.Add(btnBorrowing);
        this.panelSidebar.Controls.Add(btnReturns);
        this.panelSidebar.Controls.Add(btnOverdue);

        // lblBrand
        this.lblBrand.Text = "LIBRARIAN PORTAL";
        this.lblBrand.ForeColor = Color.White;
        this.lblBrand.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        this.lblBrand.Location = new Point(10, 20);
        this.lblBrand.Size = new Size(200, 30);
        this.lblBrand.TextAlign = ContentAlignment.MiddleCenter;

        // Navigation buttons
        int btnY = 80, btnHeight = 45, btnGap = 10;

        SetupSidebarButton(ref btnDashboard, "Dashboard", btnY);
        SetupSidebarButton(ref btnBooks, "Books Catalog", btnY + (btnHeight + btnGap));
        SetupSidebarButton(ref btnBorrowers, "Borrowers", btnY + (btnHeight + btnGap) * 2);
        SetupSidebarButton(ref btnBorrowing, "Lend Book", btnY + (btnHeight + btnGap) * 3);
        SetupSidebarButton(ref btnReturns, "Returns", btnY + (btnHeight + btnGap) * 4);
        SetupSidebarButton(ref btnOverdue, "Overdue List", btnY + (btnHeight + btnGap) * 5);

        // Sidebar clicks
        this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
        this.btnBooks.Click += new System.EventHandler(this.btnBooks_Click);
        this.btnBorrowers.Click += new System.EventHandler(this.btnBorrowers_Click);
        this.btnBorrowing.Click += new System.EventHandler(this.btnBorrowing_Click);
        this.btnReturns.Click += new System.EventHandler(this.btnReturns_Click);
        this.btnOverdue.Click += new System.EventHandler(this.btnOverdue_Click);

        // 
        // panelHeader
        // 
        this.panelHeader.BackColor = Color.FromArgb(248, 249, 250);
        this.panelHeader.Dock = DockStyle.Top;
        this.panelHeader.Height = 60;
        this.panelHeader.Controls.Add(lblHeaderTitle);

        // lblHeaderTitle
        this.lblHeaderTitle.Text = "Library Management System";
        this.lblHeaderTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
        this.lblHeaderTitle.ForeColor = Color.FromArgb(33, 37, 41);
        this.lblHeaderTitle.Location = new Point(20, 15);
        this.lblHeaderTitle.Size = new Size(500, 30);

        // 
        // tabControlMain
        // 
        this.tabControlMain.Dock = DockStyle.Fill;
        this.tabControlMain.Appearance = TabAppearance.FlatButtons;
        this.tabControlMain.ItemSize = new Size(0, 1);
        this.tabControlMain.SizeMode = TabSizeMode.Fixed;
        this.tabControlMain.Controls.Add(tabDashboard);
        this.tabControlMain.Controls.Add(tabBooks);
        this.tabControlMain.Controls.Add(tabBorrowers);
        this.tabControlMain.Controls.Add(tabBorrowing);
        this.tabControlMain.Controls.Add(tabReturns);
        this.tabControlMain.Controls.Add(tabOverdue);

        // ==========================================
        // TAB 1: Dashboard Layout
        // ==========================================
        tabDashboard.BackColor = Color.FromArgb(240, 242, 245);
        
        int cardWidth = 160, cardHeight = 100, cardGap = 15, cardX = 20, cardY = 20;

        CreateMetricCard(ref panelCardTotalBooks, ref lblValTotalBooks, ref lblTitleTotalBooks, "Total Books", Color.FromArgb(13, 110, 253), cardX, cardY, cardWidth, cardHeight);
        CreateMetricCard(ref panelCardAvailBooks, ref lblValAvailBooks, ref lblTitleAvailBooks, "Available", Color.FromArgb(25, 135, 84), cardX + (cardWidth + cardGap), cardY, cardWidth, cardHeight);
        CreateMetricCard(ref panelCardActiveBorrowings, ref lblValActiveBorrowings, ref lblTitleActiveBorrowings, "Borrowed", Color.FromArgb(255, 193, 7), cardX + (cardWidth + cardGap) * 2, cardY, cardWidth, cardHeight);
        CreateMetricCard(ref panelCardOverdueBooks, ref lblValOverdueBooks, ref lblTitleOverdueBooks, "Overdue", Color.FromArgb(220, 53, 69), cardX + (cardWidth + cardGap) * 3, cardY, cardWidth, cardHeight);
        CreateMetricCard(ref panelCardRegisteredBorrowers, ref lblValRegisteredBorrowers, ref lblTitleRegisteredBorrowers, "Borrowers", Color.FromArgb(111, 66, 193), cardX + (cardWidth + cardGap) * 4, cardY, cardWidth, cardHeight);

        // lblOverdueWarning
        lblOverdueWarning = new Label();
        lblOverdueWarning.Text = "";
        lblOverdueWarning.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        lblOverdueWarning.ForeColor = Color.Red;
        lblOverdueWarning.Location = new Point(20, 140);
        lblOverdueWarning.Size = new Size(500, 25);
        lblOverdueWarning.Visible = false;

        // Overdue list section title
        lblOverdueSectionTitle = new Label();
        lblOverdueSectionTitle.Text = "Overdue Loans Alert List:";
        lblOverdueSectionTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        lblOverdueSectionTitle.ForeColor = Color.FromArgb(33, 37, 41);
        lblOverdueSectionTitle.Location = new Point(20, 175);
        lblOverdueSectionTitle.Size = new Size(300, 25);

        // dgvDashboardOverdue
        dgvDashboardOverdue = new DataGridView();
        dgvDashboardOverdue.Location = new Point(20, 210);
        dgvDashboardOverdue.Size = new Size(840, 320);
        dgvDashboardOverdue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        dgvDashboardOverdue.AllowUserToAddRows = false;
        dgvDashboardOverdue.AllowUserToDeleteRows = false;
        dgvDashboardOverdue.ReadOnly = true;
        dgvDashboardOverdue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvDashboardOverdue.BackgroundColor = Color.White;
        dgvDashboardOverdue.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        // btnRefreshDashboard
        btnRefreshDashboard = new Button();
        btnRefreshDashboard.Text = "Refresh Dashboard";
        btnRefreshDashboard.Location = new Point(20, 545);
        btnRefreshDashboard.Size = new Size(150, 35);
        btnRefreshDashboard.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnRefreshDashboard.Click += new System.EventHandler(this.btnRefreshDashboard_Click);
        btnRefreshDashboard.FlatStyle = FlatStyle.Flat;
        btnRefreshDashboard.BackColor = Color.FromArgb(33, 37, 41);
        btnRefreshDashboard.ForeColor = Color.White;

        tabDashboard.Controls.Add(panelCardTotalBooks);
        tabDashboard.Controls.Add(panelCardAvailBooks);
        tabDashboard.Controls.Add(panelCardActiveBorrowings);
        tabDashboard.Controls.Add(panelCardOverdueBooks);
        tabDashboard.Controls.Add(panelCardRegisteredBorrowers);
        tabDashboard.Controls.Add(lblOverdueWarning);
        tabDashboard.Controls.Add(lblOverdueSectionTitle);
        tabDashboard.Controls.Add(dgvDashboardOverdue);
        tabDashboard.Controls.Add(btnRefreshDashboard);

        // ==========================================
        // TAB 2: Books Layout
        // ==========================================
        tabBooks.BackColor = Color.White;

        // Search Labels & Textboxes
        lblSearchTitle = new Label { Text = "Title:", Location = new Point(20, 20), AutoSize = true };
        txtSearchTitle = new TextBox { Location = new Point(70, 17), Size = new Size(130, 23) };

        lblSearchAuthor = new Label { Text = "Author:", Location = new Point(220, 20), AutoSize = true };
        txtSearchAuthor = new TextBox { Location = new Point(280, 17), Size = new Size(130, 23) };

        lblSearchGenre = new Label { Text = "Genre:", Location = new Point(430, 20), AutoSize = true };
        txtSearchGenre = new TextBox { Location = new Point(480, 17), Size = new Size(130, 23) };

        btnSearchBooks = new Button { Text = "Search", Location = new Point(630, 15), Size = new Size(80, 27), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(13, 110, 253), ForeColor = Color.White };
        btnClearSearch = new Button { Text = "Clear", Location = new Point(720, 15), Size = new Size(80, 27), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(108, 117, 125), ForeColor = Color.White };

        btnSearchBooks.Click += new System.EventHandler(this.btnSearchBooks_Click);
        btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);

        // dgvBooks
        dgvBooks = new DataGridView();
        dgvBooks.Location = new Point(20, 60);
        dgvBooks.Size = new Size(840, 450);
        dgvBooks.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        dgvBooks.AllowUserToAddRows = false;
        dgvBooks.AllowUserToDeleteRows = false;
        dgvBooks.ReadOnly = true;
        dgvBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvBooks.BackgroundColor = Color.White;
        dgvBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        // Action Buttons
        btnAddBook = new Button { Text = "Add New Book", Location = new Point(20, 525), Size = new Size(120, 35), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(25, 135, 84), ForeColor = Color.White };
        btnEditBook = new Button { Text = "Edit Selected", Location = new Point(150, 525), Size = new Size(120, 35), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(255, 193, 7), ForeColor = Color.Black };
        btnDeleteBook = new Button { Text = "Delete Selected", Location = new Point(280, 525), Size = new Size(120, 35), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White };
        btnRefreshBooks = new Button { Text = "Refresh", Location = new Point(410, 525), Size = new Size(100, 35), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(33, 37, 41), ForeColor = Color.White };

        btnAddBook.Click += new System.EventHandler(this.btnAddBook_Click);
        btnEditBook.Click += new System.EventHandler(this.btnEditBook_Click);
        btnDeleteBook.Click += new System.EventHandler(this.btnDeleteBook_Click);
        btnRefreshBooks.Click += new System.EventHandler(this.btnRefreshBooks_Click);

        tabBooks.Controls.AddRange(new Control[] { lblSearchTitle, txtSearchTitle, lblSearchAuthor, txtSearchAuthor, lblSearchGenre, txtSearchGenre, btnSearchBooks, btnClearSearch, dgvBooks, btnAddBook, btnEditBook, btnDeleteBook, btnRefreshBooks });

        // ==========================================
        // TAB 3: Borrowers Layout
        // ==========================================
        tabBorrowers.BackColor = Color.White;

        dgvBorrowers = new DataGridView();
        dgvBorrowers.Location = new Point(20, 20);
        dgvBorrowers.Size = new Size(840, 480);
        dgvBorrowers.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        dgvBorrowers.AllowUserToAddRows = false;
        dgvBorrowers.AllowUserToDeleteRows = false;
        dgvBorrowers.ReadOnly = true;
        dgvBorrowers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvBorrowers.BackgroundColor = Color.White;
        dgvBorrowers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        btnRegisterBorrower = new Button { Text = "Register Borrower", Location = new Point(20, 515), Size = new Size(150, 35), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(25, 135, 84), ForeColor = Color.White };
        btnEditBorrower = new Button { Text = "Edit Selected", Location = new Point(180, 515), Size = new Size(120, 35), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(255, 193, 7), ForeColor = Color.Black };
        btnDeleteBorrower = new Button { Text = "Delete Selected", Location = new Point(310, 515), Size = new Size(120, 35), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White };
        btnViewHistory = new Button { Text = "Borrowing History", Location = new Point(440, 515), Size = new Size(150, 35), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(13, 110, 253), ForeColor = Color.White };
        btnRefreshBorrowers = new Button { Text = "Refresh", Location = new Point(600, 515), Size = new Size(100, 35), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(33, 37, 41), ForeColor = Color.White };

        btnRegisterBorrower.Click += new System.EventHandler(this.btnRegisterBorrower_Click);
        btnEditBorrower.Click += new System.EventHandler(this.btnEditBorrower_Click);
        btnDeleteBorrower.Click += new System.EventHandler(this.btnDeleteBorrower_Click);
        btnViewHistory.Click += new System.EventHandler(this.btnViewHistory_Click);
        btnRefreshBorrowers.Click += new System.EventHandler(this.btnRefreshBorrowers_Click);

        tabBorrowers.Controls.AddRange(new Control[] { dgvBorrowers, btnRegisterBorrower, btnEditBorrower, btnDeleteBorrower, btnViewHistory, btnRefreshBorrowers });

        // ==========================================
        // TAB 4: Lend Book (Borrowing)
        // ==========================================
        tabBorrowing.BackColor = Color.White;

        int formX = 40, formY = 40, fGap = 60, fInputX = 180, fWidth = 350;

        lblLendBorrower = new Label { Text = "Select Borrower:", Location = new Point(formX, formY), AutoSize = true, Font = new Font("Segoe UI", 11) };
        cmbLendBorrower = new ComboBox { Location = new Point(fInputX, formY), Size = new Size(fWidth, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11) };

        lblLendBook = new Label { Text = "Select Book:", Location = new Point(formX, formY + fGap), AutoSize = true, Font = new Font("Segoe UI", 11) };
        cmbLendBook = new ComboBox { Location = new Point(fInputX, formY + fGap), Size = new Size(fWidth, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 11) };

        lblLendBorrowDate = new Label { Text = "Borrow Date:", Location = new Point(formX, formY + fGap * 2), AutoSize = true, Font = new Font("Segoe UI", 11) };
        dtpLendBorrowDate = new DateTimePicker { Location = new Point(fInputX, formY + fGap * 2), Size = new Size(200, 25), Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 11) };

        lblLendDueDate = new Label { Text = "Due Date:", Location = new Point(formX, formY + fGap * 3), AutoSize = true, Font = new Font("Segoe UI", 11) };
        dtpLendDueDate = new DateTimePicker { Location = new Point(fInputX, formY + fGap * 3), Size = new Size(200, 25), Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 11) };

        btnLendBook = new Button { Text = "Lend Book", Location = new Point(fInputX, formY + fGap * 4), Size = new Size(180, 40), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(25, 135, 84), ForeColor = Color.White, Font = new Font("Segoe UI", 11, FontStyle.Bold) };
        btnLendBook.Click += new System.EventHandler(this.btnLendBook_Click);

        tabBorrowing.Controls.AddRange(new Control[] { lblLendBorrower, cmbLendBorrower, lblLendBook, cmbLendBook, lblLendBorrowDate, dtpLendBorrowDate, lblLendDueDate, dtpLendDueDate, btnLendBook });

        // ==========================================
        // TAB 5: Returns
        // ==========================================
        tabReturns.BackColor = Color.White;

        dgvReturns = new DataGridView();
        dgvReturns.Location = new Point(20, 20);
        dgvReturns.Size = new Size(840, 480);
        dgvReturns.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        dgvReturns.AllowUserToAddRows = false;
        dgvReturns.AllowUserToDeleteRows = false;
        dgvReturns.ReadOnly = true;
        dgvReturns.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvReturns.BackgroundColor = Color.White;
        dgvReturns.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        btnReturnBook = new Button { Text = "Return Selected Book", Location = new Point(20, 515), Size = new Size(180, 35), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(13, 110, 253), ForeColor = Color.White };
        btnRefreshReturns = new Button { Text = "Refresh List", Location = new Point(210, 515), Size = new Size(120, 35), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(33, 37, 41), ForeColor = Color.White };

        btnReturnBook.Click += new System.EventHandler(this.btnReturnBook_Click);
        btnRefreshReturns.Click += new System.EventHandler(this.btnRefreshReturns_Click);

        tabReturns.Controls.AddRange(new Control[] { dgvReturns, btnReturnBook, btnRefreshReturns });

        // ==========================================
        // TAB 6: Overdue
        // ==========================================
        tabOverdue.BackColor = Color.White;

        dgvOverdue = new DataGridView();
        dgvOverdue.Location = new Point(20, 20);
        dgvOverdue.Size = new Size(840, 480);
        dgvOverdue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        dgvOverdue.AllowUserToAddRows = false;
        dgvOverdue.AllowUserToDeleteRows = false;
        dgvOverdue.ReadOnly = true;
        dgvOverdue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvOverdue.BackgroundColor = Color.White;
        dgvOverdue.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        btnReturnOverdueBook = new Button { Text = "Return Overdue Book", Location = new Point(20, 515), Size = new Size(180, 35), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White };
        btnRefreshOverdue = new Button { Text = "Refresh List", Location = new Point(210, 515), Size = new Size(120, 35), Anchor = AnchorStyles.Bottom | AnchorStyles.Left, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(33, 37, 41), ForeColor = Color.White };

        btnReturnOverdueBook.Click += new System.EventHandler(this.btnReturnOverdueBook_Click);
        btnRefreshOverdue.Click += new System.EventHandler(this.btnRefreshOverdue_Click);

        tabOverdue.Controls.AddRange(new Control[] { dgvOverdue, btnReturnOverdueBook, btnRefreshOverdue });

        // ==========================================
        // Main Form Configuration
        // ==========================================
        this.ClientSize = new Size(1120, 680);
        this.Controls.Add(tabControlMain);
        this.Controls.Add(panelHeader);
        this.Controls.Add(panelSidebar);
        
        this.Text = "Library Management System";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(950, 600);

        this.ResumeLayout(false);
    }

    private void SetupSidebarButton(ref Button btn, string text, int y)
    {
        btn = new Button();
        btn.Text = text;
        btn.Location = new Point(10, y);
        btn.Size = new Size(200, 45);
        btn.FlatStyle = FlatStyle.Flat;
        btn.ForeColor = Color.White;
        btn.FlatAppearance.BorderSize = 0;
        btn.Font = new Font("Segoe UI", 11, FontStyle.Regular);
        btn.TextAlign = ContentAlignment.MiddleLeft;
        btn.Padding = new Padding(15, 0, 0, 0);
    }

    private void CreateMetricCard(ref Panel panel, ref Label val, ref Label title, string titleText, Color valColor, int x, int y, int w, int h)
    {
        panel = new Panel();
        panel.Location = new Point(x, y);
        panel.Size = new Size(w, h);
        panel.BackColor = Color.White;
        panel.BorderStyle = BorderStyle.None;

        // Custom borders using panels is a nice visual trick!
        Panel borderPanel = new Panel();
        borderPanel.Dock = DockStyle.Top;
        borderPanel.Height = 4;
        borderPanel.BackColor = valColor;
        panel.Controls.Add(borderPanel);

        val = new Label();
        val.Text = "0";
        val.Font = new Font("Segoe UI", 20, FontStyle.Bold);
        val.ForeColor = valColor;
        val.Location = new Point(15, 15);
        val.Size = new Size(w - 30, 35);
        panel.Controls.Add(val);

        title = new Label();
        title.Text = titleText;
        title.Font = new Font("Segoe UI", 10, FontStyle.Regular);
        title.ForeColor = Color.Gray;
        title.Location = new Point(15, 55);
        title.Size = new Size(w - 30, 25);
        panel.Controls.Add(title);
    }
}
