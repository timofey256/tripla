import React, { useState } from 'react';
import axios from 'axios';
import './styles.css';

function FlightForm({ onSubmit }) {
  const MAX_FLIGHTS = 5;

  const [flights, setFlights] = useState([{ dateString: '', origin: '', dest: '' }]);

  const handleInputChange = (index, field, value) => {
    const newFlights = [...flights];
    newFlights[index][field] = value;
    setFlights(newFlights);
  };

  const addFlight = () => {
    if (flights.length < MAX_FLIGHTS) {
    	setFlights([...flights, { dateString: '', origin: '', dest: '' }]);
    }
  };

  const handleSubmit = async () => {
	  const baseUrl = 'http://localhost:5079/flights/route';
	  let queryString = '';

	  flights.forEach((flight, index) => {
	    const params = [
	      `flights[${index}].dateString=${encodeURIComponent(flight.dateString)}`,
	      `flights[${index}].origin=${encodeURIComponent(flight.origin)}`,
	      `flights[${index}].dest=${encodeURIComponent(flight.dest)}`,
	    ];
	    queryString += (queryString === '') ? `?${params.join('&')}` : `&${params.join('&')}`;
	  });

	  const fullUrl = `${baseUrl}${queryString}`;

	  try {
	    const response = await axios.get(fullUrl);
	    onSubmit(response.data);
	  } catch (error) {
	    console.error('Error:', error);
	  }
  };

  return (
    <div className="container">
      {flights.map((flight, index) => (
        <div key={index} className="input-group">
          <input
            type="text"
            placeholder="Date (YYYY-MM-DD)"
            value={flight.dateString}
            onChange={(e) => handleInputChange(index, 'dateString', e.target.value)}
          />
          <input
            type="text"
            placeholder="Origin"
            value={flight.origin}
            onChange={(e) => handleInputChange(index, 'origin', e.target.value)}
          />
          <input
            type="text"
            placeholder="Destination"
            value={flight.dest}
            onChange={(e) => handleInputChange(index, 'dest', e.target.value)}
          />
        </div>
      ))}
      <button onClick={addFlight}>Add Flight</button>
      <button onClick={handleSubmit}>Find</button>
    </div>
  );
}

function App() {
  const handleSearchResults = (data) => {
    // Handle the retrieved flight data here
    console.log(data);
  };

  return (
    <div>
      <h1>Flight Search</h1>
      <FlightForm onSubmit={handleSearchResults} />
    </div>
  );
}

export default App;

