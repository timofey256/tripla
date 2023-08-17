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

## How does it work?
When a user enters the cities they want to visit and their travel dates, the frontend sends this information to the backend. The backend processes the data and then sends a request to the Amadeus API, which provides the latest flight ticket information. Once the backend receives the response from the Amadeus API, it prepares the data and sends it back to the frontend, where users can view and choose from the available flight options. 

Project structure:
```
.
├── ClientApp                         # frontend folder
│   ├── public
│   │   └── index.html
│   └── src
│       ├── App.js
│       ├── FlightForm.js             # main frontend page
│       ├── index.js
│       ├── ResponsePage.js           # tickets(response) frontend page
│       └── styles.css
├── Server                            # backend folder
│   ├── appsettings.json              # project config
│   ├── controllers
│   │   ├── AmadeusApiClient.cs
│   │   ├── FlightsController.cs      # controller that handles API requests from the frontend on /route and /single endpoints
│   │   ├── scrapper                  # my failed attempt to scrapp data from SkyScanner instead of using external API (Amadeus)
│   │   └── UrlFormatter.cs
│   ├── models                        # data structures
│   │   ├── Airport.cs
│   │   ├── ApiSettings.cs
│   │   ├── Country.cs
│   │   ├── FlightInfo.cs
│   │   ├── FlightOffer.cs
│   │   ├── FlightOfferResponse.cs
│   │   ├── Itinerary.cs
│   │   ├── Price.cs
│   │   ├── Segment.cs
│   │   └── TokenResponse.cs
│   ├── Program.cs                    # start point for backend
│   ├── Properties
│   │   └── launchSettings.json
│   └── Server.csproj
```

## Failed scrapper attempt
In the beginning of the project I was hoping to just scrap the data from some flight tickets aggregator (I choosed [SkyScrapper](https://www.skyscrapper.com/)) but it turned out they have pretty good bot detection systems that I didn't manage to get around. Actually, it doesn't detect the first request but the subsequent ones almost always yes. Anyway, I leaved code here in `/Server/scrapper`. Here is how it works: 
# `Scrapper.cs`:
0. Since SkyScrapper is a dynamic website I needed to use headless browser to run it. For this purpose PuppeteerSharp was used. 
1. After user calls `GetHTML(url)` class setups a browser. To resemble a real user it randomly sets a bunch of headers and cookies(userAgent, monitor size, network settings).
2. Then it just sends a request via PupeeterSharp and returns HTML.

# `PageParser.cs`
0. Then we need to parse this HTML in order to get retrieved tickets. 
1. After `GetAllCards(string page)` recieves a page, it serializes HTML into an object using a HTMLAgilityPack library.
2. Then get all div objects with a class that contains a specific substring which I figured out is consistently present there (the remaining part is randomly generated each time SkyScrapper sends a page).
3. And then we construct an instance of Country object the same way by extacting data from specific HTML tags.
4. So we do for all countries (countries are in "cards" so we call extraction from them `extractCardsInfo()`) and return a list of countries.
