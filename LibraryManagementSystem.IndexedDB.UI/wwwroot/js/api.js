/**
 * Web API Client Module for Library Management System
 * Interfaces with ASP.NET Core Web API endpoints
 */

class LibraryApiClient {
    constructor() {
        // Default API base URL (falls back to Web API default port 5288 if hosted on separate UI port)
        const host = window.location.hostname || 'localhost';
        this.baseUrls = [
            'http://localhost:5288/api',
            'https://localhost:7051/api',
            `${window.location.origin}/api`
        ];
        this.activeBaseUrl = this.baseUrls[0];
        this.isOnline = false;
    }

    /**
     * Finds active Web API server URL
     */
    async checkConnection() {
        for (const url of this.baseUrls) {
            try {
                const response = await fetch(`${url}/Book`, { method: 'GET', headers: { 'Accept': 'application/json' } });
                if (response.ok) {
                    this.activeBaseUrl = url;
                    this.isOnline = true;
                    return true;
                }
            } catch (err) {
                // continuation
            }
        }
        this.isOnline = false;
        return false;
    }

    async _request(endpoint, options = {}) {
        const url = `${this.activeBaseUrl}${endpoint}`;
        const headers = {
            'Content-Type': 'application/json',
            'Accept': 'application/json',
            ...(options.headers || {})
        };

        try {
            const response = await fetch(url, { ...options, headers });
            
            if (response.status === 24) { // NoContent
                return null;
            }

            const isJson = (response.headers.get('content-type') || '').includes('application/json');
            const data = isJson ? await response.json() : await response.text();

            if (!response.ok) {
                let errorMsg = 'API Request Failed';
                if (typeof data === 'object' && data !== null) {
                    errorMsg = data.message || data.title || JSON.stringify(data);
                } else if (typeof data === 'string') {
                    errorMsg = data;
                }
                throw new Error(errorMsg);
            }

            this.isOnline = true;
            return data;
        } catch (error) {
            console.warn(`API Error [${endpoint}]:`, error.message);
            // Re-throw so caller knows API failed (for consistency rule)
            throw error;
        }
    }

    // ==========================================
    // BOOKS API
    // ==========================================
    async getBooks() {
        return this._request('/Book');
    }

    async createBook(bookDto) {
        return this._request('/Book', {
            method: 'POST',
            body: JSON.stringify(bookDto)
        });
    }

    async updateBook(id, bookDto) {
        return this._request(`/Book/${id}`, {
            method: 'PATCH',
            body: JSON.stringify(bookDto)
        });
    }

    async deleteBook(id) {
        return this._request(`/Book/${id}`, {
            method: 'DELETE'
        });
    }

    // ==========================================
    // BORROWERS API
    // ==========================================
    async getBorrowers() {
        return this._request('/Borrower');
    }

    async createBorrower(borrowerDto) {
        return this._request('/Borrower', {
            method: 'POST',
            body: JSON.stringify(borrowerDto)
        });
    }

    async updateBorrower(id, borrowerDto) {
        return this._request(`/Borrower/${id}`, {
            method: 'PATCH',
            body: JSON.stringify(borrowerDto)
        });
    }

    async deleteBorrower(id) {
        return this._request(`/Borrower/${id}`, {
            method: 'DELETE'
        });
    }

    async getBorrowerHistory(id) {
        return this._request(`/Borrower/${id}/history`);
    }

    // ==========================================
    // BORROWINGS API
    // ==========================================
    async getBorrowings() {
        return this._request('/Borrowing');
    }

    async lendBook(borrowingDto) {
        return this._request('/Borrowing', {
            method: 'POST',
            body: JSON.stringify(borrowingDto)
        });
    }

    async returnBook(id) {
        return this._request(`/Borrowing/${id}/return`, {
            method: 'POST'
        });
    }

    async getOverdueBorrowings() {
        return this._request('/Borrowing/overdue');
    }
}

window.libraryApi = new LibraryApiClient();
