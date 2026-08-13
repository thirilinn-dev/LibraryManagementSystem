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

    // Main Header & Nav
    private Panel panelHeader;
    private Label lblHeaderTitle;
    private Label lblHeaderSubtitle;
    private Panel panelNav;
    private Button btnNavBooks;
    private Button btnNavBorrowers;
    private Button btnNavBorrowings;
    private Button btnNavOverdue;

    // Tab Control
    private TabControl tabControlMain;
    
    // Tab 1: Books
    private TabPage tabBooks;
    private Panel panelBooksLeft;
    private Label lblBookSearchHeader;
    private TextBox txtBookSearch;
    private Button btnBookSearch;
    private Button btnBookViewAll;
    private Label lblBookDetailsHeader;
    private Label lblBookTitle;
    private TextBox txtBookTitle;
    private Label lblBookAuthor;
    private TextBox txtBookAuthor;
    private Label lblBookGenre;
    private TextBox txtBookGenre;
    private Label lblBookLanguage;
    private TextBox txtBookLanguage;
    private Label lblBookDescription;
    private TextBox txtBookDescription;
    private Label lblBookTotalCopies;
    private NumericUpDown numBookTotalCopies;
    private Button btnBookCreate;
    private Button btnBookUpdate;
    private Button btnBookDelete;
    private Button btnBookClear;
    private DataGridView dgvBooks;

    // Tab 2: Borrowers
    private TabPage tabBorrowers;
    private Panel panelBorrowersLeft;
    private Label lblBorrowerSearchHeader;
    private TextBox txtBorrowerSearch;
    private Button btnBorrowerSearch;
    private Button btnBorrowerViewAll;
    private Label lblBorrowerDetailsHeader;
    private Label lblBorrowerName;
    private TextBox txtBorrowerName;
    private Label lblBorrowerPhone;
    private TextBox txtBorrowerPhone;
    private Label lblBorrowerEmail;
    private TextBox txtBorrowerEmail;
    private Button btnBorrowerCreate;
    private Button btnBorrowerUpdate;
    private Button btnBorrowerDelete;
    private Button btnBorrowerClear;
    private Button btnBorrowerHistory;
    private DataGridView dgvBorrowers;

    // Tab 3: Borrowings
    private TabPage tabBorrowing;
    private Panel panelBorrowingLeft;
    private Label lblLendHeader;
    private Label lblLendBorrower;
    private ComboBox cmbLendBorrower;
    private Label lblLendBook;
    private ComboBox cmbLendBook;
    private Label lblLendBorrowDate;
    private DateTimePicker dtpLendBorrowDate;
    private Label lblLendDueDate;
    private DateTimePicker dtpLendDueDate;
    private Button btnLendBook;
    private DataGridView dgvReturns;
    private Panel panelReturnsTop;
    private Label lblActiveBorrowingsHeader;
    private Button btnReturnBook;

    // Tab 4: Overdue
    private TabPage tabOverdue;
    private DataGridView dgvOverdue;
    private Panel panelOverdueTop;
    private Label lblOverdueHeader;
    private Button btnReturnOverdueBook;

    private void InitializeComponent()
    {
        this.panelHeader = new Panel();
        this.lblHeaderTitle = new Label();
        this.lblHeaderSubtitle = new Label();
        this.panelNav = new Panel();
        this.btnNavBooks = new Button();
        this.btnNavBorrowers = new Button();
        this.btnNavBorrowings = new Button();
        this.btnNavOverdue = new Button();

        this.tabControlMain = new TabControl();
        this.tabBooks = new TabPage();
        this.tabBorrowers = new TabPage();
        this.tabBorrowing = new TabPage();
        this.tabOverdue = new TabPage();

        // Color Palette definitions
        Color darkBrown = Color.FromArgb(93, 64, 55);      // #5D4037
        Color mediumBrown = Color.FromArgb(121, 85, 72);    // #795548
        Color accentBrown = Color.FromArgb(141, 110, 99);   // #8D6E63
        Color lightBeige = Color.FromArgb(239, 235, 233);    // #EFEBE9
        Color contentWhite = Color.White;
        Color textPrimary = Color.FromArgb(62, 39, 35);     // #3E2723
        Color textHeader = Color.White;
        Color borderBrown = Color.FromArgb(188, 170, 164);   // #BCAAA4
        Color selectBeige = Color.FromArgb(215, 204, 200);   // #D7CCC8

        this.SuspendLayout();

        // 
        // panelHeader
        // 
        this.panelHeader.BackColor = darkBrown;
        this.panelHeader.Dock = DockStyle.Top;
        this.panelHeader.Height = 75;
        this.panelHeader.Controls.Add(this.lblHeaderTitle);
        this.panelHeader.Controls.Add(this.lblHeaderSubtitle);

        // lblHeaderTitle
        this.lblHeaderTitle.Text = "Library Management System";
        this.lblHeaderTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
        this.lblHeaderTitle.ForeColor = textHeader;
        this.lblHeaderTitle.Location = new Point(20, 12);
        this.lblHeaderTitle.Size = new Size(500, 30);

        // lblHeaderSubtitle
        this.lblHeaderSubtitle.Text = "Manage Books, Borrowers and Borrowings";
        this.lblHeaderSubtitle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
        this.lblHeaderSubtitle.ForeColor = selectBeige;
        this.lblHeaderSubtitle.Location = new Point(20, 42);
        this.lblHeaderSubtitle.Size = new Size(500, 20);

        // 
        // panelNav
        // 
        this.panelNav.BackColor = mediumBrown;
        this.panelNav.Dock = DockStyle.Top;
        this.panelNav.Height = 45;
        this.panelNav.Controls.Add(this.btnNavBooks);
        this.panelNav.Controls.Add(this.btnNavBorrowers);
        this.panelNav.Controls.Add(this.btnNavBorrowings);
        this.panelNav.Controls.Add(this.btnNavOverdue);

        // Nav Buttons
        SetupNavButton(this.btnNavBooks, "📚 Books", 0, accentBrown);
        SetupNavButton(this.btnNavBorrowers, "👤 Borrowers", 180, mediumBrown);
        SetupNavButton(this.btnNavBorrowings, "📖 Borrowings", 360, mediumBrown);
        SetupNavButton(this.btnNavOverdue, "⚠️ Overdue List", 540, mediumBrown);

        // 
        // tabControlMain
        // 
        this.tabControlMain.Dock = DockStyle.Fill;
        this.tabControlMain.Appearance = TabAppearance.FlatButtons;
        this.tabControlMain.ItemSize = new Size(0, 1);
        this.tabControlMain.SizeMode = TabSizeMode.Fixed;
        this.tabControlMain.Controls.Add(this.tabBooks);
        this.tabControlMain.Controls.Add(this.tabBorrowers);
        this.tabControlMain.Controls.Add(this.tabBorrowing);
        this.tabControlMain.Controls.Add(this.tabOverdue);

        // ==========================================
        // TAB 1: Books Layout
        // ==========================================
        this.tabBooks.BackColor = contentWhite;
        
        // Left details panel
        this.panelBooksLeft = new Panel();
        this.panelBooksLeft.Dock = DockStyle.Left;
        this.panelBooksLeft.Width = 350;
        this.panelBooksLeft.BackColor = lightBeige;
        this.panelBooksLeft.Padding = new Padding(15);

        // Book Search Section
        this.lblBookSearchHeader = new Label { Text = "SEARCH BOOKS", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = textPrimary, Location = new Point(15, 15), Size = new Size(320, 20) };
        this.txtBookSearch = new TextBox { Location = new Point(15, 40), Size = new Size(320, 25), Font = new Font("Segoe UI", 10) };
        this.btnBookSearch = new Button { Text = "Search", Location = new Point(15, 75), Size = new Size(150, 30), FlatStyle = FlatStyle.Flat, BackColor = accentBrown, ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        this.btnBookViewAll = new Button { Text = "View All", Location = new Point(185, 75), Size = new Size(150, 30), FlatStyle = FlatStyle.Flat, BackColor = accentBrown, ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

        this.btnBookSearch.FlatAppearance.BorderSize = 0;
        this.btnBookViewAll.FlatAppearance.BorderSize = 0;

        // Divider
        Label bookDivider = new Label { BorderStyle = BorderStyle.Fixed3D, Height = 2, Width = 320, Location = new Point(15, 120) };

        // Book Details Header
        this.lblBookDetailsHeader = new Label { Text = "BOOK DETAILS", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = textPrimary, Location = new Point(15, 135), Size = new Size(320, 20) };

        // Fields
        int bY = 165, bGap = 50;
        this.lblBookTitle = new Label { Text = "Title:", ForeColor = textPrimary, Location = new Point(15, bY), AutoSize = true };
        this.txtBookTitle = new TextBox { Location = new Point(15, bY + 18), Size = new Size(320, 25), Font = new Font("Segoe UI", 10) };

        this.lblBookAuthor = new Label { Text = "Author:", ForeColor = textPrimary, Location = new Point(15, bY + bGap), AutoSize = true };
        this.txtBookAuthor = new TextBox { Location = new Point(15, bY + bGap + 18), Size = new Size(320, 25), Font = new Font("Segoe UI", 10) };

        this.lblBookGenre = new Label { Text = "Genre:", ForeColor = textPrimary, Location = new Point(15, bY + bGap * 2), AutoSize = true };
        this.txtBookGenre = new TextBox { Location = new Point(15, bY + bGap * 2 + 18), Size = new Size(320, 25), Font = new Font("Segoe UI", 10) };

        this.lblBookLanguage = new Label { Text = "Language:", ForeColor = textPrimary, Location = new Point(15, bY + bGap * 3), AutoSize = true };
        this.txtBookLanguage = new TextBox { Location = new Point(15, bY + bGap * 3 + 18), Size = new Size(320, 25), Font = new Font("Segoe UI", 10) };

        this.lblBookDescription = new Label { Text = "Description:", ForeColor = textPrimary, Location = new Point(15, bY + bGap * 4), AutoSize = true };
        this.txtBookDescription = new TextBox { Location = new Point(15, bY + bGap * 4 + 18), Size = new Size(320, 50), Multiline = true, Font = new Font("Segoe UI", 9) };

        this.lblBookTotalCopies = new Label { Text = "Total Copies:", ForeColor = textPrimary, Location = new Point(15, bY + bGap * 5 + 25), AutoSize = true };
        this.numBookTotalCopies = new NumericUpDown { Location = new Point(120, bY + bGap * 5 + 23), Size = new Size(80, 25), Minimum = 0, Maximum = 1000, Font = new Font("Segoe UI", 10) };

        // CRUD Action Buttons
        int btnY = bY + bGap * 6 + 10;
        this.btnBookCreate = new Button { Text = "Create", Location = new Point(15, btnY), Size = new Size(150, 32), FlatStyle = FlatStyle.Flat, BackColor = accentBrown, ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        this.btnBookUpdate = new Button { Text = "Update", Location = new Point(185, btnY), Size = new Size(150, 32), FlatStyle = FlatStyle.Flat, BackColor = accentBrown, ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        this.btnBookDelete = new Button { Text = "Delete", Location = new Point(15, btnY + 40), Size = new Size(150, 32), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(176, 58, 46), ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        this.btnBookClear = new Button { Text = "Clear Fields", Location = new Point(185, btnY + 40), Size = new Size(150, 32), FlatStyle = FlatStyle.Flat, BackColor = accentBrown, ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

        this.btnBookCreate.FlatAppearance.BorderSize = 0;
        this.btnBookUpdate.FlatAppearance.BorderSize = 0;
        this.btnBookDelete.FlatAppearance.BorderSize = 0;
        this.btnBookClear.FlatAppearance.BorderSize = 0;

        this.panelBooksLeft.Controls.AddRange(new Control[] {
            this.lblBookSearchHeader, this.txtBookSearch, this.btnBookSearch, this.btnBookViewAll,
            bookDivider,
            this.lblBookDetailsHeader,
            this.lblBookTitle, this.txtBookTitle,
            this.lblBookAuthor, this.txtBookAuthor,
            this.lblBookGenre, this.txtBookGenre,
            this.lblBookLanguage, this.txtBookLanguage,
            this.lblBookDescription, this.txtBookDescription,
            this.lblBookTotalCopies, this.numBookTotalCopies,
            this.btnBookCreate, this.btnBookUpdate, this.btnBookDelete, this.btnBookClear
        });

        // Books DataGridView
        this.dgvBooks = new DataGridView();
        this.dgvBooks.Dock = DockStyle.Fill;
        this.dgvBooks.BackgroundColor = contentWhite;
        this.dgvBooks.BorderStyle = BorderStyle.None;
        this.dgvBooks.GridColor = borderBrown;
        this.dgvBooks.AllowUserToAddRows = false;
        this.dgvBooks.AllowUserToDeleteRows = false;
        this.dgvBooks.ReadOnly = true;
        this.dgvBooks.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvBooks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        SetupDataGridViewTheme(this.dgvBooks, selectBeige, textPrimary, mediumBrown, borderBrown);

        this.tabBooks.Controls.Add(this.dgvBooks);
        this.tabBooks.Controls.Add(this.panelBooksLeft);

        // ==========================================
        // TAB 2: Borrowers Layout
        // ==========================================
        this.tabBorrowers.BackColor = contentWhite;

        // Left details panel
        this.panelBorrowersLeft = new Panel();
        this.panelBorrowersLeft.Dock = DockStyle.Left;
        this.panelBorrowersLeft.Width = 350;
        this.panelBorrowersLeft.BackColor = lightBeige;
        this.panelBorrowersLeft.Padding = new Padding(15);

        // Search Section
        this.lblBorrowerSearchHeader = new Label { Text = "SEARCH BORROWERS", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = textPrimary, Location = new Point(15, 15), Size = new Size(320, 20) };
        this.txtBorrowerSearch = new TextBox { Location = new Point(15, 40), Size = new Size(320, 25), Font = new Font("Segoe UI", 10) };
        this.btnBorrowerSearch = new Button { Text = "Search", Location = new Point(15, 75), Size = new Size(150, 30), FlatStyle = FlatStyle.Flat, BackColor = accentBrown, ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        this.btnBorrowerViewAll = new Button { Text = "View All", Location = new Point(185, 75), Size = new Size(150, 30), FlatStyle = FlatStyle.Flat, BackColor = accentBrown, ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

        this.btnBorrowerSearch.FlatAppearance.BorderSize = 0;
        this.btnBorrowerViewAll.FlatAppearance.BorderSize = 0;

        // Divider
        Label borrowerDivider = new Label { BorderStyle = BorderStyle.Fixed3D, Height = 2, Width = 320, Location = new Point(15, 120) };

        // Details Header
        this.lblBorrowerDetailsHeader = new Label { Text = "BORROWER DETAILS", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = textPrimary, Location = new Point(15, 135), Size = new Size(320, 20) };

        // Fields
        int borY = 165, borGap = 55;
        this.lblBorrowerName = new Label { Text = "Name:", ForeColor = textPrimary, Location = new Point(15, borY), AutoSize = true };
        this.txtBorrowerName = new TextBox { Location = new Point(15, borY + 18), Size = new Size(320, 25), Font = new Font("Segoe UI", 10) };

        this.lblBorrowerPhone = new Label { Text = "Phone:", ForeColor = textPrimary, Location = new Point(15, borY + borGap), AutoSize = true };
        this.txtBorrowerPhone = new TextBox { Location = new Point(15, borY + borGap + 18), Size = new Size(320, 25), Font = new Font("Segoe UI", 10) };

        this.lblBorrowerEmail = new Label { Text = "Email:", ForeColor = textPrimary, Location = new Point(15, borY + borGap * 2), AutoSize = true };
        this.txtBorrowerEmail = new TextBox { Location = new Point(15, borY + borGap * 2 + 18), Size = new Size(320, 25), Font = new Font("Segoe UI", 10) };

        // Action Buttons
        int borBtnY = borY + borGap * 3 + 20;
        this.btnBorrowerCreate = new Button { Text = "Create", Location = new Point(15, borBtnY), Size = new Size(150, 32), FlatStyle = FlatStyle.Flat, BackColor = accentBrown, ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        this.btnBorrowerUpdate = new Button { Text = "Update", Location = new Point(185, borBtnY), Size = new Size(150, 32), FlatStyle = FlatStyle.Flat, BackColor = accentBrown, ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        this.btnBorrowerDelete = new Button { Text = "Delete", Location = new Point(15, borBtnY + 40), Size = new Size(150, 32), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(176, 58, 46), ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        this.btnBorrowerClear = new Button { Text = "Clear Fields", Location = new Point(185, borBtnY + 40), Size = new Size(150, 32), FlatStyle = FlatStyle.Flat, BackColor = accentBrown, ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        this.btnBorrowerHistory = new Button { Text = "⌛ View Borrowing History", Location = new Point(15, borBtnY + 85), Size = new Size(320, 35), FlatStyle = FlatStyle.Flat, BackColor = mediumBrown, ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

        this.btnBorrowerCreate.FlatAppearance.BorderSize = 0;
        this.btnBorrowerUpdate.FlatAppearance.BorderSize = 0;
        this.btnBorrowerDelete.FlatAppearance.BorderSize = 0;
        this.btnBorrowerClear.FlatAppearance.BorderSize = 0;
        this.btnBorrowerHistory.FlatAppearance.BorderSize = 0;

        this.panelBorrowersLeft.Controls.AddRange(new Control[] {
            this.lblBorrowerSearchHeader, this.txtBorrowerSearch, this.btnBorrowerSearch, this.btnBorrowerViewAll,
            borrowerDivider,
            this.lblBorrowerDetailsHeader,
            this.lblBorrowerName, this.txtBorrowerName,
            this.lblBorrowerPhone, this.txtBorrowerPhone,
            this.lblBorrowerEmail, this.txtBorrowerEmail,
            this.btnBorrowerCreate, this.btnBorrowerUpdate, this.btnBorrowerDelete, this.btnBorrowerClear, this.btnBorrowerHistory
        });

        // Borrowers DataGridView
        this.dgvBorrowers = new DataGridView();
        this.dgvBorrowers.Dock = DockStyle.Fill;
        this.dgvBorrowers.BackgroundColor = contentWhite;
        this.dgvBorrowers.BorderStyle = BorderStyle.None;
        this.dgvBorrowers.GridColor = borderBrown;
        this.dgvBorrowers.AllowUserToAddRows = false;
        this.dgvBorrowers.AllowUserToDeleteRows = false;
        this.dgvBorrowers.ReadOnly = true;
        this.dgvBorrowers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvBorrowers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        SetupDataGridViewTheme(this.dgvBorrowers, selectBeige, textPrimary, mediumBrown, borderBrown);

        this.tabBorrowers.Controls.Add(this.dgvBorrowers);
        this.tabBorrowers.Controls.Add(this.panelBorrowersLeft);

        // ==========================================
        // TAB 3: Borrowings Layout
        // ==========================================
        this.tabBorrowing.BackColor = contentWhite;

        // Left details panel
        this.panelBorrowingLeft = new Panel();
        this.panelBorrowingLeft.Dock = DockStyle.Left;
        this.panelBorrowingLeft.Width = 350;
        this.panelBorrowingLeft.BackColor = lightBeige;
        this.panelBorrowingLeft.Padding = new Padding(15);

        this.lblLendHeader = new Label { Text = "LEND A BOOK", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = textPrimary, Location = new Point(15, 15), Size = new Size(320, 25) };

        int lendY = 55, lendGap = 65;
        this.lblLendBorrower = new Label { Text = "Select Borrower:", ForeColor = textPrimary, Location = new Point(15, lendY), AutoSize = true };
        this.cmbLendBorrower = new ComboBox { Location = new Point(15, lendY + 18), Size = new Size(320, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };

        this.lblLendBook = new Label { Text = "Select Book:", ForeColor = textPrimary, Location = new Point(15, lendY + lendGap), AutoSize = true };
        this.cmbLendBook = new ComboBox { Location = new Point(15, lendY + lendGap + 18), Size = new Size(320, 25), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };

        this.lblLendBorrowDate = new Label { Text = "Borrow Date:", ForeColor = textPrimary, Location = new Point(15, lendY + lendGap * 2), AutoSize = true };
        this.dtpLendBorrowDate = new DateTimePicker { Location = new Point(15, lendY + lendGap * 2 + 18), Size = new Size(320, 25), Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10) };

        this.lblLendDueDate = new Label { Text = "Due Date:", ForeColor = textPrimary, Location = new Point(15, lendY + lendGap * 3), AutoSize = true };
        this.dtpLendDueDate = new DateTimePicker { Location = new Point(15, lendY + lendGap * 3 + 18), Size = new Size(320, 25), Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10) };

        this.btnLendBook = new Button { Text = "Lend Book", Location = new Point(15, lendY + lendGap * 4 + 10), Size = new Size(320, 40), FlatStyle = FlatStyle.Flat, BackColor = accentBrown, ForeColor = textHeader, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        this.btnLendBook.FlatAppearance.BorderSize = 0;

        this.panelBorrowingLeft.Controls.AddRange(new Control[] {
            this.lblLendHeader,
            this.lblLendBorrower, this.cmbLendBorrower,
            this.lblLendBook, this.cmbLendBook,
            this.lblLendBorrowDate, this.dtpLendBorrowDate,
            this.lblLendDueDate, this.dtpLendDueDate,
            this.btnLendBook
        });

        // Right Content: Active Borrowings & Return Book Action
        this.panelReturnsTop = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = contentWhite };
        this.lblActiveBorrowingsHeader = new Label { Text = "ACTIVE BORROWINGS (NOT RETURNED)", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = textPrimary, Location = new Point(15, 15), Size = new Size(400, 25) };
        this.btnReturnBook = new Button { Text = "↩ Return Selected Book", Location = new Point(480, 10), Size = new Size(180, 32), FlatStyle = FlatStyle.Flat, BackColor = accentBrown, ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        this.btnReturnBook.FlatAppearance.BorderSize = 0;
        this.panelReturnsTop.Controls.Add(this.lblActiveBorrowingsHeader);
        this.panelReturnsTop.Controls.Add(this.btnReturnBook);

        this.dgvReturns = new DataGridView();
        this.dgvReturns.Dock = DockStyle.Fill;
        this.dgvReturns.BackgroundColor = contentWhite;
        this.dgvReturns.BorderStyle = BorderStyle.None;
        this.dgvReturns.GridColor = borderBrown;
        this.dgvReturns.AllowUserToAddRows = false;
        this.dgvReturns.AllowUserToDeleteRows = false;
        this.dgvReturns.ReadOnly = true;
        this.dgvReturns.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvReturns.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        SetupDataGridViewTheme(this.dgvReturns, selectBeige, textPrimary, mediumBrown, borderBrown);

        Panel panelBorrowingRight = new Panel { Dock = DockStyle.Fill };
        panelBorrowingRight.Controls.Add(this.dgvReturns);
        panelBorrowingRight.Controls.Add(this.panelReturnsTop);

        this.tabBorrowing.Controls.Add(panelBorrowingRight);
        this.tabBorrowing.Controls.Add(this.panelBorrowingLeft);

        // ==========================================
        // TAB 4: Overdue Layout
        // ==========================================
        this.tabOverdue.BackColor = contentWhite;

        this.panelOverdueTop = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = contentWhite };
        this.lblOverdueHeader = new Label { Text = "OVERDUE LOANS ALERT", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.FromArgb(176, 58, 46), Location = new Point(15, 15), Size = new Size(300, 25) };
        this.btnReturnOverdueBook = new Button { Text = "↩ Return Overdue Book", Location = new Point(480, 10), Size = new Size(180, 32), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(176, 58, 46), ForeColor = textHeader, Font = new Font("Segoe UI", 9, FontStyle.Bold), Anchor = AnchorStyles.Top | AnchorStyles.Right };
        this.btnReturnOverdueBook.FlatAppearance.BorderSize = 0;
        this.panelOverdueTop.Controls.Add(this.lblOverdueHeader);
        this.panelOverdueTop.Controls.Add(this.btnReturnOverdueBook);

        this.dgvOverdue = new DataGridView();
        this.dgvOverdue.Dock = DockStyle.Fill;
        this.dgvOverdue.BackgroundColor = contentWhite;
        this.dgvOverdue.BorderStyle = BorderStyle.None;
        this.dgvOverdue.GridColor = borderBrown;
        this.dgvOverdue.AllowUserToAddRows = false;
        this.dgvOverdue.AllowUserToDeleteRows = false;
        this.dgvOverdue.ReadOnly = true;
        this.dgvOverdue.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvOverdue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        SetupDataGridViewTheme(this.dgvOverdue, selectBeige, textPrimary, mediumBrown, borderBrown);

        this.tabOverdue.Controls.Add(this.dgvOverdue);
        this.tabOverdue.Controls.Add(this.panelOverdueTop);

        // ==========================================
        // Event Subscriptions
        // ==========================================
        this.btnNavBooks.Click += new System.EventHandler(this.btnNavBooks_Click);
        this.btnNavBorrowers.Click += new System.EventHandler(this.btnNavBorrowers_Click);
        this.btnNavBorrowings.Click += new System.EventHandler(this.btnNavBorrowings_Click);
        this.btnNavOverdue.Click += new System.EventHandler(this.btnNavOverdue_Click);

        this.btnBookSearch.Click += new System.EventHandler(this.btnBookSearch_Click);
        this.btnBookViewAll.Click += new System.EventHandler(this.btnBookViewAll_Click);
        this.btnBookCreate.Click += new System.EventHandler(this.btnBookCreate_Click);
        this.btnBookUpdate.Click += new System.EventHandler(this.btnBookUpdate_Click);
        this.btnBookDelete.Click += new System.EventHandler(this.btnBookDelete_Click);
        this.btnBookClear.Click += new System.EventHandler(this.btnBookClear_Click);

        this.btnBorrowerSearch.Click += new System.EventHandler(this.btnBorrowerSearch_Click);
        this.btnBorrowerViewAll.Click += new System.EventHandler(this.btnBorrowerViewAll_Click);
        this.btnBorrowerCreate.Click += new System.EventHandler(this.btnBorrowerCreate_Click);
        this.btnBorrowerUpdate.Click += new System.EventHandler(this.btnBorrowerUpdate_Click);
        this.btnBorrowerDelete.Click += new System.EventHandler(this.btnBorrowerDelete_Click);
        this.btnBorrowerClear.Click += new System.EventHandler(this.btnBorrowerClear_Click);
        this.btnBorrowerHistory.Click += new System.EventHandler(this.btnBorrowerHistory_Click);

        this.btnLendBook.Click += new System.EventHandler(this.btnLendBook_Click);
        this.btnReturnBook.Click += new System.EventHandler(this.btnReturnBook_Click);
        this.btnReturnOverdueBook.Click += new System.EventHandler(this.btnReturnOverdueBook_Click);

        this.dgvBooks.SelectionChanged += new System.EventHandler(this.dgvBooks_SelectionChanged);
        this.dgvBorrowers.SelectionChanged += new System.EventHandler(this.dgvBorrowers_SelectionChanged);

        // ==========================================
        // Main Form Configuration
        // ==========================================
        this.ClientSize = new Size(1120, 680);
        this.Controls.Add(this.tabControlMain);
        this.Controls.Add(this.panelNav);
        this.Controls.Add(this.panelHeader);
        
        this.Text = "Library Management System";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(1000, 650);

        this.ResumeLayout(false);
    }

    private void SetupNavButton(Button btn, string text, int x, Color backColor)
    {
        btn.Text = text;
        btn.Location = new Point(x, 0);
        btn.Size = new Size(180, 45);
        btn.FlatStyle = FlatStyle.Flat;
        btn.ForeColor = Color.White;
        btn.FlatAppearance.BorderSize = 0;
        btn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        btn.BackColor = backColor;
        btn.TextAlign = ContentAlignment.MiddleCenter;
    }

    private void SetupDataGridViewTheme(DataGridView dgv, Color selectColor, Color textColor, Color headerColor, Color border)
    {
        dgv.EnableHeadersVisualStyles = false;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        dgv.ColumnHeadersHeight = 35;

        dgv.DefaultCellStyle.BackColor = Color.White;
        dgv.DefaultCellStyle.ForeColor = textColor;
        dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
        dgv.DefaultCellStyle.SelectionBackColor = selectColor;
        dgv.DefaultCellStyle.SelectionForeColor = textColor;

        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 246, 245);
        dgv.RowHeadersVisible = false;
        dgv.RowTemplate.Height = 30;
    }
}
