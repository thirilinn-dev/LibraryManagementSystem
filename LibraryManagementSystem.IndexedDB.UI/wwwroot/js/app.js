/**
 * Library Management System - UI Application & Synchronization Controller
 * Orchestrates UI state, IndexedDB cache, and Web API communication
 */

class LibraryAppController {
    constructor() {
        this.currentTab = 'books';
        this.selectedBookId = null;
        this.selectedBorrowerId = null;
        this.selectedBorrowingId = null;
    }

    async init() {
        this.bindEvents();
        
        // Initial database check
        try {
            await window.libraryDB.openDatabase();
        } catch (err) {
            this.showToast('Failed to initialize IndexedDB: ' + err.message, 'error');
        }

        // Check Web API connectivity
        await this.updateConnectionStatus();

        // Perform initial auto-sync if online, or load from IndexedDB
        if (window.libraryApi.isOnline) {
            await this.syncData(true); // silent sync
        } else {
            this.showToast('Operating in Offline / Local IndexedDB mode', 'info');
            await this.renderCurrentTab();
        }
    }

    async updateConnectionStatus() {
        const isConnected = await window.libraryApi.checkConnection();
        const badge = document.getElementById('connectionStatusBadge');
        if (badge) {
            if (isConnected) {
                badge.className = 'status-badge online';
                badge.innerHTML = '● Online (API Connected)';
            } else {
                badge.className = 'status-badge offline';
                badge.innerHTML = '○ Offline (Local IndexedDB Data)';
            }
        }
        return isConnected;
    }

