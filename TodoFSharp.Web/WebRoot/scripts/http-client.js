export class HttpClient {
    
    async _request(method, url) {
        const response = await fetch(url, { method: method });
        return this._handleResponse(response);
    }
    
    async _requestWithBody(method, url, body) {
        const response = await fetch(url, {
            method: method,
            body: JSON.stringify(body),
            headers: {
                "Content-Type": "application/json"
            }
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

export class TodoAppHttpClient extends HttpClient {
    
    newTodo(todoListName, todo) {
        return this._requestWithBody("POST", `/list/${encodeURIComponent(todoListName)}/todo`, todo);
    }

    removeTodoFromList(listName, todoId) {
        if (!listName || !todoId) {
            throw new Error("Arguments cannot be null!");
        }
        
        return this._request("DELETE", `/list/${encodeURIComponent(listName)}/todo/${encodeURIComponent(todoId)}`);
    }
    
    updateTodo(todoListName, todo) {
        return this._requestWithBody("PUT", `/list/${encodeURIComponent(todoListName)}/todo`, todo);
    }
    
    completeTodo(todoListName, todoId) {
        return this._request("POST", `/list/${encodeURIComponent(todoListName)}/todo/${encodeURIComponent(todoId)}/complete`);
    }
    
    incompleteTodo(todoListName, todoId) {
        return this._request("POST", `/list/${encodeURIComponent(todoListName)}/todo/${encodeURIComponent(todoId)}/incomplete`);
    }
}
