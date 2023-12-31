import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import './styles.css';

function FlightForm({ onSubmit }) {
  const MAX_FLIGHTS = 5;
  const navigate = useNavigate();
  const baseUrl = 'http://localhost:5079/flights/route';

  const [flights, setFlights] = useState([{ origin: '', dest: '', dateString: '' }]);
  const [errorMessage, setErrorMessage] = React.useState("");

  const handleInputChange = (index, field, value) => {
    const newFlights = [...flights];
    newFlights[index][field] = value;
    
    if (field === 'dest' && newFlights.length > index + 1) {
      newFlights[index+1]['origin'] = value;
    }
  
    if (value !== "" && newFlights[index]["origin"] === newFlights[index]["dest"]) {
      setErrorMessage("Your origin and destination can not be the same!");
    }

    setFlights(newFlights);
  };

  const addFlight = () => {
    if (flights[flights.length-1]['dest'] === "") {
      setErrorMessage("Before adding new city, choose previous destination!");
      return;
    }
    
    if (flights.length < MAX_FLIGHTS) {
      const lastFlight = flights[flights.length - 1];
      setFlights([...flights, { origin: lastFlight.dest, dest: '', dateString: '' }]);    
    }
  };

  const handleSubmit = async () => {
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
      	<button className="flight-form-button add-flight" onClick={addFlight}>Add New City</button>
      	<button className="flight-form-button find-flight" onClick={handleSubmit}>Find</button>
      </div>
      {errorMessage && <div className="error"> {errorMessage} </div>}
    </div>
  );
}

export default FlightForm;
