
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
        
        if (!response.ok) {
            const text = await response.text();
            alert(text);
            throw new Error(text);
        }
        
        return  await response.json();
    }
}
