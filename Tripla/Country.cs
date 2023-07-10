namespace SkyScannerScraper;

class Country {
	public string Name { get; private set; }
	public int Price { get; private set; }
	public string Link { get; private set; }

	public Country(string name, int price, string link) {
		Name = name;
		Price = price;
		Link = link;
	}
}
