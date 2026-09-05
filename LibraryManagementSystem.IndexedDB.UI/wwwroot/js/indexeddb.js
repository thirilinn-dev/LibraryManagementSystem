/**
 * Dedicated IndexedDB Module for Library Management System
 * Database: LibraryManagementDB
 * Stores: Books (keyPath: BookId), Borrowers (keyPath: BorrowerId), Borrowings (keyPath: BorrowingId)
 */

class LibraryIndexedDB {
    constructor() {
        this.dbName = 'LibraryManagementDB';
        this.dbVersion = 1;
        this.db = null;
    }

    /**
     * Opens or upgrades the IndexedDB database.
     */
    async openDatabase() {
        if (this.db) return this.db;

        return new Promise((resolve, reject) => {
            const request = indexedDB.open(this.dbName, this.dbVersion);

            request.onupgradeneeded = (event) => {
                const db = event.target.result;

                // 1. Books Store
                if (!db.objectStoreNames.contains('Books')) {
                    const booksStore = db.createObjectStore('Books', { keyPath: 'BookId' });
                    booksStore.createIndex('Title', 'Title', { unique: false });
                    booksStore.createIndex('Author', 'Author', { unique: false });
                    booksStore.createIndex('Genre', 'Genre', { unique: false });
                }

                // 2. Borrowers Store
                if (!db.objectStoreNames.contains('Borrowers')) {
                    const borrowersStore = db.createObjectStore('Borrowers', { keyPath: 'BorrowerId' });
                    borrowersStore.createIndex('Name', 'Name', { unique: false });
                }

                // 3. Borrowings Store
                if (!db.objectStoreNames.contains('Borrowings')) {
                    const borrowingsStore = db.createObjectStore('Borrowings', { keyPath: 'BorrowingId' });
                    borrowingsStore.createIndex('BorrowerId', 'BorrowerId', { unique: false });
                    borrowingsStore.createIndex('BookId', 'BookId', { unique: false });
                    borrowingsStore.createIndex('Status', 'Status', { unique: false });
                }
            };

            request.onsuccess = (event) => {
                this.db = event.target.result;
                resolve(this.db);
            };

            request.onerror = (event) => {
                console.error('IndexedDB open error:', event.target.error);
                reject(event.target.error);
            };
        });
    }

    // Helper: Normalize Book object
    _normalizeBook(book) {
        return {
            BookId: Number(book.BookId || book.bookId),
            Title: book.Title || book.title || '',
            Author: book.Author || book.author || '',
            Genre: book.Genre || book.genre || '',
            Language: book.Language || book.language || 'English',
            Description: book.Description || book.description || '',
            TotalBooks: Number(book.TotalBooks !== undefined ? book.TotalBooks : book.totalBooks || 0),
            AvailableBooks: Number(book.AvailableBooks !== undefined ? book.AvailableBooks : book.availableBooks || 0),
            IsDeleted: Boolean(book.IsDeleted || book.isDeleted || false)
        };
    }

    // Helper: Normalize Borrower object
    _normalizeBorrower(borrower) {
        return {
            BorrowerId: Number(borrower.BorrowerId || borrower.borrowerId),
            Name: borrower.Name || borrower.name || borrower.BorrowerName || borrower.borrowerName || '',
            Phone: borrower.Phone || borrower.phone || '',
            Email: borrower.Email || borrower.email || '',
            IsDeleted: Boolean(borrower.IsDeleted || borrower.isDeleted || false)
        };
    }

    // Helper: Normalize Borrowing object
    _normalizeBorrowing(borrowing) {
        const id = Number(borrowing.BorrowingId || borrowing.borrowingId || borrowing.BorrowId || borrowing.borrowId);
        return {
            BorrowingId: id,
            BorrowId: id,
            BorrowerId: Number(borrowing.BorrowerId || borrowing.borrowerId),
            BorrowerName: borrowing.BorrowerName || borrowing.borrowerName || '',
            BookId: Number(borrowing.BookId || borrowing.bookId),
            BookTitle: borrowing.BookTitle || borrowing.bookTitle || '',
            BorrowDate: borrowing.BorrowDate || borrowing.borrowDate || '',
            DueDate: borrowing.DueDate || borrowing.dueDate || '',
            ReturnDate: borrowing.ReturnDate || borrowing.returnDate || null,
            Status: borrowing.Status || borrowing.status || 'Borrowed'
        };
    }

