## contents
`Shared` folder contains the actor and proto file
`AddRemote` attaches the remote to the custom aspnetcore app
`WithRemote` uses the internal aspnetcore app (from protoactor repo)

## env
macos 11.5.2
dotnet 6.0.101
docker 20.10.10 (build b485636)

## try
run the following inside the terminal
`make`
*it spins up two instances of `AddRemote` and `WithRemote` services*
