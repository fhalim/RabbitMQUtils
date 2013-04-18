open RestSharp
open System

type RabbitMQResource() =
    member val name = "" with get, set
    member val vhost = "" with get, set

let e = Uri.EscapeDataString

let listResources (client:RestClient) resourceType =
    client.Execute<ResizeArray<RabbitMQResource>>(new RestRequest ("/api/" + resourceType) )

let deleteResource (client:RestClient) resourceType (resources:RabbitMQResource seq) =
    resources |> Seq.iter (fun resource -> client.Execute (new RestRequest("/api/" + resourceType + "/" + e(resource.vhost) + "/" + e(resource.name), Method.DELETE)) |> ignore)

[<EntryPoint>]
let main argv = 
    let client = new RestClient "http://localhost:15672"
    client.Authenticator <- new HttpBasicAuthenticator("guest", "guest")
    let exchanges = listResources client "exchanges"
    exchanges.Data |> Seq.filter (fun x -> x.name.StartsWith "NServiceBus") |> deleteResource client "exchanges"
    0 // return an integer exit code