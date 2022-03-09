namespace TodoFSharp.WebClient

type Url = string

type HttpGet<'TResponse> =
    Url
     -> Async<'TResponse>

type HttpPost<'TPayload, 'TResponse> =
    Url
     -> 'TPayload option
     -> Async<'TResponse>

type HttpDelete<'TResponse> =
    Url
     -> Async<'TResponse>
     
     
type HttpPut<'TPayload, 'TResponse> =
    Url
     -> 'TPayload option
     -> Async<'TResponse>

type HttpMethods<'TPayload, 'TResponse> = {
    GET: HttpGet<'TResponse>
    POST: HttpPost<'TPayload, 'TResponse>
    DELETE: HttpDelete<'TResponse>
    PUT: HttpPut<'TPayload, 'TResponse>
}