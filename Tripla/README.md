Tripla is a trip planner that helps to find the cheapest trip within given dates. It accesses SkyScanner (unfortunately, they don't provide API, so Tripls scraps data) using headless browser to get flight schedule and prices of individual flights.

Dependecies:
1) PuppeteerSharp (to retrieve HTML from dynamically generated pages).
2) HtmlAgilityPack (to parse HTML)
