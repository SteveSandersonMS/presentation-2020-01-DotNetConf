This is a fork of the ASP.NET Core 'Samples' repo, so it contains lots of stuff unrelated to the presentation.

The relevant parts of the code are in `samples\aspnetcore\blazor\FlightFinder`.

The relevant branches are:

 * `pre-demo-state`
 * `grpc` (switch to this early in the demo, and do the rest of the demo here)
 * `desktop` (switch to this near the end, to show Blazor app running in WebWindow)
 * `master` (finished post-demo state)

## Pre-demo prep

- 1920x1080 resolution
- Auto-hide taskbar
- Git in clean state on `pre-demo-state` branch
- VS
  - Open FlightFinder.sln
  - Add snippets
  - Font size 115% (based on size 10)
  - Check FlightFinder.Server.csproj runs in browser
  - Hide 'solution items' folder
- Cmder
  - In git repo dir
  - `cd samples\aspnetcore\blazor\FlightFinder`
  - `rm -rf FlightFinder.Desktop\`
  - `clear`
- Close browser
- Powerpoint
  - Slides ready
- Timer ready

## Flow

  - [4] Look at flightfinder app
        - See functionality in browser
        - See project structure in VS (server/client/shared)
        - See Main.razor
        - See shared model types
  - [8] gRPC
        - See FlightFinder doing traditional JSON
          - Problem 1: verbose on the wire
          - Problem 2: if server and client don't agree about URLs (etc) then it will fail -- versioning
        - It's good that server & client share model types, but there's still nothing to guarantee correct
          URLs, HTTP methods
        - Is there a more modern, streamlined alternative with stronger type checking? Yes there are several,
          one of which we've focused on a lot is gRPC
          - it's an alternate kind of API endpoint, using lang-independent descriptor and protobuf messages
          - pros/cons vs json-over-http: more strongly typed, more compact, no REST arguments. It's just RPC.
          - normally requires HTTP2 (and hence not callable from JS/WebAssembly) but gRPC-Web works over HTTP1.1
        - Switch to version that has .proto in Shared and implements service in .Server
          - `git checkout grpc`
          - See .proto file
          - See that the DTO types are now codegenned, but we can still add more members via partial classes
          - See gRPC-Web enabled service on server
        - Make client issue calls via gRPC
          - Add FlightDataClient DI service
          - Update AirportsList.razor to call via gRPC - see it in browser
          - Update AppState.cs to do search via gRPC - see it in browser
        - Demo adding a new RPC endpoint to track items being added to shortlist
  - [6] Unit testing
        - Explain goals, pros/cons of unit vs browser automation tests
        - Add new ShortlistTests.cs class
          - Implement test for CanDisplayEmpty - passes
          - Implement test for RoundsUpPrice - fails, then implement, then passes
        - Look at SearchTests.cs
          - Read SendsSearchCriteria and ShowsSearchResults tests
          - Add new WhileSearchIsPending_ShowsLoadingState test - fails, then implement, then passes
        - Summarise purpose and that this is an experiment so far
  - [3] Attach-to-process from VS
  - [8] WebWindow
        - create new console app project
        - make it display an HTML string
        - `git checkout -f desktop`
        - see it runs except got wrong backend URL
        - fix via adding appsettings.json & Startup.cs code
        - see same thing running on macOS

Total: 29 mins
