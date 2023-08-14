import React from 'react';
import { useLocation } from 'react-router-dom';

const ResponsePage = () => {
    const location = useLocation();
    const responseData = location.state?.responseData;

    console.log(JSON.stringify(responseData, null, 2));
    return (
      <div className='container'>
        <h2>Your journey</h2>
        <div className="tickets-block">
          {responseData.map((flight, index) => (
            <div key={index} className="flight-block">
              <div className='flight-name'>
                <p>
                From {flight.data[0].itineraries[0].segments[0].departure.iataCode} to {flight.data[0].itineraries[0].segments[flight.data[0].itineraries[0].segments.length-1].arrival.iataCode} 
                </p>
              </div>
              <div className='segments'>
		<p className='segment-title'>Segments</p>
                {flight.data[0].itineraries[0].segments.map((segment, i) => (
                  <div className='segment'>
		<p className='segment-cities'>
                    {segment.departure.iataCode} → {segment.arrival.iataCode}
		</p>
		<p className='segment-time'>(Departure: {segment.departure.at.replace('T', ' ')}. Arrival: {segment.arrival.at.replace('T', ' ')})</p>
                  </div>
                ))}
              </div>
              <div className='flight-price'><p>{flight.data[0].price.total} {flight.data[0].price.currency}</p></div>
            </div>
          ))}
        </div>
	<div className='total-price-block'>
	   <p className='total-price-block'>
	    	Total price:
		&nbsp;
		{responseData.reduce((total, flight) => total + parseInt(flight.data[0].price.total), 0)}
	    	&nbsp;
	    	{responseData[0].data[0].price.currency}	
	   </p>	
	</div>
      </div>
    );
  };

export default ResponsePage;
