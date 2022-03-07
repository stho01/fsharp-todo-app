
export class TodoAppHttpClient {
    
    static create() {
        return new TodoAppHttpClient(new HttpClient());
    }
    
    constructor(httpClient) {
        this._httpClient = httpClient;   
    }


    get httpClient() { return this._httpClient; }
    
    newTodo(todoListName, todo) {
        return this._httpClient.post(`/list/${todoListName}/todo`, todo);
    }

    removeTodoFromList(listName, todoId) {
        if (!listName || !todoId) {
            throw new Error("Arguments cannot be null!");
        }
        
        return this._httpClient.delete(`/list/${listName}/todo/${todoId}`);
    }
}

export class HttpClient {

    async post(url, body) {
        const response = await fetch(url, {
            method: "POST",
            body: JSON.stringify(body),
            headers: { 
                "Content-Type": "application/json"
            }
        });
        
        return this._handleResponse(response);
    }
    
    async delete(url) {
        const response = await fetch(url, {
            method: "DELETE"
        });
        
        return this._handleResponse(response);
    }
    
    async _handleResponse(response) {
        if (!response.ok) {
            const text = await response.text();
            alert(text);
            throw new Error(text);
        }
        
        if (response.statusCode === 204) {
            return Promise.resolve();
        }
        
        switch (response.headers["Content-Type"]) {
            case "application/json": 
                return await response.json();
            default: 
                return await response.text();
        }
    }
}
