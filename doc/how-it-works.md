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

### `/ClientApp/public/index.html`
A standard index html template. The remaining stuff is loaded dynamically. See `/ClientApp/src/`.

### `/ClientApp/src/App.js`
File where React project structure is defined (routes, pages, components for specific pages, etc.)

### `/ClientApp/src/FlightForm.js`
React component that is used on a main search page. It displays HTML, handles inputs, validates them, sends and receives requests to the backend.

### `/ClientApp/src/ResponsePage.js`
React component that is used on a result page. Essantially, its main purpose is to correctly parse received response and display it in a pretty way. No additional logic.

### `/Server/controllers/FlightController.cs`
The `FlightController.cs` file is a class that defines the controller which handles requests from the frontend.

1. The class is attributed with [ApiController] and [Route("[controller]")], indicating that it serves as a controller for handling HTTP requests and can be accessed on `[server location]/flights/` endpoint.
2. The constructor of the class initializes an instance of the AmadeusApiClient class and reads API settings from the `appsettings.json` it needs to then access data from API.
3. The [HttpGet("single")] method is used to handle GET requests for retrieving flight information for a single journey (one-way, in other words). It expects query parameters dateString, origin, and dest to be passed. It attempts to parse the provided date, retrieves flight information using the AmadeusApiClient, and returns the flight data if successful or a bad request response if date parsing fails.
4. The [HttpGet("route")] method handles GET requests for retrieving flight information for multiple segments of a journey. It expects a list of FlightInfo objects as query parameters. It iterates through the list, parsing the date, retrieving flight information for each segment using the AmadeusApiClient, and constructing an itinerary. If date parsing fails, it returns a bad request response.
5. Both methods expectedly return HTTP responses (Ok() or BadRequest()) depending on the result.

### `/Server/controllers/AmadeusApiClient.cs`
The `AmadeusApiClient.cs` file defines a class which is used to send and retrieve requests from Amadeus API.

1. The constructor of the AmadeusApiClient class initializes the client with the provided API key, API secret, and maximum number (if the number is >1, then the N cheapest tickets. otherwise, only the cheapest one) of flight offers to retrieve.
2. The GetFlights method is essantially the most important since it's a main interface with the class. The class is used to retrieve flight offers. It constructs the API endpoint URL(see `/Server/controllers/UrlFormatter.cs`)  and sends a request to the Amadeus API using the sendFlightsRequest method.
3. The sendFlightsRequest method sends an HTTP GET request to the Amadeus API using the HttpClient class, retrieves flight offer data, and deserializes it into a FlightOfferResponse object.
4. The checkIfTokenIsValid method checks if the access token is still valid (it can be expired and then we need to access a new one).
5. The setHeaders method sets headers required to make API requests.
6. The GetAccessToken method retrieves an access token from the Amadeus API by sending a POST request with the client credentials.
7. The getRequestContentForAccessToken method prepares the content for the access token request.

### `/Server/models`
A number of data sctuctures. Many used only for desearialization essantially.

### `/Server/appsetting.json`
Project file that contains necessary environment to start an application.


## Failed scrapper attempt
In the beginning of the project I was hoping to just scrap the data from some flight tickets aggregator (I choosed [SkyScrapper](https://www.skyscrapper.com/)) but it turned out they have pretty good bot detection systems that I didn't manage to get around. Actually, it doesn't detect the first request but the subsequent ones almost always yes. Anyway, I leaved code here in `/Server/scrapper`. Here is how it works: 
### `Scrapper.cs`:
0. Since SkyScrapper is a dynamic website I needed to use headless browser to run it. For this purpose PuppeteerSharp was used. 
1. After user calls `GetHTML(url)` class setups a browser. To resemble a real user it randomly sets a bunch of headers and cookies(userAgent, monitor size, network settings).
2. Then it just sends a request via PupeeterSharp and returns HTML.

### `PageParser.cs`
0. Then we need to parse this HTML in order to get retrieved tickets. 
1. After `GetAllCards(string page)` recieves a page, it serializes HTML into an object using a HTMLAgilityPack library.
2. Then get all div objects with a class that contains a specific substring which I figured out is consistently present there (the remaining part is randomly generated each time SkyScrapper sends a page).
3. And then we construct an instance of Country object the same way by extacting data from specific HTML tags.
4. So we do for all countries (countries are in "cards" so we call extraction from them `extractCardsInfo()`) and return a list of countries.