    // ==========================================
    // BOOKS OPERATIONS
    // ==========================================

    async addBook(book) {
        const db = await this.openDatabase();
        const normalized = this._normalizeBook(book);
        return new Promise((resolve, reject) => {
            const tx = db.transaction('Books', 'readwrite');
            const store = tx.objectStore('Books');
            const request = store.put(normalized);

            request.onsuccess = () => resolve(normalized);
            request.onerror = (e) => reject(e.target.error);
        });
    }

    async updateBook(book) {
        return this.addBook(book); // put updates if key exists
    }

    async deleteBook(bookId) {
        const db = await this.openDatabase();
        return new Promise((resolve, reject) => {
            const tx = db.transaction('Books', 'readwrite');
            const store = tx.objectStore('Books');
            const request = store.delete(Number(bookId));

            request.onsuccess = () => resolve(true);
            request.onerror = (e) => reject(e.target.error);
        });
    }

    async getBook(bookId) {
        const db = await this.openDatabase();
        return new Promise((resolve, reject) => {
            const tx = db.transaction('Books', 'readonly');
            const store = tx.objectStore('Books');
            const request = store.get(Number(bookId));

            request.onsuccess = () => {
                const book = request.result;
                if (book && !book.IsDeleted) {
                    resolve(book);
                } else {
                    resolve(null);
                }
            };
            request.onerror = (e) => reject(e.target.error);
        });
    }

    async getAllBooks() {
        const db = await this.openDatabase();
        return new Promise((resolve, reject) => {
            const tx = db.transaction('Books', 'readonly');
            const store = tx.objectStore('Books');
            const request = store.getAll();

            request.onsuccess = () => {
                const books = (request.result || []).filter(b => !b.IsDeleted);
                resolve(books);
            };
            request.onerror = (e) => reject(e.target.error);
        });
    }

    async searchBooks(queryText) {
        const allBooks = await this.getAllBooks();
        if (!queryText || !queryText.trim()) {
            return allBooks;
        }
        const term = queryText.trim().toLowerCase();
        return allBooks.filter(book =>
            (book.Title && book.Title.toLowerCase().includes(term)) ||
            (book.Author && book.Author.toLowerCase().includes(term)) ||
            (book.Genre && book.Genre.toLowerCase().includes(term))
        );
    }

    // ==========================================
    // BORROWERS OPERATIONS
    // ==========================================

    async addBorrower(borrower) {
        const db = await this.openDatabase();
        const normalized = this._normalizeBorrower(borrower);
        return new Promise((resolve, reject) => {
            const tx = db.transaction('Borrowers', 'readwrite');
            const store = tx.objectStore('Borrowers');
            const request = store.put(normalized);

            request.onsuccess = () => resolve(normalized);
            request.onerror = (e) => reject(e.target.error);
        });
    }

    async updateBorrower(borrower) {
        return this.addBorrower(borrower);
    }

    async deleteBorrower(borrowerId) {
        const db = await this.openDatabase();
        return new Promise((resolve, reject) => {
            const tx = db.transaction('Borrowers', 'readwrite');
            const store = tx.objectStore('Borrowers');
            const request = store.delete(Number(borrowerId));

            request.onsuccess = () => resolve(true);
            request.onerror = (e) => reject(e.target.error);
        });
    }

    async getBorrower(borrowerId) {
        const db = await this.openDatabase();
        return new Promise((resolve, reject) => {
            const tx = db.transaction('Borrowers', 'readonly');
            const store = tx.objectStore('Borrowers');
            const request = store.get(Number(borrowerId));

            request.onsuccess = () => {
                const b = request.result;
                if (b && !b.IsDeleted) resolve(b);
                else resolve(null);
            };
            request.onerror = (e) => reject(e.target.error);
        });
    }

    async getAllBorrowers() {
        const db = await this.openDatabase();
        return new Promise((resolve, reject) => {
            const tx = db.transaction('Borrowers', 'readonly');
            const store = tx.objectStore('Borrowers');
            const request = store.getAll();

            request.onsuccess = () => {
                const borrowers = (request.result || []).filter(b => !b.IsDeleted);
                resolve(borrowers);
            };
            request.onerror = (e) => reject(e.target.error);
        });
    }

