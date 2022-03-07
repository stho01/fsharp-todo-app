import {uuid} from "./utils.js";
import {TodoAppHttpClient} from "./http-client.js";

const DEFAULT_OPTIONS = {
    nameInputSelector: ".todo-list-card__name",
    completedListSelector: ".todo-list-card__completed",
    todoListSelector: ".todo-list-card__todos",
    newTodoInputSelector: ".todo-list-card__new",
    removeTodoBtnSelector: ".todo-list-card__remove"
}

const domParser = new DOMParser();
function createLiElement(id, text) {
    const doc = domParser.parseFromString(`
        <li class="list-group-item d-flex align-items-center">
            <input class="form-check-input me-3" type="checkbox" id="${id}">
            <label class="form-check-label" for="${id}">${text}</label>
            <div class="flex-grow-1"></div>
            <button class="btn btn-icon btn-sm todo-list-card__remove text-muted">
                <i class="fa fa-close"></i>
            </button>
        </li>`, "text/html");

    return doc.body.firstChild;
}

const KEYCODE_ENTER = 13;

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
        this._httpClient = TodoAppHttpClient.create();
    }

    init() {
        this._nameInput = this._getElementOrThrow(this._options.nameInputSelector);
        this._completedList = this._getElementOrThrow(this._options.completedListSelector)
        this._todoList = this._getElementOrThrow(this._options.todoListSelector);
        this._newTodoInput = this._getElementOrThrow(this._options.newTodoInputSelector);
        
        this._container.addEventListener("change", this._onChangeEventHandler.bind(this));
        this._container.addEventListener("click", this._onClickEventHandler.bind(this))
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

    _moveTodoTo(checkbox, parent) {
        let li = checkbox.closest("li");
        if (li == null) {
            return;
        }

        parent.appendChild(li);
    }

    async _addTodoToList(todo) {
        let id = uuid();
        let listName = encodeURIComponent(this.listName);
        
        await this._httpClient.newTodo(listName, { id, todo });

        const li = createLiElement(id, todo.trim());
        this._todoList.appendChild(li);
    }
    
    async _removeTodoFromList(removeBtn) {
        let li = removeBtn.closest("li");
        if (li == null) {
            console.warn("li element was not found. A li element is expected when removing a todo.")
            return;
        }
        
        let checkbox = li.querySelector("input[type=checkbox]");
        if (!checkbox?.id) {
            throw new Error("Failed to remove todo.. Not able to get Todo Id..");
        }

        await this._httpClient.removeTodoFromList(this.listName, checkbox.id);
        
        li.closest("ul")
            .removeChild(li);
    }
    
    // EVENT HANDLERS ==========================================================

    _onChangeEventHandler(event) {
        if (event.target instanceof HTMLInputElement && event.target.type === "checkbox") {
            if (event.target.checked) {
                this._moveTodoTo(event.target, this._completedList);
            } else {
                this._moveTodoTo(event.target, this._todoList);
            }
        }
    }
        
    async _onNewInputKeyUpEventHandler(event) {
        const value = event.target.value;

        if (event.keyCode === KEYCODE_ENTER && value != null && value !== "") {
            event.preventDefault();
            await this._addTodoToList(value);
            event.target.value = "";      
        }
    }
    
    async _onClickEventHandler(event) {
        let removeBtn = event.target.closest(this._options.removeTodoBtnSelector); 
        if (removeBtn) {
            event.stopImmediatePropagation();
            event.preventDefault();
            await this._removeTodoFromList(removeBtn);
        }
    }
}