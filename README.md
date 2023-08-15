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
cd Server && dotnet run
```

## How does it work?
Here's how Tripla works: When a user enters the cities they want to visit and their travel dates, the frontend sends this information to the backend. The backend processes the data and then sends a request to the Amadeus API, which provides the latest flight ticket information. Once the backend receives the response from the Amadeus API, it prepares the data and sends it back to the frontend, where users can view and choose from the available flight options. 