    bindEvents() {
        // Tab switching
        document.querySelectorAll('.library-tab-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const targetTab = e.currentTarget.getAttribute('data-tab');
                this.switchTab(targetTab);
            });
        });

        // Sync Data Button (optional — only if present in DOM)
        const btnSync = document.getElementById('btnSyncData');
        if (btnSync) {
            btnSync.addEventListener('click', () => this.syncData(false));
        }

        // --- BOOKS EVENTS ---
        document.getElementById('btnSearchBooks')?.addEventListener('click', () => this.handleBookSearch());
        document.getElementById('btnViewAllBooks')?.addEventListener('click', () => this.handleBookViewAll());
        document.getElementById('btnCreateBook')?.addEventListener('click', () => this.handleCreateBook());
        document.getElementById('btnUpdateBook')?.addEventListener('click', () => this.handleUpdateBook());
        document.getElementById('btnDeleteBook')?.addEventListener('click', () => this.handleDeleteBook());
        document.getElementById('btnClearBookForm')?.addEventListener('click', () => this.clearBookForm());

        // --- BORROWERS EVENTS ---
        document.getElementById('btnSearchBorrowers')?.addEventListener('click', () => this.handleBorrowerSearch());
        document.getElementById('btnViewAllBorrowers')?.addEventListener('click', () => this.handleBorrowerViewAll());
        document.getElementById('btnCreateBorrower')?.addEventListener('click', () => this.handleCreateBorrower());
        document.getElementById('btnUpdateBorrower')?.addEventListener('click', () => this.handleUpdateBorrower());
        document.getElementById('btnDeleteBorrower')?.addEventListener('click', () => this.handleDeleteBorrower());
        document.getElementById('btnClearBorrowerForm')?.addEventListener('click', () => this.clearBorrowerForm());
        document.getElementById('btnViewHistory')?.addEventListener('click', () => this.handleViewHistory());

        // --- BORROWINGS EVENTS ---
        document.getElementById('btnLendBook')?.addEventListener('click', () => this.handleLendBook());
        document.getElementById('btnReturnBook')?.addEventListener('click', () => this.handleReturnBook());
        document.getElementById('btnClearBorrowingForm')?.addEventListener('click', () => this.clearBorrowingForm());

        // --- OVERDUE EVENTS ---
        document.getElementById('btnRefreshOverdue')?.addEventListener('click', () => this.renderOverdueTab());

        // Close Modal Event
        document.getElementById('btnCloseModal')?.addEventListener('click', () => this.closeModal());
    }

    switchTab(tabName) {
        this.currentTab = tabName;
        document.querySelectorAll('.library-tab-btn').forEach(btn => {
            btn.classList.toggle('active', btn.getAttribute('data-tab') === tabName);
        });

        document.querySelectorAll('.tab-pane').forEach(pane => {
            pane.style.display = pane.id === `pane-${tabName}` ? 'block' : 'none';
        });

        this.renderCurrentTab();
    }

    async renderCurrentTab() {
        switch (this.currentTab) {
            case 'books':
                await this.renderBooksTab();
                break;
            case 'borrowers':
                await this.renderBorrowersTab();
                break;
            case 'borrowings':
                await this.renderBorrowingsTab();
                break;
            case 'overdue':
                await this.renderOverdueTab();
                break;
        }
    }

    // ==========================================
    // SYNC DATA (API -> IndexedDB -> UI)
    // ==========================================
    async syncData(silent = false) {
        if (!silent) this.showLoading('Synchronizing with SQL Server Web API...');

        try {
            const isOnline = await this.updateConnectionStatus();
            if (!isOnline) {
                if (!silent) this.showToast('Web API is currently offline. Cannot sync.', 'error');
                return;
            }

            // 1. Fetch from Web API
            const [books, borrowers, borrowings] = await Promise.all([
                window.libraryApi.getBooks(),
                window.libraryApi.getBorrowers(),
                window.libraryApi.getBorrowings()
            ]);

            // 2. Store into IndexedDB
            await window.libraryDB.bulkSyncBooks(books || []);
            await window.libraryDB.bulkSyncBorrowers(borrowers || []);
            await window.libraryDB.bulkSyncBorrowings(borrowings || []);

            // 3. Refresh UI from IndexedDB
            await this.renderCurrentTab();

            if (!silent) this.showToast('Data synchronized successfully.', 'success');
        } catch (err) {
            console.error('Sync failed:', err);
            if (!silent) this.showToast(`Synchronization error: ${err.message}`, 'error');
        } finally {
            if (!silent) this.hideLoading();
        }
    }

    // ==========================================
    // 1. BOOKS TAB LOGIC
    // ==========================================
    async renderBooksTab(booksToDisplay = null) {
        const books = booksToDisplay || await window.libraryDB.getAllBooks();
        const tbody = document.getElementById('booksTableBody');
        if (!tbody) return;

        tbody.innerHTML = '';
        if (books.length === 0) {
            tbody.innerHTML = `<tr><td colspan="6" class="text-center text-muted p-4">No books found in IndexedDB storage.</td></tr>`;
            return;
        }

        books.forEach(book => {
            const tr = document.createElement('tr');
            if (this.selectedBookId === book.BookId) {
                tr.classList.add('selected');
            }
            tr.innerHTML = `
                <td><strong>${this.escapeHtml(book.Title)}</strong></td>
                <td>${this.escapeHtml(book.Author)}</td>
                <td><span class="badge bg-secondary">${this.escapeHtml(book.Genre)}</span></td>
                <td>${this.escapeHtml(book.Language || 'English')}</td>
                <td>${book.TotalBooks}</td>
                <td>
                    <span class="badge ${book.AvailableBooks > 0 ? 'bg-success' : 'bg-danger'}">
                        ${book.AvailableBooks} / ${book.TotalBooks}
                    </span>
                </td>
            `;
            tr.addEventListener('click', () => this.selectBookRow(tr, book));
            tbody.appendChild(tr);
        });
    }

    selectBookRow(rowElement, book) {
        document.querySelectorAll('#booksTableBody tr').forEach(r => r.classList.remove('selected'));
        rowElement.classList.add('selected');

        this.selectedBookId = book.BookId;
        document.getElementById('bookTitle').value = book.Title;
        document.getElementById('bookAuthor').value = book.Author;
        document.getElementById('bookGenre').value = book.Genre;
        document.getElementById('bookLanguage').value = book.Language || 'English';
        document.getElementById('bookDescription').value = book.Description || '';
        document.getElementById('bookTotalBooks').value = book.TotalBooks;
        document.getElementById('bookAvailableBooks').value = book.AvailableBooks;
    }

    clearBookForm() {
        this.selectedBookId = null;
        document.getElementById('bookTitle').value = '';
        document.getElementById('bookAuthor').value = '';
        document.getElementById('bookGenre').value = '';
        document.getElementById('bookLanguage').value = 'English';
        document.getElementById('bookDescription').value = '';
        document.getElementById('bookTotalBooks').value = '1';
        document.getElementById('bookAvailableBooks').value = '1';
        document.querySelectorAll('#booksTableBody tr').forEach(r => r.classList.remove('selected'));
    }

    async handleBookSearch() {
        const query = document.getElementById('searchBookInput')?.value || '';
        // Fast local IndexedDB search!
        const results = await window.libraryDB.searchBooks(query);
        await this.renderBooksTab(results);
    }

    async handleBookViewAll() {
        if (document.getElementById('searchBookInput')) {
            document.getElementById('searchBookInput').value = '';
        }
        // Read all from IndexedDB
        const books = await window.libraryDB.getAllBooks();
        await this.renderBooksTab(books);
    }

    async handleCreateBook() {
        const dto = {
            Title: document.getElementById('bookTitle').value.trim(),
            Author: document.getElementById('bookAuthor').value.trim(),
            Genre: document.getElementById('bookGenre').value.trim(),
            Language: document.getElementById('bookLanguage').value.trim() || 'English',
            Description: document.getElementById('bookDescription').value.trim(),
            TotalBooks: parseInt(document.getElementById('bookTotalBooks').value) || 1
        };

        if (!dto.Title || !dto.Author || !dto.Genre) {
            this.showToast('Please fill in Title, Author, and Genre.', 'error');
            return;
        }

        this.showLoading('Creating book on Web API...');
        try {
            // Consistency Rule: API Request first!
            const createdBook = await window.libraryApi.createBook(dto);
            // API Success -> update IndexedDB!
            await window.libraryDB.addBook(createdBook);
            // Refresh UI from IndexedDB
            this.clearBookForm();
            await this.renderBooksTab();
            this.showToast('Book created successfully.', 'success');
        } catch (err) {
            this.showToast(`Failed to create book: ${err.message}`, 'error');
        } finally {
            this.hideLoading();
        }
    }

    async handleUpdateBook() {
        if (!this.selectedBookId) {
            this.showToast('Please select a book to update.', 'error');
            return;
        }

        const dto = {
            Title: document.getElementById('bookTitle').value.trim(),
            Author: document.getElementById('bookAuthor').value.trim(),
            Genre: document.getElementById('bookGenre').value.trim(),
            Language: document.getElementById('bookLanguage').value.trim() || 'English',
            Description: document.getElementById('bookDescription').value.trim(),
            TotalBooks: parseInt(document.getElementById('bookTotalBooks').value) || 1
        };

        this.showLoading('Updating book on Web API...');
        try {
            // Consistency Rule: API Request first!
            const updatedBook = await window.libraryApi.updateBook(this.selectedBookId, dto);
            // API Success -> update IndexedDB!
            await window.libraryDB.updateBook(updatedBook);
            // Refresh UI from IndexedDB
            await this.renderBooksTab();
            this.showToast('Book updated successfully.', 'success');
        } catch (err) {
            this.showToast(`Failed to update book: ${err.message}`, 'error');
        } finally {
            this.hideLoading();
        }
    }

    async handleDeleteBook() {
        if (!this.selectedBookId) {
            this.showToast('Please select a book to delete.', 'error');
            return;
        }

        if (!confirm('Are you sure you want to delete this book?')) return;

        this.showLoading('Deleting book on Web API...');
        try {
            // API Request first!
            await window.libraryApi.deleteBook(this.selectedBookId);
            // API Success -> remove from IndexedDB
            await window.libraryDB.deleteBook(this.selectedBookId);
            this.clearBookForm();
            await this.renderBooksTab();
            this.showToast('Book deleted successfully.', 'success');
        } catch (err) {
            this.showToast(`Failed to delete book: ${err.message}`, 'error');
        } finally {
            this.hideLoading();
        }
    }

    // ==========================================
    // 2. BORROWERS TAB LOGIC
    // ==========================================
    async renderBorrowersTab(borrowersToDisplay = null) {
        const borrowers = borrowersToDisplay || await window.libraryDB.getAllBorrowers();
        const tbody = document.getElementById('borrowersTableBody');
        if (!tbody) return;

        tbody.innerHTML = '';
        if (borrowers.length === 0) {
            tbody.innerHTML = `<tr><td colspan="4" class="text-center text-muted p-4">No borrowers found in IndexedDB storage.</td></tr>`;
            return;
        }

        borrowers.forEach(b => {
            const tr = document.createElement('tr');
            if (this.selectedBorrowerId === b.BorrowerId) {
                tr.classList.add('selected');
            }
            tr.innerHTML = `
                <td><strong>${this.escapeHtml(b.Name)}</strong></td>
                <td>${this.escapeHtml(b.Phone)}</td>
                <td>${this.escapeHtml(b.Email || '-')}</td>
                <td><button class="btn btn-sm btn-brown-outline py-1 px-2" onclick="event.stopPropagation(); window.appController.openHistoryForBorrower(${b.BorrowerId}, '${this.escapeHtml(b.Name)}')">History</button></td>
            `;
            tr.addEventListener('click', () => this.selectBorrowerRow(tr, b));
            tbody.appendChild(tr);
        });
    }

    selectBorrowerRow(rowElement, borrower) {
        document.querySelectorAll('#borrowersTableBody tr').forEach(r => r.classList.remove('selected'));
        rowElement.classList.add('selected');

        this.selectedBorrowerId = borrower.BorrowerId;
        document.getElementById('borrowerName').value = borrower.Name;
        document.getElementById('borrowerPhone').value = borrower.Phone;
        document.getElementById('borrowerEmail').value = borrower.Email || '';
    }

    clearBorrowerForm() {
        this.selectedBorrowerId = null;
        document.getElementById('borrowerName').value = '';
        document.getElementById('borrowerPhone').value = '';
        document.getElementById('borrowerEmail').value = '';
        document.querySelectorAll('#borrowersTableBody tr').forEach(r => r.classList.remove('selected'));
    }

    async handleBorrowerSearch() {
        const query = document.getElementById('searchBorrowerInput')?.value || '';
        const results = await window.libraryDB.searchBorrowers(query);
        await this.renderBorrowersTab(results);
    }

    async handleBorrowerViewAll() {
        if (document.getElementById('searchBorrowerInput')) {
            document.getElementById('searchBorrowerInput').value = '';
        }
        const borrowers = await window.libraryDB.getAllBorrowers();
        await this.renderBorrowersTab(borrowers);
    }

    async handleCreateBorrower() {
        const dto = {
            BorrowerName: document.getElementById('borrowerName').value.trim(),
            Phone: document.getElementById('borrowerPhone').value.trim(),
            Email: document.getElementById('borrowerEmail').value.trim()
        };

        if (!dto.BorrowerName || !dto.Phone) {
            this.showToast('Please enter Borrower Name and Phone number.', 'error');
            return;
        }

        this.showLoading('Registering borrower via Web API...');
        try {
            const created = await window.libraryApi.createBorrower(dto);
            await window.libraryDB.addBorrower(created);
            this.clearBorrowerForm();
            await this.renderBorrowersTab();
            this.showToast('Borrower registered successfully.', 'success');
        } catch (err) {
            this.showToast(`Failed to register borrower: ${err.message}`, 'error');
        } finally {
            this.hideLoading();
        }
    }

    async handleUpdateBorrower() {
        if (!this.selectedBorrowerId) {
            this.showToast('Please select a borrower to update.', 'error');
            return;
        }

        const dto = {
            BorrowerName: document.getElementById('borrowerName').value.trim(),
            Phone: document.getElementById('borrowerPhone').value.trim(),
            Email: document.getElementById('borrowerEmail').value.trim()
        };

        this.showLoading('Updating borrower via Web API...');
        try {
            const updated = await window.libraryApi.updateBorrower(this.selectedBorrowerId, dto);
            await window.libraryDB.updateBorrower(updated);
            await this.renderBorrowersTab();
            this.showToast('Borrower updated successfully.', 'success');
        } catch (err) {
            this.showToast(`Failed to update borrower: ${err.message}`, 'error');
        } finally {
            this.hideLoading();
        }
    }

    async handleDeleteBorrower() {
        if (!this.selectedBorrowerId) {
            this.showToast('Please select a borrower to delete.', 'error');
            return;
        }

        if (!confirm('Are you sure you want to delete this borrower?')) return;

        this.showLoading('Deleting borrower via Web API...');
        try {
            await window.libraryApi.deleteBorrower(this.selectedBorrowerId);
            await window.libraryDB.deleteBorrower(this.selectedBorrowerId);
            this.clearBorrowerForm();
            await this.renderBorrowersTab();
            this.showToast('Borrower deleted successfully.', 'success');
        } catch (err) {
            this.showToast(`Failed to delete borrower: ${err.message}`, 'error');
        } finally {
            this.hideLoading();
        }
    }

    async handleViewHistory() {
        if (!this.selectedBorrowerId) {
            this.showToast('Please select a borrower to view borrowing history.', 'error');
            return;
        }
        const b = await window.libraryDB.getBorrower(this.selectedBorrowerId);
        const name = b ? b.Name : 'Borrower';
        await this.openHistoryForBorrower(this.selectedBorrowerId, name);
    }

    async openHistoryForBorrower(borrowerId, name) {
        this.showLoading('Loading borrowing history...');
        try {
            let history = [];
            if (window.libraryApi.isOnline) {
                history = await window.libraryApi.getBorrowerHistory(borrowerId);
            } else {
                history = await window.libraryDB.getBorrowerHistory(borrowerId);
            }

            document.getElementById('modalTitle').innerText = `Borrowing History - ${name}`;
            const body = document.getElementById('modalBody');
            
            if (history.length === 0) {
                body.innerHTML = `<p class="text-muted text-center py-4">No borrowing records found for ${this.escapeHtml(name)}.</p>`;
            } else {
                let html = `
                    <table class="library-table">
                        <thead>
                            <tr>
                                <th>Book Title</th>
                                <th>Borrow Date</th>
                                <th>Due Date</th>
                                <th>Return Date</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                `;
                history.forEach(item => {
                    const statusClass = item.Status === 'Returned' ? 'badge-returned' : (item.Status === 'Overdue' ? 'badge-overdue' : 'badge-borrowed');
                    html += `
                        <tr>
                            <td><strong>${this.escapeHtml(item.BookTitle)}</strong></td>
                            <td>${item.BorrowDate}</td>
                            <td>${item.DueDate}</td>
                            <td>${item.ReturnDate || '-'}</td>
                            <td><span class="badge-status ${statusClass}">${item.Status}</span></td>
                        </tr>
                    `;
                });
                html += `</tbody></table>`;
                body.innerHTML = html;
            }

            document.getElementById('customModal').style.display = 'flex';
        } catch (err) {
            this.showToast(`Error loading history: ${err.message}`, 'error');
        } finally {
            this.hideLoading();
        }
    }

    closeModal() {
        document.getElementById('customModal').style.display = 'none';
    }

    // ==========================================
    // 3. BORROWINGS TAB LOGIC
    // ==========================================
    async renderBorrowingsTab() {
        // Load dropdown options from IndexedDB
        const [borrowers, books, borrowings] = await Promise.all([
            window.libraryDB.getAllBorrowers(),
            window.libraryDB.getAllBooks(),
            window.libraryDB.getAllBorrowings()
        ]);

        // Populate Borrower dropdown
        const borrowerSelect = document.getElementById('borrowingBorrowerSelect');
        if (borrowerSelect) {
            borrowerSelect.innerHTML = '<option value="">-- Select Borrower --</option>';
            borrowers.forEach(b => {
                borrowerSelect.innerHTML += `<option value="${b.BorrowerId}">${this.escapeHtml(b.Name)} (${this.escapeHtml(b.Phone)})</option>`;
            });
        }

        // Populate Book dropdown (Only AvailableBooks > 0)
        const bookSelect = document.getElementById('borrowingBookSelect');
        if (bookSelect) {
            bookSelect.innerHTML = '<option value="">-- Select Book --</option>';
            const availableBooks = books.filter(b => b.AvailableBooks > 0);
            availableBooks.forEach(b => {
                bookSelect.innerHTML += `<option value="${b.BookId}">${this.escapeHtml(b.Title)} (Available: ${b.AvailableBooks})</option>`;
            });
        }

        // Set default dates
        const today = new Date().toISOString().split('T')[0];
        const dueDateObj = new Date();
        dueDateObj.setDate(dueDateObj.getDate() + 14);
        const defaultDueDate = dueDateObj.toISOString().split('T')[0];

        if (document.getElementById('borrowingBorrowDate') && !document.getElementById('borrowingBorrowDate').value) {
            document.getElementById('borrowingBorrowDate').value = today;
        }
        if (document.getElementById('borrowingDueDate') && !document.getElementById('borrowingDueDate').value) {
            document.getElementById('borrowingDueDate').value = defaultDueDate;
        }

        // Render Borrowings table
        const tbody = document.getElementById('borrowingsTableBody');
        if (!tbody) return;

        tbody.innerHTML = '';
        if (borrowings.length === 0) {
            tbody.innerHTML = `<tr><td colspan="7" class="text-center text-muted p-4">No borrowing records found in IndexedDB storage.</td></tr>`;
            return;
        }

        borrowings.forEach(item => {
            const tr = document.createElement('tr');
            if (this.selectedBorrowingId === item.BorrowingId) {
                tr.classList.add('selected');
            }
            const statusClass = item.Status === 'Returned' ? 'badge-returned' : (item.Status === 'Overdue' ? 'badge-overdue' : 'badge-borrowed');

            tr.innerHTML = `
                <td><strong>${this.escapeHtml(item.BorrowerName)}</strong></td>
                <td>${this.escapeHtml(item.BookTitle)}</td>
                <td>${item.BorrowDate}</td>
                <td>${item.DueDate}</td>
                <td>${item.ReturnDate || '-'}</td>
                <td><span class="badge-status ${statusClass}">${item.Status}</span></td>
                <td>
                    ${item.Status !== 'Returned' ? `<button class="btn btn-sm btn-brown-success py-1 px-2" onclick="event.stopPropagation(); window.appController.returnSpecificBook(${item.BorrowingId})">↩ Return</button>` : '-'}
                </td>
            `;
            tr.addEventListener('click', () => {
                document.querySelectorAll('#borrowingsTableBody tr').forEach(r => r.classList.remove('selected'));
                tr.classList.add('selected');
                this.selectedBorrowingId = item.BorrowingId;
            });
            tbody.appendChild(tr);
        });
    }

    clearBorrowingForm() {
        this.selectedBorrowingId = null;
        document.getElementById('borrowingBorrowerSelect').value = '';
        document.getElementById('borrowingBookSelect').value = '';
        const today = new Date().toISOString().split('T')[0];
        const dueDateObj = new Date();
        dueDateObj.setDate(dueDateObj.getDate() + 14);
        document.getElementById('borrowingBorrowDate').value = today;
        document.getElementById('borrowingDueDate').value = dueDateObj.toISOString().split('T')[0];
        document.querySelectorAll('#borrowingsTableBody tr').forEach(r => r.classList.remove('selected'));
    }

    async handleLendBook() {
        const borrowerId = parseInt(document.getElementById('borrowingBorrowerSelect').value);
        const bookId = parseInt(document.getElementById('borrowingBookSelect').value);
        const borrowDate = document.getElementById('borrowingBorrowDate').value;
        const dueDate = document.getElementById('borrowingDueDate').value;

        if (!borrowerId || !bookId || !borrowDate || !dueDate) {
            this.showToast('Please select Borrower, Book, Borrow Date, and Due Date.', 'error');
            return;
        }

        const dto = { BorrowerId: borrowerId, BookId: bookId, BorrowDate: borrowDate, DueDate: dueDate };

        this.showLoading('Processing borrowing via Web API...');
        try {
            // Consistency Rule: Call API first!
            const created = await window.libraryApi.lendBook(dto);
            // Save borrowing record to IndexedDB
            await window.libraryDB.addBorrowing(created);

            // Update Book AvailableBooks in IndexedDB
            const book = await window.libraryDB.getBook(bookId);
            if (book && book.AvailableBooks > 0) {
                book.AvailableBooks -= 1;
                await window.libraryDB.updateBook(book);
            }

            this.clearBorrowingForm();
            await this.renderBorrowingsTab();
            this.showToast('Book lent successfully.', 'success');
        } catch (err) {
            this.showToast(`Failed to lend book: ${err.message}`, 'error');
        } finally {
            this.hideLoading();
        }
    }

    async handleReturnBook() {
        if (!this.selectedBorrowingId) {
            this.showToast('Please select a borrowing record from the table to return.', 'error');
            return;
        }
        await this.returnSpecificBook(this.selectedBorrowingId);
    }

    async returnSpecificBook(borrowingId) {
        this.showLoading('Processing book return via Web API...');
        try {
            // API call first!
            const updatedBorrowing = await window.libraryApi.returnBook(borrowingId);
            
            // Save updated borrowing to IndexedDB
            await window.libraryDB.updateBorrowing(updatedBorrowing);

            // Increment book available quantity in IndexedDB
            if (updatedBorrowing && updatedBorrowing.BookId) {
                const book = await window.libraryDB.getBook(updatedBorrowing.BookId);
                if (book) {
                    book.AvailableBooks += 1;
                    await window.libraryDB.updateBook(book);
                }
            }

            this.selectedBorrowingId = null;
            await this.renderBorrowingsTab();
            this.showToast('Book returned successfully.', 'success');
        } catch (err) {
            this.showToast(`Failed to return book: ${err.message}`, 'error');
        } finally {
            this.hideLoading();
        }
    }

    // ==========================================
    // 4. OVERDUE BOOKS TAB LOGIC
    // ==========================================
    async renderOverdueTab() {
        const overdue = await window.libraryDB.getOverdueBorrowings();
        const tbody = document.getElementById('overdueTableBody');
        if (!tbody) return;

        tbody.innerHTML = '';
        if (overdue.length === 0) {
            tbody.innerHTML = `<tr><td colspan="6" class="text-center text-muted p-4">No overdue borrowings detected in IndexedDB storage.</td></tr>`;
            return;
        }

        overdue.forEach(item => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td><strong>${this.escapeHtml(item.BorrowerName)}</strong></td>
                <td>${this.escapeHtml(item.BookTitle)}</td>
                <td>${item.BorrowDate}</td>
                <td><span class="text-danger fw-bold">${item.DueDate}</span></td>
                <td><span class="badge-status badge-overdue">Overdue</span></td>
                <td>
                    <button class="btn btn-sm btn-brown-success py-1 px-2" onclick="window.appController.returnSpecificBook(${item.BorrowingId})">↩ Return Book</button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    }

    // ==========================================
    // UI UTILITIES
    // ==========================================
    showLoading(msg = 'Loading...') {
        let overlay = document.getElementById('globalLoadingOverlay');
        if (!overlay) {
            overlay = document.createElement('div');
            overlay.id = 'globalLoadingOverlay';
            overlay.className = 'loading-overlay';
            overlay.innerHTML = `
                <div class="spinner"></div>
                <div id="loadingText" style="margin-top: 1rem; font-weight: 600;">${msg}</div>
            `;
            document.body.appendChild(overlay);
        } else {
            document.getElementById('loadingText').innerText = msg;
            overlay.style.display = 'flex';
        }
    }

    hideLoading() {
        const overlay = document.getElementById('globalLoadingOverlay');
        if (overlay) overlay.style.display = 'none';
    }

    showToast(message, type = 'info') {
        let container = document.getElementById('toastContainer');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toastContainer';
            container.className = 'toast-container';
            document.body.appendChild(container);
        }

        const toast = document.createElement('div');
        toast.className = `toast-message toast-${type}`;
        const icon = type === 'success' ? '✓' : (type === 'error' ? '✕' : 'ℹ');
        toast.innerHTML = `<span>${icon}</span> <span>${this.escapeHtml(message)}</span>`;

        container.appendChild(toast);

        setTimeout(() => {
            toast.style.opacity = '0';
            toast.style.transition = 'opacity 0.4s ease';
            setTimeout(() => toast.remove(), 400);
        }, 4000);
    }

    escapeHtml(str) {
        if (!str) return '';
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#039;');
    }
}

document.addEventListener('DOMContentLoaded', () => {
    window.appController = new LibraryAppController();
    window.appController.init();
});
