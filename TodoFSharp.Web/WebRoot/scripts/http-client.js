
export default class HttpClient {
    
    async newTodo(todoListName, todo) {
        console.log("Here");
        return await this.post(`/list/${todoListName}/todo`, todo);
    }
    
    async post(url, body) {
        const response = await fetch(url, {
            method: "POST",
            body: JSON.stringify(body),
            headers: { 
                "Content-Type": "application/json"
            }
        }) 
        
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
