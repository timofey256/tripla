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
    const response = await axios.get('http://localhost:5079/flights/route', {
      params: {
        route: flights,
      },
    });

    onSubmit(response.data);
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

