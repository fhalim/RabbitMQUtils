module ClusterAdmin
open System.Diagnostics
open System.IO

let rabbitMqHome = @"c:\Program Files (x86)\RabbitMQ Server\rabbitmq_server-3.1.0"

type node = { name:string; port:int; adminPort:int}
type cluster = {nodes:node seq;}

let simpleNodeStrategy idx = {name = sprintf "rabbitmq_%i@%s" idx System.Environment.MachineName ; port = 5673 + idx; adminPort = 15673 + idx}


let startNode nodeInfo =
    let sbinDir = Path.Combine(rabbitMqHome, "sbin")
    let startInfo = new ProcessStartInfo(Path.Combine(sbinDir, "rabbitmq-server.bat"), "")
    Map([
        ("RABBITMQ_NODENAME", nodeInfo.name);
        ("RABBITMQ_NODE_PORT", string nodeInfo.port);
        ("RABBITMQ_SERVER_START_ARGS", sprintf "-detached -rabbitmq_management listener [{{port,%i}}]" nodeInfo.adminPort)
        ])
        |> Map.iter (fun k v -> startInfo.EnvironmentVariables.Add(k, v))
    startInfo.UseShellExecute <- false
    startInfo.WorkingDirectory <- sbinDir
    startInfo.RedirectStandardOutput <- true
    startInfo.RedirectStandardError <- true
    let proc = Process.Start(startInfo)
    let stdOut = proc.StandardOutput.ReadToEnd()
    let stdErr = proc.StandardError.ReadToEnd()
    proc

let createCluster count nodestrategy =
    let nodeInfos = seq {for idx in 1..count do yield nodestrategy idx}
    Seq.map startNode nodeInfos