import React from 'react';
import { useLocation } from 'react-router-dom';

const ResponsePage = () => {
    const location = useLocation();
    const responseData = location.state?.responseData;

    console.log(JSON.stringify(responseData, null, 2));
    return (
      <div className='container'>
        <h2>Your flight</h2>
        <div className="">
          {responseData.map((flight, index) => (
            <div key={index} className="flight-response-block">
              <div className='flight-name'>
                <p>
                From {flight.data[0].itineraries[0].segments[0].departure.iataCode} to {flight.data[0].itineraries[0].segments[flight.data[0].itineraries[0].segments.length-1].departure.iataCode} 
                </p>
              </div>
              <div className='segments'>
                {flight.data[0].itineraries[0].segments.map((segment, i) => (
                  <div className='segment'>
                    {segment.departure.iataCode} → {segment.arrival.iataCode}
                  </div>
                ))}
              </div>
              <div className='flight-price'><p>{flight.data[0].price.total} {flight.data[0].price.currency}</p></div>
            </div>
          ))}
        </div>
      </div>
    );
  };

export default ResponsePage;
