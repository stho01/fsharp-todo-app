module internal TodoFSharp.WebClient.HttpMethods

open System.Net.Http
open System.Text

let GET<'a> (client: HttpClient) (url: string) =
    async {
        let! response = client.GetAsync(url) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return content |> Utils.deserialize<'a>
    }

let POST<'a> (client: HttpClient) (url: string) =
    async {
        let! response = client.PostAsync(url, null) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return content |> Utils.deserialize<'a>
    }

let POSTJson<'a> (client: HttpClient) (url: string) payload =
     async {
        let json = Utils.serialize payload
        let requestBody = new StringContent(json, Encoding.UTF8, "application/json")
        let! response = client.PostAsync(url, requestBody) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return content |> Utils.deserialize<'a>
     }
     
let DELETE<'a> (client: HttpClient) (url: string) =
    async {
        let! response = client.DeleteAsync(url) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return content |> Utils.deserialize<'a>
    }

let PUTJson<'a> (client: HttpClient) (url: string) payload =
    async {
        let json = Utils.serialize payload
        let requestBody = new StringContent(json, Encoding.UTF8, "application/json")
        let! response = client.PutAsync(url, requestBody) |> Async.AwaitTask
        let! content = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        return content |> Utils.deserialize<'a>
    }