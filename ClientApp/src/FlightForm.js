import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import './styles.css';

function FlightForm({ onSubmit }) {
  const MAX_FLIGHTS = 5;
  const navigate = useNavigate();

  const [flights, setFlights] = useState([{ origin: '', dest: '', dateString: '' }]);

  const handleInputChange = (index, field, value) => {
    const newFlights = [...flights];
    newFlights[index][field] = value;
    setFlights(newFlights);
  };

  const addFlight = () => {
    if (flights.length < MAX_FLIGHTS) {
      const lastFlight = flights[flights.length - 1];
      setFlights([...flights, { origin: lastFlight.dest, dest: '', dateString: '' }]);    }
  };

  const handleSubmit = async () => {
    const baseUrl = 'http://localhost:5079/flights/route';
    let queryString = '';

    flights.forEach((flight, index) => {
      const params = [
        `flights[${index}].origin=${encodeURIComponent(flight.origin)}`,
        `flights[${index}].dest=${encodeURIComponent(flight.dest)}`,
        `flights[${index}].dateString=${encodeURIComponent(flight.dateString)}`,
      ];
      queryString += (queryString === '') ? `?${params.join('&')}` : `&${params.join('&')}`;
    });

    const fullUrl = `${baseUrl}${queryString}`;

    try {
      const response = await axios.get(fullUrl);
      navigate('/response', { state: { responseData: response.data } });
    } catch (error) {
      console.error('Error:', error);
    }
  };

  return (
    <div className="container">
      <h1>Flights Engine</h1>
      <div className="flights-row">
      {flights.map((flight, index) => (
          <div key={index} className="flight-block">
            <div className="input-row city-name">
              <input
                type="text"
                placeholder="Origin"
                value={flight.origin}
                onChange={(e) => handleInputChange(index, 'origin', e.target.value)}
                disabled={index}
              />
              <input
                type="text"
                placeholder="Destination"
                value={flight.dest}
                onChange={(e) => handleInputChange(index, 'dest', e.target.value)}
              />
            </div>
            <div className="input-row date-row">
              <input
                type="date"
                placeholder="Date (YYYY-MM-DD)"
                value={flight.dateString}
                onChange={(e) => handleInputChange(index, 'dateString', e.target.value)}
              />
            </div>
          </div>
      ))}
      </div>
      <div className="buttons">
      	<button className="add-flight" onClick={addFlight}>Add New City</button>
      	<button className="find-flight" onClick={handleSubmit}>Find</button>
      </div>
    </div>
  );
}

export default FlightForm;