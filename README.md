# Tripla
Tripla is a user-friendly web application designed to make planning multi-city trips easier. Developed using React for the frontend and ASP.NET for the backend, it streamlines the process of finding plane tickets for journeys that involve multiple cities. 

Tripla aims to simplify the task of finding and comparing flight tickets for multi-city trips, making it a handy tool for travelers looking to efficiently plan their journeys.

## Demo
[demo.webm](https://github.com/timofey256/tripla/assets/54218713/8ca4c2c1-a43b-4736-8919-4b35f4f0d25b)

## How to use?
Firstly, clone the repository. Then, you will need your API key and secret from [Amadeus](https://developers.amadeus.com/) and you should put them into `Server/appsettings.json`. Then install all dependencies for frontend via `npm`. **Initially you will need only `dotnet` for compiling C# code and `npm` for installing frontend dependencies.**
```
git clone https://github.com/timofey256/tripla.git
# Put Amadeus API key and secret to Server/appsettings.json
cd tripla/ClientApp/ && npm install && cd .. # Install frontend dependencies

# Run application:
cd ClientApp/ && react-scripts start
cd Server/ && dotnet run
```

Then go to `localhost:3000` and enter your desired destinations (in form of airport codes) and dates.

## How to use?
See [doc/how-it-works.md](doc/how-it-works.md)
