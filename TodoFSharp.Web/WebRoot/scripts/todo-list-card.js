import {uuid} from "./utils.js";
import HttpClient from "./http-client.js";

const DEFAULT_OPTIONS = {
    nameInputSelector: ".todo-list-card__name",
    completedListSelector: ".todo-list-card__completed",
    todoListSelector: ".todo-list-card__todos",
    newTodoInputSelector: ".todo-list-card__new"
}

const domParser = new DOMParser();

function createLiElement(id, text) {
    const doc = domParser.parseFromString(`
        <li class="list-group-item">
            <input class="form-check-input me-3" type="checkbox" id="${id}">
            <label class="form-check-label" for="${id}">${text}</label>
        </li>`, "text/html");

    return doc.body.firstChild;
}


export default class TodoListCard {

    _container;
    _options;
    _nameInput;
    _completedList;
    _todoList;
    _newTodoInput;
    _httpClient;

    constructor(container, options) {
        this._container = (container instanceof HTMLElement) ? container : null;
        if (this._container == null) {
            throw new Error("Container must be an instance of HTMLElement");
        }

        this._options = {...DEFAULT_OPTIONS, ...options};
        this._completedList = null;
        this._todoList = null;
        this._httpClient = new HttpClient();
    }

    init() {
        this._nameInput = this._getElementOrThrow(this._options.nameInputSelector);
        this._completedList = this._getElementOrThrow(this._options.completedListSelector)
        this._todoList = this._getElementOrThrow(this._options.todoListSelector);
        this._newTodoInput = this._getElementOrThrow(this._options.newTodoInputSelector);
        
        this._container.addEventListener("change", this._onChangeEventHandler.bind(this));
        this._newTodoInput.addEventListener("keyup", this._onNewInputKeyUpEventHandler.bind(this));
    }
    
    get listName() {
        return this._nameInput.value;
    }

    _getElementOrThrow(selector) {
        const element = this._container.querySelector(selector);

        if (element == null) {
            throw new Error(`Element for selector ${selector} was not found within container...`)
        }

        return element;
    }

    _onChangeEventHandler(event) {
        console.log(`Checkbox checked target: ${event.target} currentTarget: ${event.currentTarget}`);
        if (event.target instanceof HTMLInputElement && event.target.type === "checkbox") {
            if (event.target.checked) {
                this._moveTodoTo(event.target, this._completedList);
            } else {
                this._moveTodoTo(event.target, this._todoList);
            }
        }
    }

    _moveTodoTo(checkbox, parent) {
        let li = checkbox.closest("li");
        if (li == null) {
            return;
        }

        parent.appendChild(li);
    }

    async _onNewInputKeyUpEventHandler(event) {
        const value = event.target.value;
        if (event.keyCode === 13 && value != null && value !== "") {
            event.preventDefault();

            let id = uuid();

            const li = createLiElement(id, value.trim());
            this._todoList.appendChild(li);

            try {
                let listName = encodeURIComponent(this.listName);
                const result = await this._httpClient.newTodo(listName, {
                    id,
                    todo: value
                });
                
                console.log(`Todo was added ${JSON.stringify(result)}`);
                event.target.value = "";
            } catch (error) {
                this._todoList.removeChild(li);
                console.error(error);
            }
        }
    }
}