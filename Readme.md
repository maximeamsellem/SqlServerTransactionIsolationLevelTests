# Prerecquisites

- Docker engine
- .Net SDK 10

# Execute tests

Run
`dotnet test`
or
`dotnet test --logger "console;verbosity=detailed"` to get details in your console.

# Limitations

The test could go further by looping several times on the cases to make sure it does not pass by luck.
Luck can happen when you test concurrency

# Going further

Testing other transaction levels
Testing with EF