    async searchBorrowers(queryText) {
        const all = await this.getAllBorrowers();
        if (!queryText || !queryText.trim()) return all;
        const term = queryText.trim().toLowerCase();
        return all.filter(b =>
            (b.Name && b.Name.toLowerCase().includes(term)) ||
            (b.Phone && b.Phone.includes(term)) ||
            (b.Email && b.Email.toLowerCase().includes(term))
        );
    }

    // ==========================================
    // BORROWINGS OPERATIONS
    // ==========================================

    async addBorrowing(borrowing) {
        const db = await this.openDatabase();
        const normalized = this._normalizeBorrowing(borrowing);
        return new Promise((resolve, reject) => {
            const tx = db.transaction('Borrowings', 'readwrite');
            const store = tx.objectStore('Borrowings');
            const request = store.put(normalized);

            request.onsuccess = () => resolve(normalized);
            request.onerror = (e) => reject(e.target.error);
        });
    }

    async updateBorrowing(borrowing) {
        return this.addBorrowing(borrowing);
    }

    async getBorrowing(borrowingId) {
        const db = await this.openDatabase();
        return new Promise((resolve, reject) => {
            const tx = db.transaction('Borrowings', 'readonly');
            const store = tx.objectStore('Borrowings');
            const request = store.get(Number(borrowingId));

            request.onsuccess = () => resolve(request.result || null);
            request.onerror = (e) => reject(e.target.error);
        });
    }

    async getAllBorrowings() {
        const db = await this.openDatabase();
        return new Promise((resolve, reject) => {
            const tx = db.transaction('Borrowings', 'readonly');
            const store = tx.objectStore('Borrowings');
            const request = store.getAll();

            request.onsuccess = () => resolve(request.result || []);
            request.onerror = (e) => reject(e.target.error);
        });
    }

    async getOverdueBorrowings() {
        const all = await this.getAllBorrowings();
        const todayStr = new Date().toISOString().split('T')[0];
        return all.filter(b => {
            const isNotReturned = !b.ReturnDate || b.Status === 'Borrowed' || b.Status === 'Overdue';
            const isPastDue = b.DueDate && b.DueDate < todayStr;
            return isNotReturned && isPastDue;
        });
    }

    async getBorrowerHistory(borrowerId) {
        const all = await this.getAllBorrowings();
        return all.filter(b => Number(b.BorrowerId) === Number(borrowerId));
    }

    // ==========================================
    // STORE & SYSTEM UTILITIES
    // ==========================================

    async bulkSyncBooks(booksList) {
        const db = await this.openDatabase();
        return new Promise((resolve, reject) => {
            const tx = db.transaction('Books', 'readwrite');
            const store = tx.objectStore('Books');
            store.clear(); // replace store contents
            booksList.forEach(b => store.put(this._normalizeBook(b)));

            tx.oncomplete = () => resolve(true);
            tx.onerror = (e) => reject(e.target.error);
        });
    }

    async bulkSyncBorrowers(borrowersList) {
        const db = await this.openDatabase();
        return new Promise((resolve, reject) => {
            const tx = db.transaction('Borrowers', 'readwrite');
            const store = tx.objectStore('Borrowers');
            store.clear();
            borrowersList.forEach(b => store.put(this._normalizeBorrower(b)));

            tx.oncomplete = () => resolve(true);
            tx.onerror = (e) => reject(e.target.error);
        });
    }

    async bulkSyncBorrowings(borrowingsList) {
        const db = await this.openDatabase();
        return new Promise((resolve, reject) => {
            const tx = db.transaction('Borrowings', 'readwrite');
            const store = tx.objectStore('Borrowings');
            store.clear();
            borrowingsList.forEach(b => store.put(this._normalizeBorrowing(b)));

            tx.oncomplete = () => resolve(true);
            tx.onerror = (e) => reject(e.target.error);
        });
    }

    async clearStore(storeName) {
        const db = await this.openDatabase();
        return new Promise((resolve, reject) => {
            const tx = db.transaction(storeName, 'readwrite');
            const store = tx.objectStore(storeName);
            const request = store.clear();

            request.onsuccess = () => resolve(true);
            request.onerror = (e) => reject(e.target.error);
        });
    }

    async clearDatabase() {
        if (this.db) {
            this.db.close();
            this.db = null;
        }
        return new Promise((resolve, reject) => {
            const req = indexedDB.deleteDatabase(this.dbName);
            req.onsuccess = () => resolve(true);
            req.onerror = (e) => reject(e.target.error);
        });
    }
}

// Global instance
window.libraryDB = new LibraryIndexedDB();
